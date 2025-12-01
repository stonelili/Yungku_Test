using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormAdjustFocus : Form
    {
        public FormAdjustFocus()
        {
            InitializeComponent();
        }
        public static int HeadIndex { get; set; } = 0;
        public static DialogResult SelectAdjustHeadIndex()
        {
            FormAdjustFocus frmJPA = new FormAdjustFocus();
            return frmJPA.ShowDialog();
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {
            HeadIndex =(int) numHeadIndex.Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
