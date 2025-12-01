using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YungkuSystem.Controls;
using YungkuSystem.Structs;

namespace Yungku.BNU01_V1.Handler.Config
{
    public enum TestSoftwareType
    {
        通用,
        涌固
    }

    /// <summary>
    /// 功能转换类
    /// </summary>
    public class FunctionSwitch : IConfigPage
    {
        [MyDisplayName("忽略代码"), MyCategory("结果处理"), MyDescription("重启生效")]
        public List<CodeConfig> IgnoreCodes { get; set; } = new List<CodeConfig>();

        [MyDisplayName("良品代码"), MyCategory("结果处理"), MyDescription("重启生效")]
        public List<CodeConfig> PassCodes { get; set; } = new List<CodeConfig>();

        private YesNo sceneStateCheck = true;
        [MyDisplayName("开启治具关盖检查"), MyCategory("安全")]
        [XmlIgnore]
        public YesNo SceneStateCheck
        {
            get { return sceneStateCheck; }
            set { sceneStateCheck = value; }
        }

        private bool codeScannerEnabled = false;
        [MyDisplayName("是否扫码"), MyCategory("功能选择")]
        public YesNo CodeScannerEnabled
        {
            get { return codeScannerEnabled; }
            set { codeScannerEnabled = value; }
        }

        private bool codeScannerFrom = true;
        [MyDisplayName("是否一直激活扫码窗体"), MyCategory("功能选择")]
        public YesNo CodeScannerFrom
        {
            get { return codeScannerFrom; }
            set { codeScannerFrom = value; }
        }

        //[MyDisplayName("是否使用极限感应器作为到位标志"), MyCategory("功能选择"), MyDescription("选择【是】时，请将【相机翻转到正面】设置为9999,【相机翻转到反面】设置为-9999")]
        //public YesNo IsSensor { get; set; } = true;

        [MyDisplayName("是否开启门控制"), MyCategory("功能选择")]
        public YesNo IsControlDoor { get; set; } = true;

        private YesNo dataIsSave = false;
        /// <summary>
        ///
        /// </summary>
        [MyDisplayName("数据是否存储"), MyCategory("数据")]
        [XmlIgnore]
        public YesNo DataIsSave
        {
            get { return dataIsSave; }
            set { dataIsSave = value; }
        }

        private YesNo doorStateCheck = true;
        /// <summary>
        /// 开启安全门检查
        /// </summary>
        [MyDisplayName("开启安全门检查"), MyCategory("安全")]
        [XmlIgnore]
        public YesNo DoorStateCheck
        {
            get { return doorStateCheck; }
            set { doorStateCheck = value; }
        }

        private bool gratingStateCheck = true;
        [MyDisplayName("开启安全光栅检查"), MyCategory("安全")]
        [XmlIgnore]
        public YesNo GratingStateCheck
        {
            get { return gratingStateCheck; }
            set { gratingStateCheck = value; }
        }

        //private YesNo jigStateCheck = true;
        //[MyDisplayName("开启软排关盖检查"), MyCategory("安全")]
        //[Browsable(false)]
        //[XmlIgnore]
        //public YesNo JigStateCheck
        //{
        //    get { return jigStateCheck; }
        //    set { jigStateCheck = value; }
        //}

        //private YesNo sceneStateCheck = true;
        //[MyDisplayName("开启治具关盖检查"), MyCategory("安全")]
        //[XmlIgnore]
        //public YesNo SceneStateCheck
        //{
        //    get { return sceneStateCheck; }
        //    set { sceneStateCheck = value; }
        //}

        private YesNo testSoftwareCheck = true;
        [MyDisplayName("开启测试软件通信检查"), MyCategory("通信相关")]
        public YesNo TestSoftwareCheck
        {
            get { return testSoftwareCheck; }
            set { testSoftwareCheck = value; }
        }

        private bool autoLinkElectricCollimator = true;
        [MyDisplayName("启动软件是否自动连接平行光管"), MyCategory("功能参数")]
        public YesNo AutoLinkElectricCollimator
        {
            get { return autoLinkElectricCollimator; }
            set { autoLinkElectricCollimator = value; }
        }
        [MyDisplayName("使用测试软件的类型[修改后请重启软件]"), MyCategory("测试参数")]
        public TestSoftwareType DeviceTestSoftwareType { get; set; } = TestSoftwareType.涌固;

        //[MyDisplayName("使用测试软件的类型[修改后请重启软件]"), MyCategory("测试参数")]
        //[XmlIgnore]
        //[ReadOnly(true)]
        //public TestSoftwareType DeviceTestSoftware { get; set; } = TestSoftwareType.弘景;

        //[MyDisplayName("按下启动按钮后是否同时发送开始采图命令"), MyCategory("测试功能")]
        //public bool AsynchronyBeginTest { get; set; } = true;
        //[MyDisplayName("按下启动按钮后是否设置默认色温和亮度"), MyCategory("测试功能")]
        //public bool AsynchronyAutoSetCCTAndEX{ get; set; } = true;
        //[MyDisplayName("按下启动按钮后是否设置默认色温和亮度"), MyCategory("测试功能")]
        //public bool AsynchronyAutoSetFX { get; set; } = true;


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