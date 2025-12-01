using System;
using System.Net;
using YungkuSystem.Globalization;
using YungkuSystem.Machine;
using YungkuSystem.ThreadMessage;

namespace Yungku.BNU01_V1.Handler.Logic
{
    class ActionHome : LogicObject
    {
        private Objects.MachineObject currentMachineObject = null;

        public ActionHome()
        {
            currentMachineObject = MyApp.GetInstance().Machine.BindingObject as Objects.MachineObject;
        }

        public override bool CheckAddAction(ActionExecuter exe)
        {
            
            return true;
        }

        double pos = 0;
        //bool homeIsDone=false;
        protected override void Handle()
        {
            if (!CheckSafety())
                return;

            switch (StateIndex)
            {
                case ActionBase.ACT_STATE_START:
                    #region
                    MyApp.NeedReset = true;
                    MyApp.ShareData.ishoming = true;
                    YungkuSystem.Controls.FormResult.AllClose();
                  
                        To(ActionBase.ACT_STATE_END);
                  
                    #endregion
                    break;

                case ActionBase.ACT_STATE_END:
                    #region 
                    //MyApp.HW.ResultLed.ResetAllLed();
                   // MyApp.HW.StopAllMotor();
                    MyApp.NeedReset = false;
                    MyApp.ShareData.ishoming = false;

                    //停止主流程，更方便使用    
                    MyApp.GetInstance().Logic.DefaultExecuter.Stop();
                    Finish();
                    #endregion
                    break;
            }
        }

        protected override void OnMotionException(Exception ex)
        {
            Logger.WriteError(ex.Message);
            OnAlarm(ex.Message);
        }

    }
}
