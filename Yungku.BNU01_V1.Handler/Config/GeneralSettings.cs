using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YungkuSystem.Controls;
using YungkuSystem.Structs;
using YungkuSystem.Tester;

namespace Yungku.BNU01_V1.Handler.Config
{
    public enum StartMode
    {
        StartButton,//启动按钮模式
        JigClosed,   //手动治具关盖启动模式
        Loop          //循环跑机模式
    }

    public enum AfterAStationMode
    {
        仍然转入,
        全部不良时不转入
    }

    /// <summary>
    /// 常规设置类
    /// </summary>
    public class GeneralSettings : IConfigPage
    {
        [MyDisplayName("是否弹出结果界面"), MyCategory("结果弹窗界面设置")]
        public YesNo ShowResultForm { get; set; } = true;

        [MyDisplayName("结果弹窗界面长度"), MyCategory("结果弹窗界面设置")]
        public int FormWidth { get; set; } = 700;
        [MyDisplayName("结果弹窗界面高度"), MyCategory("结果弹窗界面设置")]
        public int formHeight { get; set; } = 100;

        private string productName = string.Empty;
        [MyDisplayName("当前生产的机种名称"), MyCategory("工单设置")]
        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        private string runCard = string.Empty;
        [MyDisplayName("工单号"), MyCategory("工单设置")]
        public string RunCard
        {
            get { return runCard; }
            set { runCard = value; }
        }

        private CommandType commandType = CommandType.四焦段模式;
        [MyDisplayName("测试协议类型"), MyCategory("测试命令设置")]
        public CommandType CommandType
        {
            get { return commandType; }
            set { commandType = value; }
        }

        private string beginCommandName = "开始";
        [MyDisplayName("测试开始指令"), MyCategory("测试命令设置"), MyDescription("机台按下启动按钮后,向测试程序发送的一条测试开始指令")]
        public string BeginCommandName
        {
            get { return beginCommandName; }
            set { beginCommandName = value; }
        }

        private string sendcommandName = "结束";
        [MyDisplayName("复位后发送指令"), MyCategory("测试命令设置"), MyDescription("机台复位完成后,向测试程序发送的一条测试指令")]
        public string SendCommandName
        {
            get { return sendcommandName; }
            set { sendcommandName = value; }
        }

        //public YesNo reselectMachine = false;
        //[MyDisplayName("设备选择"), MyCategory("基本功能"), Description("重启生效")]
        //public YesNo ReselectMachine
        //{
        //    get { return reselectMachine; }
        //    set { reselectMachine = value; }
        //}

        private YesNo showAlarmsForm = true;
        [MyDisplayName("是否显示报警窗体"), MyCategory("基本功能")]
        public YesNo ShowAlarmsForm
        {
            get { return showAlarmsForm; }
            set { showAlarmsForm = value; }
        }
        #region 测试位置2功能开关
        private YesNo enableRAxisTestPosition2 = false;
        [MyDisplayName("启用R轴测试位置2"), MyCategory("测试位置2功能开关"), MyDescription("仅R轴移动到测试位置2（总开关关闭时有效）")]
        public YesNo EnableRAxisTestPosition2
        {
            get { return enableRAxisTestPosition2; }
            set { enableRAxisTestPosition2 = value; }
        }
        private YesNo enableZAxisTestPosition2 = false;
        [MyDisplayName("启用Z轴测试位置2"), MyCategory("测试位置2功能开关"), MyDescription("仅Z轴移动到测试位置2（总开关关闭时有效）")]
        public YesNo EnableZAxisTestPosition2
        {
            get { return enableZAxisTestPosition2; }
            set { enableZAxisTestPosition2 = value; }
        }
        private YesNo enableXAxisTestPosition2 = false;
        [MyDisplayName("启用X轴测试位置2"), MyCategory("测试位置2功能开关"), MyDescription("仅X轴移动到测试位置2（总开关关闭时有效）")]
        public YesNo EnableXAxisTestPosition2
        {
            get { return enableXAxisTestPosition2; }
            set { enableXAxisTestPosition2 = value; }
        }
        private YesNo enableYAxisTestPosition2 = false;
        [MyDisplayName("启用Y轴测试位置2"), MyCategory("测试位置2功能开关"), MyDescription("仅Y轴移动到测试位置2（总开关关闭时有效）")]
        public YesNo EnableYAxisTestPosition2
        {
            get { return enableYAxisTestPosition2; }
            set { enableYAxisTestPosition2 = value; }
        }
        private YesNo enableAllAxisTestPosition2 = true;
        [MyDisplayName("启用所有轴测试位置2"), MyCategory("测试位置2功能开关"), MyDescription("总开关，开启后XYZR所有轴都移动到测试位置2，优先级最高")]
        public YesNo EnableAllAxisTestPosition2
        {
            get { return enableAllAxisTestPosition2; }
            set { enableAllAxisTestPosition2 = value; }
        }

