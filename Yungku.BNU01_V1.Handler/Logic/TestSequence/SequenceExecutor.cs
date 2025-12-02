using System;
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
    public class SequenceExecutor
    {
        #region 字段

        private readonly TestMethodRegistry _registry;
        private TestSequence _currentSequence;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isPaused;
        private readonly object _pauseLock = new object();
        private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);

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
        /// 获取变量值（先从序列变量获取，再从上下文获取）
        /// </summary>
        private object GetVariableValue(string name)
        {
            // 首先从序列变量获取
            if (_currentSequence != null)
            {
                var seqVar = _currentSequence.GetVariable(name);
                if (seqVar != null)
                {
                    return seqVar.CurrentValue ?? seqVar.GetTypedDefaultValue();
                }
            }

            // 然后从上下文获取
            if (Context.TryGetValue(name, out var contextValue))
            {
                return contextValue;
            }

            return null;
        }

        /// <summary>
        /// 存储步骤结果到变量
        /// </summary>
        private void StoreResultToVariable(TestStep step)
        {
            if (string.IsNullOrEmpty(step.ResultVariable) || _currentSequence == null)
                return;

            var variable = _currentSequence.GetVariable(step.ResultVariable);
            if (variable == null)
            {
                // 自动创建变量
                variable = new SequenceVariable
                {
                    Name = step.ResultVariable,
                    Type = step.ActualValue?.GetType().Name.ToLower() ?? "string"
                };
                _currentSequence.Variables.Add(variable);
                WriteLog("INFO", $"自动创建变量: {step.ResultVariable}");
            }

            variable.CurrentValue = step.ActualValue;
            WriteLog("INFO", $"存储结果到变量: {step.ResultVariable} = {step.ActualValue}");
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
        /// </summary>
        private bool EvaluateCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            // 简单的条件解析，支持基本的变量替换
            try
            {
                string evaluatedCondition = condition;

                // 替换上下文变量
                foreach (var kvp in Context)
                {
                    evaluatedCondition = evaluatedCondition.Replace(kvp.Key, kvp.Value?.ToString());
                }

                // 简单的布尔表达式评估
                if (evaluatedCondition.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (evaluatedCondition.Equals("false", StringComparison.OrdinalIgnoreCase))
                    return false;

                // 其他复杂表达式暂时返回true
                return true;
            }
            catch
            {
                return true;
            }
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

        #endregion
    }
}
