using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YungkuSystem.Controls;
using YungkuSystem.Script.Actions.DeviceBase;
using YungkuSystem.Structs;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{

    public class ActionOutput : ActionObject
    {
        /// <summary>
        /// 对象类型标识
        /// </summary>
        public override string ObjectClass
        {
            get { return "输出"; }
        }

        private string selectedOutput = "";
        [Browsable(false)]
        public string SelectedOutput
        {
            get { return selectedOutput; }
            set { selectedOutput = value; }
        }

        private DeviceOutput output = null;
        [MyDisplayName("输出点"), MyCategory("参数设置")]
        [XmlIgnore]
        public DeviceOutput Output
        {
            get { return output; }
            set
            {
                if (output != value)
                {
                    output = value;
                    if (output != null)
                        selectedOutput = output.GetFullPath();
                    else
                        selectedOutput = "";
                }
            }
        }


        private YesNo result = true;
        [MyDisplayName("操作状态"), MyCategory("参数设置")]
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
            ActionOutput obj = dest as ActionOutput;
            obj.selectedOutput = this.selectedOutput;
            obj.result = this.result;
        }

        public override void Binding()
        {
            base.Binding();
            output = this.Owner.Devices.Root.GetBaseByPath(selectedOutput,
            typeof(DeviceOutput)) as DeviceOutput;
        }

        protected override void Execute()
        {
            base.Execute();
            ValidHardware();

            if (!HasPass && !MustExecute)
                return;
            if (output == null|| output.Output==null )
            {
                OnAlarm(selectedOutput + "点为NULL，请重新选择!");
            }
            else 
            {
                output.Output.Set(result);
            }
        }

      
    }
}
