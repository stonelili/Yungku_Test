using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Yungku.Devices.SmallScreen;
using Yungku.BNU01_V1.Handler.Config;
using Yungku.BNU01_V1.Handler.JsonTcp;
using Yungku.BNU01_V1.Handler.Logic;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using Yungku.BNU01_V1.Handler.Logic.StationAction;
using Yungku.BNU01_V1.Handler.Other;
using YungkuSystem;
using YungkuSystem.AlarmManage;
using YungkuSystem.Controls;
using YungkuSystem.Devices.LaserController;
using YungkuSystem.Globalization;
using YungkuSystem.LightMotion.Intf;
using YungkuSystem.LightMotion.Manage;
using YungkuSystem.Log;
using YungkuSystem.Machine;
using YungkuSystem.Motion;
using YungkuSystem.Motion.Intf;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Script.Actions.DeviceBase;
using YungkuSystem.Script.Core;
using YungkuSystem.Script.Devices;
using YungkuSystem.Script.Page;
using YungkuSystem.Statistical;
using YungkuSystem.SuperDog;
using YungkuSystem.TCMotion.Intf;
using YungkuSystem.Tester;
using YungkuSystem.TestFlow;
using YungkuSystem.Tools;
using YungkuSystem.UAC;

namespace Yungku.BNU01_V1.Handler
{
    internal class MyApp : YSApplication
    {
        public static bool CodeScanner = false;//扫码窗口标志位
        public const int LaserBaudrate = 38400;

        public static string AppName = "BNU01_V1";
        //public static string Version = "Edit:20251016 1400";//上机前代码优化
        //public static string Version = "Edit:20251018 1535";//上机调试
        //public static string Version = "Edit:20251020 0900";//上机调试
        public static string Version = "Edit:20251119 0900";//所有轴增加测试位置2，并增加测试位置2的开关按钮。

        public const bool RunDebugDemo = false;
        public const string TEST_PATH = "TEST_SYSTEM_CONFIG_FILE_1";
        public const string KEY_CONFIG_FILE = "APP_CONFIG";
        public const string ACTION_WATCHER = "ACTION_WATCHER";
        public const string ACTION_TricolorLight = "ACTION_TricolorLight";
        public const string ACTION_TEST = "ACTION_TEST";
        public const string UPCAM_CONFIG_FILE = "UPCAM_CONFIG";
        public const string DOWNCAM_CONFIG_FILE = "DOWNCAM_CONFIG";
        public const int MAX_LOG_FILE_COUNT = 30;

        public static bool LoadAppParams = true;

        public static string AppTest = "";
        private MotionService motionService;
        private TCService tcService;
        private IMotionSystem motionSystem;
        private ITCSystem tcSystem;
        private LightService lightService;
        private ILightSystem lightSystem;
        private IStatistical statistical;
        private TestService testService;
        private ITestSystem testSystem;
        private TrayService trayService;
        private LogicService logic;
        private MessageShow messageShow;
        private UACManager uac;
        private FileLogWriter fileLogWriter;
        private FileLogWriter testFileLogWriter;
        private FileAlarmCsvWriter fileAlarmCsvWriter;
        private FormScreenSimulator formScreenDisplayer;
        public static SiteShareData ShareData = new SiteShareData();
        private static DogVerify superDog;
        public static bool isStart;

        public static bool Switch_page;

        private static TricolorLight tricolorLight;
        /// <summary>
        /// 三色灯控制
        /// </summary>
        public static TricolorLight TricolorLight
        {
            get { return MyApp.tricolorLight; }
        }

        //private static CamerasManager camerasManager = new CamerasManager();
        ///// <summary>
        ///// 相机控制器
        ///// </summary>
        //public static CamerasManager CamerasManager
        //{
        //    get { return MyApp.camerasManager; }
        //}

        /// <summary>
        /// 调焦相机
        /// </summary>
        //public static UpCamera UpCamera { get; set; } = new UpCamera();

        ///// <summary>
        ///// 点胶相机
        ///// </summary>
        //public static DownCamera DownCamera { get; set; } = new DownCamera();

        /// <summary>
        /// 项目文件
        /// </summary>
        private static AppParams appParams = new AppParams();

        /// <summary>
        /// 硬件实例化
        /// </summary>
        public static Hardware HW = new Hardware();

        private static AllSameNgCount sameNgCount = new AllSameNgCount();

        /// <summary>
        /// 获取所有相同NG出现次数
        /// </summary>
        public static AllSameNgCount SameNgCount
        {
            get { return MyApp.sameNgCount; }
        }

        private static ActionMain mainAction;

        public static ActionMain MainAction
        {
            get { return mainAction; }
            set { mainAction = value; }
        }

