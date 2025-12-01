using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YungkuSystem.Controls;
using YungkuSystem.Structs;

namespace Yungku.BNU01_V1.Handler.Config.TestConfig
{
    public class ProductConfig : IConfigPage
    {
        //private double foucsOffset = new double();
        //[MyDisplayName("空治具校准偏差（相对0工位）"), MyCategory("预调焦设置")]
        //public double FoucsOffset
        //{
        //    get { return foucsOffset; }
        //    set { foucsOffset = value; }
        //}
      
        //private Position2D gluePositionOffset = new Position2D();
        //[MyDisplayName(" 点胶起始位置"), MyCategory("点胶设置")]
        //public Position2D GluePositionOffset
        //{
        //    get { return gluePositionOffset; }
        //    set { gluePositionOffset = value; }
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
