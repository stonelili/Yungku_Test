using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YungkuSystem.UAC;
using YungkuSystem.YKControls.Forms;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormLogin : Form
    {
        /// <summary>
        /// 用户名添加选项
        /// </summary>
        /// <param name="box"></param>
        public void ComboboxU()
        {
            List<string> userNameL = MyApp.GetInstance().UAC.GetUserName();      
            List<KeyValuePair<string, string>> lstCom = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < userNameL.Count; i++)
            {
                lstCom.Add(new KeyValuePair<string, string>(i.ToString(), userNameL[i]));
            }
            cbbUserName.Source = lstCom;
            //cbbUserName.SelectedIndex = 0;
        }

        public FormLogin()
        {
            InitializeComponent();
      
        }



        private Form forwardForm = null;
        /// <summary>
        /// 设置窗体
        /// </summary>
        public Form ForwardForm
        {
            get { return forwardForm; }
            set { forwardForm = value; }
        }

        private string permission = string.Empty;
        /// <summary>
        /// 所需权限
        /// </summary>
        public string Permission
        {
            get { return permission; }
            set { permission = value; }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            string userName = cbbUserName.TextValue;//"Administrator";
            string password =   txtPassword.InputText;//"yungku2020";
            User user = MyApp.GetInstance().UAC.Login(userName, password);
            if (user != null)
            {
                if (MyApp.GetInstance().UAC.CheckPermission(Permission))
                {
                    ((FormMain)MyApp.GetInstance().MainForm).DockToMain(forwardForm);               
                    cbbUserName.TextValue = string.Empty;
                    txtPassword.InputText = string.Empty;
                    MyApp.GetInstance().Logger.WriteRecord("用户: " + user.Name + "登录。");
                }
                else
                {
                    ShowTips(btnEnter, "当前登录用户权限不足！",Color.Yellow,Color.Black);
                }                
            }
            else
            {               
                ShowTips(btnEnter, "用户名或密码错误!",Color.Red,Color.White);
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnEnter_Click(null, null);
            }
        }

        private void FormLogin_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                ComboboxU();
                txtPassword.Focus();
                cbbUserName.SelectedIndex = 0;
            }
        }

        private void FormLogin_SizeChanged(object sender, EventArgs e)
        {
          
        }

        private void panelControl2_VisibleChanged(object sender, EventArgs e)
        {
            panelControl2.Left = this.Width / 2 - panelControl2.Width / 2;
            panelControl2.Top = this.Height / 2 - panelControl2.Height / 2;
        }

        private void FormLogin_Resize(object sender, EventArgs e)
        {
            panelControl2.Left = this.Width / 2 - panelControl2.Width / 2;
            panelControl2.Top = this.Height / 2 - panelControl2.Height / 2;
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
            }
            txtPassword.InputText = string.Empty;
        }

        private void ykBtnExt2_BtnClick(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void ShowTips(Control ucBtnExt1,string tipStr,Color backColor,Color foreColor)
        {
            YungkuSystem.YKControls.Forms.FrmAnchorTips.ShowTips(ucBtnExt1, tipStr, AnchorTipsLocation.BOTTOM, backColor, foreColor,null,10,1000);          
        }
    }
}
