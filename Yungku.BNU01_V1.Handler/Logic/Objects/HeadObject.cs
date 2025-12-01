using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Yungku.BNU01_V1.Handler.Config.TestConfig;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;
using YungkuSystem.Tester;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.Objects
{
    /// <summary>
    /// 定义测试头绑定对象
    /// </summary>
    public class HeadObject : MachineBase
    {
        /// <summary>
        /// 调焦服务器
        /// </summary>
        //public FocusServer FocusServer = new FocusServer();
        /// <summary>
        /// 移动是否完成
        /// </summary>
        public AutoResetEvent AutoResetMoveFinished = new AutoResetEvent(false);
        /// <summary>
        /// 记录每次调焦之后是否夹紧调焦环
        /// </summary>
        //public bool ControlRing = true;
        ///// <summary>
        ///// 收到的角度
        ///// </summary>
        //public double ReceiveAngle = 0;
        ///// <summary>
        ///// 收到移动高度
        ///// </summary>
        //public double ReceiveZ = 0;
        /// <summary>
        /// 是否可以接收移动
        /// </summary>
        public bool AllowMove = false;
        ///// <summary>
        ///// 接收到移动命令
        ///// </summary>
        //public bool ReceiveMoveCmd = false;
        /// <summary>
        /// 移动结果
        /// </summary>
        public bool MoveResult = false;

        public AxisMap HeadAxis;
        public GPIOMap HeadBrake;
        /// <summary>
        /// 测试头运行镭射数据
        /// </summary>
        //public double ProduceFocusLaserData = 0;
        /// <summary>
        /// 测试头是否校准完成
        /// </summary>
        //public YesNo AdjustFinished = false;
        /// <summary>
        /// 是否完成预调焦 进行调焦部分的防呆
        /// </summary>
        //public bool PreFocusFinished = false;
        /// <summary>
        /// 配置文件
        /// </summary>
        public HeadConfig Config
        {
            get
            {
                if (this.BaseConfig == null)
                    return new HeadConfig();
                return this.BaseConfig as HeadConfig;
            }
        }
        private HeadObject(int index)
        {
            this.Name = "Head" + index.ToString();
            this.Index = index;
            switch (index)
            {
                case 0:
                    //HeadAxis = MyApp.MachineMode == MachineModes.JF06调焦机 ? HeadAxis = MyApp.HW.HeadAxisA : null;
                    //HeadBrake = MyApp.MachineMode == MachineModes.JF06调焦机 ? HeadBrake = MyApp.HW.BrakeA : null;
                    break;
                case 1:
                    //HeadAxis = MyApp.MachineMode == MachineModes.JF06调焦机 ? MyApp.HW.HeadAxisB : null;
                    //HeadBrake = MyApp.MachineMode == MachineModes.JF06调焦机 ? HeadBrake = MyApp.HW.BrakeB : null;
                    //break;
                case 2:
                    //HeadAxis = MyApp.MachineMode == MachineModes.JF06调焦机 ? MyApp.HW.HeadAxisC : null;
                    //HeadBrake = MyApp.MachineMode == MachineModes.JF06调焦机 ? HeadBrake = MyApp.HW.BrakeC : null;
                    break;
                case 3:
                    //HeadAxis = MyApp.MachineMode == MachineModes.JF06调焦机 ? MyApp.HW.HeadAxisD : null;
                    //HeadBrake = MyApp.MachineMode == MachineModes.JF06调焦机 ? HeadBrake = MyApp.HW.BrakeD : null;
                    break;
            }
            if (MyApp.Config.TestSetting.HeadConfigs.Count <= Index)
            {
                MyApp.Config.TestSetting.HeadConfigs.Add(new HeadConfig());
            }
            this.BaseConfig = MyApp.Config.TestSetting.HeadConfigs[Index];
            //FocusServer.Name = "调焦服务器" + index.ToString();
            //FocusServer.CurrentHead = this;
            //SetServerInfo();
        }
        public override bool HomeLoop()
        {
            switch (HomeStateIndex)
            {
                case "初始状态":
                    #region   
                    ngCounter.Clear();
                    ClearPlaceNumber();
                    //testedProduceBarcode.Clear();
                    HomeStateIndex = "关闭所有治具";
                    #endregion
                    break;

                case "关闭所有治具":
                    #region
                    //if (MyApp.Config.FunctionSwitch.JigStateCheck)
                    //{
                    //    if (HeadClosedCheck)
                    //    {
                    //        Watcher.StopWatch(HomeStateIndex);
                    //        HomeStateIndex = "置位产品状态"; 
                    //    }
                    //    else if (Watcher.StartCheckIsTimeout(HomeStateIndex, 5000))
                    //    {
                    //        OnAlarm(G.Text("治具关盖检知没有检测到！"));
                    //    }
                    //}
                    //else
                    //{
                    //    HomeStateIndex = "置位产品状态" ;
                    //}
                    #endregion
                    break;

                case "置位产品状态":
                    #region
                    SendHomeOrder(MyApp.Config.General.SendCommandName);
                    SetModudeState(false);
                    (this.Owner as Head).ResetAllProduct();
                    HomeStateIndex = "回原点完成";
                    #endregion
                    break;

                case "回原点完成":
                    HomeIsFinished = true;

                    break;
            }
            return base.HomeLoop();
        }
        /// <summary>
        /// 服务器初始化
        /// </summary>
        //public void SetServerInfo()
        //{
        //    FocusServer.Ip = Config.ClientIP;
        //    FocusServer.Port = Config.ClientPort;
        //    FocusServer.Init();
        //}

        /// <summary>
        /// 发送复位命令
        /// </summary>
        /// <param name="cmdstr"></param>
        public void SendHomeOrder(string cmdstr)
        {
            if (cmdstr == string.Empty)
                return;
            foreach (Jig jig in Owner.TestItems)
            {
                if (!jig.Enabled)
                    continue;
                foreach (Product product in jig.TestItems)
                {
                    if (!product.Enabled)
                        continue;
                    ProductObject po = product.BindingObject as ProductObject;
                    foreach (YungkuSystem.TestFlow.Module module in product.TestItems)
                    {
                        YungkuSystem.Tester.IModuleGroup client = po.TestClient;
                        YungkuSystem.Tester.ITestSystem testSystem = MyApp.GetInstance().TestSystem;
                        Connect(client);
                        //连接测试程序
                        if (client != null && client.RealConnectionState == ConnectionState.Connected)
                        {
                            int mIndex = product.TestItems.IndexOf(module);
                            if (mIndex >= 0)
                            {
                                YungkuSystem.Tester.ICommand cmd = testSystem.CreateCommand(cmdstr, MyApp.Config.General.CommandType);
                                if (cmd != null)
                                {
                                    YungkuSystem.Tester.IParameter prm = cmd as YungkuSystem.Tester.IParameter;
                                    prm.ClearInputParameters();
                                    client.ClientSend(cmd, mIndex);
                                    LogWriteRecord(product.Name + "发送命令:" + cmd.Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 测试程序连接
        /// </summary>
        /// <param name="client"></param>
        private void Connect(IModuleGroup client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "客户端对象为空");

            // 已连接则直接返回
            if (client.RealConnectionState == YungkuSystem.Tester.ConnectionState.Connected)
                return;

            const int maxRetry = 3;
            const int retryDelayMs = 100;

            for (int attempt = 1; attempt <= maxRetry; attempt++)
            {
                try
                {
                    LogWriteRecord(string.Format("尝试连接测试程序(第{0}/{1}次)...", attempt, maxRetry));

                    client.Uinit();
                    Thread.Sleep(retryDelayMs);
                    client.Init();
                    Thread.Sleep(retryDelayMs);

                    if (client.RealConnectionState == YungkuSystem.Tester.ConnectionState.Connected)
                    {
                        LogWriteRecord("测试程序连接成功");
                        return;
                    }

                    LogWriteRecord(string.Format("连接失败，状态: {0}", client.RealConnectionState));
                }
                catch (Exception ex)
                {
                    LogWriteRecord(string.Format("连接异常(第{0}次): {1}", attempt, ex.Message));

                    // 最后一次尝试失败则抛出异常
                    if (attempt >= maxRetry)
                    {
                        throw new TimeoutException(
                            string.Format("测试程序连接失败，已重试{0}次: {1}", maxRetry, ex.Message), ex);
                    }
                }
            }

            throw new TimeoutException(string.Format("测试程序连接超时，已重试{0}次", maxRetry));
        }

        /// <summary>
        /// 关闭所有治具
        /// </summary>
        //public void CloseAllJig()
        //{
        //    foreach (Jig jig in Owner.TestItems)
        //    {
        //        JigObject jo = jig.BindingObject as JigObject;
        //        jo.CloseJig();
        //    }
        //}

        /// <summary>
        /// 所有治具是否打开
        /// </summary>
        //public bool AllJigIsOpened
        //{
        //    get
        //    {
        //        bool isOpened = true;
        //        foreach (Jig jig in Owner.TestItems)
        //        {
        //            JigObject jo = jig.BindingObject as JigObject;
        //            if (!jo.IsOpened)
        //            {
        //                isOpened = false;
        //                break;
        //            }
        //        }
        //        return isOpened;
        //    }
        //}

        /// <summary>
        /// 所有治具是否关闭
        /// </summary>
        //public bool AllJigIsClosed
        //{
        //    get
        //    {
        //        bool isClosed = true;
        //        foreach (Jig jig in Owner.TestItems)
        //        {
        //            JigObject jo = jig.BindingObject as JigObject;
        //            if (!jo.IsClosed)
        //            {
        //                isClosed = false;
        //                break;
        //            }
        //        }
        //        return isClosed;
        //    }
        //}

        /// <summary>
        /// 检查所有治具是否关闭到位
        /// </summary>
        public bool HeadClosedCheck
        {
            get
            {
                bool isClosedCheck = true;
                foreach (Jig jig in Owner.TestItems)
                {
                    JigObject jo = jig.BindingObject as JigObject;
                    if (!jo.IsCloseCheck)
                    {
                        isClosedCheck = false;
                        break;
                    }
                }
                return isClosedCheck;
            }
        }

        /// <summary>
        /// 设置当前治具真空
        /// </summary>
        /// <param name="val"></param>
        //public void SetHeadSetVacuum(bool val)
        //{
        //    foreach (Jig jig in Owner.TestItems)
        //    {
        //        foreach (Product product in jig.TestItems)
        //        {
        //            ProductObject pr = product.BindingObject as ProductObject;
        //            pr.SetVacuum(val);
        //        }
        //    }
        //}

        /// <summary>
        /// 获取真空状态
        /// </summary>
        /// <returns></returns>
        //public bool GetHeadVacuum()
        //{
        //    bool VacuumOK = true;
        //    foreach (Jig jig in Owner.TestItems)
        //    {
        //        foreach (Product product in jig.TestItems)
        //        {
        //            ProductObject pr = product.BindingObject as ProductObject;
        //            VacuumOK &= pr.GetVacuumValue();
        //        }
        //    }
        //    return VacuumOK;
        //}

        /// <summary>
        /// 绑定自定义治具数据到治具对象中
        /// </summary>
        /// <param name="machine"></param>
        public static void BindToMachine(Machine machine)
        {
            int index = 0;
            foreach (Turntable tt in machine.TestItems)
            {
                foreach (Head head in tt.TestItems)
                {
                    head.BindingObject = new HeadObject(index++);
                }
            }
        }

        /// <summary>
        /// 判断是否有模组
        /// </summary>
        public bool HasModudeState
        {
            get
            {
                foreach (Jig jig in Owner.TestItems)
                {
                    if (!jig.Enabled)
                        continue;
                    foreach (Product product in jig.TestItems)
                    {
                        if (!product.Enabled)
                            continue;
                        ProductObject po = product.BindingObject as ProductObject;
                        if (po.HasModule)
                            return true;
                    }
                }
                return false;
            }

        }

        internal void SetModudeState(bool hasModule)
        {
            foreach (Jig jig in Owner.TestItems)
            {
                foreach (Product product in jig.TestItems)
                {
                    product.Reset();
                    ProductObject po = product.BindingObject as ProductObject;
                    po.HasModule = hasModule;
                    po.PlaceProduceOk = hasModule;
                }
            }
        }

        internal void ReSetPlaceProduceOk()
        {
            foreach (Jig jig in Owner.TestItems)
            {
                foreach (Product product in jig.TestItems)
                {
                    ProductObject po = product.BindingObject as ProductObject;
                    po.PlaceProduceOk = false;
                }
            }
        }

        /// <summary>
        /// 开启产品计数
        /// </summary>
        public int EnabledProductCount
        {
            get
            {
                int count = 0;
                foreach (Jig jig in Owner.TestItems)
                {
                    foreach (Product product in jig.TestItems)
                    {
                        ProductObject po = product.BindingObject as ProductObject;
                        if (po.OwnerEnabled)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
        }

        private Dictionary<Dictionary<Product, string>, int> ngCounter = new Dictionary<Dictionary<Product, string>, int>();
        [DisplayName("测试机测试头NG统计")]
        [XmlIgnore()]
        public Dictionary<Dictionary<Product, string>, int> NGCounter
        {
            get { return ngCounter; }
        }

        #region 测试相关

        /// <summary>
        /// 检查NG代码报警
        /// </summary>
        public void CheckAllJigNGCountAlarm()
        {
            if (!this.Owner.Enabled)
                return;

            foreach (Jig jig in this.Owner.AutoItems)
            {
                foreach (Product product in jig.AutoItems)
                {
                    if (!product.Enabled || !(product.BindingObject as ProductObject).HasModule)
                        continue;
                    if (product.IsFail)
                        SetCounter(product, product.ResultCode);
                }
            }
        }
        private readonly object ngCounterLock = new object();
        //private Dictionary<Dictionary<Product, string>, int> ngCounter = new Dictionary<Dictionary<Product, string>, int>();

        /// <summary>
        /// 累加NG计数器（线程安全）
        /// </summary>
        private void SetCounter(Product client, string resultCode)
        {
            lock (ngCounterLock)
            {
                bool find = false;
                foreach (Dictionary<Product, string> key in NGCounter.Keys)
                {
                    if (key.ContainsKey(client) && key[client] == resultCode)
                    {
                        NGCounter[key]++;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    Dictionary<Product, string> key = new Dictionary<Product, string>();
                    key.Add(client, resultCode);
                    NGCounter.Add(key, 0);
                }
                CheckJigNGCountAlarm(client, resultCode);
            }
        }

        /// <summary>
        /// 检查连续NG的数量是否超过设置值，如果超过则报警
        /// </summary>
        /// <param name="client"></param>
        /// <param name="resultCode"></param>
        private void CheckJigNGCountAlarm(Product client, string resultCode)
        {
            if (MyApp.Config.General.MaxNGCount <= 0)
                return;
            int count = 0;
            foreach (Dictionary<Product, string> key in NGCounter.Keys)
            {
                if (key.ContainsKey(client) && key[client] == resultCode)
                {
                    count = NGCounter[key];
                    break;
                }
            }
            if (count >= MyApp.Config.General.MaxNGCount)
            {
                ClearCounter(client, resultCode);
                OnAlarm(this.Name + "中的治具" + client.Name + "的不良:" + resultCode.ToString() + "  已经超过了最大允许值" + MyApp.Config.General.MaxNGCount.ToString() + "！");
            }
        }
        /// <summary>
        /// 清除NG计数器（线程安全）
        /// </summary>
        private void ClearCounter(Product client, string resultCode)
        {
            lock (ngCounterLock)
            {
                bool find = false;
                foreach (Dictionary<Product, string> key in NGCounter.Keys)
                {
                    if (key.ContainsKey(client) && key[client] == resultCode)
                    {
                        NGCounter[key] = 0;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    Dictionary<Product, string> key = new Dictionary<Product, string>();
                    key.Add(client, resultCode);
                    NGCounter.Add(key, 0);
                }
            }
        }

        #endregion

        #region 复测相关
        private readonly object barcodeLock = new object();
        private HashSet<string> testedProduceBarcode = new HashSet<string>(); // 使用 HashSet 去重

        [DisplayName("已经测试过的产品集合")]
        [XmlIgnore()]
        public List<string> TestedProduceBarcode
        {
            get
            {
                lock (barcodeLock)
                {
                    return testedProduceBarcode.ToList();
                }
            }
        }

        /// <summary>
        /// 添加NG产品条码（线程安全）
        /// </summary>
        public void AddNgProduceBarcode(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return;

            lock (barcodeLock)
            {
                testedProduceBarcode.Add(barcode);
            }
        }
        #endregion

        public void ClearPlaceNumber()
        {
            foreach (Jig jig in this.Owner.AutoItems)
            {
                foreach (Product product in jig.AutoItems)
                {
                    (product.BindingObject as ProductObject).PlaceNumber = 0;
                }

            }
        }

        /// <summary>
        /// 获取上料产品
        /// </summary>
        /// <returns></returns>
        public Product GetPlaceProduct()
        {
            foreach (Jig jig in this.Owner.TestItems)
            {
                if (!jig.Enabled)
                    continue;
                foreach (Product product in jig.TestItems)
                {
                    if (!product.Enabled)
                        continue;
                    if (!(product.BindingObject as ProductObject).PlaceProduceOk)
                        return product;
                }
            }
            return null;
        }

        /// <summary>
        /// 界面显示结果
        /// </summary>
        public event EventHandler<Product> OnUpdateTestState;
        public event EventHandler<Product> OnClearTestState;

        /// <summary>
        /// 显示测试的结果代码
        /// </summary>
        /// <param name="testerIndex"></param>
        /// <param name="resultCode"></param>
        public void ShowTestResult(bool Clear = false)
        {
            foreach (Jig jig in Owner.TestItems)
            {
                foreach (Product product in jig.TestItems)
                {
                    if (Clear)
                    {
                        if (OnClearTestState != null)
                        {
                            OnClearTestState(this, product);
                        }
                    }
                    else
                    {
                        if (OnUpdateTestState != null)
                        {
                            OnUpdateTestState(this, product);
                        }
                    }

                }
            }

        }
        public void ShowTestResultNG(bool Clear = false)
        {
            foreach (Jig jig in Owner.TestItems)
            {
                foreach (Product product in jig.TestItems)
                {
                    product.Result = TestResult.Fail;
                    product.ResultCode = "-1";
                }
            }
        }
    }
}
