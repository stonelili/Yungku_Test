using System;
using System.Drawing;
using System.Windows.Forms;
using Yungku.BNU01_V1.Handler.Pages;
using System.Runtime.InteropServices;
using YungkuSystem.Globalization;
using YungkuSystem.ThreadMessage;
using YungkuSystem;
using YungkuSystem.Machine;
using YungkuSystem.Controls;
using Yungku.BNU01_V1.Handler.Logic;
using YungkuSystem.TestFlow;
using YungkuSystem.AlarmManage;
using YungkuSystem.ExternalControl.HelpForm;
using System.Threading.Tasks;

namespace Yungku.BNU01_V1.Handler
{
    public partial class FormMain : Form, IMessageProc
    {
        private Form currentForm = null;
        private FormAuto frmAuto = new FormAuto();
        private FormManual frmManual = new FormManual();
        private FormDebug frmDebug = new FormDebug();
        private FormFixMode pageFixMode = null;                     /*维修模式Page*/

        private Pages.FormSetting frmSetting = new Pages.FormSetting();

        private FormSetup frmSetup = new FormSetup();
        private FormLogin frmLogin = new FormLogin();
        private FormAlarm frmAlarm;
        private System.Windows.Forms.Timer tmrSuperDog;
        public bool refreshLoaderFlag;
        private bool LoadAppParams = true;

        // 在类的字段区域添加按钮控件
        private System.Windows.Forms.Button btnStartLeftStation;
        private System.Windows.Forms.Button btnStartRightStation;

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetForegroundWindow", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetF(); //获得本窗体的句柄
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetF(IntPtr hWnd); //设置此窗体为活动窗体
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);//获得对应标题窗体的句柄

