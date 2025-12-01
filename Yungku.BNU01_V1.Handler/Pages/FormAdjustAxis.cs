using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;
using YungkuSystem.ThreadMessage;
using static YungkuSystem.Controls.SingleAxisManualPage;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormAdjustAxis : Form, IManual
    {
        public static Position3D OffSet = null;

        //public AxisMap AxisR { get; set; } = MyApp.HW.AxisR;

        public FormAdjustAxis()
        {
            InitializeComponent();
        }
        public static DialogResult StartAdjust(AxisMap axisx, double targetPosZ, string title, AxisMap axisy = null)
        {
            FormAdjustAxis frmJPA = new FormAdjustAxis();
           // frmJPA.AxisR = axisx;
            frmJPA.adjustMotor.Manual = frmJPA;
            frmJPA.adjustMotor.Axis1 = axisx;
            frmJPA.adjustMotor.Title = title;
            if (axisy == null)
            {
                frmJPA.adjustMotor.PageType = AxisManualPageType.RotationLeftRight;
            }
            return frmJPA.ShowDialog();
        }

        #region "接口实现"
        public void ReverseOutput(YungkuSystem.Motion.Manage.GPIOMap io)
        {
            if (IOCanManual())
                io.Set(!io.GetValue());
        }
        private bool IOCanManual()
        {
            bool ok = MotionSystemIsInitialized();
            if (!ok)
            {
                Tip.ShowError(G.Text("运动控制组件尚未初始化"));
                return false;
            }
            return ok;
        }

        public bool MotionSystemIsInitialized()
        {
            return MyApp.GetInstance().MotionSystem.IsInitialized;
        }

        public void ManualHome(YungkuSystem.Motion.Manage.AxisMap axis)
        {
            if (axis.IsBusy)
            {
                Tip.ShowWarning(G.Text("电机正忙！"));
                return;
            }
            if (IOCanManual()&& axis !=null)
            {
                MyApp.HomeHelper.Home(axis, axis.Params.HomeOffset);
            }
        }

        public void JogStop(YungkuSystem.Motion.Manage.AxisMap axis)
        {
            if (IOCanManual() && axis != null)
                axis.DecStop();
        }

        public void JogMove(double offset, bool step, AxisMap axis, YungkuSystem.Structs.Direction dir)
        {
            if (IOCanManual() && axis != null)
            {
                offset = dir == YungkuSystem.Structs.Direction.CW ? offset : -offset;
                if (step)
                    axis.AbsMove(axis.Position + offset);
                else
                {
                    if (dir == YungkuSystem.Structs.Direction.CW)
                        axis.JogPositive();
                    else
                        axis.JogNegative();
                }
            }
        }
        public void SetOutput(YungkuSystem.Motion.Manage.GPIOMap io, bool val)
        {
            if (IOCanManual())
                io.Set(val);
        }
        #endregion

        private void btnEnter_Click(object sender, EventArgs e)
        {
           // ApplyAdjustNozzleFlag();
            if (OffSet != null)
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            else
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
        }
       

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }


    }
}