        public static void StartTestLogic()
        {
            GetInstance().AlarmPublisher.ClearAlarms();
            ActionExecuter exec = MyApp.GetInstance().Logic.DefaultExecuter;
            if (exec.Actions.Count == 0)
            {
                LogicObject testAction = new ActionMain();
                if (testAction.CheckAddAction(exec))
                {
                    GetInstance().Logic.RunAction(testAction);
                }
            }
            exec.Start();
        }


        /// <summary>
        /// 运行数据
        /// </summary>
        /// <returns></returns>
        public bool RecordRunData()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                if (!System.IO.Directory.Exists(".\\CSVLog\\"))
                    System.IO.Directory.CreateDirectory(".\\CSVLog\\");
                string fileName = DateTime.Now.ToString("yyyy-MM-dd HHmm") + ".csv";
                //System.IO.FileStream fs = null;
                // System.IO.StreamWriter sw;
                if (!System.IO.File.Exists(".\\CSVLog\\" + fileName))
                {
                    fs = System.IO.File.Create(".\\CSVLog\\" + fileName);
                    sw = new System.IO.StreamWriter(fs, Encoding.Unicode);
                    //投入数 良品数 良率 不良数 产能 运行时间 空闲时间
                    //不良分布治具名称 项目 数量 数量/不良总数 数量/投入数
                    string H = "";
                    H += "投入数" + "\t";
                    H += "良品数" + "\t";
                    H += "良率" + "\t";
                    H += "不良数" + "\t";
                    H += "产能" + "\t";
                    H += "运行时间" + "\t";
                    H += "空闲时间" + "\t";

                    H += "不良分布治具名称" + "\t";
                    H += "项目" + "\t";
                    H += "数量" + "\t";
                    H += "数量/不良总数" + "\t";
                    H += "数量/投入数" + "\t";
                    sw.WriteLine(H);
                }
                else
                {
                    fs = new System.IO.FileStream(".\\CSVLog\\" + fileName, System.IO.FileMode.Append);
                    sw = new System.IO.StreamWriter(fs, Encoding.Unicode);
                }
                int total = Statistical.GetFail() + Statistical.GetPass();

                string C = "";
                //写入数据:投入数 良品数 良率 不良数 产能 运行时间 空闲时间
                C += total.ToString() + " PCS" + "\t";
                C += Statistical.GetPass().ToString() + " PCS" + "\t";
                if (total != 0)
                    C += (Statistical.GetPass() * 100.0 / total).ToString("0.0") + "%" + "\t";
                else
                    C += "---" + "\t";
                C += Statistical.GetFail().ToString() + " PCS" + "\t";

                //产能
                C += Statistical.GetUPH().ToString("0.0") + " U/H" + "\t";

                int seconds = Statistical.GetRunTimes();
                C += (seconds / 3600).ToString() + "小时"
                    + (seconds % 3600 / 60).ToString() + "分" + (seconds % 60).ToString() + "秒" + "\t";
                seconds = Statistical.GetIdleTimes();
                C += (seconds / 3600).ToString() + "小时"
                    + (seconds % 3600 / 60).ToString() + "分" + (seconds % 60).ToString() + "秒" + "\t";
                sw.WriteLine(C);

