using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yungku.BNU01_V1.Handler.Config;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.Objects
{
    public  class MachineObject:MachineBase
    {
        /// <summary>
        /// 指示批次是否结束
        /// </summary>
        public YesNo ProductionEnd = false;
        /// <summary>
        /// 是否清料
        /// </summary>
        public YesNo UnloadTrayClear = false;
        /// <summary>
        /// 
        /// </summary>
        public  YesNo YesClearProduct = false;
        private MachineObject()
        {
            this.Name = "BNU01";  
                   
        }
        public override bool HomeLoop()
        {

            switch (HomeStateIndex)
            {
                case "初始状态":
                    #region
                    //YesClearProduct = false;
                    //UnloadTrayClear = false;
                    //ProductionEnd = false;
                    //HomeStateIndex = "转盘回原点准备";
                    #endregion
                    break;

                //case "转盘回原点准备":
                //    #region
                //    foreach (Turntable  tt in this.Owner.TestItems)
                //    {
                //        (tt.BindingObject as TurntableObject).ResetHomeStatus();
                //    }
                //    HomeStateIndex = "等待转盘回原点完成";
                //    #endregion
                //    break;
                //case "等待转盘回原点完成":
                //    #region
                //    MoveFinished = true;
                //    foreach (Turntable tt in this.Owner.TestItems)
                //    {
                //        MoveFinished &= (tt.BindingObject as TurntableObject).HomeLoop();
                //    }
                //    if (MoveFinished)
                //    {
                //        Watcher.StopWatch(HomeStateIndex);
                //        HomeStateIndex = "回原点完成";
                //    }
                //    else if (Watcher.StartCheckIsTimeout(HomeStateIndex, 120000, false))
                //    {
                //        OnAlarm("转盘回原点超时！");
                //        MyApp.NeedReset = true;
                //    }
                //    #endregion
                //    break;           

                case "回原点完成":
                    HomeIsFinished = true;
                    break;
            }
            return base.HomeLoop();
        }
        /// <summary>
        /// 阻塞式移动到指定位置（等待到位）
        /// </summary>
        public bool MoveToAndWait(AxisMap axis, double pos, double precision = 0.3, int timeout = 10000)
        {
            DateTime startTime = DateTime.Now;
            axis.AbsMove(pos);

            while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
            {
                if (Math.Abs(axis.Position - pos) < precision)
                    return true;

                System.Threading.Thread.Sleep(50);
                System.Windows.Forms.Application.DoEvents(); // 防止UI卡死
            }

            return false;
        }
        /// <summary>
        /// 绑定对象中
        /// </summary>
        /// <param name="machine"></param>
        public static void BindToMachine(Machine machine)
        {
           machine.BindingObject = new MachineObject();
        }
    }
}
