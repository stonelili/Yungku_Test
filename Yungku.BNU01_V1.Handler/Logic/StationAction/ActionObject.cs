using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yungku.BNU01_V1.Handler.Config;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using Yungku.BNU01_V1.Handler.Other;
using YungkuSystem;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Log;
using YungkuSystem.Motion;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Script.Actions;
using YungkuSystem.Script.Core;
using YungkuSystem.Structs;
using YungkuSystem.TestFlow;
using YungkuSystem.Script.Actions.DeviceBase;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    public abstract class ActionObject : ActionRoot
    {

        public ActionObject()
        {
            StateIndexChanged += ActionObject_StateIndexChanged;
        }

        private void ActionObject_StateIndexChanged(object sender, EventArgs e)
        {
            MyApp.GetInstance().Logger.WriteInfo(this.GetFullPath() + ":" + StateIndex);
        }

        [MyDisplayName("站位对象"), MyCategory("监视信息")]
        public DeviceStation Station
        {
            get
            {
                foreach (Device dev in this.Owner.Devices.Root.Childs)
                {
                    if (dev is DeviceStation)
                        return dev as DeviceStation;
                }
                return null;
            }
        }

        /// <summary>
        /// 有效硬件
        /// </summary>
        protected void ValidHardware()
        {
            ValidDevice(Station);
            if (!MyApp.GetInstance().MotionSystem.IsInitialized)
            {
                throw new AccessViolationException("运控系统没有初始化！");
            }
        }

        public void WriteInfo(string msg, bool error = false)
        {
            if (error)
                MyApp.GetInstance().Logger.WriteError(string.Format("{0}:{1}", FullPath, msg));
            else
                MyApp.GetInstance().Logger.WriteInfo(string.Format("{0}:{1}", FullPath, msg));
        }

        public void WriteRecord(string msg)
        {
                MyApp.GetInstance().Logger.WriteRecord(string.Format("{0}:{1}", FullPath, msg));
        }
        /// <summary>
        /// 标识多轴移动状态
        /// </summary>
        public bool MoveFinished = true;
        /// <summary>
        /// 配置文件
        /// </summary>
        public AppConfig Config
        {
            get { return MyApp.Config; }
        }
        /// <summary>
        /// 获取当前设备
        /// </summary>
        public Machine Machine
        {
            get
            {
                return MyApp.GetInstance().Machine;
            }
        }
        /// <summary>
        /// 获取转盘
        /// </summary>
        public Turntable Turntable
        {
            get { return Machine.TestItems[0] as Turntable; }
        }
        public HeadObject CurrentHeadObject
        {
            get
            {
                Head hd = Station.Station.GetCurrentHead();
                return hd.BindingObject as HeadObject;
            }
        }
        public HeadObject IndexStationHeadObject(int index)
        {
            if (Turntable != null)
                return Turntable.GetHead(index).BindingObject as HeadObject;
            else
                return null;
        }
        public Head CurrentHead
        {
            get
            {
                Head hd = Station.Station.GetCurrentHead();
                return hd;
            }
        }
        public List<ProductObject> CurrentProductObect
        {
            get
            {
                List<ProductObject> productObject = new List<ProductObject>();
                foreach (Jig jig in CurrentHead.TestItems)
                {
                    foreach (Product product in jig.TestItems)
                    {
                        productObject.Add(product.BindingObject as ProductObject);
                    }
                }
                return productObject;
            }
        }
        /// <summary>
        /// 判断此测试站对应的测试头上的产品是否存在Pass产品
        /// 此标志位指示流程是否需要继续往下执行
        /// </summary>
        [MyDisplayName("当前站位是否存在良品"), MyCategory("监视信息")]
        public bool HasPass
        {
            get
            {
                Head hd = Station.Station.GetCurrentHead();
                foreach (Jig jig in hd.TestItems)
                {
                    foreach (Product product in jig.TestItems)
                    {
                        if (product.Result == TestResult.Pass)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public bool IsEmpty
        {
            get
            {
                Head hd = Station.Station.GetCurrentHead();
                foreach (Jig jig in hd.TestItems)
                {
                    foreach (Product product in jig.TestItems)
                    {
                        if (product.Result == TestResult.Empty)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }


        public void SetProductState(TestResult testState, string resultCode)
        {
            Head hd = Station.Station.GetCurrentHead();
            foreach (Jig jig in hd.TestItems)
            {
                foreach (Product product in jig.TestItems)
                {
                    if (product.Result == TestResult.Pass)
                    {
                        product.Result = testState;
                        product.ResultCode = resultCode;
                    }
                }
            }
        }
        /// <summary>
        /// 非阻塞的移动指定的电机，可以重复调用
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool MoveTo(AxisMap axis, double pos, double precision = 0.02)
        {
            //如果处于运行过程中，则返回未完成
            if (axis.IsBusy)
                return false;

            //如果位置到达则直接返回成功
            if (Math.Abs(axis.Position - pos) < precision)
                return true;

            //如果不在运行过程中且又没有到达位置，则启动运行
            try
            {
                axis.AbsMove(pos);
            }
            catch (Exception ex)
            {
                OnAlarm(ex.Message);
            }

            return false;
        }

        private YesNo mustExecute = false;
        [MyDisplayName("必须执行"), MyCategory("是否必须执行"),
        Description("默认情况下，当前测试头上的产品全部Fail时，将不执行，只有设置此属性才能继续执行。")]
        public YesNo MustExecute
        {
            get { return mustExecute; }
            set { mustExecute = value; }
        }


        public AxisMap ReturnAxisByName(string name)
        {
            List<AxisMap> maps = new List<AxisMap>();
            // maps.AddRange(Hardware.Axes);
            foreach (AxisMap ax in maps)
            {
                if (ax.Params.MapName.ToString().Equals(name))
                {
                    return ax;
                }
            }
            return null;
        }
        protected GPIOMap ReturnIOByName(string name)
        {
            List<GPIOMap> maps = new List<GPIOMap>();
            //   maps.AddRange(Hardware.Inputs);
            //   maps.AddRange(Hardware.Outputs);
            foreach (GPIOMap ax in maps)
            {
                if (ax != null && ax.Params.MapName.Equals(name))
                {
                    return ax;
                }
            }
            return null;
        }
        /// <summary>
        /// 回原点控制类
        /// </summary>
        public HomeHelper HomeHelper
        {
            get { return MyApp.HomeHelper; }
        }
        /// <summary>
        /// 复制对象成员
        /// </summary>
        /// <param name="dest"></param>
        protected override void CloneMembers(Base dest)
        {
            base.CloneMembers(dest);
            ActionObject obj = dest as ActionObject;
            obj.mustExecute = this.mustExecute;
        }

        /// <summary>
        /// 调焦夹头气缸操作
        /// </summary>
        /// <param name="Set"></param>
        /// <returns></returns>
        //public bool FocusCyliner(bool Set = false)
        //{
        //    MyApp.HW.FocusCyliner.Set(Set);
        //    return GetFocusCylinerSensor(Set);
        //}
        //public bool GetFocusCylinerSensor(bool set = false)
        //{
        //    bool ret = false;
        //    ret = set ? !MyApp.HW.FocusAGripperDown.GetValue() && MyApp.HW.FocusAGripperUp.GetValue() :
        //                MyApp.HW.FocusAGripperDown.GetValue() && !MyApp.HW.FocusAGripperUp.GetValue();
        //    return ret;
        //}
    }
}
