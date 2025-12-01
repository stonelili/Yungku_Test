using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Yungku.BNU01_V1.Handler.Config;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using Yungku.BNU01_V1.Handler.Pages;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Motion.Manage;
using YungkuSystem.TestFlow;
using YungkuSystem.Tray;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormSetting : Form
    {
        public FormSetting()
        {
            InitializeComponent();
        }
        private void FormSetting_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                MyApp.GetInstance().Logger.WriteRecord(G.Text("切换到参数设置画面"));
                ReshowTree();
            }
        }
        private void ReshowTree()
        {
            tvAllSetting.BeginUpdate();
            tvAllSetting.Nodes.Clear();
            TreeNode BaseRoot = tvAllSetting.Nodes.Add(G.Text("所有配置项"));
            BaseRoot.Tag = null ;
            BaseRoot.ForeColor = Color.White;

            TreeNode general = BaseRoot.Nodes.Add(G.Text("基本设置"));
            general.Tag = MyApp.Config.General;
            general.ForeColor = Color.White;

            TreeNode fun = BaseRoot.Nodes.Add(G.Text("功能设置"));
            fun.Tag = MyApp.Config.FunctionSwitch ;
            fun.ForeColor = Color.White;

            //if(MyApp.MachineMode==MachineModes.JF06调焦机)
            //{
            //    TreeNode position = BaseRoot.Nodes.Add("调焦点胶设置");
            //    position.Tag = MyApp.Config.MachineTestSetting;
            //    position.ForeColor = Color.White;
            //}

            TreeNode machine = BaseRoot.Nodes.Add("BNU01" + G.Text("设置"));
            machine.Tag = MyApp.GetInstance().Machine;
            machine.ForeColor = Color.White;
           
            TreeNode root = BaseRoot.Nodes.Add(G.Text("测试机设置"));
            root.Tag = MyApp.Config.TestSetting;
            root.ForeColor = Color.White;

            for (int i = 0; i < MyApp.GetInstance().Machine.TestItems.Count; i++)
            {

                Turntable tt = (Turntable)MyApp.GetInstance().Machine.TestItems[i];
                TreeNode ttNode = root.Nodes.Add("转盘"
                    + (tt.Name.Equals(string.Empty) ? "" : ("(" + tt.Name + ")")));
                ttNode.Tag = tt;
                ttNode.ForeColor = Color.White;
                for (int j = 0; j < tt.TestItems.Count; j++)
                {
                    Head head = tt.TestItems[j] as Head;
                    TreeNode headNode = ttNode.Nodes.Add("测试头"
                        + (head.Name.Equals(string.Empty) ? "" : ("(" + head.Name + ")")));
                    headNode.Tag = head;
                    headNode.ForeColor = Color.White;
                    for (int k = 0; k < head.TestItems.Count; k++)
                    {
                        Jig jig = (Jig)head.TestItems[k];
                        TreeNode jigNode = headNode.Nodes.Add("治具"
                            + (jig.Name.Equals(string.Empty) ? "" : ("(" + jig.Name + ")")));
                        jigNode.ForeColor = Color.White;
                        for (int l = 0; l < jig.TestItems.Count; l++)
                        {
                            Product product = (Product)jig.TestItems[l];
                            TreeNode productNode = jigNode.Nodes.Add("产品"
                                + (product.Name.Equals(string.Empty) ? "" : ("(" + product.Name + ")")));
                            productNode.Tag = product;
                            productNode.ForeColor = Color.White;
                        }
                    }                
                }                        
            }
           
            //root = BaseRoot.Nodes.Add("上下料设置");
            //root.Tag = MyApp.Config.AutoSetting;
            //root.ForeColor = Color.White;
            //for (int i = 0; i < MyApp.GetInstance().Machine.AutoItems.Count; i++)
            //{
            //    Manipulator tt = (Manipulator)MyApp.GetInstance().Machine.AutoItems[i];
            //    TreeNode ttNode = root.Nodes.Add("机械臂"
            //        + (tt.Name.Equals(string.Empty) ? "" : ("(" + tt.Name + ")")));
            //    ttNode.Tag = tt;
            //    ttNode.ForeColor = Color.White;
            //    for (int j = 0; j < tt.AutoItems.Count; j++)
            //    {
            //        NozzleGroup group = (NozzleGroup)tt.AutoItems[j];
            //        TreeNode headNode = ttNode.Nodes.Add("吸嘴组"
            //            + (group.Name.Equals(string.Empty) ? "" : ("(" + group.Name + ")")));
            //        headNode.Tag = group;
            //        headNode.ForeColor = Color.White;
            //        for (int k = 0; k < group.AutoItems.Count; k++)
            //        {
            //            Nozzle nozzle = (Nozzle)group.AutoItems[k];
            //            TreeNode jigNode = headNode.Nodes.Add("吸嘴"
            //                + (nozzle.Name.Equals(string.Empty) ? "" : ("(" + nozzle.Name + ")")));
            //            jigNode.Tag = nozzle;
            //            jigNode.ForeColor = Color.White;
            //        }                
            //    }             
            //}

            //if(MyApp.GetInstance().TrayService!=null)
            //{
            //    root = BaseRoot.Nodes.Add("料盘设置");
            //    root.Tag =null;
            //    root.ForeColor = Color.White;
            //   foreach (string key in MyApp.GetInstance().TrayService.Publishers.Keys)
            //    {
            //        TrayPublisher tt = MyApp.GetInstance().TrayService.Publishers[key];
            //        TreeNode ttNode = root.Nodes.Add(key);
            //        ttNode.Tag = tt.GridCollection;
            //        ttNode.ForeColor = Color.White;                  
            //    }
            //}

            //TreeNode other = BaseRoot.Nodes.Add("料仓设置");
            ////other.Tag = MyApp.Config.FeederSetting;
            ////other.ForeColor = Color.White;


            tvAllSetting.SelectedNode = fun;
            BaseRoot.Expand();   
            tvAllSetting.EndUpdate();
        }

        private void tvAllSetting_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Form form = null;
            IConfigPage config = null;
            if (e.Node!=null )
            {
                if(e.Node.Tag is Machine)
                {
                    form = (e.Node.Tag as Machine).SettingForm;
                }
                else  if (e.Node.Tag is IConfigPage)
                {
                    config = e.Node.Tag as IConfigPage;
                }
                else if (e.Node.Tag is BaseObject)
                {
                    if((e.Node.Tag as BaseObject).BindingObject!=null )
                    {
                        config = ((e.Node.Tag as BaseObject).BindingObject as MachineBase).BaseConfig;
                    }
                }               
            }
            if (config != null)
                form = config.GetConfigForm();
            if (form != null)
            {
                if(form is FormPropertyGrid)
                {
                    (form as FormPropertyGrid).propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
                }
                
                    DockToPanel(form);
            }
                

            else
            {
                splitContainer1.Panel2.Controls.Clear();
                currentForm = null;
            }
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            
        }

        private Form currentForm = null;

        internal void DockToPanel(Form frm)
        {
            if (frm == currentForm)
                return;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Parent = splitContainer1.Panel2;
            frm.Left = 0;
            frm.Top = 0;
            frm.Dock = DockStyle.Fill;
            frm.Show();
            if (currentForm != null)
                currentForm.Hide();
            currentForm = frm;
        }     
    }
}
