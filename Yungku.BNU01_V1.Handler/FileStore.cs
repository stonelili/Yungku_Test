using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.FileSystem;
using YungkuSystem.Globalization;
using YungkuSystem.Tester;
using YungkuSystem.UAC;
using YungkuSystem;

namespace Yungku.BNU01_V1.Handler
{
    class FileStore : IFileStore
    {
        private string currentProjectPath = ".\\";
        /// <summary>
        /// 当前路径
        /// </summary>
        public string CurrentProjectPath
        {
            get { return currentProjectPath; }
            set { currentProjectPath = value; }
        }
        public string GetPath(string key)
        {
            switch (key)
            {
                case UACManager.KEY_CONFIG_FILE: 
                    return @".\Config\UAC.txt";
                case TestService.KEY_CONFIG_FILE: 
                    return @".\Config\TestSystem.xml";
                case MyApp.TEST_PATH:
                    return @".\Config\TestSystem1.xml";
                case MotionService.KEY_CONFIG_FILE: 
                    return @".\Config\MotionManage.xml";
                case TCService.KEY_CONFIG_FILE:
                    return @".\Config\TCManage.xml";
                case StatisticalService.KEY_CONFIG_FILE:
                    return @".\Config\Statistical.xml";
                case BilingualTranslation.KEY_CONFIG_FILE: 
                    return @".\Config\BT.xml";
                case YungkuSystem.TestFlow.Machine.KEY_CONFIG_FILE:
                    return @".\Config\TestFlow.xml";
                case TrayService.FAIL_TRAY:
                    return @".\Config\FailTray.xml";
                case TrayService.WORK_TRAY:
                    return @".\Config\WorkTray.xml";

                case "Turntable0_Station0_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station0_ConfigFile.xml";
                case "Turntable0_Station1_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station1_ConfigFile.xml";
                case "Turntable0_Station2_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station2_ConfigFile.xml";
                case "Turntable0_Station3_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station3_ConfigFile.xml";
                case "Turntable0_Station4_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station4_ConfigFile.xml";
                case "Turntable0_Station5_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station5_ConfigFile.xml";
                case "Turntable0_Station6_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station6_ConfigFile.xml";
                case "Turntable0_Station7_ConfigFile":
                    return currentProjectPath+@"Script\Turntable0_Station7_ConfigFile.xml";

                // 测试序列配置文件
                case "Turntable0_Station0_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station0_Sequence.xml";
                case "Turntable0_Station1_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station1_Sequence.xml";
                case "Turntable0_Station2_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station2_Sequence.xml";
                case "Turntable0_Station3_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station3_Sequence.xml";
                case "Turntable0_Station4_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station4_Sequence.xml";
                case "Turntable0_Station5_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station5_Sequence.xml";
                case "Turntable0_Station6_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station6_Sequence.xml";
                case "Turntable0_Station7_SequenceFile":
                    return currentProjectPath + @"Script\Turntable0_Station7_Sequence.xml";

                case MyApp.KEY_CONFIG_FILE:
                    return currentProjectPath+"AppConfig.xml";
                case MyApp.UPCAM_CONFIG_FILE:
                    return currentProjectPath + "UpCamConfig.xml";
                case MyApp.DOWNCAM_CONFIG_FILE:
                    return currentProjectPath + "DownCamConfig.xml";
                case LightService.KEY_CONFIG_FILE:
                    return currentProjectPath + "LightManage.xml";

                default:
                    throw new Exception("配置文件未定义，请定义！");
            }
        }
    }
}
