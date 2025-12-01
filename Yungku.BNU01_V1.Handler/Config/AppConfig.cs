using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Yungku.BNU01_V1.Handler.Config.TestConfig;
using YungkuSystem;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;


namespace Yungku.BNU01_V1.Handler.Config
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class AppConfig
    {

        private GeneralSettings general = new GeneralSettings();
        /// <summary>
        /// 基本设置
        /// </summary>
        public GeneralSettings General
        {
            get { return general; }
            set { general = value; }
        }

        private FunctionSwitch functionSwitch = new FunctionSwitch();
        /// <summary>
        /// 功能转换
        /// </summary>
        public FunctionSwitch FunctionSwitch
        {
            get { return functionSwitch; }
            set { functionSwitch = value; }
        }

        private MachineTestSettings machineTestSetting = new MachineTestSettings();
        /// <summary>
        /// 测试机设置
        /// </summary>
        public MachineTestSettings MachineTestSetting
        {
            get { return machineTestSetting; }
            set { machineTestSetting = value; }
        }
        private TestSetting testSetting = new TestSetting();
        /// <summary>
        /// 测试机基本设置
        /// </summary>
        public TestSetting TestSetting
        {
            get { return testSetting; }
            set { testSetting = value; }
        }
    }
}
