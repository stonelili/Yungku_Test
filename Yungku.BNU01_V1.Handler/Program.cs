using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yungku.BNU01_V1.Handler;
using Yungku.BNU01_V1.Handler.Pages;
using YungkuSystem.Controls;

namespace Yungku.BNU01_V1.Handler
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex mtx = new System.Threading.Mutex(true, "BNU01_Mutex1");
            if (mtx.WaitOne(10) == false)
            {
                MessageBox.Show("程序已经启动！请检查进程残留！/The program has been launched  Please check the process residue");
                return;
            }

            FormWelcom.StartInit += FormWelcom_StartInit;
            MyApp.GetInstance().Run(form: typeof(FormWelcom));
            //Application.Run(new Form1());
        }

        static void FormWelcom_StartInit(object sender, EventArgs e)
        {
            MyApp.GetInstance().Initialize(sender as FormWelcom);
        }
    }
}
