using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using YungkuSystem.Globalization;
using YungkuSystem.Machine;


namespace Yungku.BNU01_V1.Handler.Logic.Actions
{
    public class ActionCommands : LogicObject
    {
        private MachineObject currentMachineObject = null;

        private bool limitFalg = true;

        public float[] commandPositions { get; set; } = new float[9] { 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000 };//位置

        public float[] curLux { get; set; } = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//亮度

        public string CommandsType { get; set; } = string.Empty;
        public bool IsSucceed { get; set; } = true;
        public ActionCommands(string commandsType)
        {
            this.currentMachineObject = MyApp.GetInstance().Machine.BindingObject as MachineObject;
            this.CommandsType = commandsType;
        }

        protected override void Handle()
        {
            try
            {
                switch (StateIndex)
                {
                    case ActionBase.ACT_STATE_START:
                        #region
                        Watcher.StopAllWatch();
                        To(ActionBase.ACT_STATE_END);
                        #endregion
                        break;


                    case ActionBase.ACT_STATE_END:
                        Finish();
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog($"Exception-Message:{ex.Message}StackTrace:{ex.StackTrace};");
            }
        }

       
    }
}
