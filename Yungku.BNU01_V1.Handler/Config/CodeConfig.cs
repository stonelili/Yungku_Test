using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Controls;

namespace Yungku.BNU01_V1.Handler.Config
{
   public  class CodeConfig
    {
        private string codeStr =string.Empty;
        [MyDisplayName("结果信息")]
        public string CodeStr
        {
            get { return codeStr; }
            set { codeStr = value; }
        }
    }
}
