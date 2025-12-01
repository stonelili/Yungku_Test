using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yungku.BNU01_V1.Handler
{
    public class LightControl
    {
        
        /// <summary>
        /// 连接 积分球LED 光源
        /// <summary>
        [DllImport("LightControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool ConnectLED();//00 E8 DC D6 00 00 00 2F 
        /// <summary>
        /// 打开光源
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [DllImport("LightControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool LightOn(int mode);
        /// <summary>
        /// 关闭光源
        /// </summary>
        /// <returns></returns>
        [DllImport("LightControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool LightOff();
        /// <summary>
        /// 设置光源色温、亮度
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="CCT"></param>
        /// <param name="Lux"></param>
        /// <returns></returns>
        [DllImport("LightControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool Read_CCT_Lux(int Mode, ref double CCT, ref double Lux);
        
        private static bool integratingSphereState = false;
        public static bool IntegratingSphereState
        {
            get { return integratingSphereState; }
        }

        public static void IntegratingSphereLink()
        {
            integratingSphereState = ConnectLED();
        }
    }
}
