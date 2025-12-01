using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Controls;

namespace Yungku.BNU01_V1.Handler.Config.TestConfig
{
    public  class TestSetting : IConfigPage
    {
        private List<HeadConfig> headConfigs = new List<HeadConfig>();
        [Browsable(false)]
        /// <summary>
        /// 测试头配置
        /// </summary>
        public List<HeadConfig> HeadConfigs
        {
            get { return headConfigs; }
            set { headConfigs = value; }
        }

        private List<ProductConfig> productConfigs = new List<ProductConfig>();
        [Browsable(false)]
        /// <summary>
        /// 产品配置
        /// </summary>
        public List<ProductConfig> ProductConfigs
        {
            get { return productConfigs; }
            set { productConfigs = value; }
        }

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
