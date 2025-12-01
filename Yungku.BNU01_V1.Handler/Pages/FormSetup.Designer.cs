namespace Yungku.BNU01_V1.Handler.Pages
{
  partial class FormSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetup));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlSetupMain = new YungkuSystem.Controls.MyPanel();
            this.tvAllSetting = new YungkuSystem.YKControls.Treeview.YKTreeViewEx();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvAllSetting);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlSetupMain);
            this.splitContainer1.Size = new System.Drawing.Size(1200, 700);
            this.splitContainer1.SplitterDistance = 221;
            this.splitContainer1.TabIndex = 7;
            // 
            // pnlSetupMain
            // 
            this.pnlSetupMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSetupMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSetupMain.Location = new System.Drawing.Point(0, 0);
            this.pnlSetupMain.Name = "pnlSetupMain";
            this.pnlSetupMain.Round = 35;
            this.pnlSetupMain.Size = new System.Drawing.Size(975, 700);
            this.pnlSetupMain.TabIndex = 6;
            // 
            // tvAllSetting
            // 
            this.tvAllSetting.BackColor = System.Drawing.Color.Gray;
            this.tvAllSetting.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvAllSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvAllSetting.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.tvAllSetting.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvAllSetting.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.tvAllSetting.FullRowSelect = true;
            this.tvAllSetting.HideSelection = false;
            this.tvAllSetting.IntEmptyNode = 20;
            this.tvAllSetting.IsShowByCustomModel = true;
            this.tvAllSetting.IsShowTip = true;
            this.tvAllSetting.ItemHeight = 26;
            this.tvAllSetting.Location = new System.Drawing.Point(0, 0);
            this.tvAllSetting.LstTips = ((System.Collections.Generic.Dictionary<string, string>)(resources.GetObject("tvAllSetting.LstTips")));
            this.tvAllSetting.Name = "tvAllSetting";
            this.tvAllSetting.NodeBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.tvAllSetting.NodeDownPic = global::Yungku.BNU01_V1.Handler.Properties.Resources.right;
            this.tvAllSetting.NodeForeColor = System.Drawing.Color.WhiteSmoke;
            this.tvAllSetting.NodeHeight = 26;
            this.tvAllSetting.NodeIsShowSplitLine = false;
            this.tvAllSetting.NodeSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.tvAllSetting.NodeSelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.tvAllSetting.NodeSplitLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.tvAllSetting.NodeUpPic = global::Yungku.BNU01_V1.Handler.Properties.Resources.down;
            this.tvAllSetting.ParentNodeCanSelect = true;
            this.tvAllSetting.ShowLines = false;
            this.tvAllSetting.ShowPlusMinus = false;
            this.tvAllSetting.ShowRootLines = false;
            this.tvAllSetting.Size = new System.Drawing.Size(221, 700);
            this.tvAllSetting.TabIndex = 19;
            this.tvAllSetting.TipFont = new System.Drawing.Font("Arial Unicode MS", 12F);
            this.tvAllSetting.TipImage = ((System.Drawing.Image)(resources.GetObject("tvAllSetting.TipImage")));
            this.tvAllSetting.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvAllSetting_AfterSelect);
            // 
            // FormSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSetup";
            this.Text = "FormSetup";
            this.VisibleChanged += new System.EventHandler(this.FormSetup_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion
    private YungkuSystem.Controls.MyPanel pnlSetupMain;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private YungkuSystem.YKControls.Treeview.YKTreeViewEx tvAllSetting;
    }
}