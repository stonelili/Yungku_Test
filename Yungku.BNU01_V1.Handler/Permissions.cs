using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yungku.BNU01_V1.Handler
{
    /// <summary>
    /// 许可类
    /// </summary>
    internal static class Permissions
    {
        public const string SYSTEM = "SYSTEM";
        public const string MANUAL_OPERATION = "MANUAL_OPERATION";
        public const string SETTING_MODIFY = "SETTING_MODIFY";

        public static void LoadToUAC()
        {
            MyApp.GetInstance().UAC.DefinePermission(MANUAL_OPERATION);
            MyApp.GetInstance().UAC.DefinePermission(SETTING_MODIFY);
        }
    }
}
