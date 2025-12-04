using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence
{
    #region 事件参数类

    /// <summary>
    /// 步骤执行事件参数
    /// </summary>
    public class StepExecutionEventArgs : EventArgs
    {
        public TestStep Step { get; set; }
        public int StepIndex { get; set; }
        public int TotalSteps { get; set; }
        public StepResult Result { get; set; }
        public object ActualValue { get; set; }
        public string Message { get; set; }
        public double ExecutionTime { get; set; }
    }

    /// <summary>
    /// 序列执行事件参数
    /// </summary>
    public class SequenceExecutionEventArgs : EventArgs
    {
        public TestSequence Sequence { get; set; }
        public SequenceState State { get; set; }
        public string Message { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public double TotalExecutionTime { get; set; }
    }

    /// <summary>
    /// 日志事件参数
    /// </summary>
    public class ExecutorLogEventArgs : EventArgs
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Level { get; set; } = "INFO";
        public string Message { get; set; }
        public string StepName { get; set; }
    }

    #endregion

    /// <summary>
    /// 序列执行引擎
    /// 负责执行测试序列，使用反射调用配置的测试方法
    /// </summary>
    public class SequenceExecutor : IDisposable
    {
        #region 常量

        /// <summary>
        /// 表达式最大长度限制
        /// </summary>
        private const int MAX_EXPRESSION_LENGTH = 1000;

        /// <summary>
        /// 表达式最大递归深度限制
        /// </summary>
        private const int MAX_RECURSION_DEPTH = 50;

        /// <summary>
        /// While循环默认最大迭代次数
        /// </summary>
        private const int DEFAULT_MAX_ITERATIONS = 10000;

        /// <summary>
        /// While循环默认总超时时间（毫秒）
        /// </summary>
        private const int DEFAULT_WHILE_TIMEOUT_MS = 300000; // 5分钟

        #endregion

        #region 字段

        private readonly TestMethodRegistry _registry;
        private TestSequence _currentSequence;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isPaused;
        private readonly object _pauseLock = new object();
        private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);
        private bool _disposed = false;

        /// <summary>
        /// 全局变量存储 - 跨序列共享，使用ConcurrentDictionary保证线程安全
        /// </summary>
        private static readonly ConcurrentDictionary<string, SequenceVariable> _globalVariables = new ConcurrentDictionary<string, SequenceVariable>();
        private static readonly object _globalVarLock = new object();

        /// <summary>
        /// 局部变量存储 - 当前步骤级别，使用ConcurrentDictionary保证线程安全
        /// </summary>
        private readonly ConcurrentDictionary<string, SequenceVariable> _localVariables = new ConcurrentDictionary<string, SequenceVariable>();

        #endregion

        #region 属性

        /// <summary>
        /// 当前序列
        /// </summary>
        public TestSequence CurrentSequence => _currentSequence;

        /// <summary>
        /// 是否正在执行
        /// </summary>
        public bool IsRunning => _currentSequence?.State == SequenceState.Running;

        /// <summary>
        /// 是否已暂停
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// 是否为单步调试模式
        /// </summary>
        public bool SingleStepMode { get; set; } = false;

        /// <summary>
        /// 执行上下文（用于传递额外参数）
        /// </summary>
        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();

        /// <summary>
        /// 产品对象引用（用于测试方法）
        /// </summary>
        public Product CurrentProduct { get; set; }

        /// <summary>
        /// 模块对象引用（用于测试方法）
        /// </summary>
        public Module CurrentModule { get; set; }

        #endregion

        #region 事件

        /// <summary>
        /// 步骤开始执行事件
        /// </summary>
        public event EventHandler<StepExecutionEventArgs> StepStarted;

        /// <summary>
        /// 步骤执行完成事件
        /// </summary>
        public event EventHandler<StepExecutionEventArgs> StepCompleted;

        /// <summary>
        /// 序列开始执行事件
        /// </summary>
        public event EventHandler<SequenceExecutionEventArgs> SequenceStarted;

        /// <summary>
        /// 序列执行完成事件
        /// </summary>
        public event EventHandler<SequenceExecutionEventArgs> SequenceCompleted;

        /// <summary>
        /// 日志事件
        /// </summary>
        public event EventHandler<ExecutorLogEventArgs> Log;

        /// <summary>
        /// 进度更新事件
        /// </summary>
        public event EventHandler<int> ProgressChanged;

        #endregion

        #region 构造函数

        public SequenceExecutor()
        {
            _registry = TestMethodRegistry.Instance;
            if (!_registry.IsInitialized)
            {
                _registry.Initialize();
            }
        }

        public SequenceExecutor(TestMethodRegistry registry)
        {
            _registry = registry ?? TestMethodRegistry.Instance;
        }

        #endregion

        #region 执行控制方法

        /// <summary>
        /// 同步执行序列
        /// </summary>
        public SequenceExecutionEventArgs Execute(TestSequence sequence)
        {
            return ExecuteAsync(sequence).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 异步执行序列
        /// </summary>
        public async Task<SequenceExecutionEventArgs> ExecuteAsync(TestSequence sequence, CancellationToken cancellationToken = default)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            _currentSequence = sequence;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _isPaused = false;
            _pauseEvent.Set();

            // 重置序列状态
            sequence.Reset();
            sequence.State = SequenceState.Running;
            sequence.StartTime = DateTime.Now;

            // 初始化全局变量
            InitializeGlobalVariables();

            WriteLog("INFO", $"开始执行序列: {sequence.Name}");

            OnSequenceStarted(new SequenceExecutionEventArgs
            {
                Sequence = sequence,
                State = SequenceState.Running,
                Message = "序列开始执行"
            });

            try
            {
                // 检查前置条件
                if (!string.IsNullOrEmpty(sequence.PreCondition))
                {
                    WriteLog("INFO", $"检查前置条件: {sequence.PreCondition}");
                    if (!EvaluateCondition(sequence.PreCondition))
                    {
                        throw new InvalidOperationException($"前置条件不满足: {sequence.PreCondition}");
                    }
                }

                // 执行所有步骤
                int totalSteps = sequence.Steps.Count(s => s.Enabled);
                int executedSteps = 0;
                bool shouldEndSequence = false;

                for (int i = 0; i < sequence.Steps.Count && !shouldEndSequence; i++)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    // 清除局部变量（每个步骤开始时）
                    ClearLocalVariables();

                    // 检查暂停
                    _pauseEvent.Wait(_cancellationTokenSource.Token);

                    var step = sequence.Steps[i];
                    sequence.CurrentStepIndex = i;

                    if (!step.Enabled)
                    {
                        step.Result = StepResult.Skipped;
                        continue;
                    }

                    // 执行步骤
                    await ExecuteStepAsync(step, i, sequence.Steps.Count);
                    executedSteps++;

                    // 更新进度
                    int progress = (int)((double)executedSteps / totalSteps * 100);
                    OnProgressChanged(progress);

                    // 处理失败
                    if (step.Result == StepResult.Fail || step.Result == StepResult.Error)
                    {
                        switch (step.OnFail)
                        {
                            case FailAction.Abort:
                                WriteLog("WARN", $"步骤 {step.Name} 失败，中断序列执行");
                                sequence.State = SequenceState.Aborted;
                                sequence.ErrorMessage = $"步骤 {step.Name} 失败: {step.ResultMessage}";
                                shouldEndSequence = true;
                                break;

                            case FailAction.GotoStep:
                                if (!string.IsNullOrEmpty(step.GotoStepID))
                                {
                                    int gotoIndex = sequence.GetStepIndexById(step.GotoStepID);
                                    if (gotoIndex >= 0 && gotoIndex < sequence.Steps.Count)
                                    {
                                        WriteLog("INFO", $"跳转到步骤: {step.GotoStepID}");
                                        i = gotoIndex - 1; // -1 因为循环会+1
                                    }
                                }
                                break;

                            case FailAction.GotoPostCondition:
                                WriteLog("INFO", "跳转到后置条件");
                                shouldEndSequence = true;
                                break;

                            case FailAction.Continue:
                            default:
                                // 继续执行
                                break;
                        }
                    }

                    // 单步模式
                    if (SingleStepMode)
                    {
                        Pause();
                    }
                }

                // 执行后置条件
                if (!string.IsNullOrEmpty(sequence.PostCondition))
                {
                    WriteLog("INFO", $"执行后置条件: {sequence.PostCondition}");
                    ExecutePostCondition(sequence.PostCondition);
                }

                // 设置最终状态
                if (sequence.State == SequenceState.Running)
                {
                    sequence.State = sequence.FailCount > 0 || sequence.ErrorCount > 0
                        ? SequenceState.Failed
                        : SequenceState.Completed;
                }
            }
            catch (OperationCanceledException)
            {
                sequence.State = SequenceState.Aborted;
                sequence.ErrorMessage = "执行被取消";
                WriteLog("WARN", "序列执行被取消");
            }
            catch (Exception ex)
            {
                sequence.State = SequenceState.Failed;
                sequence.ErrorMessage = ex.Message;
                WriteLog("ERROR", $"序列执行异常: {ex.Message}");
            }
            finally
            {
                sequence.EndTime = DateTime.Now;
            }

            var result = new SequenceExecutionEventArgs
            {
                Sequence = sequence,
                State = sequence.State,
                PassCount = sequence.PassCount,
                FailCount = sequence.FailCount,
                TotalExecutionTime = sequence.ExecutionTime,
                Message = sequence.ErrorMessage ?? (sequence.OverallPass ? "序列执行成功" : "序列执行失败")
            };

            WriteLog("INFO", $"序列执行完成: {result.Message} (Pass:{result.PassCount}, Fail:{result.FailCount})");
            OnSequenceCompleted(result);

            return result;
        }

        /// <summary>
        /// 执行单个步骤
        /// </summary>
        private async Task ExecuteStepAsync(TestStep step, int stepIndex, int totalSteps)
        {
            step.StartTime = DateTime.Now;
            step.Result = StepResult.Running;

            OnStepStarted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps
            });

            WriteLog("INFO", $"开始执行步骤 [{step.ID}] {step.Name}", step.Name);

            // 检查前置条件
            if (!string.IsNullOrEmpty(step.Precondition))
            {
                if (!EvaluateCondition(step.Precondition))
                {
                    step.Result = StepResult.Skipped;
                    step.ResultMessage = $"前置条件不满足: {step.Precondition}";
                    WriteLog("INFO", $"步骤跳过（前置条件不满足）: {step.Name}");
                    step.EndTime = DateTime.Now;
                    OnStepCompleted(new StepExecutionEventArgs
                    {
                        Step = step,
                        StepIndex = stepIndex,
                        TotalSteps = totalSteps,
                        Result = step.Result,
                        Message = step.ResultMessage,
                        ExecutionTime = step.ExecutionTime
                    });
                    return;
                }
            }

            // 根据步骤类型执行不同的逻辑
            switch (step.Type)
            {
                case StepType.ForLoop:
                    await ExecuteForLoopAsync(step, stepIndex, totalSteps);
                    break;

                case StepType.WhileLoop:
                    await ExecuteWhileLoopAsync(step, stepIndex, totalSteps);
                    break;

                case StepType.ForEachLoop:
                    await ExecuteForEachLoopAsync(step, stepIndex, totalSteps);
                    break;

                case StepType.ConditionalBranch:
                    await ExecuteConditionalBranchAsync(step, stepIndex, totalSteps);
                    break;

                case StepType.SubSequenceCall:
                    await ExecuteSubSequenceCallAsync(step, stepIndex, totalSteps);
                    break;

                case StepType.SequenceGroup:
                    await ExecuteSequenceGroupAsync(step, stepIndex, totalSteps);
                    break;

                default:
                    await ExecuteStandardStepAsync(step, stepIndex, totalSteps);
                    break;
            }
        }

        /// <summary>
        /// 执行标准步骤（非控制流步骤）
        /// </summary>
        private async Task ExecuteStandardStepAsync(TestStep step, int stepIndex, int totalSteps)
        {
            int retryCount = 0;
            bool success = false;

            do
            {
                try
                {
                    // 调用测试方法
                    var result = await InvokeTestMethodAsync(step);

                    // 评估结果
                    EvaluateStepResult(step, result);

                    // 存储结果到变量（如果指定了ResultVariable）
                    StoreResultToVariable(step);

                    success = step.Result == StepResult.Pass;

                    if (!success && retryCount < step.RetryCount)
                    {
                        retryCount++;
                        step.ActualRetryCount = retryCount;
                        WriteLog("INFO", $"步骤重试 ({retryCount}/{step.RetryCount}): {step.Name}");
                        await Task.Delay(500); // 重试前等待
                    }
                }
                catch (TimeoutException)
                {
                    step.Result = StepResult.Timeout;
                    step.ResultMessage = "执行超时";
                    WriteLog("ERROR", $"步骤超时: {step.Name}", step.Name);
                }
                catch (Exception ex)
                {
                    step.Result = StepResult.Error;
                    step.ResultMessage = ex.Message;
                    WriteLog("ERROR", $"步骤异常: {step.Name} - {ex.Message}", step.Name);
                }

            } while (!success && retryCount < step.RetryCount);

            step.EndTime = DateTime.Now;

            OnStepCompleted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps,
                Result = step.Result,
                ActualValue = step.ActualValue,
                Message = step.ResultMessage,
                ExecutionTime = step.ExecutionTime
            });

            WriteLog(step.Result == StepResult.Pass ? "INFO" : "WARN",
                $"步骤完成 [{step.Result}] {step.Name}: {step.ActualValue} ({step.ExecutionTime:F0}ms)", step.Name);
        }

        /// <summary>
        /// 执行For循环
        /// </summary>
        private async Task ExecuteForLoopAsync(TestStep step, int stepIndex, int totalSteps)
        {
            if (step.SubSteps == null || step.SubSteps.Count == 0)
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = "循环体为空";
                step.EndTime = DateTime.Now;
                return;
            }

            int start = step.LoopStart;
            int end = step.LoopEnd;
            int loopStep = step.LoopStep != 0 ? step.LoopStep : 1;
            string loopVar = step.LoopVariable ?? "i";

            WriteLog("INFO", $"开始For循环: {loopVar} = {start} to {end}, step = {loopStep}");

            bool loopSuccess = true;
            int iterationCount = 0;

            for (int i = start; loopStep > 0 ? i <= end : i >= end; i += loopStep)
            {
                if (iterationCount >= step.MaxIterations)
                {
                    WriteLog("WARN", $"达到最大迭代次数: {step.MaxIterations}");
                    break;
                }

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                // 设置循环变量
                SetVariable(loopVar, i, VariableScope.Local);

                WriteLog("INFO", $"For循环迭代 {loopVar} = {i}");

                // 执行子步骤
                foreach (var subStep in step.SubSteps)
                {
                    if (!subStep.Enabled)
                        continue;

                    await ExecuteStepAsync(subStep, stepIndex, totalSteps);

                    if (subStep.Result == StepResult.Fail || subStep.Result == StepResult.Error)
                    {
                        if (subStep.OnFail == FailAction.Abort)
                        {
                            loopSuccess = false;
                            break;
                        }
                    }
                }

                if (!loopSuccess)
                    break;

                iterationCount++;
            }

            step.Result = loopSuccess ? StepResult.Pass : StepResult.Fail;
            step.ResultMessage = $"循环执行完成，共 {iterationCount} 次迭代";
            step.EndTime = DateTime.Now;

            OnStepCompleted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps,
                Result = step.Result,
                Message = step.ResultMessage,
                ExecutionTime = step.ExecutionTime
            });
        }

        /// <summary>
        /// 执行While循环
        /// </summary>
        private async Task ExecuteWhileLoopAsync(TestStep step, int stepIndex, int totalSteps)
        {
            if (step.SubSteps == null || step.SubSteps.Count == 0)
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = "循环体为空";
                step.EndTime = DateTime.Now;
                return;
            }

            // 设置默认最大迭代次数（防止死循环）
            if (step.MaxIterations <= 0)
            {
                step.MaxIterations = DEFAULT_MAX_ITERATIONS;
                WriteLog("INFO", $"While循环未设置最大迭代次数，使用默认值: {DEFAULT_MAX_ITERATIONS}");
            }

            // 计算总超时时间（防止整数溢出）
            int totalTimeout;
            if (step.Timeout > 0)
            {
                // 使用 checked 或 long 防止溢出
                long calculatedTimeout = (long)step.Timeout * step.MaxIterations;
                totalTimeout = (int)Math.Min(calculatedTimeout, DEFAULT_WHILE_TIMEOUT_MS);
            }
            else
            {
                totalTimeout = DEFAULT_WHILE_TIMEOUT_MS;
            }

            WriteLog("INFO", $"开始While循环: {step.WhileCondition} (最大迭代: {step.MaxIterations}, 总超时: {totalTimeout}ms)");

            bool loopSuccess = true;
            int iterationCount = 0;
            var loopStartTime = DateTime.Now;

            while (EvaluateCondition(step.WhileCondition))
            {
                // 检查最大迭代次数
                if (iterationCount >= step.MaxIterations)
                {
                    WriteLog("WARN", $"达到最大迭代次数: {step.MaxIterations}");
                    break;
                }

                // 检查总超时时间
                if ((DateTime.Now - loopStartTime).TotalMilliseconds >= totalTimeout)
                {
                    WriteLog("WARN", $"While循环总超时: {totalTimeout}ms");
                    break;
                }

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                WriteLog("INFO", $"While循环迭代 #{iterationCount + 1}");

                // 执行子步骤
                foreach (var subStep in step.SubSteps)
                {
                    if (!subStep.Enabled)
                        continue;

                    await ExecuteStepAsync(subStep, stepIndex, totalSteps);

                    if (subStep.Result == StepResult.Fail || subStep.Result == StepResult.Error)
                    {
                        if (subStep.OnFail == FailAction.Abort)
                        {
                            loopSuccess = false;
                            break;
                        }
                    }
                }

                if (!loopSuccess)
                    break;

                iterationCount++;
            }

            step.Result = loopSuccess ? StepResult.Pass : StepResult.Fail;
            step.ResultMessage = $"While循环完成，共 {iterationCount} 次迭代";
            step.EndTime = DateTime.Now;

            OnStepCompleted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps,
                Result = step.Result,
                Message = step.ResultMessage,
                ExecutionTime = step.ExecutionTime
            });
        }

        /// <summary>
        /// 执行ForEach循环
        /// </summary>
        private async Task ExecuteForEachLoopAsync(TestStep step, int stepIndex, int totalSteps)
        {
            if (step.SubSteps == null || step.SubSteps.Count == 0)
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = "循环体为空";
                step.EndTime = DateTime.Now;
                return;
            }

            // 获取数组变量
            var arrayValue = GetVariableValue(step.ArrayVariable);
            if (arrayValue == null || !(arrayValue is Array array))
            {
                step.Result = StepResult.Error;
                step.ResultMessage = $"数组变量 {step.ArrayVariable} 不存在或不是数组";
                step.EndTime = DateTime.Now;
                return;
            }

            string elementVar = step.ElementVariable ?? "item";
            string indexVar = step.LoopVariable ?? "index";

            WriteLog("INFO", $"开始ForEach循环: {elementVar} in {step.ArrayVariable} (共 {array.Length} 个元素)");

            bool loopSuccess = true;
            int iterationCount = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (iterationCount >= step.MaxIterations)
                {
                    WriteLog("WARN", $"达到最大迭代次数: {step.MaxIterations}");
                    break;
                }

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                object element = array.GetValue(i);
                SetVariable(elementVar, element, VariableScope.Local);
                SetVariable(indexVar, i, VariableScope.Local);

                WriteLog("INFO", $"ForEach循环迭代 [{i}]: {elementVar} = {element}");

                // 执行子步骤
                foreach (var subStep in step.SubSteps)
                {
                    if (!subStep.Enabled)
                        continue;

                    await ExecuteStepAsync(subStep, stepIndex, totalSteps);

                    if (subStep.Result == StepResult.Fail || subStep.Result == StepResult.Error)
                    {
                        if (subStep.OnFail == FailAction.Abort)
                        {
                            loopSuccess = false;
                            break;
                        }
                    }
                }

                if (!loopSuccess)
                    break;

                iterationCount++;
            }

            step.Result = loopSuccess ? StepResult.Pass : StepResult.Fail;
            step.ResultMessage = $"ForEach循环完成，共 {iterationCount} 次迭代";
            step.EndTime = DateTime.Now;

            OnStepCompleted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps,
                Result = step.Result,
                Message = step.ResultMessage,
                ExecutionTime = step.ExecutionTime
            });
        }

        /// <summary>
        /// 执行条件分支
        /// </summary>
        private async Task ExecuteConditionalBranchAsync(TestStep step, int stepIndex, int totalSteps)
        {
            bool conditionResult = EvaluateCondition(step.BranchCondition);

            WriteLog("INFO", $"条件分支评估: {step.BranchCondition} = {conditionResult}");

            var stepsToExecute = conditionResult ? step.TrueSteps : step.FalseSteps;

            if (stepsToExecute == null || stepsToExecute.Count == 0)
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = $"条件 = {conditionResult}, 无执行步骤";
                step.EndTime = DateTime.Now;
                return;
            }

            bool branchSuccess = true;

            foreach (var subStep in stepsToExecute)
            {
                if (!subStep.Enabled)
                    continue;

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                await ExecuteStepAsync(subStep, stepIndex, totalSteps);

                if (subStep.Result == StepResult.Fail || subStep.Result == StepResult.Error)
                {
                    if (subStep.OnFail == FailAction.Abort)
                    {
                        branchSuccess = false;
                        break;
                    }
                }
            }

            step.Result = branchSuccess ? StepResult.Pass : StepResult.Fail;
            step.ResultMessage = $"条件分支({conditionResult})执行完成";
            step.EndTime = DateTime.Now;

            OnStepCompleted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps,
                Result = step.Result,
                Message = step.ResultMessage,
                ExecutionTime = step.ExecutionTime
            });
        }

        /// <summary>
        /// 执行子序列调用
        /// </summary>
        private async Task ExecuteSubSequenceCallAsync(TestStep step, int stepIndex, int totalSteps)
        {
            TestSequence subSequence = null;

            // 尝试从外部文件加载
            if (!string.IsNullOrEmpty(step.SubSequenceFile))
            {
                try
                {
                    var loader = new Config.SequenceConfigLoader();
                    var config = loader.LoadConfig(step.SubSequenceFile);
                    
                    if (!string.IsNullOrEmpty(step.SubSequenceId))
                    {
                        subSequence = config.GetSequence(step.SubSequenceId);
                    }
                    else if (config.Sequences.Count > 0)
                    {
                        subSequence = config.Sequences[0];
                    }
                }
                catch (Exception ex)
                {
                    step.Result = StepResult.Error;
                    step.ResultMessage = $"加载子序列文件失败: {ex.Message}";
                    step.EndTime = DateTime.Now;
                    return;
                }
            }
            // 从当前配置中查找
            else if (!string.IsNullOrEmpty(step.SubSequenceId))
            {
                // 需要从注册的序列中查找
                subSequence = GetRegisteredSequence(step.SubSequenceId);
            }

            if (subSequence == null)
            {
                step.Result = StepResult.Error;
                step.ResultMessage = $"未找到子序列: {step.SubSequenceId ?? step.SubSequenceFile}";
                step.EndTime = DateTime.Now;
                return;
            }

            WriteLog("INFO", $"开始执行子序列: {subSequence.Name}");

            // 创建子执行器执行子序列
            var subExecutor = new SequenceExecutor(_registry);
            subExecutor.CurrentProduct = this.CurrentProduct;
            subExecutor.CurrentModule = this.CurrentModule;
            
            // 复制上下文
            foreach (var kvp in Context)
            {
                subExecutor.Context[kvp.Key] = kvp.Value;
            }

            // 订阅事件转发
            subExecutor.Log += (s, e) => WriteLog(e.Level, $"[子序列] {e.Message}", e.StepName);
            subExecutor.StepStarted += (s, e) => OnStepStarted(e);
            subExecutor.StepCompleted += (s, e) => OnStepCompleted(e);

            var result = await subExecutor.ExecuteAsync(subSequence, _cancellationTokenSource.Token);

            step.Result = result.State == SequenceState.Completed ? StepResult.Pass : StepResult.Fail;
            step.ResultMessage = $"子序列执行完成: {result.Message}";
            step.EndTime = DateTime.Now;

            OnStepCompleted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps,
                Result = step.Result,
                Message = step.ResultMessage,
                ExecutionTime = step.ExecutionTime
            });
        }

        /// <summary>
        /// 执行步骤组
        /// </summary>
        private async Task ExecuteSequenceGroupAsync(TestStep step, int stepIndex, int totalSteps)
        {
            if (step.SubSteps == null || step.SubSteps.Count == 0)
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = "步骤组为空";
                step.EndTime = DateTime.Now;
                return;
            }

            WriteLog("INFO", $"开始执行步骤组: {step.Name}");

            bool groupSuccess = true;

            foreach (var subStep in step.SubSteps)
            {
                if (!subStep.Enabled)
                    continue;

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                await ExecuteStepAsync(subStep, stepIndex, totalSteps);

                if (subStep.Result == StepResult.Fail || subStep.Result == StepResult.Error)
                {
                    if (subStep.OnFail == FailAction.Abort)
                    {
                        groupSuccess = false;
                        break;
                    }
                }
            }

            step.Result = groupSuccess ? StepResult.Pass : StepResult.Fail;
            step.ResultMessage = $"步骤组执行完成，共 {step.SubSteps.Count} 个子步骤";
            step.EndTime = DateTime.Now;

            OnStepCompleted(new StepExecutionEventArgs
            {
                Step = step,
                StepIndex = stepIndex,
                TotalSteps = totalSteps,
                Result = step.Result,
                Message = step.ResultMessage,
                ExecutionTime = step.ExecutionTime
            });
        }

        /// <summary>
        /// 已注册的序列（用于子序列调用）
        /// </summary>
        private static readonly Dictionary<string, TestSequence> _registeredSequences = new Dictionary<string, TestSequence>();
        private static readonly object _seqLock = new object();

        /// <summary>
        /// 注册序列（供子序列调用使用）
        /// </summary>
        public static void RegisterSequence(TestSequence sequence)
        {
            if (sequence == null || string.IsNullOrEmpty(sequence.ID))
                return;

            lock (_seqLock)
            {
                _registeredSequences[sequence.ID] = sequence;
            }
        }

        /// <summary>
        /// 获取已注册的序列
        /// </summary>
        public static TestSequence GetRegisteredSequence(string id)
        {
            lock (_seqLock)
            {
                _registeredSequences.TryGetValue(id, out var sequence);
                return sequence;
            }
        }

        /// <summary>
        /// 清除所有已注册的序列
        /// </summary>
        public static void ClearRegisteredSequences()
        {
            lock (_seqLock)
            {
                _registeredSequences.Clear();
            }
        }

        /// <summary>
        /// 调用测试方法
        /// </summary>
        private async Task<object> InvokeTestMethodAsync(TestStep step)
        {
            if (step.TargetMethod == null || string.IsNullOrEmpty(step.TargetMethod.MethodName))
            {
                // 对于Action类型，如果没有方法，直接返回成功
                if (step.Type == StepType.Action)
                {
                    return true;
                }
                throw new InvalidOperationException("未配置目标方法");
            }

            var registeredMethod = _registry.GetMethod(step.TargetMethod.ClassName, step.TargetMethod.MethodName);
            if (registeredMethod == null)
            {
                throw new InvalidOperationException($"未找到测试方法: {step.TargetMethod.ClassName}.{step.TargetMethod.MethodName}");
            }

            // 准备参数
            var parameters = PrepareMethodParameters(registeredMethod, step);

            // 获取实例
            object instance = null;
            if (!registeredMethod.IsStatic)
            {
                instance = registeredMethod.Instance ?? _registry.GetOrCreateInstance(registeredMethod.DeclaringType);
            }

            // 使用超时执行
            using (var cts = new CancellationTokenSource(step.Timeout))
            {
                var task = Task.Run(() =>
                {
                    return registeredMethod.Method.Invoke(instance, parameters);
                }, cts.Token);

                try
                {
                    return await task;
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException($"方法执行超时 ({step.Timeout}ms)");
                }
            }
        }

        /// <summary>
        /// 准备方法参数
        /// </summary>
        private object[] PrepareMethodParameters(RegisteredTestMethod method, TestStep step)
        {
            var paramInfos = method.Parameters;
            if (paramInfos == null || paramInfos.Length == 0)
                return new object[0];

            var args = new object[paramInfos.Length];

            for (int i = 0; i < paramInfos.Length; i++)
            {
                var paramInfo = paramInfos[i];
                object value = null;

                // 首先尝试从步骤参数中获取
                var stepParam = step.Parameters.FirstOrDefault(p => p.Name == paramInfo.Name);
                if (stepParam != null)
                {
                    // 解析变量引用（如 ${variableName}）
                    var resolvedValue = ResolveVariableReferences(stepParam.Value);
                    if (resolvedValue != stepParam.Value)
                    {
                        // 如果值被解析了，尝试转换类型
                        value = ConvertValue(resolvedValue, paramInfo.ParameterType);
                    }
                    else
                    {
                        value = ConvertValue(stepParam.GetTypedValue(), paramInfo.ParameterType);
                    }
                }
                // 尝试从序列变量获取
                else if (_currentSequence != null)
                {
                    var seqVar = _currentSequence.GetVariable(paramInfo.Name);
                    if (seqVar != null)
                    {
                        value = ConvertValue(seqVar.CurrentValue ?? seqVar.GetTypedDefaultValue(), paramInfo.ParameterType);
                    }
                }

                // 如果还是没有值，尝试从上下文获取
                if (value == null && Context.TryGetValue(paramInfo.Name, out var contextValue))
                {
                    value = ConvertValue(contextValue, paramInfo.ParameterType);
                }
                // 特殊类型处理
                else if (value == null && paramInfo.ParameterType == typeof(Product))
                {
                    value = CurrentProduct;
                }
                else if (value == null && paramInfo.ParameterType == typeof(Module))
                {
                    value = CurrentModule;
                }
                // 使用默认值
                else if (value == null && paramInfo.HasDefaultValue)
                {
                    value = paramInfo.DefaultValue;
                }

                args[i] = value;
            }

            return args;
        }

        /// <summary>
        /// 解析变量引用 - 将 ${variableName} 替换为实际值
        /// </summary>
        private object ResolveVariableReferences(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.Contains("${"))
                return value;

            // 检查是否是纯变量引用 ${variableName}
            if (value.StartsWith("${") && value.EndsWith("}") && value.IndexOf("${", 2) < 0)
            {
                string varName = value.Substring(2, value.Length - 3);
                return GetVariableValue(varName);
            }

            // 处理字符串中的多个变量引用
            string result = value;
            int startIndex = 0;
            while ((startIndex = result.IndexOf("${", startIndex)) >= 0)
            {
                int endIndex = result.IndexOf("}", startIndex);
                if (endIndex < 0)
                    break;

                string varName = result.Substring(startIndex + 2, endIndex - startIndex - 2);
                object varValue = GetVariableValue(varName);
                string replacement = varValue?.ToString() ?? "";

                result = result.Substring(0, startIndex) + replacement + result.Substring(endIndex + 1);
                startIndex += replacement.Length;
            }

            return result;
        }

        /// <summary>
        /// 获取变量值（按作用域优先级获取：局部 > 序列 > 全局 > 上下文）
        /// 支持数组索引访问，如 ${arrayVar[0]}
        /// </summary>
        private object GetVariableValue(string name)
        {
            // 检查是否是数组索引访问
            int arrayIndex = -1;
            string varName = name;
            
            int bracketStart = name.IndexOf('[');
            if (bracketStart > 0 && name.EndsWith("]"))
            {
                varName = name.Substring(0, bracketStart);
                string indexStr = name.Substring(bracketStart + 1, name.Length - bracketStart - 2);
                if (int.TryParse(indexStr, out arrayIndex))
                {
                    // 索引解析成功
                }
                else
                {
                    // 索引可能是变量引用
                    var indexValue = GetVariableValue(indexStr);
                    if (indexValue is int idx)
                    {
                        arrayIndex = idx;
                    }
                }
            }

            SequenceVariable variable = null;

            // 1. 首先从局部变量获取
            if (_localVariables.TryGetValue(varName, out var localVar))
            {
                variable = localVar;
            }
            // 2. 然后从序列变量获取
            else if (_currentSequence != null)
            {
                var seqVar = _currentSequence.GetVariable(varName);
                if (seqVar != null && seqVar.Scope != VariableScope.Global)
                {
                    variable = seqVar;
                }
            }
            // 3. 最后从全局变量获取
            if (variable == null)
            {
                lock (_globalVarLock)
                {
                    if (_globalVariables.TryGetValue(varName, out var globalVar))
                    {
                        variable = globalVar;
                    }
                }
            }

            if (variable != null)
            {
                var value = variable.CurrentValue ?? variable.GetTypedDefaultValue();
                
                // 如果是数组并且指定了索引
                if (arrayIndex >= 0 && variable.IsArray)
                {
                    return variable.GetArrayElement(arrayIndex);
                }
                
                return value;
            }

            // 4. 最后从上下文获取
            if (Context.TryGetValue(varName, out var contextValue))
            {
                if (arrayIndex >= 0 && contextValue is Array arr && arrayIndex < arr.Length)
                {
                    return arr.GetValue(arrayIndex);
                }
                return contextValue;
            }

            return null;
        }

        /// <summary>
        /// 设置变量值（根据作用域存储到对应位置）
        /// </summary>
        public bool SetVariable(string name, object value, VariableScope scope = VariableScope.Sequence)
        {
            // 检查是否是数组索引设置
            int arrayIndex = -1;
            string varName = name;
            
            int bracketStart = name.IndexOf('[');
            if (bracketStart > 0 && name.EndsWith("]"))
            {
                varName = name.Substring(0, bracketStart);
                string indexStr = name.Substring(bracketStart + 1, name.Length - bracketStart - 2);
                int.TryParse(indexStr, out arrayIndex);
            }

            SequenceVariable variable = null;

            switch (scope)
            {
                case VariableScope.Local:
                    if (!_localVariables.TryGetValue(varName, out variable))
                    {
                        variable = new SequenceVariable(varName, value?.GetType().Name.ToLower() ?? "string", null, VariableScope.Local);
                        _localVariables[varName] = variable;
                    }
                    break;

                case VariableScope.Global:
                    lock (_globalVarLock)
                    {
                        if (!_globalVariables.TryGetValue(varName, out variable))
                        {
                            variable = new SequenceVariable(varName, value?.GetType().Name.ToLower() ?? "string", null, VariableScope.Global);
                            _globalVariables[varName] = variable;
                        }
                    }
                    break;

                case VariableScope.Sequence:
                default:
                    if (_currentSequence != null)
                    {
                        variable = _currentSequence.GetVariable(varName);
                        if (variable == null)
                        {
                            variable = new SequenceVariable(varName, value?.GetType().Name.ToLower() ?? "string", null, VariableScope.Sequence);
                            _currentSequence.Variables.Add(variable);
                        }
                    }
                    break;
            }

            if (variable != null)
            {
                if (arrayIndex >= 0 && variable.IsArray)
                {
                    return variable.SetArrayElement(arrayIndex, value);
                }
                variable.CurrentValue = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取全局变量
        /// </summary>
        public static SequenceVariable GetGlobalVariable(string name)
        {
            lock (_globalVarLock)
            {
                _globalVariables.TryGetValue(name, out var variable);
                return variable;
            }
        }

        /// <summary>
        /// 设置全局变量
        /// </summary>
        public static void SetGlobalVariable(string name, SequenceVariable variable)
        {
            lock (_globalVarLock)
            {
                variable.Scope = VariableScope.Global;
                _globalVariables[name] = variable;
            }
        }

        /// <summary>
        /// 清除所有全局变量
        /// </summary>
        public static void ClearGlobalVariables()
        {
            lock (_globalVarLock)
            {
                _globalVariables.Clear();
            }
        }

        /// <summary>
        /// 清除局部变量（通常在步骤开始时调用）
        /// </summary>
        private void ClearLocalVariables()
        {
            _localVariables.Clear();
        }

        /// <summary>
        /// 初始化全局变量（从序列配置中）
        /// </summary>
        private void InitializeGlobalVariables()
        {
            if (_currentSequence == null)
                return;

            foreach (var variable in _currentSequence.Variables.Where(v => v.Scope == VariableScope.Global))
            {
                lock (_globalVarLock)
                {
                    if (!_globalVariables.ContainsKey(variable.Name))
                    {
                        variable.Reset();
                        _globalVariables[variable.Name] = variable;
                        WriteLog("INFO", $"初始化全局变量: {variable.Name} = {variable.CurrentValue}");
                    }
                }
            }
        }

        /// <summary>
        /// 存储步骤结果到变量
        /// </summary>
        private void StoreResultToVariable(TestStep step)
        {
            if (string.IsNullOrEmpty(step.ResultVariable) || _currentSequence == null)
                return;

            // 检查是否带有作用域前缀：Local.varName, Global.varName
            VariableScope scope = VariableScope.Sequence;
            string varName = step.ResultVariable;

            if (varName.StartsWith("Local.", StringComparison.OrdinalIgnoreCase))
            {
                scope = VariableScope.Local;
                varName = varName.Substring(6);
            }
            else if (varName.StartsWith("Global.", StringComparison.OrdinalIgnoreCase))
            {
                scope = VariableScope.Global;
                varName = varName.Substring(7);
            }

            SetVariable(varName, step.ActualValue, scope);
            WriteLog("INFO", $"存储结果到变量: {step.ResultVariable} = {step.ActualValue} (Scope: {scope})");
        }

        /// <summary>
        /// 转换值到目标类型
        /// </summary>
        private object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;

            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// 评估步骤结果
        /// </summary>
        private void EvaluateStepResult(TestStep step, object result)
        {
            step.ActualValue = result;

            switch (step.Type)
            {
                case StepType.NumericTest:
                    EvaluateNumericResult(step, result);
                    break;

                case StepType.StringTest:
                    EvaluateStringResult(step, result);
                    break;

                case StepType.PassFail:
                    EvaluateBoolResult(step, result);
                    break;

                case StepType.Action:
                    // 动作执行成功即为Pass（除非抛出异常）
                    step.Result = StepResult.Pass;
                    step.ResultMessage = "执行成功";
                    break;

                default:
                    step.Result = StepResult.Pass;
                    step.ResultMessage = result?.ToString();
                    break;
            }
        }

        /// <summary>
        /// 评估数值结果
        /// </summary>
        private void EvaluateNumericResult(TestStep step, object result)
        {
            if (step.Limits == null)
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = result?.ToString();
                return;
            }

            double numericValue;
            if (result is double d)
                numericValue = d;
            else if (result is float f)
                numericValue = f;
            else if (result is int i)
                numericValue = i;
            else if (result is long l)
                numericValue = l;
            else if (result is decimal dec)
                numericValue = (double)dec;
            else if (!double.TryParse(result?.ToString(), out numericValue))
            {
                step.Result = StepResult.Error;
                step.ResultMessage = "无法转换为数值";
                return;
            }

            step.ActualValue = numericValue;

            if (step.Limits.IsWithinLimits(numericValue))
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = $"{numericValue:F3} {step.Limits.Unit} (在限值范围内)";
            }
            else
            {
                step.Result = StepResult.Fail;
                step.ResultMessage = $"{numericValue:F3} {step.Limits.Unit} 超出限值范围 {step.Limits}";
            }
        }

        /// <summary>
        /// 评估字符串结果
        /// </summary>
        private void EvaluateStringResult(TestStep step, object result)
        {
            string strValue = result?.ToString() ?? "";
            step.ActualValue = strValue;

            if (string.IsNullOrEmpty(step.ExpectedString))
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = strValue;
                return;
            }

            if (strValue.Equals(step.ExpectedString, StringComparison.OrdinalIgnoreCase))
            {
                step.Result = StepResult.Pass;
                step.ResultMessage = $"匹配成功: {strValue}";
            }
            else
            {
                step.Result = StepResult.Fail;
                step.ResultMessage = $"不匹配: 期望 '{step.ExpectedString}', 实际 '{strValue}'";
            }
        }

        /// <summary>
        /// 评估布尔结果
        /// </summary>
        private void EvaluateBoolResult(TestStep step, object result)
        {
            bool boolValue;
            if (result is bool b)
                boolValue = b;
            else if (!bool.TryParse(result?.ToString(), out boolValue))
            {
                // 非零数值视为true
                double numValue;
                if (double.TryParse(result?.ToString(), out numValue))
                    boolValue = numValue != 0;
                else
                    boolValue = result != null;
            }

            step.ActualValue = boolValue;
            step.Result = boolValue ? StepResult.Pass : StepResult.Fail;
            step.ResultMessage = boolValue ? "通过" : "失败";
        }

        /// <summary>
        /// 评估条件表达式
        /// 支持的格式：
        /// - ${变量名} > 值
        /// - ${变量名} == 值
        /// - ${变量名} != 值
        /// - ${变量名} >= 值
        /// - ${变量名} <= 值
        /// - ${变量名} - 布尔变量直接作为条件
        /// - !${变量名} - 取反
        /// - ${变量名1} && ${变量名2} - 逻辑与
        /// - ${变量名1} || ${变量名2} - 逻辑或
        /// </summary>
        private bool EvaluateCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            try
            {
                // 首先解析所有变量引用
                string evaluatedCondition = ResolveAllVariableReferences(condition);

                // 简单的布尔表达式评估
                if (evaluatedCondition.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (evaluatedCondition.Equals("false", StringComparison.OrdinalIgnoreCase))
                    return false;

                // 处理逻辑运算符
                if (evaluatedCondition.Contains("&&"))
                {
                    var parts = evaluatedCondition.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                    return parts.All(p => EvaluateSimpleCondition(p.Trim()));
                }

                if (evaluatedCondition.Contains("||"))
                {
                    var parts = evaluatedCondition.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    return parts.Any(p => EvaluateSimpleCondition(p.Trim()));
                }

                return EvaluateSimpleCondition(evaluatedCondition);
            }
            catch (Exception ex)
            {
                WriteLog("WARN", $"条件表达式评估失败: {condition} - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 解析所有变量引用（包括表达式计算）
        /// </summary>
        private string ResolveAllVariableReferences(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.Contains("${"))
                return value;

            string result = value;
            int startIndex = 0;
            
            while ((startIndex = result.IndexOf("${", startIndex)) >= 0)
            {
                int endIndex = result.IndexOf("}", startIndex);
                if (endIndex < 0)
                    break;

                string varExpr = result.Substring(startIndex + 2, endIndex - startIndex - 2);
                object varValue = GetVariableValue(varExpr);
                string replacement = varValue?.ToString() ?? "";

                result = result.Substring(0, startIndex) + replacement + result.Substring(endIndex + 1);
                startIndex += replacement.Length;
            }

            return result;
        }

        /// <summary>
        /// 评估简单的条件表达式
        /// </summary>
        private bool EvaluateSimpleCondition(string condition)
        {
            condition = condition.Trim();

            // 处理取反
            if (condition.StartsWith("!"))
            {
                return !EvaluateSimpleCondition(condition.Substring(1).Trim());
            }

            // 比较运算符
            string[] operators = { ">=", "<=", "!=", "==", ">", "<" };
            foreach (var op in operators)
            {
                int opIndex = condition.IndexOf(op);
                if (opIndex > 0)
                {
                    string left = condition.Substring(0, opIndex).Trim();
                    string right = condition.Substring(opIndex + op.Length).Trim();
                    return EvaluateComparison(left, op, right);
                }
            }

            // 作为布尔值评估
            if (bool.TryParse(condition, out bool boolValue))
                return boolValue;

            // 非零数值视为true
            if (double.TryParse(condition, out double numValue))
                return numValue != 0;

            // 非空字符串视为true
            return !string.IsNullOrEmpty(condition);
        }

        /// <summary>
        /// 评估比较表达式
        /// </summary>
        private bool EvaluateComparison(string left, string op, string right)
        {
            // 尝试作为数值比较
            if (double.TryParse(left, out double leftNum) && double.TryParse(right, out double rightNum))
            {
                switch (op)
                {
                    case ">": return leftNum > rightNum;
                    case "<": return leftNum < rightNum;
                    case ">=": return leftNum >= rightNum;
                    case "<=": return leftNum <= rightNum;
                    case "==": return Math.Abs(leftNum - rightNum) < 0.00001;
                    case "!=": return Math.Abs(leftNum - rightNum) >= 0.00001;
                }
            }

            // 作为字符串比较
            switch (op)
            {
                case "==": return left.Equals(right, StringComparison.OrdinalIgnoreCase);
                case "!=": return !left.Equals(right, StringComparison.OrdinalIgnoreCase);
                default: return false;
            }
        }

        /// <summary>
        /// 计算数学表达式
        /// 支持的运算符：+, -, *, /, %, ^
        /// </summary>
        public object EvaluateExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return null;

            // 表达式长度限制
            if (expression.Length > MAX_EXPRESSION_LENGTH)
            {
                throw new InvalidOperationException($"表达式长度超过限制 ({MAX_EXPRESSION_LENGTH} 字符)");
            }

            try
            {
                // 首先解析变量引用
                string resolvedExpr = ResolveAllVariableReferences(expression);

                // 简单的数学表达式计算
                return EvaluateMathExpression(resolvedExpr, 0);
            }
            catch (Exception ex)
            {
                WriteLog("WARN", $"表达式计算失败: {expression} - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 计算数学表达式（简单实现）
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="depth">当前递归深度</param>
        private double EvaluateMathExpression(string expression, int depth = 0)
        {
            // 递归深度限制
            if (depth > MAX_RECURSION_DEPTH)
            {
                throw new InvalidOperationException($"表达式嵌套层数超过限制 ({MAX_RECURSION_DEPTH})");
            }

            // 表达式长度限制
            if (expression.Length > MAX_EXPRESSION_LENGTH)
            {
                throw new InvalidOperationException($"表达式长度超过限制 ({MAX_EXPRESSION_LENGTH} 字符)");
            }

            expression = expression.Trim();

            // 处理括号
            while (expression.Contains("("))
            {
                int openParen = expression.LastIndexOf('(');
                int closeParen = expression.IndexOf(')', openParen);
                if (closeParen < 0)
                    throw new ArgumentException("括号不匹配");

                string innerExpr = expression.Substring(openParen + 1, closeParen - openParen - 1);
                double innerResult = EvaluateMathExpression(innerExpr, depth + 1);
                expression = expression.Substring(0, openParen) + innerResult.ToString() + expression.Substring(closeParen + 1);
            }

            // 处理加减运算（从右到左，确保正确的优先级）
            for (int i = expression.Length - 1; i >= 0; i--)
            {
                if ((expression[i] == '+' || expression[i] == '-') && i > 0)
                {
                    // 确保不是负号
                    char prevChar = expression[i - 1];
                    if (char.IsDigit(prevChar) || prevChar == ')')
                    {
                        string left = expression.Substring(0, i);
                        string right = expression.Substring(i + 1);
                        double leftVal = EvaluateMathExpression(left, depth + 1);
                        double rightVal = EvaluateMathExpression(right, depth + 1);
                        return expression[i] == '+' ? leftVal + rightVal : leftVal - rightVal;
                    }
                }
            }

            // 处理乘除运算
            for (int i = expression.Length - 1; i >= 0; i--)
            {
                if (expression[i] == '*' || expression[i] == '/' || expression[i] == '%')
                {
                    string left = expression.Substring(0, i);
                    string right = expression.Substring(i + 1);
                    double leftVal = EvaluateMathExpression(left, depth + 1);
                    double rightVal = EvaluateMathExpression(right, depth + 1);
                    switch (expression[i])
                    {
                        case '*': return leftVal * rightVal;
                        case '/': return rightVal != 0 ? leftVal / rightVal : 0;
                        case '%': return rightVal != 0 ? leftVal % rightVal : 0;
                    }
                }
            }

            // 处理幂运算
            int powerIndex = expression.IndexOf('^');
            if (powerIndex > 0)
            {
                string left = expression.Substring(0, powerIndex);
                string right = expression.Substring(powerIndex + 1);
                return Math.Pow(EvaluateMathExpression(left, depth + 1), EvaluateMathExpression(right, depth + 1));
            }

            // 返回数值
            if (double.TryParse(expression.Trim(), out double result))
                return result;

            throw new ArgumentException($"无法解析表达式: {expression}");
        }

        /// <summary>
        /// 执行后置条件
        /// </summary>
        private void ExecutePostCondition(string postCondition)
        {
            // 简单实现：如果后置条件是"SaveResults"，则保存结果
            if (postCondition.Contains("SaveResults"))
            {
                WriteLog("INFO", "执行后置条件: 保存结果");
                // 实际保存逻辑由外部实现
            }
        }

        /// <summary>
        /// 暂停执行
        /// </summary>
        public void Pause()
        {
            lock (_pauseLock)
            {
                _isPaused = true;
                _pauseEvent.Reset();
            }
            WriteLog("INFO", "序列执行已暂停");
        }

        /// <summary>
        /// 继续执行
        /// </summary>
        public void Resume()
        {
            lock (_pauseLock)
            {
                _isPaused = false;
                _pauseEvent.Set();
            }
            WriteLog("INFO", "序列执行已恢复");
        }

        /// <summary>
        /// 停止执行
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _pauseEvent.Set(); // 确保不会卡在暂停状态
            WriteLog("INFO", "序列执行已停止");
        }

        /// <summary>
        /// 执行单步
        /// </summary>
        public void StepOver()
        {
            if (SingleStepMode && IsPaused)
            {
                Resume();
            }
        }

        #endregion

        #region 事件触发方法

        protected virtual void OnStepStarted(StepExecutionEventArgs e)
        {
            StepStarted?.Invoke(this, e);
        }

        protected virtual void OnStepCompleted(StepExecutionEventArgs e)
        {
            StepCompleted?.Invoke(this, e);
        }

        protected virtual void OnSequenceStarted(SequenceExecutionEventArgs e)
        {
            SequenceStarted?.Invoke(this, e);
        }

        protected virtual void OnSequenceCompleted(SequenceExecutionEventArgs e)
        {
            SequenceCompleted?.Invoke(this, e);
        }

        protected virtual void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        protected void WriteLog(string level, string message, string stepName = null)
        {
            Log?.Invoke(this, new ExecutorLogEventArgs
            {
                Level = level,
                Message = message,
                StepName = stepName
            });
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 清除上下文
        /// </summary>
        public void ClearContext()
        {
            Context.Clear();
        }

        /// <summary>
        /// 设置上下文变量
        /// </summary>
        public void SetContextVariable(string name, object value)
        {
            Context[name] = value;
        }

        /// <summary>
        /// 获取上下文变量
        /// </summary>
        public object GetContextVariable(string name)
        {
            return Context.TryGetValue(name, out var value) ? value : null;
        }

        /// <summary>
        /// 清理过期的全局变量
        /// </summary>
        /// <param name="maxAge">最大存活时间</param>
        public static void CleanupStaleGlobalVariables(TimeSpan maxAge)
        {
            var now = DateTime.Now;
            lock (_globalVarLock)
            {
                var staleKeys = _globalVariables
                    .Where(kvp => now - kvp.Value.LastAccessTime > maxAge)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in staleKeys)
                {
                    _globalVariables.TryRemove(key, out _);
                }
            }
        }

        /// <summary>
        /// 清理过期的已注册序列
        /// </summary>
        /// <param name="maxAge">最大存活时间</param>
        public static void CleanupStaleRegisteredSequences(TimeSpan maxAge)
        {
            var now = DateTime.Now;
            lock (_seqLock)
            {
                var staleKeys = _registeredSequences
                    .Where(kvp => now - kvp.Value.ModifiedTime > maxAge)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in staleKeys)
                {
                    _registeredSequences.Remove(key);
                }
            }
        }

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    _pauseEvent?.Dispose();
                    _cancellationTokenSource?.Dispose();
                    _localVariables.Clear();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
