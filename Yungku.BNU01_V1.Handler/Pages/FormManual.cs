using System;
using System.Drawing;
using System.Windows.Forms;
using YungkuSystem.Controls;
using YungkuSystem.ThreadMessage;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Manage;
using Yungku.BNU01_V1.Handler.Logic;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using System.Threading.Tasks;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormManual : Form, IManual, ILedManual
    {
        //private bool AxisIO_0_0 = false;
        //private bool AxisIO_0_1 = false;
        //private bool AxisIO_0_2 = false;

        //private bool AxisIO_1_0 = false;
        //private bool AxisIO_1_1 = false;
        //private bool AxisIO_1_2 = false;

        private MachineObject currentMachineObject = null;
        public FormManual()
        {
            InitializeComponent();
        }

        private void tabPane1_SelectedPageIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                Form frm = null;
                switch (tabPane1.SelectedIndex)
                {
                    case 1:
                        frm = MyApp.GetInstance().MotionSystem.GetLedShowForm(MyApp.GetInstance().Logic.DefaultExecuter.IsRunning, this);
                        if (frm != null)
                        {
                            DockToPanel(frm);
                        }
                        else
                            currentForm = null;
                        break;
                        //case 4:
                        //    frm = MyApp.GetInstance().LightSystem.GetLedShowForm();
                        //    if (frm != null)
                        //        DockToPanel1(frm);
                        //    else
                        //        currentForm = null;
                        //    break;
                        //case 6:
                        //    frm = MyApp.GetInstance().TCSystem.GetTCShowForm(MyApp.GetInstance().Logic.DefaultExecuter.IsRunning);
                        //    if (frm != null)
                        //        DockToPanel2(frm);
                        //    else
                        //        currentForm = null;
                        //    break;
                }
            }
        }

        //private Led[] Server = null;
        private void FormManual_Load(object sender, EventArgs e)
        {
            ShowRefresh();
            sampAxisXY.Manual = this;
           
        }

        private void FormManual_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                MyApp.GetInstance().Logger.WriteRecord(G.Text("切换到手动操作画面"));
            }
        }

        private Form currentForm = null;
        internal void DockToPanel(Form frm)
        {
            if (frm == currentForm)
                return;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Parent = tabPage3;
            frm.Left = 0;
            frm.Top = 0;
            frm.Dock = DockStyle.Fill;
            frm.Show();
            if (currentForm != null)
                currentForm.Hide();
            currentForm = frm;
        }

        #region 接口实现
        public void ReverseOutput(YungkuSystem.Motion.Manage.GPIOMap io)
        {
            if (IOCanManual())
                io.Set(!io.GetValue());
        }

        private bool AxisCanManual(AxisMap axis, bool bHoming = false, bool Step = false)
        {
            bool ok = (MotionSystemIsInitialized() && axis != null);
       
            return ok;
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

        public bool IOCanManual(GPIOMap io = null, bool SetState = false)//重写
        {
            bool MotionSystemIsOk = MotionSystemIsInitialized();
            
            return MotionSystemIsOk;
        }

        public bool MotionSystemIsInitialized()
        {
            //return MyApp.GetInstance().MotionSystem.IsInitialized;
            return true;
        }

        public void ManualHome(YungkuSystem.Motion.Manage.AxisMap axis)
        {
            if (axis.IsBusy)
            {
                Tip.ShowWarning(G.Text("电机正忙！"));
                return;
            }
            StopAllConti();
            if (AxisCanManual(axis, true))
            {
                MyApp.HomeHelper.Home(axis, axis.Params.HomeOffset);
            }
        }

        #endregion 接口实现


        /// <summary>
        /// 停止插补
        /// </summary>E:\机台软件\江西盛泰\JXST_JF06&CVT02\JXST_Demo\Yungku.BNU01_V1.Handler\Logic\Objects\
        public void StopAllConti()
        {
            for (int i = 0; i < 4; i++)
            {
                YungkuSystem.Devices.HardWareMotion.LeiSai.DMC.LTDMC.dmc_conti_stop_list(0, (ushort)i, 0);
            }
        }

        public void JogStop(YungkuSystem.Motion.Manage.AxisMap axis)
        {
            axis.DecStop();
        }

        public void JogMove(double offset, bool step, AxisMap axis, YungkuSystem.Structs.Direction dir)
        {
            if (AxisCanManual(axis, false, step))
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

        /// <summary>
        /// 按钮移动检测
        /// </summary>
        /// <returns></returns>
        private bool CanMove(AxisMap axis)
        {
            if (!AxisCanManual(axis))
            {
                // Tip.ShowError(G.Text("电机不在安全位置!"));
                return false;
            }

            if (MessageBox.Show(G.Text("确定要运动到当前位置吗？"), G.Text("提示"),
               MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                return true;
            else
                return false;
        }

        private void tmrUpdateUI_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                // this.tmrUpdateUI.Enabled = false;
                if (MotionSystemIsInitialized())
                {
                }
                if (ElectricCollimator.CollimatorState && ledElectricCollimatorAuto.State)
                {
                    float[] nEncPos = new float[20];//10
                    float[] nSimulatePos = new float[20];//10
                    for (byte i = 1; i < 10; i++)//1,10
                    {
                        if (!ElectricCollimator.GetActPos(i, ref nEncPos[i], ElectricCollimator.ProductType))
                        {
                            Tip.ShowError(G.Text("读取平行光管编码器位置失败！9"));
                            return;
                        }

                        tbActDistance0.Text = nEncPos[1].ToString("f3");
                        tbActDistance1.Text = nEncPos[2].ToString("f3");
                        tbActDistance2.Text = nEncPos[3].ToString("f3");
                        tbActDistance3.Text = nEncPos[4].ToString("f3");
                        tbActDistance4.Text = nEncPos[5].ToString("f3");
                        tbActDistance5.Text = nEncPos[6].ToString("f3");
                        tbActDistance6.Text = nEncPos[7].ToString("f3");
                        tbActDistance7.Text = nEncPos[8].ToString("f3");
                        tbActDistance8.Text = nEncPos[9].ToString("f3");

                        ElectricCollimator.GetActPos2(i, ref nSimulatePos[i], ElectricCollimator.ProductType);

                        tbSimulateDistance_0.Text = nSimulatePos[1].ToString("f3");
                        tbSimulateDistance_1.Text = nSimulatePos[2].ToString("f3");
                        tbSimulateDistance_2.Text = nSimulatePos[3].ToString("f3");
                        tbSimulateDistance_3.Text = nSimulatePos[4].ToString("f3");
                        tbSimulateDistance_4.Text = nSimulatePos[5].ToString("f3");
                        tbSimulateDistance_5.Text = nSimulatePos[6].ToString("f3");
                        tbSimulateDistance_6.Text = nSimulatePos[7].ToString("f3");
                        tbSimulateDistance_7.Text = nSimulatePos[8].ToString("f3");
                        tbSimulateDistance_8.Text = nSimulatePos[9].ToString("f3");
                    }

                    if (MyApp.GetInstance().Logic.DefaultExecuter.IsRunning)
                    {
                        Tip.ShowWarning("自动状态刷新一次位置自动关闭刷新!");
                        ledElectricCollimatorAuto.State = false;
                    }
                }
                //this.tmrUpdateUI.Enabled = true;
            }
        }

        private void SaveLocation_Click(object sender, EventArgs e)
        {
            //if (!CanApply())
            //{
            //    return;
            //}

            //int index = int.Parse((sender as Control).Tag.ToString());
            //if (index == 0)
            //{
            //    MyApp.Config.General.AxisYLoadPosition = MyApp.HW.AxisR.Position;
            //}
            //else if (index == 1)
            //{
            //    MyApp.Config.General.AxisYTestPosition = MyApp.HW.AxisR.Position;
            //}
            //else if (index == 2)
            //{
            //    MyApp.Config.General.AxisRTestPosition = MyApp.HW.AxisR.Position;
            //}
            //else if (index == 3)
            //{
            //    MyApp.Config.General.AxisRTestPosition = MyApp.HW.AxisR.Position;
            //}
            //ShowRefresh();
        }

        private void MoveLocation_Click(object sender, EventArgs e)
        {
            //int index = int.Parse((sender as Control).Tag.ToString());
            //if (CanMove(MyApp.HW.AxisR))
            //{
            //    if (index == 0)
            //    {
            //        currentMachineObject.MoveTo(MyApp.HW.AxisR, MyApp.Config.General.AxisYLoadPosition);
            //    }
            //    else if (index == 1)
            //    {
            //        currentMachineObject.MoveTo(MyApp.HW.AxisR, MyApp.Config.General.AxisYTestPosition);
            //    }
            //    else if (index == 2)
            //    {
            //        currentMachineObject.MoveTo(MyApp.HW.AxisR, MyApp.Config.General.AxisRTestPosition);
            //    }
            //    else if (index == 3)
            //    {
            //        currentMachineObject.MoveTo(MyApp.HW.AxisR, MyApp.Config.General.AxisRTestPosition);
            //    }
            //}
        }

        private void ShowRefresh()
        {
            //this.lblPositiveAngle.Text = "A: " + MyApp.Config.General.PositiveTestAngle.ToString("0.00");
            //this.lblNegativeAngle.Text = "A: " + MyApp.Config.General.NegativeTestAngle.ToString("0.00");
            //this.lblLoadAngle.Text = "A: " + MyApp.Config.General.LoadAngle.ToString("0.00");

            currentMachineObject = MyApp.GetInstance().Machine.BindingObject as MachineObject;
        }

        public bool CanApply()
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

            //if (ManipulatorObject == null || FitNozzleGroupObject == null)
            //{
            //    Tip.ShowWarning("机械臂或者吸嘴组未初始化！");
            //    return false;
            //}

            if (MyApp.NeedReset && !MyApp.ShareData.ishoming)
            {
                Tip.ShowWarning(G.Text("设备需要复位才能进行此操作！"));
                return false;
            }

            if (MessageBox.Show(G.Text("确定要执行这个保存操作吗？"), G.Text("提示"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                return true;
            else
                return false;
        }

        private void BtnLinkCollimator_Click(object sender, EventArgs e)
        {
            ledCollimatorState.State = ElectricCollimator.CollimatorState;
            if (ledCollimatorState.State)
            {
                BtnLinkCollimator.Enabled = false;
                btnCollimatorMove.Enabled = true;
                BtnCloseC.Enabled = true;
                btnInit.Enabled = true;

                #region
                tbActDistance0.Enabled = true;
                tbActDistance1.Enabled = true;
                tbActDistance2.Enabled = true;
                tbActDistance3.Enabled = true;
                tbActDistance4.Enabled = true;
                tbActDistance5.Enabled = true;
                tbActDistance6.Enabled = true;
                tbActDistance7.Enabled = true;
                tbActDistance8.Enabled = true;
                tbActDistance9.Enabled = true;
                tbActDistance10.Enabled = true;
                tbActDistance11.Enabled = true;
                tbActDistance12.Enabled = true;

                tbSimulateDistance_0.Enabled = true;
                tbSimulateDistance_1.Enabled = true;
                tbSimulateDistance_2.Enabled = true;
                tbSimulateDistance_3.Enabled = true;
                tbSimulateDistance_4.Enabled = true;
                tbSimulateDistance_5.Enabled = true;
                tbSimulateDistance_6.Enabled = true;
                tbSimulateDistance_7.Enabled = true;
                tbSimulateDistance_8.Enabled = true;
                tbSimulateDistance_9.Enabled = true;
                tbSimulateDistance_10.Enabled = true;
                tbSimulateDistance_11.Enabled = true;
                tbSimulateDistance_12.Enabled = true;
                #endregion

                MyApp.GetInstance().Logger.WriteInfo("[手动界面]:平行光管连接成功！");
                Tip.ShowWarning("[手动界面]:平行光管连接成功！");
            }
            else
            {
                MyApp.GetInstance().Logger.WriteError("[手动界面]:平行光管连接失败！");
                Tip.ShowError("[手动界面]:平行光管连接失败！");
            }

            if (ElectricCollimator.CollimatorState)
            {
                Tip.ShowWarning("[手动界面]:当前光管已经连接！");
                return;
            }
            ElectricCollimator.CollimatorLink();
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            Tip.ShowWarning("[手动界面]:当前光管开始复位，请等待！", "操作提示", 3000);
            bool executeOk = true;
            //if (ElectricCollimator.CollimatorState)
            //{
            for (byte i = 1; i < 10; i++)
            {
                executeOk &= ElectricCollimator.MX_Homing(i);
                MyApp.GetInstance().Logger.WriteRecord($"初始化地址为[{i}]的光管");
            }

            if (executeOk)
            {
                MyApp.GetInstance().Logger.WriteRecord("平行光管电机复位成功");
            }
            else
            {
                MyApp.GetInstance().Logger.WriteError("平行光管电机复位失败");
                Tip.ShowError("平行光管电机复位失败!");
            }
            //}
            MyApp.GetInstance().Logger.WriteRecord($"当前光管连接状态:{ElectricCollimator.CollimatorState}");
        }

        private void btnParamWrite_Click(object sender, EventArgs e)
        {
            byte Add = (byte)numCollimatorAdd.Value;
            int CCT = (int)numCollimatorCCT.Value;
            int EX = (int)numCollimatorEX.Value;
            float duv = 0f;
            Tip.ShowWarning("光管参数写入中，请等待!", "", 3000);
            MyApp.GetInstance().Logger.WriteRecord($"手动设置:光管 色温CCT:{CCT} 和 照度EX:{EX}");

            if (chkUnifiedControl.Checked)//统一控制
            {
                #region
                for (byte add = 1; add < 10; add++)
                {
                    //Task.Run(new Action(() =>
                    //{
                    if (ElectricCollimator.MX_SetLedEx(add, CCT, EX, ref duv) == 1)
                    {
                        this.Invoke(new Action(() =>
                        {
                            ledCollimatorLedState.State = true;
                        }));
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            ledCollimatorLedState.State = false;
                        }));
                    }
                    //}));
                }
                #endregion
            }
            else
            {
                #region
                if (ElectricCollimator.MX_SetLedEx(Add, CCT, EX, ref duv) == 1)
                {
                    ledCollimatorLedState.State = true;
                }
                else
                {
                    ledCollimatorLedState.State = false;
                }
                #endregion
            }
        }

        private void btnCollimatorMove_Click(object sender, EventArgs e)
        {
            float Distance = 0f;

            byte Add = (byte)numCollimatorAdd.Value;

            float.TryParse(tbCollimatorPos1.Text, out Distance);

            if (Distance < 0)
            {
                Distance = 600.0f;
            }

            ledElectricCollimatorAuto.State = false;
            Tip.ShowWarning("光管移动中，请等待!", "", 3000);

            MyApp.GetInstance().Logger.WriteRecord($"手动设置: 光管距离{Distance}");
            if (chkUnifiedControl.Checked)//统一控制
            {
                #region
                for (byte add = 1; add < 10; add++)
                {
                    //Task.Run(new Action(() =>
                    //{
                    if (!ElectricCollimator.MoveAbsByFun(add, Distance, ElectricCollimator.ProductType))
                    {
                        this.Invoke(new Action(() =>
                        {
                            tbCollimatorPos1.Text = "0";
                        }));
                    }
                    //}));
                }
                #endregion
            }
            else
            {
                #region
                if (!ElectricCollimator.MoveAbsByFun(Add, Distance, ElectricCollimator.ProductType))
                {
                    tbCollimatorPos1.Text = "0";
                }
                #endregion
            }
        }


        private void BtnCloseC_Click(object sender, EventArgs e)
        {
            if (chkUnifiedControl.Checked)//统一控制
            {
                for (byte add = 1; add < 10; add++)
                {
                    if (ElectricCollimator.MX_CloseLedEx(add))
                    {
                        ledCollimatorLedState.State = false;
                    }
                }
            }
            else
            {
                byte Add = (byte)numCollimatorAdd.Value;
                if (ElectricCollimator.MX_CloseLedEx(Add))
                {
                    ledCollimatorLedState.State = false;
                }
            }
        }

        private void ledElectricCollimatorAuto_Click(object sender, EventArgs e)
        {
            if (ledElectricCollimatorAuto.State)
            {
                ledElectricCollimatorAuto.State = false;
            }
            else
            {
                ledElectricCollimatorAuto.State = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float lux = 0;
            ElectricCollimator.MX_GetLedEx(1, ref lux);
        }


    }
}

