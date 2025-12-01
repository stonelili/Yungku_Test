using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Yungku.BNU01_V1.Handler
{
    public class AppParams
    {
        private string currentProjectFile = string.Empty;

        public string CurrentProjectFile
        {
            get { return currentProjectFile; }
            set { currentProjectFile = value; }
        }

        /// <summary>
        /// 验证当前工程文件是否有效
        /// </summary>
        /// <returns></returns>
        public bool ProjectFileIsValid
        {
            get
            {
                return File.Exists(currentProjectFile);
            }
        }

        public static AppParams LoadAppParams()
        {
            AppParams config = new AppParams();
            XmlSerializer xmlSer = new XmlSerializer(config.GetType());

            using (TextReader tr = new StreamReader(".\\AppParams.xml"))
            {
                config = xmlSer.Deserialize(tr) as AppParams;
            }

            return config;
        }

        public static void SaveProject(AppParams config)
        {
            XmlSerializer xmlSer = new XmlSerializer(config.GetType());

            using (TextWriter tw = new StreamWriter(".\\AppParams.xml"))
            {
                xmlSer.Serialize(tw, config);
            }
        }
    }
}