        #endregion

        /* private AfterAStationMode afterAStationMode = AfterAStationMode.仍然转入;
         [MyDisplayName("A工位测试不良是否转入"), MyCategory("测试参数"), Description("只有当启动模式选择为:StartButton模式和JigClosed模式时有效")]
         public AfterAStationMode AfterAStationMode
         {
             get { return afterAStationMode; }
             set { afterAStationMode = value; }
         }*/

        #region 平行光管参数
        [MyDisplayName("平行光管默认色温值"), MyCategory("平行光管参数")]
        public int ElectricCollimator_CCT { get; set; } = 5000;
        [MyDisplayName("平行光管默认照度值"), MyCategory("平行光管参数")]
        public int ElectricCollimator_EX { get; set; } = 10;
        [MyDisplayName("平行光管默认位置"), MyCategory("平行光管参数")]
        public int ElectricCollimator_FX { get; set; } = 10000;

        //[MyDisplayName("光管移动到0.6米模拟距离处参考值"), MyCategory("平行光管参数")]
        //public float[] CollimatorDistance60mm { get; set; } = {600,600,600,600,600,600,600,600,600 };

        //[MyDisplayName("光管移动到10米模拟距离处参考值"), MyCategory("平行光管参数")]
        //public float[] CollimatorDistance10000mm { get; set; } = { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
        [MyDisplayName("光管移动到2米模拟距离处参考值"), MyCategory("平行光管参数")]
        public float[] CollimatorDistance2000mm { get; set; } = { 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000 };

        [MyDisplayName("光管亮度参考值1"), MyCategory("平行光管参数")]
        public float[] Collimator_EX { get; set; } = { 14, 20, 20, 20, 20, 30, 30, 30, 30 };

        [MyDisplayName("光管亮度参考值2"), MyCategory("平行光管参数")]
        public float[] Collimator_EX2 { get; set; } = { 14, 20, 20, 20, 20, 30, 30, 30, 30 };
        #endregion


        #region X轴位置
        private double axisXLoadPosition = 0;
        [MyDisplayName("X轴上料位置"), MyCategory("X轴位置设置")]
        public double AxisXLoadPosition
        {
            get { return axisXLoadPosition; }
            set { axisXLoadPosition = value; }
        }

        //private double axisXBufferPosition = 20;
        //[MyDisplayName("X轴过渡位置"), MyCategory("X轴位置设置")]
        //public double AxisXBufferPosition
        //{
        //    get { return axisXBufferPosition; }
        //    set { axisXBufferPosition = value; }
        //}

        private double axisXTestPosition = 40;
        [MyDisplayName("X轴测试位置1"), MyCategory("X轴位置设置")]
        public double AxisXTestPosition
        {
            get { return axisXTestPosition; }
            set { axisXTestPosition = value; }
        }

        private double axisXTestPosition2 = 40;
        [MyDisplayName("X轴测试位置2"), MyCategory("X轴位置设置")]
        public double AxisXTestPosition2
        {
            get { return axisXTestPosition2; }
            set { axisXTestPosition2 = value; }
        }
        #endregion

        #region Y轴位置
        private double axisYLoadPosition = 0;
        [MyDisplayName("Y轴上料位置"), MyCategory("Y轴位置设置")]
        public double AxisYLoadPosition
        {
            get { return axisYLoadPosition; }
            set { axisYLoadPosition = value; }
        }