        //激活指定窗口、、将创建指定窗口的线程设置到前台，并且激活该窗口。
        //键盘输入转向该窗口，并为用户改各种可视的记号。系统给创建前台窗口的线程分配的权限稍高于其他线程。
        [DllImport("user32")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //获取当前窗口句柄:GetForegroundWindow()

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        public FormMain()
        {
            InitializeComponent();
            DockToMain(frmAuto);
            StartSuperDogTimer();
        }

        private void StartSuperDogTimer()
        {
            tmrSuperDog = new System.Windows.Forms.Timer();
            tmrSuperDog.Interval = 300000;
            tmrSuperDog.Tick += tmrSuperDog_Tick;
            tmrSuperDog.Enabled = true;
        }

        void tmrSuperDog_Tick(object sender, EventArgs e)
        {
            CheckSuperDog();
        }

        private static void CheckSuperDog()
        {
            MyApp.SuperDog.SuperDogIsAlive();
            if (!MyApp.SuperDog.IsAlive)
            {
                MyApp.GetInstance().Logic.DefaultExecuter.Stop();
                MyApp.GetInstance().AlarmPublisher.Write("没有检测到加密狗，请插入加密狗或查看加密狗是否损坏。");
            }
            else
            {
                int superDogSurPlusTime = MyApp.SuperDog.GetDaysLeft();
                if (superDogSurPlusTime <= 0)
                {
                    MyApp.GetInstance().Logic.DefaultExecuter.Stop();

                    MyApp.GetInstance().AlarmPublisher.Write("加密狗已经过期，请联系供应商售后处理。");
                }
                else if (superDogSurPlusTime <= 10)
                {
                    TimeSpan timeSpan = DateTime.Now - MyApp.Config.General.SuperDogExpireSoonAlarmTime;
                    if (timeSpan.Days > 0)
                    {
                        MyApp.Config.General.SuperDogExpireSoonAlarmTime = DateTime.Now;
                        MyApp.GetInstance().SaveAll();
                        MyApp.GetInstance().AlarmPublisher.Write(string.Format("加密狗只剩下{0}天了，请联系供应商及时处理。", MyApp.Config.General.SuperDogExpireSoonAlarmTime));
                    }
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MyApp.GetInstance().FW.SystemEvent += FW_SystemEvent;
            MyApp.GetInstance().MessageShow.SetMessageProc(this);
            Tip.CreateTipForm(this);
            MyApp.GetInstance().AlarmPublisher.AlarmListChanged += AlarmPublisher_AlarmListChanged; ;
            frmAlarm = new FormAlarm(MyApp.GetInstance().AlarmPublisher);
            pageFixMode = new FormFixMode();
            MyApp.GetInstance().Logic.DefaultExecuter.Actions.AutoRemoveOnFinished = true;
            BarcodeScanner.CreateScannerForm(this);
            CheckSuperDog();
            InitializeStationStartButtons();
            //RunWatchAction();
            RunTricolorLightAction();
            MyApp.GetInstance().Logic.DefaultExecuter.StartExecute += DefaultExecuter_StartExecute;
            MyApp.GetInstance().Logic.DefaultExecuter.StopExecute += DefaultExecuter_StopExecute;
            this.Text = MyApp.AppName + " - " + MyApp.Version + "-" + System.IO.Path.GetDirectoryName(MyApp.AppParams.CurrentProjectFile);
            MyApp.AppTest = this.Text;
            formHandle1.Text = this.Text;
            Framework.BilingualTranslation.CurrentLanguageChanged += BilingualTranslation_CurrentLanguageChanged;
            Framework.ApplyLanguage(this);
            LoadAppParams = MyApp.LoadAppParams;
            //Link();
            //LinkElectricCollimator();
            MyApp.GetInstance().Logger.WriteRecord($"当前启动软件版本信息：{this.Text}");
        }

        /// <summary>
        /// 初始化左右工位启动按钮
        /// </summary>
        private void InitializeStationStartButtons()
        {
            try
            {
                MyApp.GetInstance().Logger.WriteRecord("========================================");
                MyApp.GetInstance().Logger.WriteRecord("开始初始化工位启动按钮");
                MyApp.GetInstance().Logger.WriteRecord("========================================");

                // 创建左工位启动按钮
                btnStartLeftStation = new System.Windows.Forms.Button();
                btnStartLeftStation.Name = "btnStartLeftStation";
                btnStartLeftStation.Text = "左工位启动";
                btnStartLeftStation.Size = new Size(100, 35);
                btnStartLeftStation.BackColor = Color.Gray; // 初始灰色
                btnStartLeftStation.ForeColor = Color.White;
                btnStartLeftStation.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
                btnStartLeftStation.FlatStyle = FlatStyle.Flat;
                btnStartLeftStation.FlatAppearance.BorderSize = 1;
                btnStartLeftStation.FlatAppearance.BorderColor = Color.White;
                btnStartLeftStation.Cursor = Cursors.Hand;
                btnStartLeftStation.Click += BtnStartLeftStation_Click;
                btnStartLeftStation.Enabled = false; // 初始禁用

                // 创建右工位启动按钮
                btnStartRightStation = new System.Windows.Forms.Button();
                btnStartRightStation.Name = "btnStartRightStation";
                btnStartRightStation.Text = "右工位启动";
                btnStartRightStation.Size = new Size(100, 35);
                btnStartRightStation.BackColor = Color.Gray; // 初始灰色
                btnStartRightStation.ForeColor = Color.White;
                btnStartRightStation.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
                btnStartRightStation.FlatStyle = FlatStyle.Flat;
                btnStartRightStation.FlatAppearance.BorderSize = 1;
                btnStartRightStation.FlatAppearance.BorderColor = Color.White;
                btnStartRightStation.Cursor = Cursors.Hand;
                btnStartRightStation.Click += BtnStartRightStation_Click;
                btnStartRightStation.Enabled = false; // 初始禁用

                MyApp.GetInstance().Logger.WriteRecord("按钮控件已创建");

                // ✅ 方案1：直接添加到 pnlBottom，使用绝对定位
                if (this.pnlBottom != null)
                {
                    MyApp.GetInstance().Logger.WriteRecord($"找到 pnlBottom: Size={pnlBottom.Size}, Location={pnlBottom.Location}");

                    // 使用 BeginInvoke 延迟执行，确保界面完全加载
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // 计算按钮位置（从底部面板左侧开始，留出空间给 controlHandle1）
                            // 假设 controlHandle1 占据左侧约 600 像素
                            int baseX = 650; // controlHandle1 右侧的位置
                            int baseY = (pnlBottom.Height - btnStartLeftStation.Height) / 2; // 垂直居中

                            btnStartLeftStation.Location = new Point(baseX, baseY);
                            btnStartRightStation.Location = new Point(baseX + btnStartLeftStation.Width + 10, baseY);

                            pnlBottom.Controls.Add(btnStartLeftStation);
                            pnlBottom.Controls.Add(btnStartRightStation);

                            // 确保按钮在最上层
                            btnStartLeftStation.BringToFront();
                            btnStartRightStation.BringToFront();

                            MyApp.GetInstance().Logger.WriteRecord($"✅ 按钮已添加到 pnlBottom");
                            MyApp.GetInstance().Logger.WriteRecord($"   左工位按钮位置: ({btnStartLeftStation.Left}, {btnStartLeftStation.Top})");
                            MyApp.GetInstance().Logger.WriteRecord($"   右工位按钮位置: ({btnStartRightStation.Left}, {btnStartRightStation.Top})");
                            MyApp.GetInstance().Logger.WriteRecord($"   按钮可见性: 左={btnStartLeftStation.Visible}, 右={btnStartRightStation.Visible}");

                            // 强制刷新
                            pnlBottom.Refresh();
                        }
                        catch (Exception ex)
                        {
                            MyApp.GetInstance().Logger.WriteError($"BeginInvoke 内部异常: {ex.Message}");
                            MyApp.GetInstance().Logger.WriteError($"堆栈: {ex.StackTrace}");
                        }
                    }));
                }
                else
                {
                    MyApp.GetInstance().Logger.WriteError("❌ 找不到 pnlBottom 面板！");

                    // 备用方案：添加到主窗体
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            btnStartLeftStation.Location = new Point(650, this.Height - 60);
                            btnStartRightStation.Location = new Point(760, this.Height - 60);

                            this.Controls.Add(btnStartLeftStation);
                            this.Controls.Add(btnStartRightStation);

                            btnStartLeftStation.BringToFront();
                            btnStartRightStation.BringToFront();

                            MyApp.GetInstance().Logger.WriteRecord("按钮已添加到主窗体（备用方案）");
                        }
                        catch (Exception ex)
                        {
                            MyApp.GetInstance().Logger.WriteError($"备用方案异常: {ex.Message}");
                        }
                    }));
                }

                MyApp.GetInstance().Logger.WriteRecord("========================================");
                MyApp.GetInstance().Logger.WriteRecord("工位启动按钮初始化完成");
                MyApp.GetInstance().Logger.WriteRecord("========================================");
            }
            catch (Exception ex)
            {
                MyApp.GetInstance().Logger.WriteError($"初始化工位启动按钮失败: {ex.Message}");
                MyApp.GetInstance().Logger.WriteError($"异常堆栈: {ex.StackTrace}");
            }
        }
        /// <summary>
        /// 左工位启动按钮点击事件
        /// </summary>
        private void BtnStartLeftStation_Click(object sender, EventArgs e)
        {
            try
            {
                MyApp.GetInstance().Logger.WriteRecord("========================================");
                MyApp.GetInstance().Logger.WriteRecord("✅ 【左工位启动】按钮被点击");
                MyApp.GetInstance().Logger.WriteRecord("========================================");

                if (MyApp.MainAction == null)
                {
                    string errorMsg = "主控流程未初始化，无法启动左工位！";
                    Tip.ShowError(errorMsg);
                    MyApp.GetInstance().Logger.WriteError($"[左工位启动]:MainAction 为 null");
                    MessageBox.Show(errorMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MyApp.GetInstance().Logger.WriteRecord($"[左工位启动]:MainAction 存在");

                // 检查是否正在运行
                bool isRunning = MyApp.GetInstance().Logic.DefaultExecuter.IsRunning;
                MyApp.GetInstance().Logger.WriteRecord($"[左工位启动]:主流程运行状态: {isRunning}");

                if (!isRunning)
                {
                    string warnMsg = "请先点击【运行】按钮启动主流程！";
                    Tip.ShowWarning(warnMsg);
                    MyApp.GetInstance().Logger.WriteRecord($"[左工位启动]:{warnMsg}");
                    MessageBox.Show(warnMsg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //立即切换到自动测试界面
                DockToMain(frmAuto);
                MyApp.GetInstance().Logger.WriteRecord("[左工位启动]:已切换到自动测试界面");

                //确保 FormAuto 切换到测试参数标签页（而不是日志标签页）
                // 假设 FormAuto 有一个公共方法可以切换到测试参数页
                frmAuto.SwitchToTestParameterView();

                // 调用 ActionMain 的启动方法
                MyApp.GetInstance().Logger.WriteRecord("[左工位启动]:调用 MyApp.MainAction.StartLeftStation()");
                MyApp.MainAction.StartLeftStation();
                MyApp.GetInstance().Logger.WriteRecord("[左工位启动]:✅ 已成功调用 StartLeftStation()");

                // 更新按钮状态
                btnStartLeftStation.BackColor = Color.Gray;
                btnStartLeftStation.Enabled = false;
                btnStartLeftStation.Text = "左工位运行中";

                Tip.Show("左工位已启动测试");
                MyApp.GetInstance().Logger.WriteRecord("[主界面]:用户启动左工位测试");
                MyApp.GetInstance().Logger.WriteRecord("========================================");
            }
            catch (Exception ex)
            {
                string errorMsg = $"启动左工位失败: {ex.Message}";
                Tip.ShowError(errorMsg);
                MyApp.GetInstance().Logger.WriteError($"[主界面]:启动左工位异常 - {ex.Message}");
                MyApp.GetInstance().Logger.WriteError($"异常堆栈: {ex.StackTrace}");
                MessageBox.Show($"{errorMsg}\n\n详细信息:\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 右工位启动按钮点击事件
        /// </summary>
        private void BtnStartRightStation_Click(object sender, EventArgs e)
        {
            try
            {
                MyApp.GetInstance().Logger.WriteRecord("========================================");
                MyApp.GetInstance().Logger.WriteRecord("✅ 【右工位启动】按钮被点击");
                MyApp.GetInstance().Logger.WriteRecord("========================================");

                if (MyApp.MainAction == null)
                {
                    string errorMsg = "主控流程未初始化，无法启动右工位！";
                    Tip.ShowError(errorMsg);
                    MyApp.GetInstance().Logger.WriteError($"[右工位启动]:MainAction 为 null");
                    MessageBox.Show(errorMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MyApp.GetInstance().Logger.WriteRecord($"[右工位启动]:MainAction 存在");

                // 检查是否正在运行
                bool isRunning = MyApp.GetInstance().Logic.DefaultExecuter.IsRunning;
                MyApp.GetInstance().Logger.WriteRecord($"[右工位启动]:主流程运行状态: {isRunning}");

                if (!isRunning)
                {
                    string warnMsg = "请先点击【运行】按钮启动主流程！";
                    Tip.ShowWarning(warnMsg);
                    MyApp.GetInstance().Logger.WriteRecord($"[右工位启动]:{warnMsg}");
                    MessageBox.Show(warnMsg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //立即切换到自动测试界面
                DockToMain(frmAuto);
                MyApp.GetInstance().Logger.WriteRecord("[右工位启动]:已切换到自动测试界面");

                //确保 FormAuto 切换到测试参数标签页
                frmAuto.SwitchToTestParameterView();

                // 调用 ActionMain 的启动方法
                MyApp.GetInstance().Logger.WriteRecord("[右工位启动]:调用 MyApp.MainAction.StartRightStation()");
                MyApp.MainAction.StartRightStation();
                MyApp.GetInstance().Logger.WriteRecord("[右工位启动]:✅ 已成功调用 StartRightStation()");

                // 更新按钮状态
                btnStartRightStation.BackColor = Color.Gray;
                btnStartRightStation.Enabled = false;
                btnStartRightStation.Text = "右工位运行中";

                Tip.Show("右工位已启动测试");
                MyApp.GetInstance().Logger.WriteRecord("[主界面]:用户启动右工位测试");
                MyApp.GetInstance().Logger.WriteRecord("========================================");
            }
            catch (Exception ex)
            {
                string errorMsg = $"启动右工位失败: {ex.Message}";
                Tip.ShowError(errorMsg);
                MyApp.GetInstance().Logger.WriteError($"[主界面]:启动右工位异常 - {ex.Message}");
                MyApp.GetInstance().Logger.WriteError($"异常堆栈: {ex.StackTrace}");
                MessageBox.Show($"{errorMsg}\n\n详细信息:\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 更新工位启动按钮状态
        /// </summary>
        private void UpdateStationStartButtons()
        {
            try
            {
                if (btnStartLeftStation == null || btnStartRightStation == null)
                    return;

                bool isRunning = MyApp.GetInstance().Logic.DefaultExecuter.IsRunning;

                // 检查 ActionMain 是否处于"等待工位启动"状态
                bool isWaitingForStation = false;

                if (MyApp.MainAction != null && isRunning)
                {
                    // 检查当前状态是否是"等待工位启动"
                    string currentState = MyApp.MainAction.StateIndex;
                    isWaitingForStation = (currentState == "等待工位启动");

                    // 调试日志（可选，正式版本可以删除）
                    // MyApp.GetInstance().Logger.WriteRecord($"[按钮状态更新] 当前状态:{currentState}, 等待中:{isWaitingForStation}");
                }

                // 只有当主流程运行且处于"等待工位启动"状态时，才启用按钮
                if (isRunning && isWaitingForStation)
                {
                    // 启用左工位按钮（如果尚未启动）
                    if (!btnStartLeftStation.Enabled)
                    {
                        btnStartLeftStation.Enabled = true;
                        btnStartLeftStation.BackColor = Color.FromArgb(0, 192, 0); // 绿色
                        btnStartLeftStation.Text = "左工位启动";
                        MyApp.GetInstance().Logger.WriteRecord("[按钮状态] 左工位按钮已启用");
                    }

                    // 启用右工位按钮（如果尚未启动）
                    if (!btnStartRightStation.Enabled)
                    {
                        btnStartRightStation.Enabled = true;
                        btnStartRightStation.BackColor = Color.FromArgb(0, 123, 255); // 蓝色
                        btnStartRightStation.Text = "右工位启动";
                        MyApp.GetInstance().Logger.WriteRecord("[按钮状态] 右工位按钮已启用");
                    }
                }
                else if (!isRunning)
                {
                    // 停止运行时，重置按钮状态
                    if (btnStartLeftStation.Enabled || btnStartLeftStation.BackColor != Color.Gray)
                    {
                        btnStartLeftStation.Enabled = false;
                        btnStartLeftStation.BackColor = Color.Gray;
                        btnStartLeftStation.Text = "左工位启动";
                    }

                    if (btnStartRightStation.Enabled || btnStartRightStation.BackColor != Color.Gray)
                    {
                        btnStartRightStation.Enabled = false;
                        btnStartRightStation.BackColor = Color.Gray;
                        btnStartRightStation.Text = "右工位启动";
                    }
                }
            }
            catch (Exception ex)
            {
                // 避免UI更新异常影响主流程
                MyApp.GetInstance()?.Logger?.WriteError($"[按钮状态更新] 异常: {ex.Message}");
            }
        }
        private void BilingualTranslation_CurrentLanguageChanged(object sender, EventArgs e)
        {
            Framework.ApplyLanguage(this);
        }

        /// <summary>
        /// 发生报警
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlarmPublisher_AlarmListChanged(object sender, YungkuSystem.AlarmManage.Alarm e)
        {
            MyApp.RunOnUI(new System.Action(() =>
            {
                if (MyApp.GetInstance().AlarmPublisher.IsAlarm)
                {
                    MyApp.isStart = false;
                    MyApp.GetInstance().Logic.DefaultExecuter.Stop();
                    MyApp.GetInstance().Logger.WriteRecord("报警,线程暂停！");
                    if (MyApp.Config.General.ShowAlarmsForm && !frmAlarm.Visible)
                        frmAlarm.Show();
                    //发生报警时停止所有电机
                    //MyApp.HW.StopAllMotor();
                    MyApp.GetInstance().Logger.WriteError("报警变更：");
                    MyApp.GetInstance().Logger.WriteError(e.ToString());
                    MyApp.GetInstance().Machine.PauseAllStation();
                    //ActionExecuter exec = MyApp.GetInstance().Logic.CreateExecuter(MyApp.ACTION_WATCHER);
                    //exec.Stop();
                }
                else if (frmAlarm.Visible)
                {
                    frmAlarm.Close();
                }
            }));
        }

        /// <summary>
        /// 停止执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DefaultExecuter_StopExecute(object sender, EventArgs e)
        {
            MyApp.GetInstance().Machine.PauseAllStation();
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DefaultExecuter_StartExecute(object sender, EventArgs e)
        {
            if (!MyApp.ShareData.ishoming)
            {
                MyApp.GetInstance().Machine.ContinuAllStation();
            }
            //RunWatchAction();
            //MyApp.GetInstance().Logic[MyApp.ACTION_TricolorLight].Start();
        }

        private void RunTricolorLightAction()
        {
            if (MyApp.GetInstance().ActionTricolorLightExe.Actions.Count == 0)
            {
                if (MyApp.TricolorLight != null)
                    MyApp.GetInstance().Logic.RunAction(MyApp.TricolorLight, MyApp.ACTION_TricolorLight);
            }
            if (!MyApp.GetInstance().ActionTricolorLightExe.IsRunning)
            {
                MyApp.GetInstance().ActionTricolorLightExe.Start();
            }
        }

        private void RunWatchAction()
        {
            if (MyApp.GetInstance().ActionWatcherExe.Actions.Count == 0)
            {
                MyApp.GetInstance().Logic.RunAction(new ActionWatcher(), MyApp.ACTION_WATCHER);
            }
            if (!MyApp.GetInstance().ActionWatcherExe.IsRunning)
            {
                MyApp.GetInstance().ActionWatcherExe.Start();
            }

        }

        public void ProcMessage(YungkuSystem.ThreadMessage.Message msg)
        {
            this.Invoke(new System.Action(() =>
            {
                msg.Show();
            }));
        }

        private void FW_SystemEvent(object sender, YungkuSystem.SystemEventArgs e)
        {
            switch (e.EventType)
            {
                case SystemEventArgs.CONFIG_CHANGED:
                    //btnSaveAll.Enabled = true;
                    break;
                case SystemEventArgs.CONFIG_SAVED:
                    //btnSaveAll.Enabled = false;
                    break;
                case SystemEventArgs.TRIGGER_ALARM:
                    MyApp.GetInstance().AlarmPublisher.Write(
                       new Alarm(e.GetParameter("Message").ToString(), Alarm.ALARM_TYPE_EXCEPTION,
                        (bool)e.GetParameter("NeedReset")));
                    break;
            }
        }

        /// <summary>
        /// 设置当前活动窗体
        /// </summary>
        /// <param name="frm"></param>
        internal void DockToMain(Form frm)
        {
            if (frm == currentForm)
            {
                if (frm.Visible == false)
                {
                    frm.Show();
                }
                frm.Focus();
                return;
            }
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Parent = pnlMain;
            frm.Left = 0;
            frm.Top = 0;
            frm.Dock = DockStyle.Fill;
            frm.Show();
            if (currentForm != null)
                currentForm.Hide();
            currentForm = frm;
        }

        private void btnAutoPage_Click(object sender, EventArgs e)
        {
            DockToMain(frmAuto);
        }

        private void btnManualPage_Click(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().UAC.CheckPermission(Permissions.MANUAL_OPERATION))
            {
                DockToMain(frmManual);
            }
            else
            {
                frmLogin.ForwardForm = frmManual;
                frmLogin.Permission = Permissions.MANUAL_OPERATION;
                DockToMain(frmLogin);
            }
        }

        private void btnSettingPage_Click(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().UAC.CheckPermission(Permissions.SETTING_MODIFY))
            {
                DockToMain(frmSetting);

            }
            else
            {
                frmLogin.ForwardForm = frmSetting;
                frmLogin.Permission = Permissions.SETTING_MODIFY;
                DockToMain(frmLogin);
            }
        }

        private void btnSystemPage_Click(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().UAC.CheckPermission(Permissions.SYSTEM))
            {
                DockToMain(frmSetup);

            }
            else
            {
                frmLogin.ForwardForm = frmSetup;
                frmLogin.Permission = Permissions.SYSTEM;
                DockToMain(frmLogin);
            }
        }

        internal void DockAutoForm()
        {
            DockToMain(frmAuto);
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            MyApp.GetInstance().SaveAll();
            this.Cursor = Cursors.Default;
        }

        private void menuHandle1_HelpClick(object sender, EventArgs e)
        {
            // FormPdfView frm = new FormPdfView();
            // frm.Show();
        }

        private void Test_RunOnUIThread(System.Action action)
        {
            Framework.RunOnUIThread(action);
        }

        private void btnClearAlarms_Click(object sender, EventArgs e)
        {
           // MyApp.HW.StopAllMotor();
            MyApp.GetInstance().AlarmPublisher.ClearAlarms();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            MyApp.StartTestLogic();
            MyApp.isStart = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            MyApp.GetInstance().Logic.DefaultExecuter.Stop();
            MyApp.isStart = false;

            // 重置工位启动按钮
            if (btnStartLeftStation != null)
            {
                btnStartLeftStation.Enabled = false;
                btnStartLeftStation.BackColor = Color.Gray;
                btnStartLeftStation.Text = "左工位启动";
            }

            if (btnStartRightStation != null)
            {
                btnStartRightStation.Enabled = false;
                btnStartRightStation.BackColor = Color.Gray;
                btnStartRightStation.Text = "右工位启动";
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                // 先检查关键对象是否存在
                if (MyApp.GetInstance() == null)
                {
                    MessageBox.Show("系统未初始化！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MyApp.GetInstance().AlarmPublisher == null)
                {
                    MessageBox.Show("报警系统未初始化！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MyApp.GetInstance().AlarmPublisher.ClearAlarms();

                if (MyApp.GetInstance().Logic == null || MyApp.GetInstance().Logic.DefaultExecuter == null)
                {
                    MessageBox.Show("逻辑控制系统未初始化！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!MyApp.GetInstance().Logic.DefaultExecuter.IsRunning)
                {
                    string s1 = G.Text("确认要复位设备吗？");
                    string s2 = G.Text("数据更新");

                    if (MessageBox.Show(s1, s2, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == System.Windows.Forms.DialogResult.Yes)
                    {
                        // 检查 ShareData 是否初始化
                        if (MyApp.ShareData == null)
                        {
                            MyApp.GetInstance().Logger.WriteError("[主界面-复位]:ShareData 未初始化！");
                            MessageBox.Show("系统共享数据未初始化，无法执行复位操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        MyApp.NeedReset = true;
                        MyApp.ShareData.ishoming = true;

                        MyApp.GetInstance().Logic.DefaultExecuter.Actions.Clear();
                        ActionExecuter exec = MyApp.GetInstance().Logic.CreateExecuter();

                        if (exec == null)
                        {
                            MyApp.GetInstance().Logger.WriteError("[主界面-复位]:创建执行器失败！");
                            MessageBox.Show("创建执行器失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MyApp.ShareData.ishoming = false;
                            MyApp.NeedReset = false;
                            return;
                        }

                        // 安全停止所有工位
                        try
                        {
                            if (MyApp.GetInstance().Machine != null)
                            {
                                MyApp.GetInstance().Machine.StopAllStation();
                            }
                        }
                        catch (Exception ex)
                        {
                            MyApp.GetInstance().Logger.WriteError($"[主界面-复位]:停止工位异常 - {ex.Message}");
                        }

                        System.Threading.Thread.Sleep(1000);
                        exec.Stop();
                        exec.Actions.Clear();

                        // 添加复位Action
                        try
                        {
                            LogicObject Action = new ActionHome();
                            if (Action == null)
                            {
                                MyApp.GetInstance().Logger.WriteError("[主界面-复位]:创建复位动作失败！");
                                MessageBox.Show("创建复位动作失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            Action.CheckAddAction(exec);
                            exec.Actions.Add(Action);
                            exec.Start();

                            MyApp.GetInstance().Logger.WriteRecord("[主界面-复位]:复位程序已启动");
                        }
                        catch (Exception ex)
                        {
                            MyApp.GetInstance().Logger.WriteError($"[主界面-复位]:添加复位动作异常 - {ex.Message}\n堆栈：{ex.StackTrace}");
                            MessageBox.Show($"添加复位动作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // 恢复标志
                            MyApp.ShareData.ishoming = false;
                            MyApp.NeedReset = false;
                        }
                    }
                }
                else
                {
                    Tip.ShowWarning(G.Text("当前正在运行，不能复位！"));
                }
            }
            catch (NullReferenceException nex)
            {
                string errorMsg = $"空引用异常：{nex.Message}\n\n堆栈跟踪：\n{nex.StackTrace}";

                if (MyApp.GetInstance() != null && MyApp.GetInstance().Logger != null)
                {
                    MyApp.GetInstance().Logger.WriteError($"[主界面-复位按钮]:{errorMsg}");
                }

                MessageBox.Show($"系统对象未初始化！\n\n详细信息：{nex.Message}\n\n请检查日志文件获取更多信息。",
                    "空引用错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 尝试恢复标志状态
                try
                {
                    if (MyApp.ShareData != null)
                    {
                        MyApp.ShareData.ishoming = false;
                    }
                    MyApp.NeedReset = false;
                }
                catch { }
            }
            catch (Exception ex)
            {
                string errorMsg = $"复位异常：{ex.Message}\n\n堆栈跟踪：\n{ex.StackTrace}";

                if (MyApp.GetInstance() != null && MyApp.GetInstance().Logger != null)
                {
                    MyApp.GetInstance().Logger.WriteError($"[主界面-复位按钮]:{errorMsg}");
                }

                MessageBox.Show($"复位操作失败！\n\n详细信息：{ex.Message}\n\n请检查日志文件获取更多信息。",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 尝试恢复标志状态
                try
                {
                    if (MyApp.ShareData != null)
                    {
                        MyApp.ShareData.ishoming = false;
                    }
                    MyApp.NeedReset = false;
                }
                catch { }
            }
        }
        

        private void tmrUpdateUI_Tick(object sender, EventArgs e)
        {
            if (MyApp.Config.FunctionSwitch.CodeScannerFrom && MyApp.CodeScanner)
            {
                IntPtr hWnd = FindWindow(null, "Barcode Scanning");
                if (MyApp.GetInstance().Logic.DefaultExecuter.IsRunning)
                {
                    if (hWnd != GetForegroundWindow())
                    {
                        ShowWindow(hWnd, 6);//最小化
                        ShowWindow(hWnd, 9);//还原
                    }
                    SetForegroundWindow(hWnd);
                    SetF(hWnd); //设置Barcode Scanning标题窗口获得焦点
                }
            }
            if (this.Visible)
            {
                UpdateStationStartButtons();
                if (MyApp.Config.General.IsUseExportProductData)
                {
                    if ((MyApp.GetInstance().Statistical.GetPass() > 0) ||
                        (MyApp.GetInstance().Statistical.GetFail() > 0))
                    {
                        DateTime tempDT = MyApp.Config.General.MorningProductDataTime;
                        TimeSpan morningTS = tempDT.TimeOfDay;
                        tempDT = MyApp.Config.General.NightProductDataTime;
                        TimeSpan nightTS = tempDT.TimeOfDay;
                        TimeSpan nowTS = DateTime.Now.TimeOfDay;
                        if ((nowTS.Hours == morningTS.Hours &&
                                nowTS.Minutes == morningTS.Minutes) ||
                                (nowTS.Hours == nightTS.Hours &&
                                nowTS.Minutes == nightTS.Minutes))
                        {
                            if (MyApp.GetInstance().RecordRunData())
                                MyApp.GetInstance().Logger.WriteInfo("[主界面]:自动导出UPH数据，并清除UPH数据,成功！");
                            else
                                MyApp.GetInstance().Logger.WriteInfo("[主界面]:自动导出UPH数据，并清除UPH数据,失败！");
                        }
                    }
                }

                MyApp.GetInstance().RunTricolorLightAction();
                RunWatchAction();
                controlHandle1.StartEnable = !MyApp.GetInstance().Logic.DefaultExecuter.IsRunning && !MyApp.NeedReset;

                controlHandle1.StopEnable = MyApp.GetInstance().Logic.DefaultExecuter.IsRunning;
                controlHandle1.ResetEnable = !MyApp.GetInstance().Logic.DefaultExecuter.IsRunning;
                controlHandle1.ClearAlarmEnable = MyApp.GetInstance().AlarmPublisher.IsAlarm;
                lblWorkState.Text = MyApp.GetInstance().Logic.GetStateString();
                menuHandle1.ClearUserEnable = MyApp.GetInstance().UAC.CurrentLoginUser != null;

                if (MyApp.GetInstance().UAC.CurrentLoginUser != null)
                    menuHandle1.ClearUserTxt = MyApp.GetInstance().UAC.CurrentLoginUser.Role;

                else
                    menuHandle1.ClearUserTxt = "无权限";

                menuHandle1.SwithFileEnabled = MyApp.GetInstance().UAC.CurrentLoginUser == null ? false : MyApp.GetInstance().UAC.CurrentLoginUser.Role.ToString().ToUpper() == Permissions.SYSTEM;
                menuHandle1.FastDebugEnalbed = MyApp.GetInstance().UAC.CurrentLoginUser == null ? false : (MyApp.GetInstance().UAC.CurrentLoginUser.Role.ToString().ToUpper() == Permissions.SETTING_MODIFY || MyApp.GetInstance().UAC.CurrentLoginUser.Role.ToString().ToUpper() == Permissions.SYSTEM);

                if (!LoadAppParams && !MyApp.GetInstance().AlarmPublisher.IsAlarm)
                {
                    MyApp.GetInstance().AlarmPublisher.Write("机种选择失败，请选择机种并重启！");
                }
            }

            MyApp.GetInstance().RunTricolorLightAction();//警报
            RunWatchAction();//按钮

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            string s1 = G.Text("确定要退出软件吗？\r\n\r\n如果确实要退出，请先确保安全后点击【是(Yes)】按钮。");
            string s2 = G.Text("安全提示");
            if (MessageBox.Show(s1, s2,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    if (ElectricCollimator.CollimatorState)
                    {
                        Task.Run(new Action(() =>
                        {                            
                            //断开平行光管连接
                            for (int i = 1; i < 10; i++)
                            {
                                if (ElectricCollimator.Disconnect())
                                {
                                    i = 10;
                                }
                                else if (i == 9)
                                {
                                    MessageBox.Show("断开平行光管连接失败");
                                }
                            }
                        }));
                    }

                    s1 = G.Text("需要保存参数吗？\r\n\r\n如果需要保存参数，请点击【是(Yes)】按钮。");
                    s2 = G.Text("保存提示");
                    if (MessageBox.Show(s1, s2,
                     MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        MyApp.GetInstance().SaveAll();
                    }
                    System.Threading.Thread.Sleep(2000);
                }
                catch
                {

                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {

            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            MyApp.GetInstance().UAC.Logout();
            DockToMain(frmAuto);
        }

        private void tsmiPasswordChange_Click(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().UAC.CurrentLoginUser != null
                && MyApp.GetInstance().UAC.CurrentLoginUser.Role != "System")
                MyApp.GetInstance().UAC.GetPasswordModifyForm(MyApp.GetInstance().UAC.CurrentLoginUser).Show();
            else
                Tip.ShowError(G.Text("系统不允许当前用户修改密码！"));
        }

        private void controlHandle1_ContinueOrStepClick(object sender, bool e)
        {
            MyApp.GetInstance().Logic.DefaultExecuter.StepRun = !e;
            MyApp.GetInstance().Machine.SetStepRun(!e ? YungkuSystem.Script.Core.RunMode.Step : YungkuSystem.Script.Core.RunMode.Continue);
        }

        private void tmrShowMaintenanceTip_Tick(object sender, EventArgs e)
        {
            if (checkBox_Fixmode.Checked)
                Tip.ShowWarning(G.Text("维修模式，请勿乱动！"), "", 500);
        }

        private void checkBox_Fixmode_CheckedChanged(object sender, EventArgs e)
        {
            if (!WaitAllTestDone())
            {
                Tip.ShowWarning(G.Text("请先停止测试!！"));
                checkBox_Fixmode.Checked = false;
                return;
            }
            if (MyApp.ShareData.ishoming)
            {
                Tip.ShowWarning(G.Text("请先停止复位!！"));
                checkBox_Fixmode.Checked = false;
                return;
            }
            pageFixMode.Width = this.Width;
            pageFixMode.EnableTime();
            MyApp.GetInstance().Logger.WriteInfo(G.Text("[调试]:维修模式。"));

            if (DialogResult.Cancel == pageFixMode.ShowDialog())
            {
                checkBox_Fixmode.Checked = false;
            }
            pageFixMode.DisableTime();
            MyApp.NeedReset = true;
            MyApp.GetInstance().Logger.WriteInfo(G.Text("[调试]:需要复位。"));
        }

        private bool WaitAllTestDone()
        {
            bool isDone = true;
            Turntable tt = (Turntable)MyApp.GetInstance().Machine.TestItems[0];
            foreach (Station st in tt.Stations)
            {

                if (st.Executor.IsRunning || st.Executor.IsPause)
                {
                    // Logger.WriteError(st.Name + "未完成");
                    isDone = false;
                    break;
                }
            }
            return isDone;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            MyApp.OpenConfig();
            this.Cursor = Cursors.Default;
        }

        private void BtnShowDegubForm_Click(object sender, EventArgs e)
        {
            Form AxisIofrm = MyApp.GetInstance().MotionSystem.GetAxisIOForm(MyApp.GetInstance().Logic.DefaultExecuter.IsRunning);
            AxisIofrm.Show();
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            if (e.Control == true & key == Keys.F1)//如果没按Ctrl键 
            {
                FormPdfView frm = new FormPdfView();
                frm.Show();
            }
            if (e.Control == true & key == Keys.F12)
            {
                FormVersionView frm = new FormVersionView();
                frm.Show();
            }
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
            SolidBrush br = new SolidBrush(this.BackColor);
            e.Graphics.FillRectangle(br, rect);
            br.Dispose();
            e.Graphics.DrawRectangle(Pens.Blue, rect);
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void formHandle1_MaxClick(object sender, EventArgs e)
        {
            this.WindowState = this.WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void formHandle1_MinClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void formHandle1_CloseClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool mouseDownFlag = false;
        private Point mouseDownPoint = Point.Empty;
        private void formHandle1_MouseMove(object sender, MouseEventArgs e)
        {

            if (mouseDownFlag)
            {
                Point offset = new Point(e.Location.X - mouseDownPoint.X, e.Location.Y - mouseDownPoint.Y);
                this.Location += new Size(offset);
            }
        }

        private void formHandle1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDownFlag && this.WindowState == FormWindowState.Normal)
            {
                mouseDownFlag = false;
            }
            this.Cursor = Cursors.Default;
        }

        private void formHandle1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!mouseDownFlag && this.WindowState == FormWindowState.Normal)
            {
                mouseDownFlag = true;
                mouseDownPoint = e.Location;
                this.Cursor = Cursors.SizeAll;
            }
        }

        private void menuHandle1_DebugClick(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().UAC.CheckPermission(Permissions.SETTING_MODIFY))
            {
                DockToMain(frmDebug);
            }
            else
            {
                frmLogin.ForwardForm = frmDebug;
                frmLogin.Permission = Permissions.SETTING_MODIFY;
                DockToMain(frmLogin);
            }
        }
        /// <summary>
        /// 校准程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ActionAdjust_Click_1()
        //{
        //    string s1 = G.Text("确认要添加校准程序吗？");
        //    string s2 = G.Text("程序校准");
        //    if (MessageBox.Show(s1, s2,
        //        MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        //        == System.Windows.Forms.DialogResult.Yes)
        //    {
        //        if (MyApp.GetInstance().AlarmPublisher.IsAlarm)
        //        {
        //            MyApp.GetInstance().AlarmPublisher.ClearAlarms();
        //        }
        //        ActionExecuter exe = MyApp.GetInstance().Logic.CreateExecuter();
        //        if (exe.Actions.Count > 0)
        //        {
        //            Tip.ShowWarning(G.Text("当前有其他任务，无法添加校准程序！"));
        //        }
        //        else if (exe.IsRunning)
        //        {
        //            Tip.ShowWarning(G.Text("当前正在运行，无法添加校准程序！"));
        //        }
        //        else
        //        {
        //            LogicObject Action = (MyApp.MachineMode == MachineModes.CVT检测机) ? null : new ActionAdjustLaser();
        //            if (Action != null)
        //            {
        //                exe.Actions.Clear();
        //                if (Action.CheckAddAction(exe))
        //                {
        //                    exe.Actions.Add(Action);
        //                }
        //                exe.Start();
        //            }

        //        }
        //    }
        //}
        //private void 添加调焦环镭射校准程序ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ActionAdjust_Click_1();
        //}

        private void 强制清除所有动作ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionExecuter exe = MyApp.GetInstance().Logic.CreateExecuter();
            exe.Stop();
            exe.Actions.Clear();
        }

        private FormAxisHandler frmR = null;
        private void menuHandle1_FastDebugClick(object sender, EventArgs e)
        {
            if (frmR == null)
            {
                frmR = new FormAxisHandler();
            }
            frmR.Hide();
            frmR.TopMost = true;
            frmR.Show();
            frmR.TopMost = false;
        }

        /// <summary>
        /// 获取FormAuto实例的静态方法
        /// </summary>
        /// <returns></returns>
        public static FormAuto GetFormAuto()
        {
            try
            {
                FormMain formMain = MyApp.GetInstance().MainForm as FormMain;
                if (formMain != null)
                {
      return formMain.frmAuto;
                }
  }
    catch (Exception ex)
    {
        MyApp.GetInstance().Logger.WriteError($"获取FormAuto实例失败: {ex.Message}");
    }
    return null;
      }

    }
}
