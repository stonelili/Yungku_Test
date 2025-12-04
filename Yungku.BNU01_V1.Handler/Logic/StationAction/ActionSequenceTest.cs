using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using Yungku.BNU01_V1.Handler.Logic.TestSequence;
using Yungku.BNU01_V1.Handler.Logic.TestSequence.Config;
using YungkuSystem.Controls;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    /// <summary>
    /// 基于测试序列的动作类
    /// 替代ActionTest，使用内部测试方法而非外部DLL
    /// </summary>
    public class ActionSequenceTest : ActionObject
    {
        #region 字段

        private SequenceExecutor _executor;
        private TestSequence.TestSequence _currentSequence;
        private SequenceConfigLoader _configLoader;
        private string _lastError;

        #endregion

        #region 属性

        /// <summary>
        /// 对象类型标识
        /// </summary>
        public override string ObjectClass
        {
            get { return "序列测试"; }
        }

        /// <summary>
        /// 指示此对象是否是一个容器
        /// </summary>
        public override bool IsContainner
        {
            get { return false; }
        }

        private string _sequenceFilePath = string.Empty;
        /// <summary>
        /// 序列配置文件路径
        /// </summary>
        [MyDisplayName("序列配置文件"), MyCategory("序列配置")]
        [Description("测试序列XML配置文件的路径")]
        public string SequenceFilePath
        {
            get { return _sequenceFilePath; }
            set { _sequenceFilePath = value; }
        }

        private string _sequenceId = string.Empty;
        /// <summary>
        /// 要执行的序列ID
        /// </summary>
        [MyDisplayName("序列ID"), MyCategory("序列配置")]
        [Description("要执行的测试序列ID，如果为空则执行第一个序列")]
        public string SequenceId
        {
            get { return _sequenceId; }
            set { _sequenceId = value; }
        }

        private bool _singleStepMode = false;
        /// <summary>
        /// 是否启用单步调试模式
        /// </summary>
        [MyDisplayName("单步调试模式"), MyCategory("序列配置")]
        [Description("启用后每执行一个步骤会暂停")]
        public bool SingleStepMode
        {
            get { return _singleStepMode; }
            set { _singleStepMode = value; }
        }

        private bool _stopOnFirstFail = false;
        /// <summary>
        /// 首个失败时停止
        /// </summary>
        [MyDisplayName("首个失败时停止"), MyCategory("序列配置")]
        [Description("遇到第一个失败的步骤时停止执行")]
        public bool StopOnFirstFail
        {
            get { return _stopOnFirstFail; }
            set { _stopOnFirstFail = value; }
        }

        private bool _displayToUI = true;
        /// <summary>
        /// 是否显示到UI
        /// </summary>
        [MyDisplayName("显示到UI"), MyCategory("序列配置")]
        [Description("是否将测试结果显示到界面")]
        public bool DisplayToUI
        {
            get { return _displayToUI; }
            set { _displayToUI = value; }
        }

        /// <summary>
        /// 当前序列
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public TestSequence.TestSequence CurrentSequence => _currentSequence;

        /// <summary>
        /// 执行器
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public SequenceExecutor Executor => _executor;

        /// <summary>
        /// 最后一次错误信息
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string LastError => _lastError;

        #endregion

        #region 构造函数

        public ActionSequenceTest()
        {
            InitializeExecutor();
        }

        private void InitializeExecutor()
        {
            _executor = new SequenceExecutor();
            _configLoader = new SequenceConfigLoader();

            // 订阅事件
            _executor.StepStarted += OnStepStarted;
            _executor.StepCompleted += OnStepCompleted;
            _executor.SequenceStarted += OnSequenceStarted;
            _executor.SequenceCompleted += OnSequenceCompleted;
            _executor.Log += OnExecutorLog;

            // 确保测试方法注册表已初始化
            if (!TestMethodRegistry.Instance.IsInitialized)
            {
                TestMethodRegistry.Instance.Initialize();
            }
        }

        #endregion

        #region 执行方法

        /// <summary>
        /// 执行测试序列
        /// </summary>
        protected override void Execute()
        {
            base.Execute();

            Station st = Station.Station;
            int stationIndex = st.Turntable.Stations.IndexOf(st);
            string stationName = stationIndex == 0 ? "左工位" : "右工位";

            WriteRecord($"========== [{stationName}] ActionSequenceTest 启动 ==========");
            WriteRecord($"动作名称: {Name}");
            WriteRecord($"序列文件: {SequenceFilePath}");
            WriteRecord($"序列ID: {SequenceId}");

            // 检查前置条件
            if (!CurrentHeadObject.HasModudeState)
            {
                WriteRecord($"[{stationName}] 当前测试头无模块状态，跳过");
                return;
            }

            if (!HasPass && !MustExecute)
            {
                WriteRecord($"[{stationName}] 无Pass产品且非强制执行，跳过");
                return;
            }

            if (IsEmpty)
            {
                WriteRecord($"[{stationName}] IsEmpty=true，跳过");
                return;
            }

            try
            {
                // 加载序列配置
                if (!LoadSequenceConfig())
                {
                    WriteInfo($"[{stationName}] 加载序列配置失败: {_lastError}", true);
                    if (!MustExecute)
                    {
                        OnAlarm($"加载序列配置失败: {_lastError}");
                    }
                    return;
                }

                // 设置执行上下文
                SetupExecutionContext(st, stationIndex);

                // 更新UI状态
                if (_displayToUI)
                {
                    UpdateUIStatus(stationIndex, "序列执行中...");
                }

                // 执行序列
                _executor.SingleStepMode = _singleStepMode;
                var result = _executor.Execute(_currentSequence);

                // 处理结果
                ProcessExecutionResult(result, st, stationIndex, stationName);
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                WriteInfo($"[{stationName}] 序列执行异常: {ex.Message}", true);

                if (!MustExecute)
                {
                    OnAlarm($"序列执行异常: {ex.Message}");
                }
            }

            WriteRecord($"========== [{stationName}] ActionSequenceTest 完成 ==========");
        }

        /// <summary>
        /// 加载序列配置
        /// </summary>
        private bool LoadSequenceConfig()
        {
            try
            {
                string filePath = SequenceFilePath;

                // 如果是相对路径，转换为绝对路径
                if (!Path.IsPathRooted(filePath))
                {
                    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
                }

                if (!File.Exists(filePath))
                {
                    _lastError = $"配置文件不存在: {filePath}";
                    return false;
                }

                var config = _configLoader.LoadConfig(filePath);

                if (config == null || config.Sequences.Count == 0)
                {
                    _lastError = "配置文件中没有定义序列";
                    return false;
                }

                // 根据ID获取序列，如果没有指定ID则使用第一个
                if (!string.IsNullOrEmpty(_sequenceId))
                {
                    _currentSequence = config.GetSequence(_sequenceId);
                    if (_currentSequence == null)
                    {
                        _lastError = $"未找到指定的序列: {_sequenceId}";
                        return false;
                    }
                }
                else
                {
                    _currentSequence = config.Sequences[0];
                }

                WriteRecord($"已加载序列: {_currentSequence.Name} ({_currentSequence.Steps.Count} 个步骤)");
                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 设置执行上下文
        /// </summary>
        private void SetupExecutionContext(Station st, int stationIndex)
        {
            _executor.ClearContext();

            // 添加站位信息到上下文
            _executor.SetContextVariable("StationIndex", stationIndex);
            _executor.SetContextVariable("StationName", stationIndex == 0 ? "左工位" : "右工位");

            // 添加产品信息到上下文
            Head hd = st.Turntable.GetHeadByStation(st);
            if (hd != null)
            {
                foreach (Jig jig in hd.TestItems)
                {
                    foreach (Product product in jig.TestItems)
                    {
                        ProductObject po = product.BindingObject as ProductObject;
                        if (po != null && po.OwnerEnabled && product.Result == TestResult.Pass)
                        {
                            _executor.CurrentProduct = product;
                            _executor.SetContextVariable("ProductName", product.Name);
                            _executor.SetContextVariable("Barcode", product.BarCodeString);
                            break;
                        }
                    }
                }
            }

            // 添加条件变量
            _executor.SetContextVariable("HasPass", HasPass.ToString().ToLower());
            _executor.SetContextVariable("IsEmpty", IsEmpty.ToString().ToLower());
            _executor.SetContextVariable("HasProduct", (!IsEmpty).ToString().ToLower());
        }

        /// <summary>
        /// 处理执行结果
        /// </summary>
        private void ProcessExecutionResult(SequenceExecutionEventArgs result, Station st, int stationIndex, string stationName)
        {
            WriteRecord($"[{stationName}] 序列执行完成: {result.State}");
            WriteRecord($"  Pass: {result.PassCount}, Fail: {result.FailCount}");
            WriteRecord($"  耗时: {result.TotalExecutionTime:F0} ms");

            // 更新产品状态
            Head hd = st.Turntable.GetHeadByStation(st);
            if (hd != null)
            {
                foreach (Jig jig in hd.TestItems)
                {
                    foreach (Product product in jig.TestItems)
                    {
                        if (product.Result == TestResult.Pass)
                        {
                            if (result.FailCount > 0 || result.State == SequenceState.Failed)
                            {
                                product.Result = TestResult.Fail;
                                product.ResultCode = "SEQ_FAIL";
                            }
                        }
                    }
                }
            }

            // 更新UI
            if (_displayToUI)
            {
                string resultStr = result.State == SequenceState.Completed && result.FailCount == 0
                    ? "PASS"
                    : $"FAIL (P:{result.PassCount}/F:{result.FailCount})";
                UpdateUIStatus(stationIndex, resultStr);
            }
        }

        /// <summary>
        /// 更新UI状态
        /// </summary>
        private void UpdateUIStatus(int stationIndex, string status)
        {
            try
            {
                if (stationIndex == 0)
                {
                    TestDisplayHelper.UpdateLeftStation("序列测试", status);
                }
                else if (stationIndex == 1)
                {
                    TestDisplayHelper.UpdateRightStation("序列测试", status);
                }
            }
            catch
            {
                // 忽略UI更新错误
            }
        }

        #endregion

        #region 事件处理

        private void OnStepStarted(object sender, StepExecutionEventArgs e)
        {
            WriteRecord($"  → 步骤开始: [{e.StepIndex + 1}/{e.TotalSteps}] {e.Step.Name}");

            if (_displayToUI)
            {
                Station st = Station.Station;
                int stationIndex = st.Turntable.Stations.IndexOf(st);

                try
                {
                    if (stationIndex == 0)
                    {
                        TestDisplayHelper.UpdateLeftStation(e.Step.Name, "执行中...");
                    }
                    else if (stationIndex == 1)
                    {
                        TestDisplayHelper.UpdateRightStation(e.Step.Name, "执行中...");
                    }
                }
                catch { }
            }
        }

        private void OnStepCompleted(object sender, StepExecutionEventArgs e)
        {
            string resultStr = e.Result == StepResult.Pass ? "PASS" :
                              e.Result == StepResult.Fail ? "FAIL" :
                              e.Result.ToString();

            WriteRecord($"  ← 步骤完成: [{e.StepIndex + 1}/{e.TotalSteps}] {e.Step.Name} = {resultStr} ({e.ExecutionTime:F0}ms)");

            if (e.Step.Type == StepType.NumericTest && e.Step.Limits != null)
            {
                WriteRecord($"     值: {e.ActualValue} {e.Step.Limits.Unit} [限值: {e.Step.Limits}]");
            }

            if (_displayToUI)
            {
                Station st = Station.Station;
                int stationIndex = st.Turntable.Stations.IndexOf(st);

                try
                {
                    string displayResult = resultStr;
                    if (e.Step.Type == StepType.NumericTest && e.ActualValue != null)
                    {
                        displayResult = $"{resultStr} ({e.ActualValue})";
                    }

                    if (stationIndex == 0)
                    {
                        if (e.Step.Limits != null)
                        {
                            TestDisplayHelper.UpdateLeftStation(e.Step.Name,
                                e.Step.Limits.Lower.ToString("F3"),
                                e.Step.Limits.Upper.ToString("F3"),
                                e.ActualValue?.ToString() ?? "",
                                resultStr);
                        }
                        else
                        {
                            TestDisplayHelper.UpdateLeftStation(e.Step.Name, displayResult);
                        }
                    }
                    else if (stationIndex == 1)
                    {
                        if (e.Step.Limits != null)
                        {
                            TestDisplayHelper.UpdateRightStation(e.Step.Name,
                                e.Step.Limits.Lower.ToString("F3"),
                                e.Step.Limits.Upper.ToString("F3"),
                                e.ActualValue?.ToString() ?? "",
                                resultStr);
                        }
                        else
                        {
                            TestDisplayHelper.UpdateRightStation(e.Step.Name, displayResult);
                        }
                    }
                }
                catch { }
            }
        }

        private void OnSequenceStarted(object sender, SequenceExecutionEventArgs e)
        {
            WriteRecord($"序列开始: {e.Sequence.Name}");
        }

        private void OnSequenceCompleted(object sender, SequenceExecutionEventArgs e)
        {
            WriteRecord($"序列完成: {e.Sequence.Name} - {e.Message}");
        }

        private void OnExecutorLog(object sender, ExecutorLogEventArgs e)
        {
            if (e.Level == "ERROR")
            {
                WriteInfo(e.Message, true);
            }
            else
            {
                WriteRecord($"[{e.Level}] {e.Message}");
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 停止序列执行
        /// </summary>
        public void StopSequence()
        {
            _executor?.Stop();
        }

        /// <summary>
        /// 暂停序列执行
        /// </summary>
        public void PauseSequence()
        {
            _executor?.Pause();
        }

        /// <summary>
        /// 恢复序列执行
        /// </summary>
        public void ResumeSequence()
        {
            _executor?.Resume();
        }

        /// <summary>
        /// 单步执行
        /// </summary>
        public void StepOver()
        {
            _executor?.StepOver();
        }

        /// <summary>
        /// 重新加载序列配置
        /// </summary>
        public bool ReloadSequence()
        {
            return LoadSequenceConfig();
        }

        #endregion
    }
}
