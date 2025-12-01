using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yungku.BNU01_V1.Handler
{
    public class MyLabel : Label
    {
        private object tag;

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private bool state;

        public bool State
        {
            get { return state; }
            set
            {
                state = value;
                if (value)
                {
                    this.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    this.BackColor = System.Drawing.Color.LightGreen;
                }
                
            }
        }



    }
}