                //写入数据:不良分布治具名称 项目 数量 数量/不良总数 数量/投入数
                //for (int i = 0; i < 32; i++)
                foreach (string itemName in Statistical.GetItemNames())
                {
                    //string itemName = "Product" + i.ToString();
                    int passCnt = Statistical.GetPassByJig(itemName);
                    int failCnt = Statistical.GetFailByItemName(itemName);
                    int jigTotal = passCnt + failCnt;
                    if (jigTotal <= 0)
                        continue;

                    Dictionary<string, int> codes = new Dictionary<string, int>();
                    Statistical.GetFailCodeDetailByItemName(itemName, codes);
                    int touru = Statistical.GetPass() + Statistical.GetFail();
                    total = 0;
                    foreach (string code in codes.Keys)
                    {
                        total += codes[code];
                    }
                    if (total != 0)
                    {
                        foreach (string code in codes.Keys)
                        {
                            C = "";
                            C += "" + "\t";
                            C += "" + "\t";
                            C += "" + "\t";
                            C += "" + "\t";
                            C += "" + "\t";
                            C += "" + "\t";
                            C += "" + "\t";

                            C += itemName + "\t";
                            C += Statistical.GetCodeName(code) + "\t";
                            C += codes[code].ToString() + "\t";
                            C += (codes[code] * 100.0 / total).ToString("0.00") + "%" + "\t";
                            C += (codes[code] * 100.0 / touru).ToString("0.00") + "%" + "\t";

                            sw.WriteLine(C);
                        }
                    }
                }
                //*/
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MyApp.GetInstance().Logger.WriteError($"记录运行数据失败: {ex.Message}");
                return false;
            }
            finally
            {
                sw?.Close();
                fs?.Close();
            }
            return true;
        }

        public static AppParams AppParams
        {
            get { return MyApp.appParams; }
        }

        /// <summary>
        /// 加密狗接口
        /// </summary>
        public static DogVerify SuperDog
        {
            get { return superDog; }
        }

        public FormScreenSimulator FormScreenDisplayer
        {
            get { return formScreenDisplayer; }
        }

        private Machine machine;

        private static HomeHelper homeHelper;

        public UACManager UAC
        {
            get { return uac; }
        }

        public static HomeHelper HomeHelper
        {
            get { return MyApp.homeHelper; }
        }

        public MotionService MotionService
        {
            get { return motionService; }
        }

        public TCService TcService
        {
            get { return tcService; }
        }

        public IMotionSystem MotionSystem
        {
            get { return motionSystem; }
        }

        public ITCSystem TCSystem
        {
            get { return tcSystem; }
        }

        public LightService LightService
        {
            get { return lightService; }
        }

        public ILightSystem LightSystem
        {
            get { return lightSystem; }
        }

        public IStatistical Statistical
        {
            get { return statistical; }
        }

        public TestService TestService
        {
            get { return testService; }
        }

        public ITestSystem TestSystem
        {
            get { return testSystem; }
        }

        public LogicService Logic
        {
            get { return logic; }
        }

        private static bool needReset = true;

        public static bool NeedReset
        {
            get { return MyApp.needReset; }
            set { MyApp.needReset = value; }
        }

        private static AppConfig config;

        public static AppConfig Config
        {
            get { return MyApp.config; }
        }

        public MessageShow MessageShow
        {
            get { return messageShow; }
        }

        public Machine Machine
        {
            get { return machine; }
        }


        private AlarmPublisher alarmPublisher;

        public AlarmPublisher AlarmPublisher
        {
            get { return alarmPublisher; }
        }

        private LogPublisher testLogger;
        private LogPublisher logger;

        public LogPublisher Logger
        {
            get { return logger; }
        }

        public string CurrentCMD { get; set; } = string.Empty;
        public ActionExecuter ActionWatcherExe = null;
        public ActionExecuter ActionTricolorLightExe = null;

        public IModuleGroup TestSoftwareClient = null;

        internal void Initialize(FormWelcom fw)
        {
            SlmRuntime.Debug = true;//加密远程调试

            fw.MachineName = "   ";
            this.WelcomForm = fw;
            fw.MachineName = AppName;
            fw.Version = Version;

            logger = (LogPublisher)FW.LogService.CreateLogPublisher(Framework.KEY);
            testLogger = (LogPublisher)FW.LogService.CreateLogPublisher(Framework.TEST);
            //用户初始化开始
            Framework.FileSystem.FileStore = new FileStore();

            //建立文件Log记录器
            fileLogWriter = new FileLogWriter(MAX_LOG_FILE_COUNT);
            fileLogWriter.PathHead = @"D:\YUNGKU\SYSTEM_LogData";
            logger.AddLogWriter(fileLogWriter);

            testFileLogWriter = new FileLogWriter(MAX_LOG_FILE_COUNT);
            testFileLogWriter.PathHead = @"D:\YUNGKU\Test_LogData";
            testLogger.AddLogWriter(testFileLogWriter);

            alarmPublisher = (AlarmPublisher)FW.AlarmService.CreateAlarmPublisher(Framework.KEY);
            fileAlarmCsvWriter = new FileAlarmCsvWriter();
            alarmPublisher.AddAlarmWriter(fileAlarmCsvWriter);

            homeHelper = new YungkuSystem.Motion.HomeHelper();//回原点实例

            try
            {
                logger.Write(G.Text("选择机种文件..."));
                fw.UpdateInfo(0.05, "初始化选择机种文件...");

                if (OpenProject())
                {
                    logger.WriteRecord(G.Text("初始化选择机种文件完成"));
                }
                else
                {
                    LoadAppParams = false;
                    logger.WriteError(G.Text("初始化选择机种文件失败"));
                }
            }
            catch (Exception ex)
            {
                logger.WriteError(G.Text("初始化选择机种文件失败:\r\n" + ex.Message));
                alarmPublisher.Write(G.Text("初始化选择机种文件失败:\r\n" + ex.Message));
            }

            //加载应用程序配置
            try
            {
                logger.Write(G.Text("加载应用程序配置..."));
                fw.UpdateInfo(0.15, "加载应用程序配置...");
                config = new AppConfig();
                XmlSerial<AppConfig>.Load(Framework.GetSystemPath(KEY_CONFIG_FILE), ref config);
                logger.WriteRecord(G.Text("加载应用程序配置完成"));
                //加载机器类型
                //MachineModeSelect();
                logger.WriteRecord(G.Text("加载机器配置完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError(G.Text("加载应用程序配置失败:\r\n" + ex.Message));
                alarmPublisher.Write(G.Text("加载应用程序配置失败:\r\n" + ex.Message));
            }

            RunOnUI(new System.Action(() =>
            {
                superDog = new DogVerify();
                //if (machineMode == MachineModes.CVT检测机)
                //{
                superDog.InitSuperDog(35482);
                //}
                //else
                //    superDog.InitSuperDog(35429);
            }));



            #region
            ////初始化温度控制服务
            //try
            //{
            //    logger.Write("初始化温度控制服务...");
            //    fw.UpdateInfo(0.125, "初始化温度控制服务...");
            //    tcService = (TCService)Framework.GetSystemService(Context.SERVICE_TC);
            //    tcSystem = tcService.CreateMotionSystem(TCService.KEY_CONFIG_FILE);

            //    tcService.Load();
            //    //将硬件对象映射到Hardware中
            //    MyApp.HW.Init(tcSystem);
            //    tcService.Open();
            //    logger.WriteInfo("温度控制服务初始化完成");
            //}
            //catch (Exception ex)
            //{
            //    logger.WriteError("初始化温度控制系统失败:\r\n" + ex.Message);
            //    alarmPublisher.Write("初始化温度控制系统失败:\r\n" + ex.Message);
            //}
            #endregion

            //初始化数据统计服务
            try
            {
                logger.Write(G.Text("初始化数据统计服务..."));
                fw.UpdateInfo(0.2, "初始化数据统计服务...");
                StatisticalService statisticalService = (StatisticalService)Framework.GetSystemService(Context.SERVICE_STATISTICAL);
                statistical = statisticalService.CreateStatisticalSystem(StatisticalService.KEY_CONFIG_FILE);
                statisticalService.Load();
                foreach (CodeConfig code in config.FunctionSwitch.PassCodes)
                {
                    if (!statistical.PassCodes.Contains(code.CodeStr))
                        statistical.PassCodes.Add(code.CodeStr);
                }
                foreach (CodeConfig code in config.FunctionSwitch.IgnoreCodes)
                {
                    if (!statistical.IgnoreCodes.Contains(code.CodeStr))
                        statistical.IgnoreCodes.Add(code.CodeStr);
                }
                statistical.IgnoreCodes.Add("");
                statistical.IgnoreCodes.Add(" ");
                logger.WriteRecord(G.Text("初始化数据统计服务完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError(G.Text("初始化数据统计服务失败:\r\n" + ex.Message));
                alarmPublisher.Write(G.Text("初始化数据统计服务失败:\r\n" + ex.Message));
            }

            //初始化测试服务
            try
            {
                logger.Write(G.Text("初始化测试服务..."));
                fw.UpdateInfo(0.25, "初始化测试服务...");
                testService = (TestService)Framework.GetSystemService(Context.SERVICE_TEST_SYSTEM);
                testSystem = testService.CreateTestSystem(TestService.KEY_CONFIG_FILE);
                testSystem.JsonCommands.Add("YYS调焦", new YYSCommand());
                testSystem.JsonCommands.Add("测试通信", new OfilmCommand());
                testService.Load();
                testSystem.OnLogMessage += TestSystem_OnLogMessage;
                testSystem.OnCmdChange += TestSystem_OnCmdChange; ;
                logger.WriteRecord(G.Text("初始化测试服务完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError(G.Text("初始化测试服务失败:\r\n" + ex.Message));
                alarmPublisher.Write(G.Text("初始化测试服务失败:\r\n" + ex.Message));
            }

            #region
            //try
            //{
            //    logger.Write("初始化料盘服务...");
            //    fw.UpdateInfo(0.3, "初始化料盘服务...");
            //    trayService = (TrayService)Framework.GetSystemService(Context.SERVICE_TRAY);
            //    workTray = trayService.CreateTrayPublisher(TrayService.WORK_TRAY);
            //    failTray = trayService.CreateTrayPublisher(TrayService.FAIL_TRAY);
            //    trayService.Load();
            //    workTray.GridCollection.BuildTray(4, 13);
            //    workTray.GridCollection.LayOutType = LayOutType.Lengthways;
            //    failTray.GridCollection.BuildTray(13, 4);
            //    logger.WriteInfo("初始化料盘服务完成");
            //}
            //catch (Exception ex)
            //{
            //    logger.WriteError("初始化料盘服务失败:\r\n" + ex.Message);
            //    alarmPublisher.Write("初始化料盘服务失败:\r\n" + ex.Message);
            //}
            #endregion

            //初始化逻辑控制器服务
            try
            {
                logger.Write(G.Text("初始化逻辑控制器服务..."));
                fw.UpdateInfo(0.35, "初始化逻辑控制器服务...");
                logic = (LogicService)Framework.GetSystemService(Context.SERVICE_LOGIC);
                MyApp.GetInstance().ActionWatcherExe = MyApp.GetInstance().Logic.CreateExecuter(MyApp.ACTION_WATCHER);
                MyApp.GetInstance().ActionTricolorLightExe = MyApp.GetInstance().Logic.CreateExecuter(MyApp.ACTION_TricolorLight);

                MyApp.GetInstance().ActionWatcherExe.Actions.AutoRemoveOnFinished = true;
                MyApp.GetInstance().ActionTricolorLightExe.Actions.AutoRemoveOnFinished = true;

                if (tricolorLight != null)
                    logic.RunAction(tricolorLight, MyApp.ACTION_TricolorLight);

                logger.WriteRecord(G.Text("初始化逻辑控制器服务完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError(G.Text("初始化逻辑控制器服务失败:\r\n" + ex.Message));
                alarmPublisher.Write(G.Text("初始化逻辑控制器服务失败:\r\n" + ex.Message));
            }

            //加载用户访问控制管理器
            try
            {
                logger.Write(G.Text("加载用户访问控制管理器..."));
                fw.UpdateInfo(0.4, "加载用户访问控制管理器...");
                uac = new UACManager();
                uac.Load();
                Permissions.LoadToUAC();
                logger.WriteRecord(G.Text("加载用户访问控制管理器完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError(G.Text("初始化逻辑控制器服务失败:\r\n" + ex.Message));
                alarmPublisher.Write(G.Text("初始化逻辑控制器服务失败:\r\n" + ex.Message));
            }

            //加载设备测试配置
            try
            {
                logger.Write(G.Text("加载设备测试配置..."));
                fw.UpdateInfo(0.45, "加载设备测试配置...");
                RunOnUI(new System.Action(() =>//下面的操作涉及到界面的创建
                {
                    machine = Machine.Load();
                    //检查配置是否合适,修改治具数量涉及到硬件传感器，所以需要修改程序
                    if (!machine.Check(1, 2, 1, 1, 1, 1, 0, 0, 0))
                    {
                        machine.Build(1, 2, 1, 1, 1, 1, 0, 0, 0);
                        SetMachineProperties();
                    }

                    // ✅ 强制启用所有Station（确保左右工位都启用）
                    foreach (Turntable tt in machine.TestItems)
                    {
                        logger.WriteRecord(string.Format("Turntable: {0}, Stations数量: {1}", tt.Name, tt.Stations.Count));

                        for (int i = 0; i < tt.Stations.Count; i++)
                        {
                            Station st = tt.Stations[i];
                            bool wasEnabled = st.Enabled;
                            st.Enabled = true;  // 强制启用

                            string stationName = i == 0 ? "左工位" : "右工位";
                            logger.WriteRecord(string.Format("{0} (Station{1}): Enabled={2} -> {3}",
                     stationName, i, wasEnabled, st.Enabled));
                        }
                    }

                    //绑定上自定义对象
                    MachineObject.BindToMachine(machine);
                    TurntableObject.BindToMachine(machine);
                    HeadObject.BindToMachine(machine);
                    JigObject.BindToMachine(machine);
                    ProductObject.BindToMachine(machine);
                    //ManipulatorObject.BindToMachine(machine);
                    //NozzleGroupObject.BindToMachine(machine);
                    //NozzleObject.BindToMachine(machine);
                    try
                    {
                        RegScriptObjects();
                    }
                    catch (Exception ex)
                    {
                        logger.WriteError(G.Text("加载设备测试配置失败:\r\n" + ex.Message));
                        alarmPublisher.Write(G.Text("加载设备测试配置失败:\r\n" + ex.Message));
                    }
                    LoadDevice();
                    LoadScript();
                }));
                logger.WriteRecord(G.Text("加载设备测试配置完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError("加载设备测试配置失败:\r\n" + ex.Message);
                alarmPublisher.Write("加载设备测试配置失败:\r\n" + ex.Message);
            }

            //加载信息显示服务
            try
            {
                logger.Write(G.Text("加载信息显示服务..."));
                fw.UpdateInfo(0.5, "加载信息显示服务...");
                messageShow = (MessageShow)Framework.GetSystemService(Context.SERVICE_MESSAGE_SHOW);
                logger.WriteRecord(G.Text("加载信息显示服务完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError("加载信息显示服务失败:\r\n" + ex.Message);
                alarmPublisher.Write("加载信息显示服务失败:\r\n" + ex.Message);
            }

            #region
            //初始化小屏幕
            //try
            //{
            //    logger.Write("初始化小屏幕...");
            //    fw.UpdateInfo(0.55, "初始化小屏幕...");
            //    RunOnUI(new System.Action(() =>
            //    {
            //        formScreenDisplayer = new FormScreenSimulator();
            //        LedScreen screen = this.FormScreenDisplayer.Screen;
            //        if (!screen.InitPort((int)Config.General.SmallScreenPort))
            //        {
            //            logger.WriteError("小屏幕初始化失败，没有端口" + Config.General.SmallScreenPort.ToString());
            //        }
            //        FormScreenDisplayer.Show();

            //        int jigCountPerHead = Machine.TestItems[0].TestItems[0].TestItems.Count;
            //        int productCountPerJig = Machine.TestItems[0].TestItems[0].TestItems[0].TestItems.Count;
            //        int totalProductCount = jigCountPerHead * productCountPerJig;
            //        screen.InitDisplay(480, 320, jigCountPerHead, productCountPerJig);
            //        //这个组件依赖界面的结构
            //        Application.DoEvents();
            //        for (int i = 0; i < totalProductCount; i++)
            //        {
            //            screen.SetResultState(i, ScreenResultStatus.UNKNOW);
            //        }
            //        //这个组件依赖界面的结构
            //        Application.DoEvents();
            //        screen.RefreshDisplay();
            //        FormScreenDisplayer.Hide();
            //    }));
            //    logger.WriteInfo("初始化小屏幕完成");
            //}
            //catch (Exception ex)
            //{
            //    logger.WriteError("初始化小屏幕失败:\r\n" + ex.Message);
            //    alarmPublisher.Write("初始化小屏幕失败:\r\n" + ex.Message);
            //}
            #endregion

            #region
            //初始化光源
            //try
            //{
            //    logger.Write("初始化光源控制服务...");
            //    fw.UpdateInfo(0.6, "初始化光源控制服务...");
            //    lightService = (LightService)Framework.GetSystemService(Context.SERVICE_LIGHT);
            //    lightSystem = lightService.CreateLightSystem(LightService.KEY_CONFIG_FILE);
            //    lightService.Load();

            //    //将硬件对象映射到Hardware中
            //    MyApp.HW.Init(lightSystem);
            //    lightSystem.Open();
            //    lightSystem.ReLoadLightMap += Manager_ReLoadLightMap;
            //    logger.WriteInfo("光源控制服务初始化完成");
            //}
            //catch (Exception ex)
            //{
            //    logger.WriteError("初始化光源控制服务失败:\r\n" + ex.Message);
            //    alarmPublisher.Write("初始化光源控制服务失败:\r\n" + ex.Message);
            //}
            #endregion

            //初始化结果弹窗界面
            try
            {
                logger.Write(G.Text("初始化结果弹窗界面..."));
                fw.UpdateInfo(0.65, "初始化结果弹窗界面...");
                RunOnUI(new System.Action(() =>
                {
                    FormResult.CreateForms(machine.TestItems[0].TestItems[0].TestItems[0].TestItems.Count, config.General.FormWidth, config.General.formHeight);
                }));
                logger.WriteRecord(G.Text("初始化结果弹窗界面完成"));
            }
            catch (Exception ex)
            {
                logger.WriteError(G.Text("初始化结果弹窗界面失败:\r\n" + ex.Message));
                alarmPublisher.Write(G.Text("初始化结果弹窗界面失败:\r\n" + ex.Message));
            }

            #region
            ////初始化镭射
            //try
            //{
            //    logger.Write("初始化镭射...");
            //    fw.UpdateInfo(0.65, "初始化镭射...");
            //    RunOnUI(new System.Action(() =>
            //    {
            //        LaserInit(LaserControlFocus);
            //    }));
            //    logger.WriteInfo("初始化镭射完成");
            //}
            //catch (Exception ex)
            //{
            //    logger.WriteError("初始化光源失败:\r\n" + ex.Message);
            //}
            #endregion

            //用户初始化结束
            logger.WriteRecord(G.Text("用户初始化完成"));
            fw.UpdateInfo(1, "用户初始化完成");
            Thread.Sleep(1000);
            StartMainForm(fw);
        }

        public static bool LaserInit(LaserReader laser)
        {
            //laser.Port = MyApp.Config.MachineTestSetting.FousLaserPort;
            laser.Baudrate = LaserBaudrate;
            return laser.Init();
        }

        public void RunTricolorLightAction()
        {
            if (MyApp.GetInstance().ActionTricolorLightExe.Actions.Count == 0)
            {
                if (MyApp.tricolorLight != null)
                    MyApp.GetInstance().Logic.RunAction(MyApp.tricolorLight, MyApp.ACTION_TricolorLight);
            }
            if (!MyApp.GetInstance().ActionTricolorLightExe.IsRunning)
            {
                MyApp.GetInstance().ActionTricolorLightExe.Start();
            }
        }

        private static bool OpenProject()
        {
            try
            {
                appParams = AppParams.LoadAppParams();
            }
            catch
            {
                AppParams.SaveProject(appParams);
                return OpenProject();
            }
            if (appParams.ProjectFileIsValid)
            {
                (Framework.FileSystem.FileStore as FileStore).CurrentProjectPath = System.IO.Path.GetDirectoryName(AppParams.CurrentProjectFile) + "\\";
                return true;
            }
            return false;
        }

        public static void OpenConfig()
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "XML files (*.XML)|*.XML";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string strFilePathName = sfd.FileName.ToString();
                AppParams.CurrentProjectFile = strFilePathName;
                AppParams.SaveProject(appParams);
            }
        }



        //public bool GetGratingSensor()
        //{
        //    if (!Config.FunctionSwitch.GratingStateCheck)//|| MyApp.HW.IO_In_Cylinder_FB_In.Value
        //        return true;
        //    return HW.GratingSensor.Value;
        //}

        private void TestSystem_OnLogMessage(object sender, string e)
        {
            if (testLogger != null)
                testLogger.WriteInfo(e);
            logger.WriteInfo(e);
        }

        //private void Manager_ReLoadLightMap(object sender, EventArgs e)
        //{
        //    if (!lightSystem.IsInitialized)
        //        lightSystem.Open();
        //    HW.CreateLightMap(lightSystem);
        //}

        public static void SetName(string x, out int y)
        {
            y = -1;
        }

        private static void RegScriptObjects()
        {
            int i = 0;
            Assembly module = Assembly.GetExecutingAssembly();
            foreach (Type t in module.GetTypes())
            {
                if (t.IsClass && !t.IsAbstract && t.IsPublic
                  && typeof(ActionObject).IsAssignableFrom(t))
                {
                    ActionObject ms = Activator.CreateInstance(t) as ActionObject;
                    RegTab.RegAction(new TypeTags(ms.ObjectClass, AppName), t, 62 + i);
                    i++;
                }
            }
        }

        /// <summary>
        /// 测试系统命令操作事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestSystem_OnCmdChange(object sender, ChangeEvent e)
        {
            if (e != null)
            {
                foreach (Turntable tt in machine.TestItems)//turntables
                {
                    foreach (Station st in tt.Stations)
                    {
                        try
                        {
                            DeviceCategory BaseRoot;
                            DeviceCategory Root;
                            DeviceCommand deviceCmd;
                            BaseRoot = st.Executor.Devices.Root.GetChildDeviceByName("测试命令") as DeviceCategory;
                            if (BaseRoot != null && e.CmdType != "")
                            {
                                Root = BaseRoot.GetChildDeviceByName(e.CmdType) as DeviceCategory;
                                switch (e.ChangeType)
                                {
                                    case ChangeType.Delete:
                                        if (Root != null && e.Cmd == null)
                                        {
                                            BaseRoot.Childs.Remove(Root);
                                        }
                                        else if (Root != null && e.Cmd != null)
                                        {
                                            deviceCmd = Root.GetChildDeviceByName(e.Cmd.Name) as DeviceCommand;
                                            if (deviceCmd != null)
                                            {
                                                Root.Childs.Remove(deviceCmd);
                                            }
                                        }
                                        st.Executor.Actions.Root.AllBinding();
                                        break;

                                    case ChangeType.Add:
                                        if (Root == null)
                                        {
                                            Root = new DeviceCategory(e.CmdType);
                                            BaseRoot.Childs.Add(Root);
                                        }
                                        if (e.Cmd != null)
                                        {
                                            deviceCmd = new DeviceCommand(e.CmdType, e.Cmd);
                                            RegTab.RegTypeToImageMap(deviceCmd.GetType(), 64);
                                            Root.Childs.Add(deviceCmd);
                                        }
                                        break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void LoadDevice()
        {
            foreach (Turntable tt in machine.TestItems)//turntables
            {
                foreach (Station st in tt.Stations)
                {
                    try
                    {

                        DeviceCategory root = new DeviceCategory("测试命令");
                        st.Executor.Devices.Root.Childs.Add(root);
                        Dictionary<string, ICommandCollection> commands = testSystem.Commands;
                        foreach (string name in commands.Keys)
                        {
                            DeviceCategory rootChild = new DeviceCategory(name);
                            root.Childs.Add(rootChild);
                            List<ICommand> allCmds = commands[name].AllCommands;
                            foreach (ICommand cmd in allCmds)
                            {
                                DeviceCommand deviceCmd = new DeviceCommand(name, cmd);
                                RegTab.RegTypeToImageMap(deviceCmd.GetType(), 64);
                                rootChild.Childs.Add(deviceCmd);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void LoadScript()
        {
            //为每个站位设置配置文件KEY
            foreach (Turntable tt in machine.TestItems)//turntables
            {
                foreach (Station st in tt.Stations)
                {
                    try
                    {
                        st.Executor.Editor.MenuStripVisible = false;
                        st.Executor.Editor.ToolStripVisible = true;
                        st.Executor.Editor.DeviceCanEdit = true;
                        st.Executor.IO.FileName = Framework.GetSystemPath(st.Properties["ConfigFile"]);
                        st.Executor.IO.Reload();
                        st.Executor.AlarmListChangeEventHandler += Executor_AlarmListChangeEventHandler;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void Executor_AlarmListChangeEventHandler(object sender, Alarm e)
        {
            if (alarmPublisher != null)
            {
                alarmPublisher.Write(e);
            }
        }

        /// <summary>
        /// 设置设备属性
        /// </summary>
        private void SetMachineProperties()
        {
            int i = 0;
            int j = 0;
            //为每个站位设置配置文件KEY
            foreach (Turntable tt in machine.TestItems)//turntables
            {
                j = 0;
                foreach (Station st in tt.Stations)
                {
                    st.Properties["ConfigFile"] = "Turntable" + i.ToString() + "_" + "Station" + j.ToString() + "_ConfigFile";
                    j++;
                }
                i++;
            }
        }

        /// <summary>
        /// 在UI上运行
        /// </summary>
        /// <param name="action"></param>
        public static void RunOnUI(System.Action action)
        {
            MyApp.RunOnUIThread(action);
        }

        #region 默认操作

        private void StartMainForm(FormWelcom fw)
        {
            this.MainForm = fw.ToMainForm(typeof(FormMain));
            this.MainForm.FormClosed += mainForm_FormClosed;
        }

        private void mainForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Uninitialize();
        }

        /// <summary>
        /// 未初始化
        /// </summary>
        internal void Uninitialize()
        {
            WelcomForm.Close();
        }

        private static MyApp instance = null;

        internal static MyApp GetInstance()
        {
            if (instance == null)
                instance = new MyApp();
            return instance;
        }

        #endregion 默认操作

        /// <summary>
        /// 获取最接近的度数
        /// </summary>
        /// <param name="curAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public static double MathBestAngle(double curAngle, double endAngle)
        {
            curAngle = curAngle % 360;
            if ((endAngle - curAngle) > 180)
            {
                return curAngle + 360;
            }
            else if ((endAngle - curAngle) < -180)
            {
                return curAngle - 360;
            }
            return curAngle;
        }

        public static void HomeStart(AxisMap axis)
        {
            TimeoutWatch Watcher = new TimeoutWatch();
            if (axis.GetVirtualIo(axis.Params.AxisIndex))
            {
                Task curSendTask = Task.Run(new System.Action(() =>
                {
                    try
                    {
                        Watcher.StopWatch("回原点");
                        axis.JogPositive();
                        do
                        {
                            if (Watcher.StartCheckIsTimeout("回原点", 5000))
                            {
                                break;
                            }
                            Thread.Sleep(5);
                        }
                        while (axis.GetVirtualIo(axis.Params.AxisIndex));
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        Watcher.StopWatch("回原点");
                        axis.DecStop();
                    }
                }));
                curSendTask.Wait();
                curSendTask = null;
                do
                {
                    Thread.Sleep(50);
                } while (axis.IsBusy);
            }
        }

        /// <summary>
        /// 将给定的点以0，0为中心按照da旋转后，确定此点新的坐标
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="da"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        //public static void AffineTransXY(double dx, double dy, double da, out double nx, out double ny, double centerx, double centery)
        //{
        //    HTuple homMat2DIdentity;
        //    HOperatorSet.HomMat2dIdentity(out homMat2DIdentity);
        //    HTuple homMat2DRotate;
        //    HTuple rad;
        //    HOperatorSet.TupleRad(da, out rad);
        //    HOperatorSet.HomMat2dRotate(homMat2DIdentity, rad, centerx, centery, out homMat2DRotate);
        //    HTuple qx, qy;
        //    HOperatorSet.AffineTransPoint2d(homMat2DRotate, dx, dy, out qx, out qy);
        //    nx = qx.D;
        //    ny = qy.D;
        //}

        /// <summary>
        /// 保存所有配置
        /// </summary>
        internal void SaveAll()
        {
            if (LoadAppParams)
            {
                XmlSerial<AppConfig>.Save(Framework.GetSystemPath(KEY_CONFIG_FILE), config);
                FW.Save();
                uac.Save();
                machine.Save();
                SaveStations();
                Framework.NotifyConfigSaved();
            }
        }

        /// <summary>
        /// 保存站位
        /// </summary>
        private void SaveStations()
        {
            foreach (Turntable tt in machine.TestItems)
            {
                foreach (Station st in tt.Stations)
                {
                    st.Executor.IO.Save();
                }
            }
        }
    }
}