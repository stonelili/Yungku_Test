using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Controls;

namespace Yungku.BNU01_V1.Handler.Config.TestConfig
{
    public class HeadConfig : IConfigPage
    {
        //private string clientIP = "192.168.1.1";
        //[MyDisplayName("调焦服务器IP")]
        //public string ClientIP
        //{
        //    get { return clientIP; }
        //    set { clientIP = value; }
        //}
        //private int clientPort = 20000;
        //[MyDisplayName("调焦服务器Port")]
        //public int ClientPort
        //{
        //    get { return clientPort; }
        //    set { clientPort = value; }
        //}



        private FormPropertyGrid configForm = new FormPropertyGrid();
        /// <summary>
        /// 获取配置窗口
        /// </summary>
        /// <returns></returns>
        public System.Windows.Forms.Form GetConfigForm()
        {
            configForm.propertyGrid1.SelectedObject = this;
            return configForm;
        }
    }
}
