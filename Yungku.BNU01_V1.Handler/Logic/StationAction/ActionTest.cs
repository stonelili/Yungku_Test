using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using YungkuSystem.Controls;
using YungkuSystem.Machine;
using YungkuSystem.Script.Actions.DeviceBase;
using YungkuSystem.Statistical;
using YungkuSystem.Structs;
using YungkuSystem.Tester;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    public class ActionTest : ActionObject
    {
        /// <summary>
        /// 测试命令对应字典
        /// </summary>
        private Dictionary<Product, List<ICommand>> testCommands = new Dictionary<Product, List<ICommand>>();
        // private List<string> listPrintCmds = new List<string>();
        private readonly object lockObj = new object();
        private HashSet<string> listPrintCmds = new HashSet<string>();


        /// <summary>
        /// 对象类型标识
        /// </summary>
        public override string ObjectClass
        {
            get { return "测试"; }
        }

        /// <summary>
        /// 指示此对象是否是一个容器
        /// </summary>
        public override bool IsContainner
        {
            get { return false; }
        }

        private string testCommandName = string.Empty;
        /// <summary>
        /// 获取或设置测试命令的名称
        /// </summary>
        [MyDisplayName("测试命令名称"), MyCategory("测试对象"), Description("请在测试系统配置中查看")]
        [Browsable(false)]
        public string TestCommandName
        {
            get { return testCommandName; }
            set { testCommandName = value; }
        }

        private CommandType commandType = CommandType.四焦段模式;
        [MyDisplayName("测试协议"), MyCategory("测试对象")]
        [Browsable(false)]
        public CommandType CommandType
        {
            get { return commandType; }
            set { commandType = value; }
        }



        private YesNo useBarcode = false;
        [MyDisplayName("是否附加二维码测试"), MyCategory("测试对象"), Description("显示在自动页面的状态标签")]
        public YesNo UseBarcode
        {
            get { return useBarcode; }
            set { useBarcode = value; }
        }
        private YesNo useresultcode = false;
        [MyDisplayName("是否附加结果代码"), MyCategory("测试对象"), Description("显示在自动页面的状态标签")]
        public YesNo UseResultcode
        {
            get { return useresultcode; }
            set { useresultcode = value; }
        }

        private string lastErrorcode;
        private string noteNgcode;
        private string ResultCode = "";

        /// <summary>
        /// 复制对象成员
        /// </summary>
        /// <param name="dest"></param>
        protected override void CloneMembers(YungkuSystem.Script.Core.Base dest)
        {
            base.CloneMembers(dest);
            ActionTest at = dest as ActionTest;
            at.testCommandName = this.testCommandName;

        }
        private DeviceCommand testCommand = null;
        [MyDisplayName("测试命令配置"), MyCategory("测试对象")]
        [XmlIgnore]
        public DeviceCommand TestCommand
        {
            get { return testCommand; }
            set
            {
                if (testCommand != value)
                {
                    if (testCommand != null)
                    {
                        testCommand.Cmd.OnCmdChange -= Cmd_OnCmdChange;
                    }
                    testCommand = value;
                    if (testCommand != null)
                    {
                        CmdPath = testCommand.GetFullPath();
                        testCommand.Cmd.OnCmdChange += Cmd_OnCmdChange;
                    }
                    else
                    {
                        CmdPath = "";
                    }
                }
            }
        }
        /// <summary>
        /// 子命令更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_OnCmdChange(object sender, ICommand e)
        {
            if (testCommand == null)
            {
                CmdPath = "";
            }
            else
            {

                CmdPath = testCommand.GetFullPath();
            }
        }

        private string cmdPath = string.Empty;
        [MyDisplayName("测试命令配置路径"), MyCategory("测试对象")]
        [Browsable(false)]
        public string CmdPath
        {
            get { return cmdPath; }
            set { cmdPath = value; }
        }
        public override void Binding()
        {
            base.Binding();
            TestCommand = this.Owner.Devices.Root.GetBaseByPath(cmdPath, typeof(DeviceCommand)) as DeviceCommand;
            if (testCommand != null)
            {
                testCommandName = testCommand.Cmd.Name;
                commandType = (CommandType)Enum.Parse(typeof(CommandType), testCommand.CmdType);
            }
            else
            {
                testCommandName = "";
                CmdPath = "";
            }
        }

        ///// <summary>
        ///// 执行
        ///// </summary>
        //protected override void Execute()
        //{
        //    base.Execute();

        //    //ValidHardware();
        //    if (!CurrentHeadObject.HasModudeState)
        //        return;
        //    if (!HasPass && !MustExecute)
        //        return;
        //    if (IsEmpty)
        //        return;
        //    try
        //    {
        //        StartTest();
        //        listPrintCmds.Clear();
        //        while (!WaitTestDone())
        //        {
        //            Thread.Sleep(100);
        //        }             
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteInfo(ex.Message, true);
        //    }
        //}
        /// <summary>
        /// 检查是否有后续的强制执行动作
        /// </summary>
        private bool HasForceExecuteNextAction()
        {
            try
            {
                var nextActionProperty = this.GetType().BaseType
                    .GetProperty("NextAction",
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                if (nextActionProperty == null)
                    return false;

                ActionBase next = nextActionProperty.GetValue(this) as ActionBase;

                while (next != null)
                {
                    var mustExecuteProperty = next.GetType()
                        .GetProperty("MustExecute",
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.Instance);

                    if (mustExecuteProperty != null)
                    {
                        var mustExecuteValue = mustExecuteProperty.GetValue(next);
                        if (mustExecuteValue != null)
                        {
                            string valueStr = mustExecuteValue.ToString();
                            if (valueStr == "Yes" || valueStr == "True" || valueStr == "1")
                            {
                                WriteRecord($"检测到后续强制执行动作: {next.Name}");
                                return true;
                            }
                        }
                    }

                    next = nextActionProperty.GetValue(next) as ActionBase;
                }
            }
            catch (Exception ex)
            {
                WriteRecord($"检查后续强制执行动作异常: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 执行
        /// </summary>
        protected override void Execute()
        {
            base.Execute();

            Station st = Station.Station;
            int stationIndex = st.Turntable.Stations.IndexOf(st);
            string stationName = stationIndex == 0 ? "左工位" : "右工位";

            WriteRecord($"========== [{stationName}] ActionCalculator 启动检查 ==========");
            WriteRecord($"动作名称: {Name}");
            WriteRecord($"HasPass: {HasPass}");
            WriteRecord($"MustExecute: {MustExecute}");

            //检查后续是否有强制执行动作
            bool hasForceExecuteNext = HasForceExecuteNextAction();

            if (!HasPass && !MustExecute && !hasForceExecuteNext)
            {
                WriteRecord($"[{stationName}][跳过] 无Pass产品且非强制执行且无后续强制执行动作");
                return;
            }
            else if (!HasPass && (MustExecute || hasForceExecuteNext))
            {
                WriteRecord($"[{stationName}][强制执行模式] 即使所有产品Fail也继续执行");
            }

            WriteRecord($"[{stationName}] 开始执行计算操作");

            // 检查3: 是否为空
            if (IsEmpty)
            {
                WriteRecord($"[{stationName}][跳过] IsEmpty=true");
                return;
            }

            // 检查4: 测试命令是否有效
            if (TestCommand == null || TestCommand.Cmd == null)
            {
                WriteRecord($"[{stationName}] 测试命令未配置");

                // ✅ 如果是强制执行或有后续强制执行，不报警，只记录日志
                if (MustExecute || hasForceExecuteNext)
                {
                    WriteRecord($"[{stationName}][强制执行模式] 测试命令未配置，但继续流程");
                    return;
                }

                OnAlarm($"测试命令未配置: {Name}");
                return;
            }

            WriteRecord($"✓ [{stationName}] 开始执行: {TestCommand.Cmd.Name}");
            WriteRecord($"===============================================");

            try
            {
                StartTest();
                listPrintCmds.Clear();
                while (!WaitTestDone())
                {
                    Thread.Sleep(100);
                }
                WriteRecord($"✓ [{stationName}] 测试完成: {TestCommand.Cmd.Name}");
            }
            catch (Exception ex)
            {
                WriteInfo($"[{stationName}] 异常: {ex.Message}", true);

                // ✅ 必须执行模式或有后续强制执行动作时，异常不停止流程
                if (!MustExecute && !hasForceExecuteNext)
                {
                    throw;
                }
                WriteRecord($"[{stationName}][强制执行模式] 忽略异常，继续执行");
            }
        }
        /// <summary>
        /// 开始测试
        /// </summary>
        private void StartTest()
        {
            //获取当前站位和对应转盘上的测试头
            Station st = Station.Station;
            Head hd = st.Turntable.GetHeadByStation(st);
            //站位或测试头有一个被关闭时都不应该继续执行
            if (!st.Enabled || !hd.Enabled)
                return;

            //获取工位索引，用于界面显示
            int stationIndex = st.Turntable.Stations.IndexOf(st);
            string stationName = stationIndex == 0 ? "左工位" : "右工位";

            //显示测试开始
            WriteRecord(string.Format("[{0}] 开始执行测试", stationName));
            try
            {
                if (stationIndex == 0)
                {
                    TestDisplayHelper.UpdateLeftStation("测试状态", "准备发送命令...");
                }
                else if (stationIndex == 1)
                {
                    TestDisplayHelper.UpdateRightStation("测试状态", "准备发送命令...");
                }
            }
            catch { }

            //遍历测试头中的站位对象
            foreach (Jig jig in hd.TestItems)
            {
                if (jig.Enabled)
                {
                    foreach (Product product in jig.TestItems)
                    {
                        //找到产品在测试头上的索引
                        int pIndex = hd.ProductIndexOf(product);
                        if (product.Enabled)
                        {
                            ProductObject po = product.BindingObject as ProductObject;

                            //显示产品测试开始
                            try
                            {
                                if (stationIndex == 0)
                                {
                                    TestDisplayHelper.UpdateLeftStation(product.Name, "测试中...");
                                }
                                else if (stationIndex == 1)
                                {
                                    TestDisplayHelper.UpdateRightStation(product.Name, "测试中...");
                                }
                            }
                            catch { }

                            foreach (Module module in product.TestItems)
                            {
                                if (module.Enabled)
                                {
                                    try
                                    {
                                        if (CanContinue(product))
                                            Test(product, module);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteInfo(po.Name + "测试时发生异常！");
                                        WriteInfo(ex.ToString());
                                        OnAlarm(ex.Message + ":" + po.Name + "测试时发生异常！");
                                        product.Result = TestResult.Fail;
                                        product.ResultCode = "Error";

                                        // ✅ 显示错误
                                        try
                                        {
                                            if (stationIndex == 0)
                                            {
                                                TestDisplayHelper.UpdateLeftStation(product.Name, "ERROR");
                                            }
                                            else if (stationIndex == 1)
                                            {
                                                TestDisplayHelper.UpdateRightStation(product.Name, "ERROR");
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // ✅ 显示命令发送完成
            try
            {
                if (stationIndex == 0)
                {
                    TestDisplayHelper.UpdateLeftStation("测试状态", "等待测试结果...");
                }
                else if (stationIndex == 1)
                {
                    TestDisplayHelper.UpdateRightStation("测试状态", "等待测试结果...");
                }
            }
            catch { }
        }

        /// <summary>
        /// 判断是否可以继续测试产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool CanContinue(Product product)
        {
            if (product.Result == TestResult.Empty)
                return false;
            if (this.MustExecute)
                return true;
            return product.Result == TestResult.Pass;
        }

        /// <summary>
        /// 等待测试完成
        /// </summary>
        /// <returns></returns>
        private bool WaitTestDone()
        {
            IStatistical statis = MyApp.GetInstance().Statistical;

            Station st = Station.Station;
            int stationIndex = st.Turntable.Stations.IndexOf(st);

            foreach (Product product in testCommands.Keys)
            {
                IModuleGroup client = GetClient(product);
                if (client == null)
                {
                    WriteInfo($"产品 {product.Name} 的客户端为空,跳过");
                    continue;
                }

                foreach (ICommand cmd in testCommands[product])
                {
                    ICommand doneCmd = client.GetDone(cmd, true);
                    if (doneCmd != null)
                    {
                        switch (doneCmd.CmdState)
                        {
                            case CmdState.AlreadySent:
                            case CmdState.Error:
                            case CmdState.Ready:
                                product.Result = TestResult.Error;
                                //必须执行模式下不触发报警
                                if (!MustExecute)
                                {
                                    WriteInfo($"产品 {product.Name} 测试Error，命令状态: {doneCmd.CmdState}", true);
                                }
                                break;

                            case CmdState.Timeout:
                                product.Result = TestResult.Timeout;
                                product.ResultCode = "-1";
                                //必须执行模式下不触发报警
                                if (!MustExecute)
                                {
                                    WriteInfo($"产品 {product.Name} 测试Timeout", true);
                                }
                                break;

                            case CmdState.OK:
                                //保留原有逻辑：必须执行时，已经Fail的产品不改变状态
                                if (MustExecute && product.IsFail)
                                {
                                    WriteRecord($"产品 {product.Name} 已经Fail，必须执行模式下保持Fail状态");
                                }
                                else
                                {
                                    if (statis.PassCodes.Contains(doneCmd.ResultCode))
                                    {
                                        product.Result = TestResult.Pass;
                                    }
                                    else
                                    {
                                        product.Result = TestResult.Fail;
                                        product.ResultCode = doneCmd.ResultCode;
                                    }
                                }
                                break;
                        }

                        // 更新测试结果到界面
                        try
                        {
                            string result = product.Result == TestResult.Pass ? "PASS" :
                                           product.Result == TestResult.Fail ? string.Format("FAIL - {0}", product.ResultCode) :
                                           product.Result.ToString();

                            if (stationIndex == 0)
                            {
                                TestDisplayHelper.UpdateLeftStation(product.Name, result);
                            }
                            else if (stationIndex == 1)
                            {
                                TestDisplayHelper.UpdateRightStation(product.Name, result);
                            }
                        }
                        catch { }

                        string PrintID = string.Format("{0}_{1}", product.Name, doneCmd.Id);
                        if (listPrintCmds.Contains(PrintID) == false)
                        {
                            string strResult = doneCmd.ResultCode;
                            Head hd = Station.Station.Turntable.GetHeadByStation(Station.Station);
                            string msg = string.Format("测试头:{0},治具:{1},模组:{2},收到回复指令{3},指令当前状态:{4},指令运行结果:{5}",
                                             hd.Name, product.Jig.Name, product.Name, doneCmd.Name, doneCmd.CmdState.ToString(), strResult);
                            WriteInfo(msg);
                            listPrintCmds.Add(PrintID);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            testCommands.Clear();
            listPrintCmds.Clear();
            return true;
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private IModuleGroup GetClient(Product product)
        {
            try
            {
                ProductObject obj = (product.BindingObject as ProductObject);
                //IModuleGroup client;
                //client = (product.BindingObject as ProductObject).TestClient;
                //if (client == null)
                //    throw new NullReferenceException("未能获取到测试名为：" + product.Name + "的客户端，请配置！");
                //return client;
                return null;
            }
            catch (Exception ex)
            {
                WriteInfo(ex.Message, true);
                return null;
            }
        }

        private void Test(Product product, Module module)
        {
            IModuleGroup client = GetClient(product);
            ITestSystem testSystem = MyApp.GetInstance().TestSystem;
            if (client == null)
            {
                WriteInfo("客户端为空：");
            }
            client.OnReceiveCompleted += Client_OnReceiveCompleted; ;
            //连接测试程序           
            Connect(client);
            //获取当前模块索引
            int mIndex = product.TestItems.IndexOf(module);
            ProductObject po = product.BindingObject as ProductObject;
            if (mIndex >= 0)
            {
                if (module.Enabled)
                {
                    ICommand cmd = testSystem.CreateCommand(this.TestCommandName, CommandType);
                    if (this.useBarcode)
                    {
                        IParameter param = cmd as IParameter;
                        param.ClearInputParameters();
                        param.AddInputParameter(product.BarCodeString);
                        WriteInfo("添加摄像头二维码参数：" + product.BarCodeString);
                    }
                    else if (useresultcode)
                    {
                        IParameter param = cmd as IParameter;
                        param.ClearInputParameters();
                        param.AddInputParameter((po.Owner as Product).ResultCode.ToString());
                        WriteInfo("添加结果代码：" + (po.Owner as Product).ResultCode.ToString());
                    }
                    client.ClientSend(cmd, mIndex);

                    Head hd = Station.Station.Turntable.GetHeadByStation(Station.Station);
                    string msg = string.Format("测试头:{0},治具:{1},模组:{2},ModuleIndex:{3},发送指令{4}",
                                      hd.Name, product.Jig.Name, product.Name, mIndex.ToString(), cmd.Name);
                    WriteInfo(msg);
                    AddToList(product, cmd);
                }
            }
        }

        private void Client_OnReceiveCompleted(object sender, SocketDatagramReceivedEventArgs<Dictionary<string, string>> e)
        {
            Dictionary<string, string> data = e.Datagram;
            foreach (string name in data.Keys)
            {
                WriteInfo(string.Format("{0}发送消息【{1}】至【{2}】", e.Socket != null ? e.Socket.RemoteEndPoint.ToString() : "", data[name], name));
            }

        }


        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cmd"></param>
        private void AddToList(Product product, ICommand cmd)
        {
            if (!testCommands.ContainsKey(product))
                testCommands.Add(product, new List<ICommand>());
            if (!testCommands[product].Contains(cmd))
                testCommands[product].Add(cmd);
        }

        /// <summary>
        /// 测试程序连接
        /// </summary>
        /// <param name="client"></param>
        private void Connect(IModuleGroup client)
        {
            try
            {
                int count = 0;
                while (client.RealConnectionState != YungkuSystem.Tester.ConnectionState.Connected)
                {
                    client.Uinit();
                    Thread.Sleep(100);
                    client.Init();
                    Thread.Sleep(100);
                    count++;
                    if (count > 3)
                        throw new TimeoutException("测试程序连接超时!");
                }
            }
            catch (Exception ex)
            {
                OnAlarm("测试头与测试程序链接失败：" + ex.ToString());
            }

        }

    }
}
