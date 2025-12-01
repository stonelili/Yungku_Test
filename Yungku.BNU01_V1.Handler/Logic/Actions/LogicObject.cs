using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Yungku.BNU01_V1.Handler.Config;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using YungkuSystem;
using YungkuSystem.AlarmManage;
using YungkuSystem.Globalization;
using YungkuSystem.Machine;
using YungkuSystem.Motion;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic
{
    public abstract class LogicObject : ActionBase
    {
        public LogicObject()
        {
            StateIndexChanged += LogicObject_StateIndexChanged;
        }
        public LogicObject(Turntable turntable)
        {
            //this.Turntable = turntable;
            StateIndexChanged += LogicObject_StateIndexChanged;
        }

        public bool MoveFinished = true;

        #region 调试触发事件
        public event EventHandler<int> MoveChanged;
        /// <summary>
        /// 当切换移动坐标系的时候触发，主要用于高度校准的时候
        /// </summary>
        protected void OnMoveChanged(int PosIndex)
        {
            if (MoveChanged != null)
                MoveChanged(this, PosIndex);
        }
        #endregion

        protected override YungkuSystem.Log.LogPublisher Logger
        {
            get { return MyApp.GetInstance().Logger; }
        }

        /// <summary>
        /// 获取动作的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogicObject_StateIndexChanged(object sender, EventArgs e)
        {
            Logger.WriteInfo(this.GetType().Name + ":" + StateIndex);
        }

        public override void Reset()
        {
            Watcher.ClearAllWatch();
            base.Reset();
        }

        /// <summary>
        /// 报警
        /// </summary>
        /// <param name="content"></param>
        /// <param name="needReset"></param>
        public void OnAlarm(string content, bool needReset = false)
        {
            MyApp.GetInstance().AlarmPublisher.Write(new Alarm(content, Alarm.ALARM_TYPE_DEFAULT, needReset));

        }

        #region 获取对象
        /// <summary>
        /// 回原点实例
        /// </summary>
        public HomeHelper HomeHelper
        {
            get { return MyApp.HomeHelper; }
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
        /// 获取当前设备实例
        /// </summary>
        public MachineObject MachineObject
        {
            get
            {
                return MyApp.GetInstance().Machine.BindingObject as MachineObject;
            }
        }
        /// <summary>
        /// 获取转盘
        /// </summary>
        public Turntable Turntable
        {
            get { return Machine.TestItems[0] as Turntable; }
        }
        /// <summary>
        /// 当前转盘实例(Turntable!=null)
        /// </summary>
        public TurntableObject CurrentTurntableObject
        {
            get
            {
                if (Turntable != null)
                    return Turntable.BindingObject as TurntableObject;
                else
                    return null;
            }
        }

        /// <summary>
        /// 获取当前测试头
        /// </summary>
        public Head CurrentHead
        {
            get
            {
                if (Turntable != null)
                    return Turntable.GetCurrent();
                else
                    return null;
            }
        }

        /// <summary>
        /// 获取当前测试头实例
        /// </summary>
        public HeadObject CurrentHeadObject
        {
            get
            {
                if (Turntable != null)
                    return Turntable.GetCurrent().BindingObject as HeadObject;
                else
                    return null;
            }
        }
        /// <summary>
        /// 获取对应编号测试头实例
        /// </summary>
        /// <param name="headIndex"></param>
        /// <returns></returns>
        public HeadObject IndexHeadGetHeadObject(int headIndex)
        {
            if (Turntable != null)
                return Turntable.GetHeadByIndex(headIndex).BindingObject as HeadObject;
            else
                return null;
        }
        /// <summary>
        /// 通过站位编号获取测试头
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HeadObject IndexStationHeadObject(int index)
        {
            if (Turntable != null)
                return Turntable.GetHead(index).BindingObject as HeadObject;
            else
                return null;
        }
        /// <summary>
        /// 获取当前站位对象（上料站位）
        /// </summary>
        public Station CurrentStation
        {
            get
            {
                if (Turntable != null)
                    return Turntable.GetStationByIndex(0);
                else
                    return null; ;
            }
        }
        #endregion

        public AppConfig Config
        {
            get { return MyApp.Config; }
        }

        /// <summary>
        /// 当前测试头增加二维码
        /// </summary>
        /// <param name="Barcode"></param>
        public void BindCodeToProduct(List<string> Barcodes)
        {
            int i = 0;
            foreach (Jig jig in CurrentHead.TestItems)
            {
                foreach (Product product in jig.TestItems)
                {
                    ProductObject po = product.BindingObject as ProductObject;
                    product.BarCodeString = Barcodes.Count > i ? Barcodes[i] : " ";
                    po.CodeString = Barcodes.Count > i ? Barcodes[i] : " ";
                    i++;
                }
            }
        }

        /// <summary>
        /// 停止所有站位的执行流程
        /// </summary>
        internal void StopAllStation()
        {
            for (int i = 0; i < Turntable.Stations.Count; i++)
            {
                Station st = Turntable.GetStationByIndex(i);
                st.Executor.Stop();
            }
        }

        /// <summary>
        /// 获取设备驱动信号
        /// </summary>
        /// <returns></returns>
        internal bool GetStartSingal()
        {
            bool startSingal = false;
            switch (Config.General.StartMode)
            {
                case StartMode.JigClosed://手动治具无法实现治具启动
                
                case StartMode.Loop:
                    startSingal = true;
                    break;
            }
            return startSingal;
        }

        /// <summary>
        /// 获取复位按钮信号
        /// </summary>
        /// <returns></returns>
        internal bool GetResetSingal()
        {
            bool resetSingal = false;
            #region
            //switch (Config.General.StartMode)
            //{
            //    case StartMode.JigClosed://手动治具无法实现治具启动
            //    case StartMode.StartButton:
            //        resetSingal = (MyApp.HW.ReTestButton.Value) && !MyApp.HW.StopButton.Value;
            //        break;
            //    case StartMode.Loop:
            //        resetSingal = true;
            //        break;
            //}
            ////skip = startSingal && MyApp.HW.SkipButton.Value;
            #endregion
            return resetSingal;
        }


        /// <summary>
        /// 检查所有治具是否关闭到位
        /// </summary>
        public bool AllJigIsClosedCheck
        {
            get
            {
                bool isClosed = true;
                foreach (Turntable tt in Machine.TestItems)
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

        public virtual bool CheckAddAction(ActionExecuter ml)
        {
            if (MyApp.RunDebugDemo)
            {
                return true;
            }
           
            //判断报警是否清除
            if (MyApp.GetInstance().AlarmPublisher.IsAlarm)
            {
                return false;
            }
            if (MyApp.NeedReset || MyApp.ShareData.ishoming)
            {
                OnAlarm(G.Text("设备需要复位！"));
                return false;
            }
            if (Turntable == null)
            {
                OnAlarm(G.Text("转盘未赋值！"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 安全检查(主流程获取)
        /// </summary>
        /// <returns></returns>
        public bool CheckSafety()
        {
            try
            {
                if (MyApp.RunDebugDemo)
                {
                    return true;
                }

                //判断报警是否清除
                if (MyApp.GetInstance().AlarmPublisher.IsAlarm)
                {
                    OnAlarm(G.Text("报警未清除"));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnAlarm(ex.Message);
                return false;
            }
        }
    }
}
