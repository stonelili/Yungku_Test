namespace Yungku.BNU01_V1.Handler.Pages
{
  partial class FormManual
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.tmrUpdateUI = new System.Windows.Forms.Timer(this.components);
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPane1 = new YungkuSystem.YKControls.Tab.YKTabControlExt();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.sampAxisR = new YungkuSystem.Controls.SingleAxisManualPage();
            this.sampAxisZ = new YungkuSystem.Controls.SingleAxisManualPage();
            this.sampAxisXY = new YungkuSystem.Controls.SingleAxisManualPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.chkUnifiedControl = new System.Windows.Forms.CheckBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.label35 = new System.Windows.Forms.Label();
            this.ledElectricCollimatorAuto = new YungkuSystem.Controls.Led();
            this.tbSimulateDistance_12 = new System.Windows.Forms.TextBox();
            this.tbActDistance12 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_11 = new System.Windows.Forms.TextBox();
            this.tbActDistance11 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_10 = new System.Windows.Forms.TextBox();
            this.tbActDistance10 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_9 = new System.Windows.Forms.TextBox();
            this.tbActDistance9 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.numCollimatorEX = new System.Windows.Forms.NumericUpDown();
            this.numCollimatorCCT = new System.Windows.Forms.NumericUpDown();
            this.numCollimatorAdd = new System.Windows.Forms.NumericUpDown();
            this.tbCollimatorPos1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.BtnCloseC = new System.Windows.Forms.Button();
            this.btnCollimatorParameterWrite = new System.Windows.Forms.Button();
            this.btnCollimatorMove = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.ledCollimatorLedState = new YungkuSystem.Controls.Led();
            this.label7 = new System.Windows.Forms.Label();
            this.BtnLinkCollimator = new System.Windows.Forms.Button();
            this.label48 = new System.Windows.Forms.Label();
            this.ledCollimatorState = new YungkuSystem.Controls.Led();
            this.tbSimulateDistance_8 = new System.Windows.Forms.TextBox();
            this.tbActDistance8 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_7 = new System.Windows.Forms.TextBox();
            this.tbActDistance7 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_6 = new System.Windows.Forms.TextBox();
            this.tbActDistance6 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_5 = new System.Windows.Forms.TextBox();
            this.tbActDistance5 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_4 = new System.Windows.Forms.TextBox();
            this.tbActDistance4 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_3 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_2 = new System.Windows.Forms.TextBox();
            this.tbActDistance3 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_1 = new System.Windows.Forms.TextBox();
            this.tbActDistance2 = new System.Windows.Forms.TextBox();
            this.tbSimulateDistance_0 = new System.Windows.Forms.TextBox();
            this.tbActDistance1 = new System.Windows.Forms.TextBox();
            this.tbActDistance0 = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPane1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCollimatorEX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCollimatorCCT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCollimatorAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrUpdateUI
            // 
            this.tmrUpdateUI.Enabled = true;
            this.tmrUpdateUI.Interval = 1000;
            this.tmrUpdateUI.Tick += new System.EventHandler(this.tmrUpdateUI_Tick);
            // 
            // tabPage3
            // 
            this.tabPage3.ForeColor = System.Drawing.Color.Silver;
            this.tabPage3.Location = new System.Drawing.Point(4, 42);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1004, 654);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = " IO Control ";
            // 
            // tabPane1
            // 
            this.tabPane1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.tabPane1.CloseBtnColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(85)))), ((int)(((byte)(51)))));
            this.tabPane1.Controls.Add(this.tabPage2);
            this.tabPane1.Controls.Add(this.tabPage3);
            this.tabPane1.Controls.Add(this.tabPage1);
            this.tabPane1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPane1.GapHeight = 2;
            this.tabPane1.GapWidth = 2;
            this.tabPane1.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.tabPane1.HeadSelectedBackColor = System.Drawing.Color.White;
            this.tabPane1.IsShowCloseBtn = false;
            this.tabPane1.ItemSize = new System.Drawing.Size(0, 38);
            this.tabPane1.ItemSizeHeight = 38;
            this.tabPane1.Location = new System.Drawing.Point(0, 0);
            this.tabPane1.Name = "tabPane1";
            this.tabPane1.SelectedIndex = 0;
            this.tabPane1.Size = new System.Drawing.Size(1012, 700);
            this.tabPane1.TabIndex = 12;
            this.tabPane1.TabRadiu = 5;
            this.tabPane1.UncloseTabIndexs = null;
            this.tabPane1.SelectedIndexChanged += new System.EventHandler(this.tabPane1_SelectedPageIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.sampAxisR);
            this.tabPage2.Controls.Add(this.sampAxisZ);
            this.tabPage2.Controls.Add(this.sampAxisXY);
            this.tabPage2.ForeColor = System.Drawing.Color.Silver;
            this.tabPage2.Location = new System.Drawing.Point(4, 42);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1004, 654);
            this.tabPage2.TabIndex = 6;
            this.tabPage2.Text = " 电机\\Motor ";
            // 
            // sampAxisR
            // 
            this.sampAxisR.Axis0 = null;
            this.sampAxisR.Axis1 = null;
            this.sampAxisR.Axis2 = null;
            this.sampAxisR.BackColor = System.Drawing.Color.Gray;
            this.sampAxisR.ChkStepEnabled = true;
            this.sampAxisR.HomeButton1Visible = true;
            this.sampAxisR.HomeButton2Visible = false;
            this.sampAxisR.HomeButton3Visible = false;
            this.sampAxisR.IsAlignment = true;
            this.sampAxisR.Lbl1Visible = true;
            this.sampAxisR.Lbl2Visible = false;
            this.sampAxisR.Lbl3Visible = false;
            this.sampAxisR.Location = new System.Drawing.Point(669, 7);
            this.sampAxisR.Manual = null;
            this.sampAxisR.Name = "sampAxisR";
            this.sampAxisR.PageType = YungkuSystem.Controls.SingleAxisManualPage.AxisManualPageType.RotationLeftRight;
            this.sampAxisR.Position = new double[] {
        0.001D,
        0.002D,
        0.005D,
        0.01D,
        0.02D,
        0.05D,
        0.1D,
        0.2D,
        0.5D,
        1D,
        2D,
        5D};
            this.sampAxisR.Position0Label = "  R轴";
            this.sampAxisR.Position1Label = "  R轴";
            this.sampAxisR.Position2Label = "  R轴";
            this.sampAxisR.PositiveIsCW = false;
            this.sampAxisR.Size = new System.Drawing.Size(327, 644);
            this.sampAxisR.StepChecked = true;
            this.sampAxisR.TabIndex = 10;
            this.sampAxisR.Title = "测试R轴";
            // 
            // sampAxisZ
            // 
            this.sampAxisZ.Axis0 = null;
            this.sampAxisZ.Axis1 = null;
            this.sampAxisZ.Axis2 = null;
            this.sampAxisZ.BackColor = System.Drawing.Color.Gray;
            this.sampAxisZ.ChkStepEnabled = true;
            this.sampAxisZ.HomeButton1Visible = false;
            this.sampAxisZ.HomeButton2Visible = true;
            this.sampAxisZ.HomeButton3Visible = false;
            this.sampAxisZ.IsAlignment = false;
            this.sampAxisZ.Lbl1Visible = false;
            this.sampAxisZ.Lbl2Visible = true;
            this.sampAxisZ.Lbl3Visible = false;
            this.sampAxisZ.Location = new System.Drawing.Point(336, 7);
            this.sampAxisZ.Manual = null;
            this.sampAxisZ.Name = "sampAxisZ";
            this.sampAxisZ.PageType = YungkuSystem.Controls.SingleAxisManualPage.AxisManualPageType.LineUpDown;
            this.sampAxisZ.Position = new double[] {
        0.01D,
        0.02D,
        0.05D,
        0.1D,
        0.2D,
        0.5D,
        1D,
        2D,
        5D,
        10D,
        20D,
        50D};
            this.sampAxisZ.Position0Label = "";
            this.sampAxisZ.Position1Label = "Z轴";
            this.sampAxisZ.Position2Label = "";
            this.sampAxisZ.PositiveIsCW = true;
            this.sampAxisZ.Size = new System.Drawing.Size(327, 644);
            this.sampAxisZ.StepChecked = true;
            this.sampAxisZ.TabIndex = 9;
            this.sampAxisZ.Title = "Z轴";
            // 
            // sampAxisXY
            // 
            this.sampAxisXY.Axis0 = null;
            this.sampAxisXY.Axis1 = null;
            this.sampAxisXY.Axis2 = null;
            this.sampAxisXY.BackColor = System.Drawing.Color.Gray;
            this.sampAxisXY.ChkStepEnabled = true;
            this.sampAxisXY.HomeButton1Visible = true;
            this.sampAxisXY.HomeButton2Visible = true;
            this.sampAxisXY.HomeButton3Visible = false;
            this.sampAxisXY.IsAlignment = true;
            this.sampAxisXY.Lbl1Visible = true;
            this.sampAxisXY.Lbl2Visible = true;
            this.sampAxisXY.Lbl3Visible = false;
            this.sampAxisXY.Location = new System.Drawing.Point(3, 7);
            this.sampAxisXY.Manual = null;
            this.sampAxisXY.Name = "sampAxisXY";
            this.sampAxisXY.PageType = YungkuSystem.Controls.SingleAxisManualPage.AxisManualPageType.LineUpDownLeftRight;
            this.sampAxisXY.Position = new double[] {
        0.01D,
        0.02D,
        0.05D,
        0.1D,
        0.2D,
        0.5D,
        1D,
        2D,
        5D,
        10D,
        20D,
        50D};
            this.sampAxisXY.Position0Label = "  X轴";
            this.sampAxisXY.Position1Label = "  Y轴";
            this.sampAxisXY.Position2Label = "";
            this.sampAxisXY.PositiveIsCW = true;
            this.sampAxisXY.Size = new System.Drawing.Size(327, 644);
            this.sampAxisXY.StepChecked = true;
            this.sampAxisXY.TabIndex = 7;
            this.sampAxisXY.Title = "XY轴";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Gray;
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.chkUnifiedControl);
            this.tabPage1.Controls.Add(this.btnInit);
            this.tabPage1.Controls.Add(this.label35);
            this.tabPage1.Controls.Add(this.ledElectricCollimatorAuto);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_12);
            this.tabPage1.Controls.Add(this.tbActDistance12);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_11);
            this.tabPage1.Controls.Add(this.tbActDistance11);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_10);
            this.tabPage1.Controls.Add(this.tbActDistance10);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_9);
            this.tabPage1.Controls.Add(this.tbActDistance9);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.numCollimatorEX);
            this.tabPage1.Controls.Add(this.numCollimatorCCT);
            this.tabPage1.Controls.Add(this.numCollimatorAdd);
            this.tabPage1.Controls.Add(this.tbCollimatorPos1);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.BtnCloseC);
            this.tabPage1.Controls.Add(this.btnCollimatorParameterWrite);
            this.tabPage1.Controls.Add(this.btnCollimatorMove);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.ledCollimatorLedState);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.BtnLinkCollimator);
            this.tabPage1.Controls.Add(this.label48);
            this.tabPage1.Controls.Add(this.ledCollimatorState);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_8);
            this.tabPage1.Controls.Add(this.tbActDistance8);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_7);
            this.tabPage1.Controls.Add(this.tbActDistance7);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_6);
            this.tabPage1.Controls.Add(this.tbActDistance6);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_5);
            this.tabPage1.Controls.Add(this.tbActDistance5);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_4);
            this.tabPage1.Controls.Add(this.tbActDistance4);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_3);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_2);
            this.tabPage1.Controls.Add(this.tbActDistance3);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_1);
            this.tabPage1.Controls.Add(this.tbActDistance2);
            this.tabPage1.Controls.Add(this.tbSimulateDistance_0);
            this.tabPage1.Controls.Add(this.tbActDistance1);
            this.tabPage1.Controls.Add(this.tbActDistance0);
            this.tabPage1.Controls.Add(this.label36);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.label24);
            this.tabPage1.Controls.Add(this.label28);
            this.tabPage1.Controls.Add(this.label32);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.tabPage1.Location = new System.Drawing.Point(4, 42);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1004, 654);
            this.tabPage1.TabIndex = 7;
            this.tabPage1.Text = " 平行光管手动控制 ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Gray;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(452, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 337;
            this.label2.Text = "获取模拟位置m";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Gray;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(308, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 337;
            this.label1.Text = "获取编码器实际位置mm";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(753, 454);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(131, 12);
            this.label13.TabIndex = 336;
            this.label13.Text = "平行光管模拟距离[mm]:";
            // 
            // chkUnifiedControl
            // 
            this.chkUnifiedControl.AutoSize = true;
            this.chkUnifiedControl.Checked = true;
            this.chkUnifiedControl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnifiedControl.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkUnifiedControl.ForeColor = System.Drawing.Color.Yellow;
            this.chkUnifiedControl.Location = new System.Drawing.Point(959, 324);
            this.chkUnifiedControl.Name = "chkUnifiedControl";
            this.chkUnifiedControl.Size = new System.Drawing.Size(128, 16);
            this.chkUnifiedControl.TabIndex = 335;
            this.chkUnifiedControl.Text = "统一控制所有光管";
            this.chkUnifiedControl.UseVisualStyleBackColor = true;
            // 
            // btnInit
            // 
            this.btnInit.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnInit.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnInit.ForeColor = System.Drawing.Color.Black;
            this.btnInit.Location = new System.Drawing.Point(1017, 214);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(79, 26);
            this.btnInit.TabIndex = 334;
            this.btnInit.Text = "复位Init";
            this.btnInit.UseVisualStyleBackColor = false;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.ForeColor = System.Drawing.Color.White;
            this.label35.Location = new System.Drawing.Point(808, 503);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(293, 12);
            this.label35.TabIndex = 333;
            this.label35.Text = "自动刷新按钮[在运行自动流程时会自动关闭连续刷新]";
            // 
            // ledElectricCollimatorAuto
            // 
            this.ledElectricCollimatorAuto.BorderColor = System.Drawing.Color.Black;
            this.ledElectricCollimatorAuto.Location = new System.Drawing.Point(775, 496);
            this.ledElectricCollimatorAuto.Name = "ledElectricCollimatorAuto";
            this.ledElectricCollimatorAuto.OffColor = System.Drawing.Color.Black;
            this.ledElectricCollimatorAuto.OnColor = System.Drawing.Color.Lime;
            this.ledElectricCollimatorAuto.Size = new System.Drawing.Size(27, 26);
            this.ledElectricCollimatorAuto.State = false;
            this.ledElectricCollimatorAuto.TabIndex = 332;
            this.ledElectricCollimatorAuto.Text = "led1";
            this.ledElectricCollimatorAuto.Click += new System.EventHandler(this.ledElectricCollimatorAuto_Click);
            // 
            // tbSimulateDistance_12
            // 
            this.tbSimulateDistance_12.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_12.Location = new System.Drawing.Point(451, 596);
            this.tbSimulateDistance_12.Name = "tbSimulateDistance_12";
            this.tbSimulateDistance_12.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_12.TabIndex = 331;
            // 
            // tbActDistance12
            // 
            this.tbActDistance12.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance12.Location = new System.Drawing.Point(310, 596);
            this.tbActDistance12.Name = "tbActDistance12";
            this.tbActDistance12.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance12.TabIndex = 331;
            // 
            // tbSimulateDistance_11
            // 
            this.tbSimulateDistance_11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_11.Location = new System.Drawing.Point(451, 549);
            this.tbSimulateDistance_11.Name = "tbSimulateDistance_11";
            this.tbSimulateDistance_11.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_11.TabIndex = 329;
            // 
            // tbActDistance11
            // 
            this.tbActDistance11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance11.Location = new System.Drawing.Point(310, 549);
            this.tbActDistance11.Name = "tbActDistance11";
            this.tbActDistance11.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance11.TabIndex = 329;
            // 
            // tbSimulateDistance_10
            // 
            this.tbSimulateDistance_10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_10.Location = new System.Drawing.Point(451, 502);
            this.tbSimulateDistance_10.Name = "tbSimulateDistance_10";
            this.tbSimulateDistance_10.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_10.TabIndex = 327;
            // 
            // tbActDistance10
            // 
            this.tbActDistance10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance10.Location = new System.Drawing.Point(310, 502);
            this.tbActDistance10.Name = "tbActDistance10";
            this.tbActDistance10.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance10.TabIndex = 327;
            // 
            // tbSimulateDistance_9
            // 
            this.tbSimulateDistance_9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_9.Location = new System.Drawing.Point(451, 455);
            this.tbSimulateDistance_9.Name = "tbSimulateDistance_9";
            this.tbSimulateDistance_9.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_9.TabIndex = 325;
            // 
            // tbActDistance9
            // 
            this.tbActDistance9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance9.Location = new System.Drawing.Point(310, 455);
            this.tbActDistance9.Name = "tbActDistance9";
            this.tbActDistance9.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance9.TabIndex = 325;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(97, 600);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(208, 16);
            this.label11.TabIndex = 330;
            this.label11.Text = "平行光管13编码器当前位置:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(97, 553);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(208, 16);
            this.label14.TabIndex = 328;
            this.label14.Text = "平行光管12编码器当前位置:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(97, 506);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(208, 16);
            this.label15.TabIndex = 326;
            this.label15.Text = "平行光管11编码器当前位置:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(97, 459);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(208, 16);
            this.label17.TabIndex = 324;
            this.label17.Text = "平行光管10编码器当前位置:";
            // 
            // numCollimatorEX
            // 
            this.numCollimatorEX.Location = new System.Drawing.Point(886, 403);
            this.numCollimatorEX.Maximum = new decimal(new int[] {
            45,
            0,
            0,
            0});
            this.numCollimatorEX.Name = "numCollimatorEX";
            this.numCollimatorEX.Size = new System.Drawing.Size(67, 21);
            this.numCollimatorEX.TabIndex = 323;
            this.numCollimatorEX.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numCollimatorCCT
            // 
            this.numCollimatorCCT.Location = new System.Drawing.Point(886, 363);
            this.numCollimatorCCT.Maximum = new decimal(new int[] {
            6500,
            0,
            0,
            0});
            this.numCollimatorCCT.Minimum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numCollimatorCCT.Name = "numCollimatorCCT";
            this.numCollimatorCCT.Size = new System.Drawing.Size(67, 21);
            this.numCollimatorCCT.TabIndex = 322;
            this.numCollimatorCCT.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // numCollimatorAdd
            // 
            this.numCollimatorAdd.Location = new System.Drawing.Point(886, 322);
            this.numCollimatorAdd.Maximum = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.numCollimatorAdd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCollimatorAdd.Name = "numCollimatorAdd";
            this.numCollimatorAdd.Size = new System.Drawing.Size(67, 21);
            this.numCollimatorAdd.TabIndex = 321;
            this.numCollimatorAdd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tbCollimatorPos1
            // 
            this.tbCollimatorPos1.Location = new System.Drawing.Point(886, 449);
            this.tbCollimatorPos1.Name = "tbCollimatorPos1";
            this.tbCollimatorPos1.Size = new System.Drawing.Size(67, 21);
            this.tbCollimatorPos1.TabIndex = 320;
            this.tbCollimatorPos1.Text = "10000";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(798, 406);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 12);
            this.label10.TabIndex = 319;
            this.label10.Text = "平行光管亮度:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(798, 365);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 12);
            this.label9.TabIndex = 318;
            this.label9.Text = "平行光管色温:";
            // 
            // BtnCloseC
            // 
            this.BtnCloseC.BackColor = System.Drawing.Color.Red;
            this.BtnCloseC.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnCloseC.ForeColor = System.Drawing.Color.Black;
            this.BtnCloseC.Location = new System.Drawing.Point(916, 259);
            this.BtnCloseC.Name = "BtnCloseC";
            this.BtnCloseC.Size = new System.Drawing.Size(95, 26);
            this.BtnCloseC.TabIndex = 317;
            this.BtnCloseC.Text = "关闭Close";
            this.BtnCloseC.UseVisualStyleBackColor = false;
            this.BtnCloseC.Click += new System.EventHandler(this.BtnCloseC_Click);
            // 
            // btnCollimatorParameterWrite
            // 
            this.btnCollimatorParameterWrite.BackColor = System.Drawing.Color.SpringGreen;
            this.btnCollimatorParameterWrite.ForeColor = System.Drawing.Color.Black;
            this.btnCollimatorParameterWrite.Location = new System.Drawing.Point(969, 402);
            this.btnCollimatorParameterWrite.Name = "btnCollimatorParameterWrite";
            this.btnCollimatorParameterWrite.Size = new System.Drawing.Size(84, 26);
            this.btnCollimatorParameterWrite.TabIndex = 316;
            this.btnCollimatorParameterWrite.Text = "参数写入";
            this.btnCollimatorParameterWrite.UseVisualStyleBackColor = false;
            this.btnCollimatorParameterWrite.Click += new System.EventHandler(this.btnParamWrite_Click);
            // 
            // btnCollimatorMove
            // 
            this.btnCollimatorMove.BackColor = System.Drawing.Color.Yellow;
            this.btnCollimatorMove.ForeColor = System.Drawing.Color.Black;
            this.btnCollimatorMove.Location = new System.Drawing.Point(969, 445);
            this.btnCollimatorMove.Name = "btnCollimatorMove";
            this.btnCollimatorMove.Size = new System.Drawing.Size(84, 26);
            this.btnCollimatorMove.TabIndex = 316;
            this.btnCollimatorMove.Text = "光管移动";
            this.btnCollimatorMove.UseVisualStyleBackColor = false;
            this.btnCollimatorMove.Click += new System.EventHandler(this.btnCollimatorMove_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(798, 324);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 12);
            this.label6.TabIndex = 315;
            this.label6.Text = "平行光管地址:";
            // 
            // ledCollimatorLedState
            // 
            this.ledCollimatorLedState.BorderColor = System.Drawing.Color.Black;
            this.ledCollimatorLedState.Location = new System.Drawing.Point(776, 264);
            this.ledCollimatorLedState.Name = "ledCollimatorLedState";
            this.ledCollimatorLedState.OffColor = System.Drawing.Color.Black;
            this.ledCollimatorLedState.OnColor = System.Drawing.Color.SpringGreen;
            this.ledCollimatorLedState.Size = new System.Drawing.Size(27, 26);
            this.ledCollimatorLedState.State = false;
            this.ledCollimatorLedState.TabIndex = 314;
            this.ledCollimatorLedState.Text = "led2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(809, 271);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 12);
            this.label7.TabIndex = 313;
            this.label7.Text = "平行光管光源状态";
            // 
            // BtnLinkCollimator
            // 
            this.BtnLinkCollimator.BackColor = System.Drawing.Color.SpringGreen;
            this.BtnLinkCollimator.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnLinkCollimator.ForeColor = System.Drawing.Color.Black;
            this.BtnLinkCollimator.Location = new System.Drawing.Point(916, 214);
            this.BtnLinkCollimator.Name = "BtnLinkCollimator";
            this.BtnLinkCollimator.Size = new System.Drawing.Size(95, 26);
            this.BtnLinkCollimator.TabIndex = 312;
            this.BtnLinkCollimator.Text = "连接Connect";
            this.BtnLinkCollimator.UseVisualStyleBackColor = false;
            this.BtnLinkCollimator.Click += new System.EventHandler(this.BtnLinkCollimator_Click);
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.ForeColor = System.Drawing.Color.White;
            this.label48.Location = new System.Drawing.Point(809, 221);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(101, 12);
            this.label48.TabIndex = 310;
            this.label48.Text = "平行光管连接状态";
            // 
            // ledCollimatorState
            // 
            this.ledCollimatorState.BorderColor = System.Drawing.Color.Black;
            this.ledCollimatorState.Location = new System.Drawing.Point(775, 214);
            this.ledCollimatorState.Name = "ledCollimatorState";
            this.ledCollimatorState.OffColor = System.Drawing.Color.Black;
            this.ledCollimatorState.OnColor = System.Drawing.Color.Lime;
            this.ledCollimatorState.Size = new System.Drawing.Size(27, 26);
            this.ledCollimatorState.State = true;
            this.ledCollimatorState.TabIndex = 311;
            this.ledCollimatorState.Text = "led1";
            // 
            // tbSimulateDistance_8
            // 
            this.tbSimulateDistance_8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_8.Location = new System.Drawing.Point(451, 408);
            this.tbSimulateDistance_8.Name = "tbSimulateDistance_8";
            this.tbSimulateDistance_8.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_8.TabIndex = 309;
            // 
            // tbActDistance8
            // 
            this.tbActDistance8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance8.Location = new System.Drawing.Point(310, 408);
            this.tbActDistance8.Name = "tbActDistance8";
            this.tbActDistance8.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance8.TabIndex = 309;
            // 
            // tbSimulateDistance_7
            // 
            this.tbSimulateDistance_7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_7.Location = new System.Drawing.Point(451, 361);
            this.tbSimulateDistance_7.Name = "tbSimulateDistance_7";
            this.tbSimulateDistance_7.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_7.TabIndex = 307;
            // 
            // tbActDistance7
            // 
            this.tbActDistance7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance7.Location = new System.Drawing.Point(310, 361);
            this.tbActDistance7.Name = "tbActDistance7";
            this.tbActDistance7.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance7.TabIndex = 307;
            // 
            // tbSimulateDistance_6
            // 
            this.tbSimulateDistance_6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_6.Location = new System.Drawing.Point(451, 314);
            this.tbSimulateDistance_6.Name = "tbSimulateDistance_6";
            this.tbSimulateDistance_6.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_6.TabIndex = 305;
            // 
            // tbActDistance6
            // 
            this.tbActDistance6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance6.Location = new System.Drawing.Point(310, 314);
            this.tbActDistance6.Name = "tbActDistance6";
            this.tbActDistance6.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance6.TabIndex = 305;
            // 
            // tbSimulateDistance_5
            // 
            this.tbSimulateDistance_5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_5.Location = new System.Drawing.Point(451, 267);
            this.tbSimulateDistance_5.Name = "tbSimulateDistance_5";
            this.tbSimulateDistance_5.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_5.TabIndex = 303;
            // 
            // tbActDistance5
            // 
            this.tbActDistance5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance5.Location = new System.Drawing.Point(310, 267);
            this.tbActDistance5.Name = "tbActDistance5";
            this.tbActDistance5.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance5.TabIndex = 303;
            // 
            // tbSimulateDistance_4
            // 
            this.tbSimulateDistance_4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_4.Location = new System.Drawing.Point(451, 220);
            this.tbSimulateDistance_4.Name = "tbSimulateDistance_4";
            this.tbSimulateDistance_4.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_4.TabIndex = 301;
            // 
            // tbActDistance4
            // 
            this.tbActDistance4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance4.Location = new System.Drawing.Point(310, 220);
            this.tbActDistance4.Name = "tbActDistance4";
            this.tbActDistance4.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance4.TabIndex = 301;
            // 
            // tbSimulateDistance_3
            // 
            this.tbSimulateDistance_3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_3.Location = new System.Drawing.Point(451, 173);
            this.tbSimulateDistance_3.Name = "tbSimulateDistance_3";
            this.tbSimulateDistance_3.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_3.TabIndex = 299;
            // 
            // tbSimulateDistance_2
            // 
            this.tbSimulateDistance_2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_2.Location = new System.Drawing.Point(451, 126);
            this.tbSimulateDistance_2.Name = "tbSimulateDistance_2";
            this.tbSimulateDistance_2.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_2.TabIndex = 297;
            // 
            // tbActDistance3
            // 
            this.tbActDistance3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance3.Location = new System.Drawing.Point(310, 173);
            this.tbActDistance3.Name = "tbActDistance3";
            this.tbActDistance3.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance3.TabIndex = 299;
            // 
            // tbSimulateDistance_1
            // 
            this.tbSimulateDistance_1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_1.Location = new System.Drawing.Point(451, 79);
            this.tbSimulateDistance_1.Name = "tbSimulateDistance_1";
            this.tbSimulateDistance_1.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_1.TabIndex = 295;
            // 
            // tbActDistance2
            // 
            this.tbActDistance2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance2.Location = new System.Drawing.Point(310, 126);
            this.tbActDistance2.Name = "tbActDistance2";
            this.tbActDistance2.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance2.TabIndex = 297;
            // 
            // tbSimulateDistance_0
            // 
            this.tbSimulateDistance_0.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbSimulateDistance_0.Location = new System.Drawing.Point(451, 32);
            this.tbSimulateDistance_0.Name = "tbSimulateDistance_0";
            this.tbSimulateDistance_0.Size = new System.Drawing.Size(117, 26);
            this.tbSimulateDistance_0.TabIndex = 293;
            // 
            // tbActDistance1
            // 
            this.tbActDistance1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance1.Location = new System.Drawing.Point(310, 79);
            this.tbActDistance1.Name = "tbActDistance1";
            this.tbActDistance1.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance1.TabIndex = 295;
            // 
            // tbActDistance0
            // 
            this.tbActDistance0.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbActDistance0.Location = new System.Drawing.Point(310, 32);
            this.tbActDistance0.Name = "tbActDistance0";
            this.tbActDistance0.Size = new System.Drawing.Size(123, 26);
            this.tbActDistance0.TabIndex = 293;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label36.ForeColor = System.Drawing.Color.Black;
            this.label36.Location = new System.Drawing.Point(105, 412);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(200, 16);
            this.label36.TabIndex = 308;
            this.label36.Text = "平行光管9编码器当前位置:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.ForeColor = System.Drawing.Color.Black;
            this.label20.Location = new System.Drawing.Point(105, 365);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(200, 16);
            this.label20.TabIndex = 306;
            this.label20.Text = "平行光管8编码器当前位置:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.ForeColor = System.Drawing.Color.Black;
            this.label24.Location = new System.Drawing.Point(105, 318);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(200, 16);
            this.label24.TabIndex = 304;
            this.label24.Text = "平行光管7编码器当前位置:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label28.ForeColor = System.Drawing.Color.Black;
            this.label28.Location = new System.Drawing.Point(105, 271);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(200, 16);
            this.label28.TabIndex = 302;
            this.label28.Text = "平行光管6编码器当前位置:";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label32.ForeColor = System.Drawing.Color.Black;
            this.label32.Location = new System.Drawing.Point(105, 224);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(200, 16);
            this.label32.TabIndex = 300;
            this.label32.Text = "平行光管5编码器当前位置:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(105, 177);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(200, 16);
            this.label12.TabIndex = 298;
            this.label12.Text = "平行光管4编码器当前位置:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Location = new System.Drawing.Point(105, 130);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(200, 16);
            this.label16.TabIndex = 296;
            this.label16.Text = "平行光管3编码器当前位置:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(105, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(200, 16);
            this.label8.TabIndex = 294;
            this.label8.Text = "平行光管2编码器当前位置:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(105, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(200, 16);
            this.label5.TabIndex = 292;
            this.label5.Text = "平行光管1编码器当前位置:";
            // 
            // FormManual
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1012, 700);
            this.Controls.Add(this.tabPane1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormManual";
            this.Text = "FormAuto";
            this.Load += new System.EventHandler(this.FormManual_Load);
            this.VisibleChanged += new System.EventHandler(this.FormManual_VisibleChanged);
            this.tabPane1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCollimatorEX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCollimatorCCT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCollimatorAdd)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer tmrUpdateUI;
        private System.Windows.Forms.TabPage tabPage3;
        private YungkuSystem.YKControls.Tab.YKTabControlExt tabPane1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox chkUnifiedControl;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Label label35;
        private YungkuSystem.Controls.Led ledElectricCollimatorAuto;
        private System.Windows.Forms.TextBox tbActDistance12;
        private System.Windows.Forms.TextBox tbActDistance11;
        private System.Windows.Forms.TextBox tbActDistance10;
        private System.Windows.Forms.TextBox tbActDistance9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown numCollimatorEX;
        private System.Windows.Forms.NumericUpDown numCollimatorCCT;
        private System.Windows.Forms.NumericUpDown numCollimatorAdd;
        private System.Windows.Forms.TextBox tbCollimatorPos1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button BtnCloseC;
        private System.Windows.Forms.Button btnCollimatorMove;
        private System.Windows.Forms.Label label6;
        private YungkuSystem.Controls.Led ledCollimatorLedState;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button BtnLinkCollimator;
        private System.Windows.Forms.Label label48;
        private YungkuSystem.Controls.Led ledCollimatorState;
        private System.Windows.Forms.TextBox tbActDistance8;
        private System.Windows.Forms.TextBox tbActDistance7;
        private System.Windows.Forms.TextBox tbActDistance6;
        private System.Windows.Forms.TextBox tbActDistance5;
        private System.Windows.Forms.TextBox tbActDistance4;
        private System.Windows.Forms.TextBox tbActDistance3;
        private System.Windows.Forms.TextBox tbActDistance2;
        private System.Windows.Forms.TextBox tbActDistance1;
        private System.Windows.Forms.TextBox tbActDistance0;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSimulateDistance_12;
        private System.Windows.Forms.TextBox tbSimulateDistance_11;
        private System.Windows.Forms.TextBox tbSimulateDistance_10;
        private System.Windows.Forms.TextBox tbSimulateDistance_9;
        private System.Windows.Forms.TextBox tbSimulateDistance_8;
        private System.Windows.Forms.TextBox tbSimulateDistance_7;
        private System.Windows.Forms.TextBox tbSimulateDistance_6;
        private System.Windows.Forms.TextBox tbSimulateDistance_5;
        private System.Windows.Forms.TextBox tbSimulateDistance_4;
        private System.Windows.Forms.TextBox tbSimulateDistance_3;
        private System.Windows.Forms.TextBox tbSimulateDistance_2;
        private System.Windows.Forms.TextBox tbSimulateDistance_1;
        private System.Windows.Forms.TextBox tbSimulateDistance_0;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCollimatorParameterWrite;
        private YungkuSystem.Controls.SingleAxisManualPage sampAxisZ;
        private YungkuSystem.Controls.SingleAxisManualPage sampAxisXY;
        private YungkuSystem.Controls.SingleAxisManualPage sampAxisR;
    }
}