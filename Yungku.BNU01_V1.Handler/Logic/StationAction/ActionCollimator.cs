using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Structs;
using YungkuSystem.TestFlow;
using YungkuSystem.ThreadMessage;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    public enum CollimatorType
    {
        光管复位到无穷远处,
        调整平行光管模拟距离,
        设置光管色温和照度,
        关闭平行光管,
    }

    public class ActionCollimator : ActionObject
    {
        public override string ObjectClass
        {
            get { return "平行光管控制"; }
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

        [MyDisplayName("运动到模拟距离[mm]"), MyCategory("平行光管参数组"), Description("null")]
        public float Collimator_FX { get; set; } = 0.00f;       //运动到模拟距离，fX为要运动到的模拟距离(单位mm)
        [MyDisplayName("平行光管色温值"), MyCategory("平行光管参数")]
        public int Collimator_CCT { get; set; } = 100;

        [MyDisplayName("平行光管亮度参数1/2"), MyCategory("平行光管参数")]
        public int Collimator_ExSetNum { get; set; } = 1;

        [MyDisplayName("平行光管动作类型"), MyCategory("参数")]
        public CollimatorType CollimatorType { get; set; } = CollimatorType.关闭平行光管;

        [MyDisplayName("是否等待平行光管到位"), MyCategory("平行光管参数")]
        public bool isWaitCollimator { get; set; } = true;//Luminance

        public YesNo result = true;

        public YesNo Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// 复制对象成员
        /// </summary>
        /// <param name="dest"></param>
        protected override void CloneMembers(YungkuSystem.Script.Core.Base dest)
        {
            base.CloneMembers(dest);
            ActionCollimator obj = dest as ActionCollimator;
            //obj.luminance = this.luminance;
            //obj.distance = this.distance;
            //obj.cCT = this.cCT;
            //obj.mode = this.mode;
            obj.CollimatorType = this.CollimatorType;
            obj.Collimator_FX = this.Collimator_FX;
            obj.Collimator_CCT = this.Collimator_CCT;
        }

        public override void Binding()
        {
            base.Binding();
            
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
                        if (!HasPass && !MustExecute)
                        {
                            To(ACT_STATE_END);
                        }
                        else
                        {
                            WriteRecord($"开始平行光管控制：{CollimatorType.ToString()}");
                            if (CollimatorType == CollimatorType.光管复位到无穷远处)
                            {
                                To("光管复位到无穷远处");
                            }
                            if (CollimatorType == CollimatorType.调整平行光管模拟距离)
                            {
                                To("调整平行光管模拟距离");
                            }
                            else if (CollimatorType == CollimatorType.设置光管色温和照度)
                            {
                                To("调整平行光管色温和亮度");
                            }
                            else if (CollimatorType == CollimatorType.关闭平行光管)
                            {
                                To("关闭平行光管LED");
                            }
                        }
                        #endregion
                        break;

                    case "光管复位到无穷远处":
                        #region                                                  
                        if (CollimatorHoming())
                        {
                            To(ACT_STATE_END);
                        }
                        #endregion
                        break;

                    case "调整平行光管模拟距离":
                        #region       
                        if (isWaitCollimator)
                        {
                            if (CollimatorDistance())
                            {
                                Watcher.StopWatch(StateIndex);
                                WriteRecord($"调整平行光管模拟距离：{Collimator_FX}");
                                //To(ACT_STATE_END);
                                To("确认平行光管模拟距离到位");
                            }
                            else if (Watcher.StartCheckIsTimeout(StateIndex, 10000))
                            {
                                To("OnAlarm");
                                OnAlarm(G.Text("平行光管色温和亮度调整失败!"));
                            }
                        }
                        else
                        {
                            Task.Run(new Action(() =>
                            {                     
                                CollimatorDistance();
                                WriteRecord($"调整平行光管模拟距离：{Collimator_FX}");
                            }));
                            To(ACT_STATE_END);
                        }                                                                                      
                        #endregion
                        break;

                    case "调整平行光管色温和亮度":
                        #region            
                        if (isWaitCollimator)
                        {
                            if (CollimatorCCTAndEX())
                            {
                                WriteRecord($"调整平行光管：色温:{Collimator_CCT} 亮度:{MyApp.Config.General.Collimator_EX}");
                                Watcher.StopWatch(StateIndex);
                                To(ACT_STATE_END);
                            }
                            else if (Watcher.StartCheckIsTimeout(StateIndex, 10000))
                            {
                                To("OnAlarm");
                                OnAlarm(G.Text("平行光管色温和亮度调整失败!"));
                            }
                        }
                        else
                        {
                            Task.Run(new Action(() =>
                            {
                                CollimatorCCTAndEX();
                                WriteRecord($"调整平行光管：色温:{Collimator_CCT} 亮度:{MyApp.Config.General.Collimator_EX}");
                            }));

                            To(ACT_STATE_END);
                        }

                        #endregion
                        break;

                    case "关闭平行光管LED":
                        #region                                                  
                        if (CollimatorCloseLED())
                        {
                            Watcher.StopWatch(StateIndex);
                            To(ACT_STATE_END);
                        }
                        else if (Watcher.StartCheckIsTimeout(StateIndex, 10000))
                        {
                            To("OnAlarm");
                            OnAlarm(G.Text("关闭平行光管LED失败!"));
                        }
                        #endregion
                        break;

                    case "确认平行光管模拟距离到位":
                        #region                                 
                        if (Watcher.StartCheckIsTimeout("平行光管状态检测延时", 500))
                        {
                            Watcher.StopWatch("平行光管状态检测延时");
                            //if (ElectricCollimator.status) 
                            if (Astatus())
                            {
                                Watcher.StopWatch(StateIndex);
                                for (int i = 0; i < 9; i++)
                                {
                                    statuses[i] = false;
                                }
                                To(ACT_STATE_END);
                            }
                            else if (Watcher.StartCheckIsTimeout(StateIndex, 10000))//10000
                            {
                                OnAlarm(G.Text("平行光管模拟距离到位超时!"));
                                To("OnAlarm");
                            }
                            //}
                        }
                        #endregion
                        break;

                    case ACT_STATE_END:
                        #region
                        Finish();
                        WriteRecord("平行光管控制完成");
                        #endregion
                        break;

                    case "OnAlarm":
                        #region
                        //获取当前站位和对应转盘上的测试头
                        Station st = Station.Station;
                        Head hd = st.Turntable.GetHeadByStation(st);
                        //站位或测试头有一个被关闭时都不应该继续执行
                        if (!st.Enabled || !hd.Enabled)
                            return;

                        //遍历测试头中的站位对象
                        foreach (Jig jig in hd.TestItems)
                        {
                            if (jig.Enabled)
                            {
                                foreach (Product product in jig.TestItems)
                                {
                                    product.Result = TestResult.Fail;
                                }
                            }
                        }
                        To(ACT_STATE_END);
                        #endregion
                        break;

                    default:
                        #region
                        OnAlarm(G.Text("平行光管Action程序逻辑错误!"), true);
                        State = YungkuSystem.Script.Core.ActionState.Error;
                        To(ACT_STATE_END);
                        #endregion
                        break;
                }
            }
            catch (Exception ex)
            {
                OnAlarm($"Exception:Message-{ex.Message} StackTrace:{ex.StackTrace}", true);
            }
        }

        /// <summary>
        /// 光管复位
        /// </summary>
        /// <returns></returns>
        private bool CollimatorHoming()
        {
            for (byte add = 1; add < 10; add++)
            {
                ElectricCollimator.MX_Homing(add);
                WriteRecord($"初始化地址为[{add}]的光管");
            }
            return true;
        }

        /// <summary>
        /// 关闭平行光管
        /// </summary>
        /// <returns></returns>
        private bool CollimatorCloseLED()
        {
            if (CollimatorType == CollimatorType.关闭平行光管)
            {
                bool b = true;
                bool b1;
                for (byte add = 1; add < 10; add++)
                {
                    b1 = ElectricCollimator.MX_CloseLedEx(add);
                    b = b & b1;
                }
                return b;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置模拟距离参数
        /// </summary>
        /// <returns></returns>
        private bool CollimatorDistance()
        {
            for (byte add = 1; add < 10; add++)
            {
                ElectricCollimator.MoveAbsByFun(add, Collimator_FX, ElectricCollimator.ProductType);
                //ElectricCollimator.MX_SetLedEx(i, Collimator_CCT, Collimator_EX, ref ElectricCollimator.DUV);
            }
            return true;
        }

        /// <summary>
        /// 设置色温亮度参数并打开平行光管
        /// </summary>
        /// <returns></returns>
        private bool CollimatorCCTAndEX()
        {
            for (byte add = 1; add < 10; add++)
            {
                if (Collimator_ExSetNum == 1)                               
                    ElectricCollimator.MX_SetLedEx(add, Collimator_CCT, MyApp.Config.General.Collimator_EX[add-1], ref ElectricCollimator.DUV);
                else
                    ElectricCollimator.MX_SetLedEx(add, Collimator_CCT, MyApp.Config.General.Collimator_EX2[add - 1], ref ElectricCollimator.DUV);
            }
            return true;
        }

        private bool[] statuses = new bool[9] { false, false, false, false, false, false, false, false, false };
        private bool status;
        /// <summary>
        /// 查询平行光管状态
        /// </summary>
        /// <returns></returns>
        private bool Astatus()
        {
            ushort Status = 0;
            byte i = 0;
            status = true;
            //for (byte i = 1; i < 10; i++)
            foreach (bool b in statuses)
            {
                #region
                i++;
                //if (GetStatus(i, ref Statuses[i], productType))
                if (!b && ElectricCollimator.GetStatus(i, ref Status, ElectricCollimator.ProductType))
                {
                    //switch (Statuses[i
                    switch (Status)
                    {
                        //case 1:
                        //    MessageBox.Show("定位中（零位不确定）");
                        //    break;
                        case 2:
                            MyApp.GetInstance().Logger.WriteInfo("[平行光管状态]:(零位确定)定位完成-平行光管" + i.ToString());
                            statuses[i - 1] = true;
                            break;
                        case 3:
                            MyApp.GetInstance().Logger.WriteInfo("[平行光管状态]:移动定位中（零位确定)-平行光管" + i.ToString());
                            //i = 10;
                            return status = false;
                            //break;
                        case 4:
                            MyApp.GetInstance().Logger.WriteError("[平行光管状态]:过流或过热（零位不确定）-平行光管" + i.ToString());
                            return status = false;
                            //break;
                        case 6:
                            MyApp.GetInstance().Logger.WriteError("[平行光管状态]:过流或过热（零位确定）-平行光管" + i.ToString());
                            return status = false;
                            //break;
                        case 8:
                            MyApp.GetInstance().Logger.WriteError("[平行光管状态]:软件限位或光电开关限位（零位不确定）-平行光管" + i.ToString());
                            return status = false;
                            //break;
                        case 10:
                            MyApp.GetInstance().Logger.WriteError("[平行光管状态]:软件限位或光电开关限位（零位确定）-平行光管" + i.ToString());
                            return status = false;
                           // break;
                    }
                }
                #endregion
            }
            return status;
        }

    }
}
