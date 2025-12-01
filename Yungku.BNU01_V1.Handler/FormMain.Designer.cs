namespace Yungku.BNU01_V1.Handler
{
  partial class FormMain
  {
    /// <summary>
    /// 必需的设计器变量。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 清理所有正在使用的资源。
    /// </summary>
    /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows 窗体设计器生成的代码

    /// <summary>
    /// 设计器支持所需的方法 - 不要
    /// 使用代码编辑器修改此方法的内容。
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tmrUpdateUI = new System.Windows.Forms.Timer(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.formHandle1 = new YungkuSystem.Controls.FormHandle();
            this.menuHandle1 = new YungkuSystem.Controls.MenuHandle();
            this.checkBox_Fixmode = new System.Windows.Forms.CheckBox();
            this.DebugUAC = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.添加调焦环镭射校准程序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.强制清除所有动作ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.controlHandle1 = new YungkuSystem.Controls.ControlHandle();
            this.lblWorkState = new System.Windows.Forms.Label();
            this.pnlMain = new YungkuSystem.Controls.MyPanel();
            this.pnlHeader.SuspendLayout();
            this.menuHandle1.SuspendLayout();
            this.DebugUAC.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.controlHandle1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmrUpdateUI
            // 
            this.tmrUpdateUI.Enabled = true;
            this.tmrUpdateUI.Tick += new System.EventHandler(this.tmrUpdateUI_Tick);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Controls.Add(this.formHandle1);
            this.pnlHeader.Controls.Add(this.menuHandle1);
            resources.ApplyResources(this.pnlHeader, "pnlHeader");
            this.pnlHeader.Name = "pnlHeader";
            // 
            // formHandle1
            // 
            this.formHandle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.formHandle1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.formHandle1, "formHandle1");
            this.formHandle1.MaxVisible = true;
            this.formHandle1.MinVisible = true;
            this.formHandle1.Name = "formHandle1";
            this.formHandle1.CloseClick += new System.EventHandler(this.formHandle1_CloseClick);
            this.formHandle1.MaxClick += new System.EventHandler(this.formHandle1_MaxClick);
            this.formHandle1.MinClick += new System.EventHandler(this.formHandle1_MinClick);
            this.formHandle1.MouseMoveAll += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.formHandle1_MouseMove);
            this.formHandle1.MouseUpAll += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.formHandle1_MouseUp);
            this.formHandle1.MouseDownAll += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.formHandle1_MouseDown);
            // 
            // menuHandle1
            // 
            this.menuHandle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.menuHandle1.Controls.Add(this.checkBox_Fixmode);
            this.menuHandle1.DebugGuideEnabled = true;
            this.menuHandle1.DebugGuideVisible = true;
            resources.ApplyResources(this.menuHandle1, "menuHandle1");
            this.menuHandle1.FastDebugEnalbed = true;
            this.menuHandle1.FastDebugVisible = true;
            this.menuHandle1.Name = "menuHandle1";
            this.menuHandle1.SwithFileEnabled = true;
            this.menuHandle1.AutoClick += new System.EventHandler(this.btnAutoPage_Click);
            this.menuHandle1.ManualClick += new System.EventHandler(this.btnManualPage_Click);
            this.menuHandle1.SettingClick += new System.EventHandler(this.btnSettingPage_Click);
            this.menuHandle1.DebugClick += new System.EventHandler(this.menuHandle1_DebugClick);
            this.menuHandle1.SystemClick += new System.EventHandler(this.btnSystemPage_Click);
            this.menuHandle1.FastDebugClick += new System.EventHandler(this.menuHandle1_FastDebugClick);
            this.menuHandle1.DebugGuideClick += new System.EventHandler(this.BtnShowDegubForm_Click);
            this.menuHandle1.SwithFileClick += new System.EventHandler(this.btnOpenFile_Click);
            this.menuHandle1.SaveAllClick += new System.EventHandler(this.btnSaveAll_Click);
            this.menuHandle1.HelpClick += new System.EventHandler(this.menuHandle1_HelpClick);
            this.menuHandle1.ClearUserClick += new System.EventHandler(this.btnLogout_Click);
            // 
            // checkBox_Fixmode
            // 
            resources.ApplyResources(this.checkBox_Fixmode, "checkBox_Fixmode");
            this.checkBox_Fixmode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.checkBox_Fixmode.ForeColor = System.Drawing.Color.White;
            this.checkBox_Fixmode.Name = "checkBox_Fixmode";
            this.checkBox_Fixmode.UseVisualStyleBackColor = false;
            this.checkBox_Fixmode.CheckedChanged += new System.EventHandler(this.checkBox_Fixmode_CheckedChanged);
            // 
            // DebugUAC
            // 
            this.DebugUAC.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加调焦环镭射校准程序ToolStripMenuItem,
            this.强制清除所有动作ToolStripMenuItem});
            this.DebugUAC.Name = "cmsUAC";
            resources.ApplyResources(this.DebugUAC, "DebugUAC");
            // 
            // 添加调焦环镭射校准程序ToolStripMenuItem
            // 
            resources.ApplyResources(this.添加调焦环镭射校准程序ToolStripMenuItem, "添加调焦环镭射校准程序ToolStripMenuItem");
            this.添加调焦环镭射校准程序ToolStripMenuItem.Name = "添加调焦环镭射校准程序ToolStripMenuItem";
            // 
            // 强制清除所有动作ToolStripMenuItem
            // 
            this.强制清除所有动作ToolStripMenuItem.Name = "强制清除所有动作ToolStripMenuItem";
            resources.ApplyResources(this.强制清除所有动作ToolStripMenuItem, "强制清除所有动作ToolStripMenuItem");
            this.强制清除所有动作ToolStripMenuItem.Click += new System.EventHandler(this.强制清除所有动作ToolStripMenuItem_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlBottom.Controls.Add(this.controlHandle1);
            resources.ApplyResources(this.pnlBottom, "pnlBottom");
            this.pnlBottom.Name = "pnlBottom";
            // 
            // controlHandle1
            // 
            this.controlHandle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.controlHandle1.Controls.Add(this.lblWorkState);
            resources.ApplyResources(this.controlHandle1, "controlHandle1");
            this.controlHandle1.Name = "controlHandle1";
            this.controlHandle1.StartClick += new System.EventHandler(this.btnStart_Click);
            this.controlHandle1.StopClick += new System.EventHandler(this.btnStop_Click);
            this.controlHandle1.ContinueOrStepClick += new System.EventHandler<bool>(this.controlHandle1_ContinueOrStepClick);
            this.controlHandle1.HomeClick += new System.EventHandler(this.btnReset_Click);
            this.controlHandle1.ClearAlarmClick += new System.EventHandler(this.btnClearAlarms_Click);
            // 
            // lblWorkState
            // 
            this.lblWorkState.BackColor = System.Drawing.Color.Transparent;
            this.lblWorkState.ContextMenuStrip = this.DebugUAC;
            resources.ApplyResources(this.lblWorkState, "lblWorkState");
            this.lblWorkState.ForeColor = System.Drawing.Color.White;
            this.lblWorkState.Name = "lblWorkState";
            // 
            // pnlMain
            // 
            resources.ApplyResources(this.pnlMain, "pnlMain");
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Round = 35;
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlHeader);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
            this.pnlHeader.ResumeLayout(false);
            this.menuHandle1.ResumeLayout(false);
            this.menuHandle1.PerformLayout();
            this.DebugUAC.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.controlHandle1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer tmrUpdateUI;
    private System.Windows.Forms.Panel pnlHeader;
        private YungkuSystem.Controls.MyPanel pnlMain;
        private System.Windows.Forms.ContextMenuStrip DebugUAC;
        private System.Windows.Forms.CheckBox checkBox_Fixmode;
        private System.Windows.Forms.Label lblWorkState;
        private System.Windows.Forms.Panel pnlBottom;
        private YungkuSystem.Controls.ControlHandle controlHandle1;
        private YungkuSystem.Controls.MenuHandle menuHandle1;
        private YungkuSystem.Controls.FormHandle formHandle1;
        private System.Windows.Forms.ToolStripMenuItem 添加调焦环镭射校准程序ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 强制清除所有动作ToolStripMenuItem;
    }
}

