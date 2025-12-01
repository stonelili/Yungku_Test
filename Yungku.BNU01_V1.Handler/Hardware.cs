using System;
using System.Collections.Generic;
using System.Reflection;
using YungkuSystem.Devices.HardWareMotion.LeiSai.DMC;
using YungkuSystem.LightMotion.Intf;
using YungkuSystem.LightMotion.Manage;
using YungkuSystem.Machine;
using YungkuSystem.Motion.Intf;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;
using YungkuSystem.TCMotion.Intf;
using YungkuSystem.TCMotion.Manage;

namespace Yungku.BNU01_V1.Handler
{
    public class Hardware 
    {
        public void StopAllConti()
        {
            for (int i = 0; i < 4; i++)
            {
                LTDMC.dmc_conti_stop_list(0, (ushort)i, 0);
            }
        }
    }
}