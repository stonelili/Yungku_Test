using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YungkuSystem.Controls;
using YungkuSystem.Script.Actions.DeviceBase;
using YungkuSystem.Structs;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    public class ActionWaitIO : ActionObject
    {
        /// <summary>
        /// 对象类型标识
        /// </summary>
        public override string ObjectClass
        {
            get { return "等待信号"; }
        }

        private string selectedInput = "";
        [Browsable(false)]
        public string SelectedInput
        {
            get { return selectedInput; }
            set { selectedInput = value; }
        }

        private DeviceInput input = null;
        [MyDisplayName("输入点"), MyCategory("参数设置")]
        [XmlIgnore]
        public DeviceInput Intput
        {
            get { return input; }
            set
            {
                if (input != value)
                {
                    input = value;
                    if (input != null)
                        selectedInput = input.GetFullPath();
                    else
                        selectedInput = "";
                }
            }
        }

        private int timeout = 0;
        /// <summary>
        /// 超时时间(ms)
        /// </summary>
        [MyDisplayName("超时时间(ms)"), MyCategory("参数设置")]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        private YesNo result = true;
        [MyDisplayName("获取状态"), MyCategory("参数设置")]
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
            ActionWaitIO obj = dest as ActionWaitIO;
            obj.timeout = this.timeout;
            obj.selectedInput = this.selectedInput;
            obj.result = this.result;          
        }
        public override void Binding()
        {
            base.Binding();
            input  = this.Owner.Devices.Root.GetBaseByPath(selectedInput,
            typeof(DeviceInput)) as DeviceInput;
        }
        protected override void Execute()
        {
            base.Execute();
            ValidHardware();

            if (!HasPass && !MustExecute)
                return;
            if (input == null || input.Input == null)
            {
                OnAlarm(selectedInput + "点为NULL，请重新选择!");
            }
            else
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (input.Input.GetValue()!=result)
                {
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        OnAlarm(Owner.StationName + ":等待IO"+ selectedInput + "已超时！");
                        WriteInfo(Owner.StationName + ":等待IO" + selectedInput + "已超时！", true);
                        sw.Restart();
                        break;
                    }
                    Thread.Sleep(10);
                }
            }        
        }

       
    }
}
