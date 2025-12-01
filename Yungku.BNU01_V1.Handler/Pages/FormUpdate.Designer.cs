namespace Yungku.BNU01_V1.Handler.Pages
{
    partial class FormUpdate
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
            YungkuSystem.YKControls.TimeLine.TimeLineItem timeLineItem1 = new YungkuSystem.YKControls.TimeLine.TimeLineItem();
            YungkuSystem.YKControls.TimeLine.TimeLineItem timeLineItem2 = new YungkuSystem.YKControls.TimeLine.TimeLineItem();
            this.ykTimeLine1 = new YungkuSystem.YKControls.TimeLine.YKTimeLine();
            this.SuspendLayout();
            // 
            // ykTimeLine1
            // 
            this.ykTimeLine1.AutoScroll = true;
            this.ykTimeLine1.DetailsFont = new System.Drawing.Font("微软雅黑", 9F);
            this.ykTimeLine1.DetailsForcolor = System.Drawing.Color.White;
            this.ykTimeLine1.Dock = System.Windows.Forms.DockStyle.Fill;
            timeLineItem1.Details = "1、基础版本。\\n2、完善完成。";
            timeLineItem1.Title = "2025年10月28日";
            timeLineItem2.Details = "1、增加测试位2设置功能";
            timeLineItem2.Title = "2025年11月19日";
            this.ykTimeLine1.Items = new YungkuSystem.YKControls.TimeLine.TimeLineItem[] {
        timeLineItem1,
        timeLineItem2};
            this.ykTimeLine1.LineColor = System.Drawing.Color.Lime;
            this.ykTimeLine1.Location = new System.Drawing.Point(0, 0);
            this.ykTimeLine1.Name = "ykTimeLine1";
            this.ykTimeLine1.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.ykTimeLine1.Size = new System.Drawing.Size(975, 700);
            this.ykTimeLine1.TabIndex = 0;
            this.ykTimeLine1.TitleFont = new System.Drawing.Font("微软雅黑", 12F);
            this.ykTimeLine1.TitleForcolor = System.Drawing.Color.Lime;
            // 
            // FormUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(975, 700);
            this.Controls.Add(this.ykTimeLine1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormUpdate";
            this.Text = "FormUpdate";
            this.ResumeLayout(false);

        }

        #endregion

        private YungkuSystem.YKControls.TimeLine.YKTimeLine ykTimeLine1;
    }
}