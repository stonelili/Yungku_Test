namespace Yungku.BNU01_V1.Handler.Pages
{
  partial class FormSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetting));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("节点18");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("节点14", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("节点15");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("节点5", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("节点6");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("节点0", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("节点16");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("节点7", new System.Windows.Forms.TreeNode[] {
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("节点8");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("节点1", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("节点17");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("节点9", new System.Windows.Forms.TreeNode[] {
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("节点10");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("节点2", new System.Windows.Forms.TreeNode[] {
            treeNode12,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("节点11");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("节点12");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("节点3", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("节点13");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("节点4", new System.Windows.Forms.TreeNode[] {
            treeNode18});
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvAllSetting = new YungkuSystem.YKControls.Treeview.YKTreeViewEx();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
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
            this.splitContainer1.Size = new System.Drawing.Size(1200, 700);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 1;
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
            treeNode1.Name = "节点18";
            treeNode1.Text = "节点18";
            treeNode2.Name = "节点14";
            treeNode2.Text = "节点14";
            treeNode3.Name = "节点15";
            treeNode3.Text = "节点15";
            treeNode4.Name = "节点5";
            treeNode4.Text = "节点5";
            treeNode5.Name = "节点6";
            treeNode5.Text = "节点6";
            treeNode6.Name = "节点0";
            treeNode6.Text = "节点0";
            treeNode7.Name = "节点16";
            treeNode7.Text = "节点16";
            treeNode8.Name = "节点7";
            treeNode8.Text = "节点7";
            treeNode9.Name = "节点8";
            treeNode9.Text = "节点8";
            treeNode10.Name = "节点1";
            treeNode10.Text = "节点1";
            treeNode11.Name = "节点17";
            treeNode11.Text = "节点17";
            treeNode12.Name = "节点9";
            treeNode12.Text = "节点9";
            treeNode13.Name = "节点10";
            treeNode13.Text = "节点10";
            treeNode14.Name = "节点2";
            treeNode14.Text = "节点2";
            treeNode15.Name = "节点11";
            treeNode15.Text = "节点11";
            treeNode16.Name = "节点12";
            treeNode16.Text = "节点12";
            treeNode17.Name = "节点3";
            treeNode17.Text = "节点3";
            treeNode18.Name = "节点13";
            treeNode18.Text = "节点13";
            treeNode19.Name = "节点4";
            treeNode19.Text = "节点4";
            this.tvAllSetting.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode10,
            treeNode14,
            treeNode17,
            treeNode19});
            this.tvAllSetting.NodeSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.tvAllSetting.NodeSelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.tvAllSetting.NodeSplitLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.tvAllSetting.NodeUpPic = global::Yungku.BNU01_V1.Handler.Properties.Resources.down;
            this.tvAllSetting.ParentNodeCanSelect = true;
            this.tvAllSetting.ShowLines = false;
            this.tvAllSetting.ShowPlusMinus = false;
            this.tvAllSetting.ShowRootLines = false;
            this.tvAllSetting.Size = new System.Drawing.Size(300, 700);
            this.tvAllSetting.TabIndex = 19;
            this.tvAllSetting.TipFont = new System.Drawing.Font("Arial Unicode MS", 12F);
            this.tvAllSetting.TipImage = ((System.Drawing.Image)(resources.GetObject("tvAllSetting.TipImage")));
            this.tvAllSetting.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvAllSetting_AfterSelect);
            // 
            // FormSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormSetting";
            this.Text = "FormSetting";
            this.VisibleChanged += new System.EventHandler(this.FormSetting_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.SplitContainer splitContainer1;
        private YungkuSystem.YKControls.Treeview.YKTreeViewEx tvAllSetting;
    }
}