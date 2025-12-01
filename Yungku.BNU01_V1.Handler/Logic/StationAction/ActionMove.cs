using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Script.Actions.DeviceBase;
using YungkuSystem.Script.Core;
using YungkuSystem.Script.Datas;
using YungkuSystem.Structs;
using YungkuSystem.ThreadMessage;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    public class ActionMove : ActionObject
    {
        /// <summary>
        /// 对象类型标识
        /// </summary>
        /// 
        public override string ObjectClass
        {
            get { return "移动"; }
        }

        public override bool IsContainner
        {
            get { return false; }
        }
        public override bool IsRunState
        {
            get
            {
                return true;
            }
        }
        private string  selectedAxis = "";
        [Browsable(false)]
        public string SelectedAxis
        {
            get { return selectedAxis; }
            set { selectedAxis = value; }
        }
        private string selectedInt = "";
        [Browsable(false)]
        public string SelectedInt
        {
            get { return selectedInt; }
            set { selectedInt = value; }
        }
        private  DeviceAxisMap axisMove  = null;
        [MyDisplayName("指定轴"), MyCategory("移动")]
        [XmlIgnore]
        public DeviceAxisMap AxisMove
        {
            get { return axisMove; }
            set
            {
                if (axisMove != value)
                {
                    axisMove = value;
                    if (axisMove != null)
                        selectedAxis = axisMove.GetFullPath();
                    else
                        selectedAxis = "";
                }
            }
        }

        private IntData index = null;
        [MyDisplayName("输入值"), MyCategory("移动")]
        [XmlIgnore]
        public IntData Index
        {
            get { return index; }
            set
            {
                if (index != value)
                {
                    index = value;
                    if (index != null)
                        selectedInt = index.FullPath;
                    else
                        selectedInt = "";
                }
            }
        }
        private double position = 0;
        [MyDisplayName("目标位置"), MyCategory("移动")]
        public double Position
        {
            get { return position; }
            set { position = value; }
        }
        private int timeout = 10000;
        /// <summary>
        /// 超时时间
        /// </summary>
        [MyDisplayName("超时时间"), MyCategory("移动")]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        private YesNo waitDone = true;
        [MyDisplayName("是否等待到达"), MyCategory("移动")]
        public YesNo WaitDone
        {
            get { return waitDone; }
            set { waitDone = value; }
        }
        private AxisMap CurrentAxis;
        /// <summary>
        /// 复制对象成员
        /// </summary>
        /// <param name="dest"></param>
        protected override void CloneMembers(YungkuSystem.Script.Core.Base dest)
        {
            base.CloneMembers(dest);
            ActionMove obj = dest as ActionMove;
            obj.selectedAxis = this.selectedAxis;
            obj.position = this.position;
            obj.timeout = this.timeout;
        }
        public override void Binding()
        {
            base.Binding();
            axisMove = this.Owner.Devices.Root.GetBaseByPath(selectedAxis,
            typeof(DeviceAxisMap)) as DeviceAxisMap;
            index = this.Owner.Devices.Root.GetBaseByPath(selectedInt,
            typeof(IntData)) as IntData;
        }
        public override Form SettingForm
        {
            get
            {
                return base.SettingForm;
            }
        }
        protected override void Execute()
        {
            try
            {
                switch (StateIndex)
                {

                    case ACT_STATE_START:
                        #region    
                        ValidHardware();
                        Watcher.StopAllWatch();
                        if (MyApp.NeedReset || MyApp.ShareData.ishoming)
                        {
                            return;
                        }
                        if (axisMove == null)
                        {
                            OnAlarm(selectedAxis + "轴为NULL，请设置轴名称!");                           
                        }
                        else
                        {
                            To("初始化硬件");
                        }                       
                        #endregion
                        break;
                    case "初始化硬件":                     
                        CurrentAxis = axisMove.Axis;                       
                        if (CurrentAxis == null)
                        {
                            OnAlarm(selectedAxis+"轴为NULL，请设置轴名称!");
                        }                      
                        else
                        {
                            if (!CurrentHeadObject.HasModudeState )
                            {
                                To(ACT_STATE_END);
                            }
                            else if (!HasPass && !MustExecute )
                            {
                                To(ACT_STATE_END);
                            }
                            else
                            {
                                To("开始轴移动");
                            }

                        }
                        break;

                    case "开始轴移动":
                        #region
                        MoveFinished = MoveTo(CurrentAxis, position);                      
                        if (MoveFinished ||!waitDone)
                        {
                            Watcher.StopWatch(StateIndex);
                            To(ACT_STATE_END);
                        }
                        else if (Watcher.StartCheckIsTimeout(StateIndex, timeout))
                        {
                            OnAlarm(selectedAxis+"移动失败!");
                        }
                        #endregion
                        break;               
                    case ACT_STATE_END:                   
                        Finish();
                        break;
                    default:
                        OnAlarm("轴移动程序逻辑错误!", true);
                        State = YungkuSystem.Script.Core.ActionState.Error;
                        To(ACT_STATE_END);
                        break;
                }
            }
            catch (Exception ex)
            {
                OnAlarm(ex.Message, true);
            }
        }
        
    }
}
