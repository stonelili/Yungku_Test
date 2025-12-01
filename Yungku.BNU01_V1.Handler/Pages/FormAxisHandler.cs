using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;
using YungkuSystem.ThreadMessage;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormAxisHandler : Form
    {
        private MachineObject currentMachineObject = null;

        public FormAxisHandler()
        {
            InitializeComponent();
        }

        private void FormRHandler_Load(object sender, EventArgs e)
        {
            currentMachineObject = MyApp.GetInstance().Machine.BindingObject as MachineObject;

            //sampAxisX.Manual = this;
            

            ShowRefresh();
        }


     

        public void JogStop(AxisMap axis)
        {
            axis.DecStop();
        }

        public void ManualHome(AxisMap axis)
        {
            if (axis.IsBusy)
            {
                Tip.ShowWarning(G.Text("电机正忙！"));
                return;
            }
           
        }

        public bool MotionSystemIsInitialized()
        {
            return MyApp.GetInstance().MotionSystem.IsInitialized;
        }

        public void ReverseOutput(GPIOMap io)
        {
            if (IOCanManual())
                io.Set(!io.GetValue());
        }

        public void SetOutput(GPIOMap io, bool val)
        {
            if (IOCanManual())
                io.Set(val);
        }
        private bool IOCanManual()
        {
            bool ok = MotionSystemIsInitialized();
            if (!ok)
            {
                Tip.ShowError(G.Text("运动控制组件尚未初始化"));
                return false;
            }
            if (MyApp.GetInstance().Logic.DefaultExecuter.IsRunning)
            {
                Tip.ShowWarning(G.Text("运行中，不允许手动操作"));
                return false;
            }
            return ok;
        }
     

        private void ShowRefresh()
        {
            this.lblXLoadPosition.Text = "X: " + MyApp.Config.General.AxisXLoadPosition.ToString("0.00");
            this.lblXTestPosition.Text = "X: " + MyApp.Config.General.AxisXTestPosition.ToString("0.00");
            this.lblXTestPosition2.Text = "X: " + MyApp.Config.General.AxisXTestPosition2.ToString("0.00");

            this.lblYLoadPosition.Text = "Y: " + MyApp.Config.General.AxisYLoadPosition.ToString("0.00");
            this.lblYBufferPosition.Text = "Y: " + MyApp.Config.General.AxisYBufferPosition.ToString("0.00");
            this.lblYTestPosition.Text = "Y: " + MyApp.Config.General.AxisYTestPosition.ToString("0.00");
            this.lblYTestPosition2.Text = "Y: " + MyApp.Config.General.AxisYTestPosition2.ToString("0.00");

            this.lblZLoadPosition.Text = "Z: " + MyApp.Config.General.AxisZLoadPosition.ToString("0.00");
            this.lblZTestPosition.Text = "Z: " + MyApp.Config.General.AxisZTestPosition.ToString("0.00");
            this.lblZTestPosition2.Text = "Z: " + MyApp.Config.General.AxisZTestPosition2.ToString("0.00");

            this.lblRLoadPosition.Text = "R: " + MyApp.Config.General.AxisRLoadPosition.ToString("0.00");
            this.lblRTestPosition.Text = "R: " + MyApp.Config.General.AxisRTestPosition.ToString("0.00");
            this.lblRTestPosition2.Text = "R: " + MyApp.Config.General.AxisRTestPosition2.ToString("0.00");
        }

        public bool CanApply()
        {
            bool ok = MotionSystemIsInitialized();

            if (!ok)
            {
                Tip.ShowError("运动控制组件尚未初始化");
                return false;
            }

            if (MyApp.GetInstance().Logic.DefaultExecuter.IsRunning)
            {
                Tip.ShowWarning(G.Text("运行中，不允许手动操作"));
                return false;
            }

            //if (ManipulatorObject == null || FitNozzleGroupObject == null)
            //{
            //    Tip.ShowWarning(G.Text("机械臂或者吸嘴组未初始化！"));
            //    return false;
            //}

            if (MyApp.NeedReset && !MyApp.ShareData.ishoming)
            {
                Tip.ShowWarning(G.Text("设备需要复位才能进行此操作！"));
                return false;
            }

            //if (MessageBox.Show(G.Text("确定要执行这个保存操作吗？"), G.Text("提示"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    return true;
            //else
            //    return false;

            return true;
        }

        private void FormRHandler_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 设置Cancel属性为true以取消关闭
            e.Cancel = true;
            Hide();
            return;
        }

        private void FormRHandler_VisibleChanged(object sender, EventArgs e)
        {
            ShowRefresh();
        }
    }
}
