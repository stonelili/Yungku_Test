using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Manage;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.Objects
{

    public class TurntableObject : MachineBase
    {
        /// <summary>
        /// 子治具中放料失败的所有二维码
        /// </summary>
        public List<string> JigPlaceFailBarCodes = new List<string>();
        /// <summary>
        /// 转盘旋转电机
        /// </summary>
        private AxisMap AxisTurntable;
        //private AxisMap AxisFous;


        public override bool HomeLoop()
        {
            switch (HomeStateIndex)
            {
                case "初始状态":
                    #region

                    JigPlaceFailBarCodes.Clear();
                    HomeStateIndex = "复位完成";
                    #endregion
                    break;
                
                case "复位完成":
                    this.HomeIsFinished = true;
                    break;
            }
            return base.HomeLoop();
        }

        /// <summary>
        /// 绑定对象中
        /// </summary>
        /// <param name="machine"></param>
        public static void BindToMachine(Machine machine)
        {
            int index = 0;
            foreach (Turntable tt in machine.TestItems)
            {
                //tt.BindingObject = new TurntableObject(index++);
            }
        }

        //public YesNo AllJigIsClosed
        //{
        //    get
        //    {
        //        foreach (Head header in this.Owner.TestItems)
        //        {
        //            if (!header.Enabled)
        //                continue;
        //            if (!(header.BindingObject as HeadObject).AllJigIsClosed)
        //                return false;
        //        }
        //        return true;
        //    }
        //}

        /// <summary>
        /// 获取当前设备
        /// </summary>20220310
        public Machine Machine
        {
            get
            {
                return MyApp.GetInstance().Machine;
            }
        }

        /// <summary>
        /// 检查所有治具是否关闭到位
        /// </summary>
        /// 
        public bool AllJigIsClosedCheck
        {
            get
            {
                bool isClosed = true;
                foreach (Turntable tt in Machine.TestItems)//Owner.TestItems
                {
                    foreach (Head head in tt.TestItems)
                    {
                        HeadObject ho = head.BindingObject as HeadObject;
                        if (!ho.HeadClosedCheck)
                        {
                            isClosed = false;
                            break;
                        }
                    }
                }
                return isClosed;
            }
        }

    }
}
