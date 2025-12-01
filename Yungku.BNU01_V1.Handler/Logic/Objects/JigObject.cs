using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.Objects
{
    class JigObject : MachineBase
    {
        //private GPIOMap cylinder;
        //private GPIOMap CloseSensor;
        //private GPIOMap OpenSensor;
        private GPIOMap CloseCheck;
        /// <summary>
        /// 判断治具是否打开
        /// </summary>
        //public bool IsOpened
        //{
        //    get
        //    {
        //        if (CloseSensor == null)
        //            return false;
        //        return !CloseSensor.Value;
        //    }
        //}

        /// <summary>
        /// 判断治具是否关闭
        /// </summary>
        //public bool IsClosed
        //{
        //    get
        //    {
        //        if (CloseSensor == null)
        //            return false;
        //        return CloseSensor.Value;
        //    }
        //}
        /// <summary>
        /// 判断治具关闭是否到位
        /// </summary>
        public bool IsCloseCheck
        {
            get
            {
                if (CloseCheck == null)
                    return false;
                return CloseCheck.Value;
            }
        }
        //public void CloseJig()
        //{
        //    if (cylinder == null)
        //        OnAlarm("治具气缸未初始化");
        //    cylinder.Set();
        //}
        //public void OpenJig()
        //{
        //    if (cylinder == null)
        //        OnAlarm("治具气缸未初始化");
        //    cylinder.Reset();
        //}
        private JigObject(int index)
        {
            this.Name = "Jig" + index.ToString();
            switch (index)
            {
                //case 0:
                //    //cylinder = MyApp.HW.AJigCylinder_Out;
                //    //OpenSensor = MyApp.HW.AJigOpenSensor_in;
                //    //CloseSensor = MyApp.HW.AJigCloseSensor_in;
                //    CloseCheck = MyApp.HW.AJigCloseSensor;
                //    break;
                //case 1:
                //    //cylinder = MyApp.HW.BJigCylinder_Out;
                //    //OpenSensor = MyApp.HW.BJigOpenSensor_in;
                //    //CloseSensor = MyApp.HW.BJigCloseSensor_in;
                //    CloseCheck = MyApp.HW.BJigCloseSensor;
                //    break;
                //case 2:
                //    //cylinder = MyApp.HW.CJigCylinder_Out;
                //    //OpenSensor = MyApp.HW.CJigOpenSensor_in;
                //    //CloseSensor = MyApp.HW.CJigCloseSensor_in;
                //    CloseCheck = MyApp.HW.CJigCloseSensor;
                //    break;
                //case 3:
                //    //cylinder = MyApp.HW.DJigCylinder_Out;
                //    //OpenSensor = MyApp.HW.DJigOpenSensor_in;
                //    //CloseSensor = MyApp.HW.DJigCloseSensor_in;
                //    CloseCheck = MyApp.HW.DJigCloseSensor;
                //    break;
            }
        }
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
                    foreach (YungkuSystem.TestFlow.Jig jig in head.TestItems)
                    {
                        jig.BindingObject = new JigObject(index++);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有治具的名称
        /// </summary>
        /// <param name="machine"></param>
        public static List<string> GetNames(Machine machine)
        {
            List<string> names = new List<string>();
            foreach (Turntable tt in machine.TestItems)
            {
                foreach (Head head in tt.TestItems)
                {
                    foreach (YungkuSystem.TestFlow.Jig jig in head.TestItems)
                    {
                        JigObject j = jig.BindingObject as JigObject;
                        if (j != null)
                            names.Add(j.Name);
                    }
                }
            }
            return names;
        }
    }
}
