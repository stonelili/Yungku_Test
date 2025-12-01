using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yungku.Devices.SmallScreen;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.LightMotion.Intf;
using YungkuSystem.TCMotion.Intf;
using YungkuSystem.Tester;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormDebug : Form
    {
        public FormDebug()
        {
            InitializeComponent();
        }
        private FormUpdate frmUpdate = new FormUpdate();
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

        private void FormDebug_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                MyApp.GetInstance().Logger.WriteRecord(G.Text("切换到安装调试画面"));
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
          
            if (MyApp.GetInstance().LightService != null)
            {
                #region 
                //root = BaseRoot.Nodes.Add(G.Text("光源控制设置"));
                //root.Tag = null;
                //root.ForeColor = Color.White;
                //i = 0;
                //foreach (string key in MyApp.GetInstance().LightService.ManagerList.Keys)
                //{
                //    ILightSystem tt = MyApp.GetInstance().LightService.ManagerList[key];
                //    TreeNode ttNode = root.Nodes.Add(string.Format("{0}_{1}", key, i.ToString()));
                //    ttNode.Tag = tt;
                //    ttNode.ForeColor = Color.White;
                //    i++;
                //}
                #endregion
            }

            if (MyApp.GetInstance().TcService != null)
            {
                #region 
                //root = BaseRoot.Nodes.Add("温度控制设置");
                //root.Tag = null;
                //root.ForeColor = Color.White;
                //i = 0;
                //foreach (string key in MyApp.GetInstance().TcService.ManagerList.Keys)
                //{
                //    ITCSystem tt = MyApp.GetInstance().TcService.ManagerList[key];
                //    TreeNode ttNode = root.Nodes.Add(string.Format("{0}_{1}", key, i.ToString()));
                //    ttNode.Tag = tt;
                //    ttNode.ForeColor = Color.White;
                //    i++;
                //}
                #endregion
            }

            if (MyApp.GetInstance().TestService != null)
            {
                root = BaseRoot.Nodes.Add(G.Text("测试控制设置"));
                root.Tag = null;
                root.ForeColor = Color.White;
                i = 0;
                foreach (string key in MyApp.GetInstance().TestService.ManagerList.Keys)
                {
                    ITestSystem tt = MyApp.GetInstance().TestService.ManagerList[key];
                    TreeNode ttNode = root.Nodes.Add(string.Format("{0}_{1}", key, i.ToString()));
                    ttNode.Tag = tt;
                    ttNode.ForeColor = Color.White;
                    i++;
                }
            }

            if (MyApp.GetInstance().FormScreenDisplayer != null)
            {
                //root = BaseRoot.Nodes.Add("小屏幕设置");
                //root.Tag = MyApp.GetInstance().FormScreenDisplayer;
                //root.ForeColor = Color.White;
            }

            root = BaseRoot.Nodes.Add(G.Text("更新说明"));
            root.Tag = frmUpdate;
            root.ForeColor = Color.White;

            BaseRoot.ExpandAll();
            tvAllSetting.EndUpdate();
        }


        private void tvAllSetting_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Form form = null;
            IConfigPage config = null;
            if (e.Node != null)
            {
                if(e.Node .Tag is FormUpdate )
                {
                    form = frmUpdate;
                }
               else  if (e.Node.Tag is FormScreenSimulator)
                {
                    if (MyApp.GetInstance().FormScreenDisplayer.Screen.IsOpen)
                    {
                        form = (e.Node.Tag as FormScreenSimulator);
                    }
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

