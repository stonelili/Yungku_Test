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

namespace Yungku.BNU01_V1.Handler
{
    [DataContract]
    public class OfilmCommand : JsonCommand
    {
        [MyDisplayName("指令类型"), MyCategory("参数")]
        [DataMember]
        public string CmdType { get; set; } = string.Empty;

     
        [MyDisplayName("站位编号"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public int Head { get; set; } = 0;
        [MyDisplayName("治具编号"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public int Jig { get; set; } = 0;
        [MyDisplayName("产品编号"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public int Product { get; set; } = 0;

        [MyDisplayName("模组类型"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public int Module { get; set; } = 0;
        [MyDisplayName("结果代码"), MyCategory("参数")]
        [DataMember]
        [Browsable(false)]
        public string Result { get; set; } = "PASS";


        [Browsable(false)]
        public override string SaveInfo
        {
            get
            {
                return CmdType;
            }
        }
        [Browsable(false)]
        public override string CmdOperation
        {
            get
            {
                return CmdType;
            }
        }

        /// <summary>
        /// 执行复制操作
        /// </summary>
        /// <returns></returns>
        public override JsonCommand Clone()
        {
            OfilmCommand cmd = new OfilmCommand();
            cmd.CmdType = this.CmdType;
            cmd.Head = this.Head;
            cmd.Jig = this.Jig;
            cmd.Product = this.Product;
            cmd.Module = this.Module;
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
            if (cmd is OfilmCommand)
            {
                bool result = true;;
                result &= (cmd as OfilmCommand).CmdType == this.CmdType;
                result &= (cmd as OfilmCommand).Head == this.Head;
                result &= (cmd as OfilmCommand).Jig == this.Jig;
                result &= (cmd as OfilmCommand).Product == this.Product;
                result &= (cmd as OfilmCommand).Module == this.Module;
                return result;
            }

            return base.IsResultCmd(cmd);
        }

        public override JsonCommand BiuldNewCmd()
        {
            return new OfilmCommand();
        }

        public override JsonCommand BiuldNewCmd(string info)
        {
            OfilmCommand cmd = new OfilmCommand();
            cmd.CmdType = info;
            return cmd;
        }
        public override JsonCommand ParseObject(string resultStr)
        {
            return Json.ParseObject<OfilmCommand>(resultStr);
        }

    }
}
