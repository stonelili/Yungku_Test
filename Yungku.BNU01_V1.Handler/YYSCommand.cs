using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Controls;
using YungkuSystem.Tester.Module.Command;
using YungkuSystem.Tools;

namespace Yungku.BNU01_V1.Handler.JsonTcp
{
    [DataContract]
    public class YYSCommand:JsonCommand
    {     
        [MyDisplayName("指令类型"), MyCategory("参数")]
        [DataMember]      
        public string Operation { get; set; } = string.Empty;
      
        [MyDisplayName("六轴类型"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public string AxisType { get; set; } = string.Empty;
        [MyDisplayName("轴移动距离"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public string AxisDistance { get; set; } = string.Empty;
        [MyDisplayName("工装编号"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public int Device { get; set; } = 0;
        [MyDisplayName("站位编号"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public int Station { get; set; } = 0;
        [MyDisplayName("模组类型"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public string CameraType { get; set; } = string.Empty;
        [MyDisplayName("结果代码"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public int Result { get; set; } = 0;


        [Browsable(false)]
        public override  string SaveInfo
        {
            get
            {
                return Operation ;
            }
        }
        [Browsable(false)]
        public override string CmdOperation
        {
            get
            {
                return Operation;
            }
        }

        /// <summary>
        /// 执行复制操作
        /// </summary>
        /// <returns></returns>
        public override JsonCommand Clone()
        {
            YYSCommand cmd = new YYSCommand();
            cmd.Operation = this.Operation;
            cmd.AxisType = this.AxisType;
            cmd.AxisDistance = this.AxisDistance;
            cmd.Device = this.Device;
            cmd.Station = this.Station;
            cmd.CameraType = this.CameraType;
            cmd.Result = this.Result;
            return cmd;

        }
        /// <summary>
        /// 判断是否为回复
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override bool IsResultCmd(JsonCommand cmd)
        {         
            if(cmd is YYSCommand)
            {
                bool result = true;
                result &= (cmd as YYSCommand) .Operation == this.Operation;
                result &= (cmd as YYSCommand).AxisType == this.AxisType;
                result &= (cmd as YYSCommand).Device == this.Device;
                result &= (cmd as YYSCommand).Station == this.Station;
                result &= (cmd as YYSCommand).CameraType == this.CameraType;
                return result;
            }

            return base .IsResultCmd(cmd);
        }

        public override JsonCommand BiuldNewCmd()
        {
            return new YYSCommand();
        }

        public override JsonCommand BiuldNewCmd(string info)
        {
            YYSCommand cmd = new YYSCommand();
            cmd.Operation = info;
            return cmd;
        }
        public override JsonCommand ParseObject(string resultStr)
        {
            return Json.ParseObject<YYSCommand>(resultStr);
        }

    }
}