        private double axisYBufferPosition = 250;
        [MyDisplayName("Y轴过渡位置"), MyCategory("Y轴位置设置")]
        public double AxisYBufferPosition
        {
            get { return axisYBufferPosition; }
            set { axisYBufferPosition = value; }
        }

        private double axisYTestPosition = 400;
        [MyDisplayName("Y轴测试位置1"), MyCategory("Y轴位置设置")]
        public double AxisYTestPosition
        {
            get { return axisYTestPosition; }
            set { axisYTestPosition = value; }
        }

        private double axisYTestPosition2 = 400;
        [MyDisplayName("Y轴测试位置2"), MyCategory("Y轴位置设置")]
        public double AxisYTestPosition2
        {
            get { return axisYTestPosition2; }
            set { axisYTestPosition2 = value; }
        }
        #endregion

        #region Z轴位置
        private double axisZLoadPosition = 0;
        [MyDisplayName("Z轴上料位置"), MyCategory("Z轴位置设置")]
        public double AxisZLoadPosition
        {
            get { return axisZLoadPosition; }
            set { axisZLoadPosition = value; }
        }


        //private double axisZBufferPosition = 20;
        //[MyDisplayName("Z轴过渡位置"), MyCategory("Z轴位置设置")]
        //public double AxisZBufferPosition
        //{
        //    get { return axisZBufferPosition; }
        //    set { axisZBufferPosition = value; }
        //}

        private double axisZTestPosition = 40;
        [MyDisplayName("Z轴测试位置1"), MyCategory("Z轴位置设置")]
        public double AxisZTestPosition
        {
            get { return axisZTestPosition; }
            set { axisZTestPosition = value; }
        }

        private double axisZTestPosition2 = 40;
        [MyDisplayName("Z轴测试位置2"), MyCategory("Z轴位置设置")]
        public double AxisZTestPosition2
        {
            get { return axisZTestPosition2; }
            set { axisZTestPosition2 = value; }
        }
        #endregion

        #region R轴位置
        private double axisRLoadPosition = 0;
        [MyDisplayName("R轴上料位置"), MyCategory("R轴位置设置")]
        public double AxisRLoadPosition
        {
            get { return axisRLoadPosition; }
            set { axisRLoadPosition = value; }
        }

        private double axisRTestPosition = 90;
        [MyDisplayName("R轴测试位置1"), MyCategory("R轴位置设置")]
        public double AxisRTestPosition
        {
            get { return axisRTestPosition; }
            set { axisRTestPosition = value; }
        }
        private double axisRTestPosition2 = 180;
        [MyDisplayName("R轴测试位置2"), MyCategory("R轴位置设置")]
        public double AxisRTestPosition2
        {
            get { return axisRTestPosition2; }
            set { axisRTestPosition2 = value; }
        }
        #endregion

     
        //测试参数 同一种NG到达次数
        private int sameTypeNgCount = 0;
        [MyDisplayName("同一种NG到达次数"), MyCategory("测试参数"), MyDescription("同一种NG出现次数到达设定的次数则报警,参数设为0，则不报警")]
        public int SameTypeNgCount
        {
            get { return sameTypeNgCount; }
            set { sameTypeNgCount = value; }
        }

       /* private Direction turntableDirection = Direction.CCW;
        [MyDisplayName("转盘方向"), MyCategory("基本功能"), Description("CW  : 顺时针旋转\t\nCCW : 逆时针旋转")]
        //[Browsable(false)]
        //[XmlIgnore]
        public Direction TurntableDirection
        {
            get { return turntableDirection; }
            set { turntableDirection = value; }
        }*/

        private StartMode startMode = StartMode.StartButton;
        [MyDisplayName("启动模式"), MyCategory("基本功能"), MyDescription("StartButton : 启动按钮模式\t\nJigClosed   : 手动治具关盖启动模式\t\nLoop        : 循环跑机模式")]
        public StartMode StartMode
        {
            get { return startMode; }
            set { startMode = value; }
        }

        private int waitOperationTimeout = 600000;
        [MyDisplayName("启动等待时间"), MyCategory("基本功能"), MyDescription("启动按钮计时完成后自动转为停止状态")]
        public int WaitOperationTimeout
        {
            get { return waitOperationTimeout; }
            set { waitOperationTimeout = value; }
        }

