using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Controls;
using YungkuSystem.Structs;

namespace Yungku.BNU01_V1.Handler.Config
{
    /// <summary>
    /// 测试机设置
    /// </summary>
    public class MachineTestSettings : IConfigPage
    {


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
