using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormFixMode : Form
    {
        public FormFixMode()
        {            
            InitializeComponent();            
        }

        /// <summary>
        /// 定时器中关闭轴运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (MyApp.GetInstance().MotionSystem.IsInitialized)
            //{
            //    MyApp.HW.StopAllMotor();
            //}
        }

        private void FormFixMode_Load(object sender, EventArgs e)
        {
            int y = btnCancel.Location.Y;
            btnCancel.Location = new Point(Width / 2 - btnCancel.Width / 2, y);
        }

        /// <summary>
        /// 设置标签文字
        /// </summary>
        /// <param name="text"></param>
        public void SetLabelText(string text)
        {
            label1.Text = text;
        }

        /// <summary>
        /// 关闭定时器
        /// </summary>
        public void DisableTime()
        {
            timer1.Enabled = false;
        }

        /// <summary>
        /// 打开定时器
        /// </summary>
        public void EnableTime()
        {
            timer1.Enabled = true;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void FormFixMode_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