        private bool codeScannerCheck = false;
        [MyDisplayName("是否管控二维码"), MyCategory("扫码")]
        public YesNo CodeScannerCheck
        {
            get { return codeScannerCheck; }
            set { codeScannerCheck = value; }
        }

        private int codeLength = 7;
        /// <summary>
        /// 二维码长度
        /// </summary>
        [MyDisplayName("二维码长度"), MyCategory("扫码")]
        public int CodeLength
        {
            get { return codeLength; }
            set { codeLength = value; }
        }

        private string codeHead = "AAAAAAAA";
        /// <summary>
        /// 二维码头
        /// </summary>
        [MyDisplayName("二维码头"), MyCategory("扫码")]
        public string CodeHead
        {
            get { return codeHead; }
            set { codeHead = value; }
        }

        private int sensorDely = 10000;
        [MyDisplayName("气缸到位判断延时[ms]"), MyCategory("基本功能"), MyDescription("单位:毫秒")]
        public int SensorDely
        {
            get { return sensorDely; }
            set { sensorDely = value; }
        }

        private int axisMoveTimeout = 10000;
        [MyDisplayName("轴移动到位超时时间[ms]"), MyCategory("基本功能"), MyDescription("单位:毫秒")]
        public int AxisMoveTimeout
        {
            get { return axisMoveTimeout; }
            set { axisMoveTimeout = value; }
        }

        private int outTime = 300000;
        [MyDisplayName("等待命令超时时间（ms）"), MyCategory("基本功能"), MyDescription("等待测试软件发送或回复命令超时时间，单位:毫秒")]
        public int OutTime
        {
            get { return outTime; }
            set { outTime = value; }
        }

        //private uint screenPort = 4;
        //[MyDisplayName("小屏幕通讯口"), MyCategory("通讯设置")]
        //public uint SmallScreenPort
        //{
        //    get { return screenPort; }
        //    set { screenPort = value; }
        //}

        private DateTime superDogExpireSoonAlarmTime = DateTime.Now;
        /// <summary>
        /// 上次提醒时间
        /// </summary>
        [Browsable(false)]
        public DateTime SuperDogExpireSoonAlarmTime
        {
            get { return superDogExpireSoonAlarmTime; }
            set { superDogExpireSoonAlarmTime = value; }
        }

        //统计参数
        private YesNo isUseExportProductData = false;
        /// <summary>
        /// 是否启用自动导出UPH数据
        /// </summary>
        [MyDisplayName("是否启用自动导出UPH数据"), MyCategory("统计参数"), MyDescription("自动导出UPH数据,然后清空UPH数据")]
        public YesNo IsUseExportProductData
        {
            get { return isUseExportProductData; }
            set { isUseExportProductData = value; }
        }

        private DateTime morningProductDataTime = DateTime.Now;
        /// <summary>
        /// 早班生成时间
        /// </summary>
        [MyDisplayName("早班生成时间"), MyCategory("统计参数"), MyDescription("年月日无效，只判断时和分")]
        public DateTime MorningProductDataTime
        {
            get { return morningProductDataTime; }
            set { morningProductDataTime = value; }
        }

        private DateTime nightProductDataTime = DateTime.Now;
        /// <summary>
        /// 晚班生产时间
        /// </summary>
        [MyDisplayName("晚班生成时间"), MyCategory("统计参数"), MyDescription("年月日无效，只判断时和分")]
        public DateTime NightProductDataTime
        {
            get { return nightProductDataTime; }
            set { nightProductDataTime = value; }
        }

        private int maxNGCount = 3;
        [DisplayName("单个治具相同不良报警上限"), MyCategory("测试")]
        public int MaxNGCount
        {
            get { return maxNGCount; }
            set
            {
                if (value > 10)
                    maxNGCount = 10;
                else
                    maxNGCount = value;
            }
        }

        private FormPropertyGrid configForm = new FormPropertyGrid();
        /// <summary>
        /// 获取配置窗口
        /// </summary>
        /// <returns></returns>
        public System.Windows.Forms.Form GetConfigForm()
        {
            configForm.propertyGrid1.SelectedObject = this;
            return configForm;
        }

    }
}
