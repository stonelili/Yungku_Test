using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Yungku.BNU01_V1.Handler.Config;
using YungkuSystem.Controls;
using YungkuSystem.Motion;
using YungkuSystem.Motion.Manage;
using YungkuSystem.TestFlow;
using YungkuSystem.Tools;

namespace Yungku.BNU01_V1.Handler.Logic.Objects
{
    public abstract  class MachineBase: BindingObject
    {
        private string name = "Unname";
        [DisplayName("名称")]
        public   string Name
        {
            get { return name; }
            set { name = value; }
        }
        private int index = -1;
        [DisplayName("编号")]
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        /// <summary>
        /// 配置
        /// </summary>
        public  IConfigPage BaseConfig = null;

        /// <summary>
        /// 输出日志信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sender"></param>
        /// <param name="tags"></param>
        public void LogWriteRecord(string msg)
        {
            MyApp.GetInstance().Logger.WriteRecord(msg);
        }

        private TimeoutWatch watcher = new TimeoutWatch();
        [XmlIgnore()]
        [Browsable(false)]
        public TimeoutWatch Watcher
        {
            get { return watcher; }
        }

        /// <summary>
        /// 触发报警，报警会时执行流程停止
        /// </summary>
        /// <param name="msg"></param>
        public void OnAlarm(string msg)
        {
            MyApp.GetInstance().AlarmPublisher.Write(msg);
            if (MyApp.ShareData.ishoming)
            {
                MyApp.NeedReset = true;
                MyApp.ShareData.ishoming = false;//复位过程中报警需要重新复位
            }   
        }
        /// <summary>
        /// 回原点
        /// </summary>
        public HomeHelper HomeHelper
        {
            get { return MyApp.HomeHelper; }
        }
        /// <summary>
        /// 移动到指定的位置，成功后返回true，否则返回false，非阻塞
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="pos"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public virtual bool MoveTo(AxisMap axis, double pos, double precision = 0.3)
        {
            //如果处于运行过程中，则返回未完成
            if (axis.IsBusy)
                return false;

            //如果位置到达则直接返回成功
            if (Math.Abs(axis.Position - pos) < precision)
                return true;

            //如果不在运行过程中且又没有到达位置，则启动运行
            try
            {
                axis.AbsMove(pos);
            }
            catch (Exception ex)
            {
                OnAlarm(ex.Message);
            }

            return false;
        }

        private string homeStateIndex = "初始状态";
        [XmlIgnore()]
        [Browsable(false)]
        public string HomeStateIndex
        {
            get { return homeStateIndex; }
            protected set
            {
                if (value != homeStateIndex)
                {
                    LogWriteRecord(string.Format("复位过程：{0}-[{1}]", name, value));
                }
                homeStateIndex = value;
            }
        }

        public void ResetHomeStatus()
        {
            homeStateIndex = "初始状态";
            watcher.StopAllWatch();
            homeIsFinished = false;
        }
        public bool MoveFinished = true;

        private bool homeIsFinished = false;
        [XmlIgnore()]
        [Browsable(false)]
        public bool HomeIsFinished
        {
            get { return homeIsFinished; }
            protected set { homeIsFinished = value; }
        }

        public virtual bool HomeLoop()
        {
            return homeIsFinished;
        }
           
    }
}
