using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YungkuSystem.Motion.Manage;
using YungkuSystem;
using YungkuSystem.ThreadMessage;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Intf;
using YungkuSystem.LightMotion.Intf;
using YungkuSystem.Tester;
using YungkuSystem.Controls;
using Yungku.Devices.SmallScreen;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormSetup : Form
    {


        public FormSetup()
        {
            InitializeComponent();
        }
        private Form currentForm = null;
        internal void DockToPanel(Form frm)
        {
            if (frm == currentForm)
                return;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Parent = pnlSetupMain;
            frm.Left = 0;
            frm.Top = 0;
            frm.Dock = DockStyle.Fill;
            frm.Show();
            if (currentForm != null)
                currentForm.Hide();
            currentForm = frm;
        }

        private void FormSetup_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                MyApp.GetInstance().Logger.WriteRecord(G.Text("切换到系统设置画面"));
                ReshowTree();
            }
        }
        private void ReshowTree()
        {
            tvAllSetting.BeginUpdate();
            tvAllSetting.Nodes.Clear();
            TreeNode BaseRoot = tvAllSetting.Nodes.Add(G.Text("系统设置"));
            BaseRoot.Tag = null;
            BaseRoot.ForeColor = Color.White;
            int i = 0;
            TreeNode root = null;
            if (MyApp.GetInstance().MotionService != null)
            {
                root = BaseRoot.Nodes.Add(G.Text("硬件控制设置"));
                root.Tag = null;
                root.ForeColor = Color.White;
                i = 0;
                foreach (string key in MyApp.GetInstance().MotionService.ManagerList.Keys)
                {
                    IMotionSystem tt = MyApp.GetInstance().MotionService.ManagerList[key];
                    TreeNode ttNode = root.Nodes.Add(string.Format("{0}_{1}", key, i.ToString()));
                    ttNode.Tag = tt;
                    ttNode.ForeColor = Color.White;
                    TreeNode offsetNode = ttNode.Nodes.Add(G.Text("原点补偿"));
                    offsetNode.Tag = tt;
                    offsetNode.ForeColor = Color.White;
                    i++;
                }
            }


            root = BaseRoot.Nodes.Add(G.Text("语言设置"));
            root.Tag = BilingualTranslation.GetInstance();
            root.ForeColor = Color.White;

            if (MyApp.GetInstance().UAC != null)
            {
                root = BaseRoot.Nodes.Add(G.Text("用户设置"));
                root.Tag = MyApp.GetInstance().UAC;
                root.ForeColor = Color.White;
            }


            BaseRoot.ExpandAll();
            tvAllSetting.EndUpdate();
        }


        private void tvAllSetting_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Form form = null;
            IConfigPage config = null;
            if (e.Node != null)
            {
                if (e.Node.Tag is IMotionSystem && e.Node.Parent != null && e.Node.Parent.Tag is IMotionSystem)
                {
                    form = (e.Node.Tag as IMotionSystem).GetHomeOffSetForm();
                }
                else if (e.Node.Tag is IConfigPage)
                {
                    config = e.Node.Tag as IConfigPage;
                }
            }
            if (config != null)
                form = config.GetConfigForm();
            if (form != null)
                DockToPanel(form);
            else
            {
                pnlSetupMain.Controls.Clear();
                currentForm = null;
            }
        }

    }
}
