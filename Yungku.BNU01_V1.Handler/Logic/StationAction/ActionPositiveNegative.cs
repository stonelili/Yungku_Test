using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;


//======================================================================
//
//        Copyright (C) 2019-2022 深圳市涌固精密治具有限公司    
//        All rights reserved
//
//        Filename :ActionEngine
//        Description :
//
//        Created by 吴华明 at:  2024/10/10 9:33:58
//        Email:wu.hm@yungku.com
//
//        Modify by 吴华明 at time: 2024/10/10 9:33:58
//        Content :  修正参数保存Bug
//======================================================================

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    public enum PositiveNegativeType
    {
        正面环境,
        背面环境,
    }
    public class ActionPositiveNegative : ActionObject
    {
        public override string ObjectClass
        {
            get { return "正背面环境"; }
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

        public bool IsSucceed { get; set; } = true;
        private bool limitFalg = true;

        [MyDisplayName("正反面环境类型"), MyCategory("参数")]
        public PositiveNegativeType StationPositiveNegativeType { get; set; } = PositiveNegativeType.正面环境;

        /// <summary>
        /// 复制对象成员
        /// </summary>
        /// <param name="dest"></param>
        protected override void CloneMembers(YungkuSystem.Script.Core.Base dest)
        {
            base.CloneMembers(dest);
            ActionPositiveNegative obj = dest as ActionPositiveNegative;
            obj.StationPositiveNegativeType = this.StationPositiveNegativeType;
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
                            To(ACT_STATE_END);
                        }
                        if (!CurrentHeadObject.HasModudeState)
                        {
                            To(ACT_STATE_END);
                        }
                        else if (!HasPass && !MustExecute)
                        {
                            To(ACT_STATE_END);
                        }
                        else
                        {
                            if (StationPositiveNegativeType == PositiveNegativeType.正面环境)
                            {
                                To("将相机翻转到正面");
                            }
                            else
                            {
                                To("将相机翻转到背面");
                            }
                        }
                        #endregion
                        break;

                    case "将相机翻转到正面":
                        #region
                        //if (MyApp.HW.IO_In_Cylinder_UDOut.Value && MyApp.HW.IO_In_Cylinder_FBOut.Value)
                        //{
                        //    Watcher.StopWatch(StateIndex);
                        //    WriteRecord(G.Text($"当前轴位置:{MyApp.HW.AxisR.Position.ToString("0.000")}"));
                        //    To("等待将相机翻转到正面完成");
                        //}
                        //else if (Watcher.StartCheckIsTimeout(StateIndex, 5000, false))
                        //{
                        //    string alarm = MyApp.HW.IO_In_Cylinder_UDOut.Value ? "上下气缸没有伸出" : "前后气缸没有到位";
                        //    OnAlarm(G.Text($"{alarm}，请检查！"));
                        //}
                        #endregion
                        break;
                    case "等待将相机翻转到正面完成":
                        #region 
                        //MoveFinished = MoveTo(MyApp.HW.AxisR, MyApp.Config.General.AxisYLoadPosition);
                        //limitFalg = MyApp.Config.FunctionSwitch.IsSensor ? MyApp.HW.AxisR.PositiveLimit : MoveFinished;
                        //if (MoveFinished)
                        //{
                        //    Watcher.StopWatch(StateIndex);
                        //    WriteRecord(G.Text($"当前轴位置:{MyApp.HW.AxisR.Position.ToString("0.000")}"));                           
                        //    To(ACT_STATE_END);
                        //}
                        //else if (Watcher.StartCheckIsTimeout(StateIndex, 10000, false))
                        //{
                        //    IsSucceed = false;                          
                        //    WriteRecord(G.Text($"当前轴位置:{MyApp.HW.AxisR.Position.ToString("0.000")}"));
                        //    OnAlarm("等待相机翻转到正面超时，请检查！");
                        //}
                        #endregion
                        break;

                    case "将相机翻转到背面":
                        #region
                        //if (MyApp.HW.IO_In_Cylinder_UDOut.Value && MyApp.HW.IO_In_Cylinder_FBOut.Value)
                        //{
                        //    Watcher.StopWatch(StateIndex);
                        //    WriteRecord(G.Text($"当前轴位置:{MyApp.HW.AxisR.Position.ToString("0.000")}"));                          
                        //    To("等待将相机翻转到背面完成");
                        //}
                        //else if (Watcher.StartCheckIsTimeout(StateIndex, 10000, false))
                        //{
                        //    string alarm = MyApp.HW.IO_In_Cylinder_UDOut.GetValue() ? "上下气缸没有伸出" : "前后气缸没有到位";
                        //    OnAlarm(G.Text($"{alarm}，请检查！"));
                        //}
                        #endregion
                        break;
                    case "等待将相机翻转到背面完成":
                        #region 
                        //MoveFinished = MoveTo(MyApp.HW.AxisR, MyApp.Config.General.AxisYTestPosition);
                        //limitFalg = MyApp.Config.FunctionSwitch.IsSensor ? MyApp.HW.AxisR.NegativeLimit : MoveFinished;                      
                        //if (MoveFinished)
                        //{
                        //    Watcher.StopWatch(StateIndex);
                        //    WriteRecord(G.Text($"当前轴位置:{MyApp.HW.AxisR.Position.ToString("0.000")}"));
                        //    To(ACT_STATE_END);
                        //}
                        //else if (Watcher.StartCheckIsTimeout(StateIndex, 10000, false))
                        //{
                        //    IsSucceed = false;
                        //    OnAlarm("等待相机翻转到背面超时，请检查！");
                        //}
                        #endregion
                        break;

                    case ACT_STATE_END:
                        #region
                        if (StationPositiveNegativeType == PositiveNegativeType.正面环境)
                        {
                            WriteInfo("正面环境完成！");
                        }
                        else
                        {
                            WriteInfo("反面环境完成！");
                        }
                        Finish();
                        #endregion
                        break;

                    default:
                        OnAlarm(G.Text("正反面Action程序逻辑错误!"), true);
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
