namespace Yungku.BNU01_V1.Handler.Pages
{
  partial class FormLogin
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
            this.panelControl2 = new YungkuSystem.Controls.MyPanel();
            this.btnEnter = new YungkuSystem.YKControls.Btn.YKBtnExt();
            this.myPanel2 = new YungkuSystem.Controls.MyPanel();
            this.ykBtnExt2 = new YungkuSystem.YKControls.Btn.YKBtnExt();
            this.cbbUserName = new YungkuSystem.YKControls.ComboBox.YKCombox();
            this.txtPassword = new YungkuSystem.YKControls.Text.YKTextBoxEx();
            this.panelControl2.SuspendLayout();
            this.myPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl2
            // 
            this.panelControl2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelControl2.Controls.Add(this.btnEnter);
            this.panelControl2.Controls.Add(this.myPanel2);
            this.panelControl2.Controls.Add(this.cbbUserName);
            this.panelControl2.Controls.Add(this.txtPassword);
            this.panelControl2.ForeColor = System.Drawing.Color.White;
            this.panelControl2.Location = new System.Drawing.Point(370, 195);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Round = 55;
            this.panelControl2.Size = new System.Drawing.Size(460, 310);
            this.panelControl2.TabIndex = 21;
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnEnter.BtnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnEnter.BtnFont = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnEnter.BtnForeColor = System.Drawing.Color.White;
            this.btnEnter.BtnText = "登入";
            this.btnEnter.ConerRadius = 25;
            this.btnEnter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnter.DownColor = System.Drawing.Color.White;
            this.btnEnter.EnabledMouseEffect = true;
            this.btnEnter.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(84)))));
            this.btnEnter.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.btnEnter.ForeColor = System.Drawing.Color.White;
            this.btnEnter.IsRadius = true;
            this.btnEnter.IsShowRect = true;
            this.btnEnter.IsShowTips = false;
            this.btnEnter.Location = new System.Drawing.Point(170, 229);
            this.btnEnter.Margin = new System.Windows.Forms.Padding(0);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnEnter.RectWidth = 1;
            this.btnEnter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEnter.Size = new System.Drawing.Size(120, 45);
            this.btnEnter.TabIndex = 21;
            this.btnEnter.TabStop = false;
            this.btnEnter.TipsColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnEnter.TipsText = "";
            this.btnEnter.BtnClick += new System.EventHandler(this.btnEnter_Click);
            // 
            // myPanel2
            // 
            this.myPanel2.BackColor = System.Drawing.Color.Silver;
            this.myPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.myPanel2.Controls.Add(this.ykBtnExt2);
            this.myPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.myPanel2.Location = new System.Drawing.Point(0, 0);
            this.myPanel2.Name = "myPanel2";
            this.myPanel2.Round = 15;
            this.myPanel2.Size = new System.Drawing.Size(460, 35);
            this.myPanel2.TabIndex = 20;
            this.myPanel2.Text = "安全验证";
            // 
            // ykBtnExt2
            // 
            this.ykBtnExt2.BackColor = System.Drawing.Color.White;
            this.ykBtnExt2.BtnBackColor = System.Drawing.Color.White;
            this.ykBtnExt2.BtnFont = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.ykBtnExt2.BtnForeColor = System.Drawing.Color.Black;
            this.ykBtnExt2.BtnText = "X";
            this.ykBtnExt2.ConerRadius = 5;
            this.ykBtnExt2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ykBtnExt2.DownColor = System.Drawing.Color.White;
            this.ykBtnExt2.EnabledMouseEffect = true;
            this.ykBtnExt2.FillColor = System.Drawing.Color.Silver;
            this.ykBtnExt2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ykBtnExt2.IsRadius = true;
            this.ykBtnExt2.IsShowRect = true;
            this.ykBtnExt2.IsShowTips = false;
            this.ykBtnExt2.Location = new System.Drawing.Point(430, 5);
            this.ykBtnExt2.Margin = new System.Windows.Forms.Padding(0);
            this.ykBtnExt2.Name = "ykBtnExt2";
            this.ykBtnExt2.RectColor = System.Drawing.Color.Black;
            this.ykBtnExt2.RectWidth = 1;
            this.ykBtnExt2.Size = new System.Drawing.Size(25, 25);
            this.ykBtnExt2.TabIndex = 22;
            this.ykBtnExt2.TabStop = false;
            this.ykBtnExt2.TipsColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(30)))), ((int)(((byte)(99)))));
            this.ykBtnExt2.TipsText = "";
            this.ykBtnExt2.BtnClick += new System.EventHandler(this.ykBtnExt2_BtnClick);
            // 
            // cbbUserName
            // 
            this.cbbUserName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.cbbUserName.BoxStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cbbUserName.ConerRadius = 5;
            this.cbbUserName.DropPanelHeight = -1;
            this.cbbUserName.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.cbbUserName.IsRadius = true;
            this.cbbUserName.IsShowKeyImg = true;
            this.cbbUserName.IsShowRect = true;
            this.cbbUserName.ItemWidth = 70;
            this.cbbUserName.Location = new System.Drawing.Point(80, 76);
            this.cbbUserName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbbUserName.Name = "cbbUserName";
            this.cbbUserName.PenColor = System.Drawing.Color.White;
            this.cbbUserName.PromptColor = System.Drawing.Color.Silver;
            this.cbbUserName.PromptFont = new System.Drawing.Font("微软雅黑", 15F);
            this.cbbUserName.PromptText = "用户名";
            this.cbbUserName.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.cbbUserName.RectWidth = 1;
            this.cbbUserName.SelectedIndex = -1;
            this.cbbUserName.SelectedValue = "";
            this.cbbUserName.Size = new System.Drawing.Size(300, 45);
            this.cbbUserName.Source = null;
            this.cbbUserName.TabIndex = 18;
            this.cbbUserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cbbUserName.TextValue = "";
            this.cbbUserName.TxtBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cbbUserName.KeyDownChanged += new System.EventHandler<System.Windows.Forms.KeyEventArgs>(this.txtUserName_KeyDown);
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtPassword.ConerRadius = 5;
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.DecLength = 2;
            this.txtPassword.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtPassword.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtPassword.InputText = "";
            this.txtPassword.InputType = YungkuSystem.YKControls.Helpers.TextInputType.NotControl;
            this.txtPassword.IsFocusColor = true;
            this.txtPassword.IsRadius = true;
            this.txtPassword.IsShowKeyboard = true;
            this.txtPassword.IsShowKeyImg = true;
            this.txtPassword.IsShowRect = true;
            this.txtPassword.KeyBoardType = YungkuSystem.YKControls.Text.KeyBoardType.UCKeyBorderAll_EN;
            this.txtPassword.Location = new System.Drawing.Point(80, 145);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.txtPassword.MinValue = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.PenColor = System.Drawing.Color.White;
            this.txtPassword.PromptColor = System.Drawing.Color.Silver;
            this.txtPassword.PromptFont = new System.Drawing.Font("微软雅黑", 15F);
            this.txtPassword.PromptText = "密码";
            this.txtPassword.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtPassword.RectWidth = 1;
            this.txtPassword.RegexPattern = "";
            this.txtPassword.Size = new System.Drawing.Size(300, 45);
            this.txtPassword.TabIndex = 19;
            this.txtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPassword.TxtBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.KeyDownChanged += new System.EventHandler<System.Windows.Forms.KeyEventArgs>(this.txtPassword_KeyDown);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.panelControl2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.Text = "FormLogin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.SizeChanged += new System.EventHandler(this.FormLogin_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.FormLogin_VisibleChanged);
            this.Resize += new System.EventHandler(this.FormLogin_Resize);
            this.panelControl2.ResumeLayout(false);
            this.myPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion
        private YungkuSystem.Controls.MyPanel panelControl2;
        private YungkuSystem.Controls.MyPanel myPanel2;
        private YungkuSystem.YKControls.ComboBox.YKCombox cbbUserName;
        private YungkuSystem.YKControls.Text.YKTextBoxEx txtPassword;
        private YungkuSystem.YKControls.Btn.YKBtnExt btnEnter;
        private YungkuSystem.YKControls.Btn.YKBtnExt ykBtnExt2;
    }
}