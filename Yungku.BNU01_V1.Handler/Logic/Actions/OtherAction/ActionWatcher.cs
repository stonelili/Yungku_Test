using System;
using System.Threading;
using YungkuSystem.Machine;
using YungkuSystem.TestFlow;
using System.Windows.Forms;
using YungkuSystem.Globalization;

namespace Yungku.BNU01_V1.Handler.Logic
{

    public class ActionWatcher : LogicObject
    {
        private bool emgStopState = false;
        private bool positivePressureCheck = false;
        private bool stopButtonState = false;
        private bool resetButtonState = false;
        private bool startButtonState = false;
        private bool doorSensorState = true;
        protected override void Handle()
        {
            //if (!MyApp.GetInstance().MotionSystem.IsInitialized)
            //{
            //    Thread.Sleep(100);
            //    return;
            //}

        }

        private static ProcessEnum[] UpProcess = new ProcessEnum[100];//定义一个枚举数组

        public static bool[] AlarmFlag = new bool[14];
       


        
      

        public void in_PositivePressureCheck_Check(int index)
        {
            #region
            //bool ret = !MyApp.HW.PositivePressureCheck.Value;
            //switch (UpProcess[index])
            //{
            //    case ProcessEnum.WaitTrig:
            //        if (ret)
            //        {
            //            UpProcess[index] = ProcessEnum.DoWork;
            //        }
            //        break;
            //    case ProcessEnum.DoWork:
            //        Logger.WriteRecord("供气异常！");
            //        OnAlarm("供气气压异常");
            //        AlarmFlag[4] = true;
            //        UpProcess[index] = ProcessEnum.End;

            //        break;
            //    case ProcessEnum.End:
            //        if (!ret)
            //        {
            //            AlarmFlag[4] = false;
            //            Logger.WriteRecord("供气正常！");
            //            UpProcess[index] = ProcessEnum.WaitTrig;
            //        }
            //        break;
            //}
            #endregion
        }

        public void in_PowerLed_Check(int index)
        {
            //bool ret = true;
            switch (UpProcess[index])
            {
                case ProcessEnum.WaitTrig:
                    #region
                    //    if (ret)
                    //    {
                    //        UpProcess[index] = ProcessEnum.DoWork;
                    //    }
                    #endregion
                    break;

                case ProcessEnum.DoWork:
                    #region
                    Logger.WriteRecord("急停按钮按下！");
                    //MyApp.HW.EmgStopAllMotor();
                    OnAlarm("急停按钮按下，设备需要重新复位！");
                    MyApp.NeedReset = true;
                    AlarmFlag[5] = true;
                    UpProcess[index] = ProcessEnum.End;
                    #endregion
                    break;

                case ProcessEnum.End:
                    #region
                    //if (!ret)
                    //{
                    //    AlarmFlag[5] = false;
                    //    Logger.WriteRecord("急停按钮弹起！");
                    //    UpProcess[index] = ProcessEnum.WaitTrig;
                    //}
                    #endregion
                    break;
            }
        }

        public void in_MachineJig_Check(int index)
        {
            #region
            //bool ret = false;
            //if (MyApp.HW.JigCloseSensor != null)
            //{
            //    if (MyApp.GetInstance().Logic.DefaultExecuter.IsRunning && !MyApp.HW.IO_In_Cylinder_FB_In.Value)
            //        ret = !MyApp.HW.JigCloseSensor.GetValue();
            //}
            //switch (UpProcess[index])
            //{
            //    case ProcessEnum.WaitTrig:
            //        if (ret)
            //        {
            //            UpProcess[index] = ProcessEnum.DoWork;
            //        }
            //        break;
            //    case ProcessEnum.DoWork:
            //        //  Logger.WriteRecord("有治具未关闭！");
            //        OnAlarm("有治具未关闭");
            //        AlarmFlag[index] = true;
            //        UpProcess[index] = ProcessEnum.End;
            //        break;
            //    case ProcessEnum.End:
            //        if (!ret)
            //        {
            //            AlarmFlag[index] = false;
            //            //  Logger.WriteRecord("治具全部关闭！");
            //            UpProcess[index] = ProcessEnum.WaitTrig;
            //        }
            //        break;
            //}
            #endregion
        }

        public static bool CheckAlarmFlag()
        {
            bool ret = false;
            for (int i = 0; i < AlarmFlag.Length; i++)
            {
                ret = ret || AlarmFlag[i];
            }
            return ret;
        }

        public static void InitializeEnum()
        {
            for (int i = 14; i >= 0; i--)
            {
                UpProcess[i] = ProcessEnum.WaitTrig;
            }
        }

        public enum ProcessEnum
        {
            WaitTrig, //等待触发信号
            DoWork,   //处理数据
            End,   //
        }

    }
}