using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using YungkuSystem.Machine;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class IOListen : Form
    {
        public IOListen()
        {
            InitializeComponent();
        }
        private List<int> Cards = new List<int>();
        private List<MyLabel> labels = new List<MyLabel>();
        private void IOListen_Load(object sender, EventArgs e)
        {
            int num2 = 0;
            int num3 = 1;

            int num4 = 0;
            foreach (GPIOMap gpiomap in (MyApp.GetInstance().MotionSystem as MotionSystemManager).AllGPIOMaps)
            {
                if (gpiomap.Params.IOType == IOType.Input)
                {
                    MyLabel label1 = new MyLabel();
                    label1.ForeColor = Color.Black;
                    label1.Location = new Point(20, 20 + 30 * num2);
                    label1.Name = "lbl" + num3;
                    label1.Size = new Size(70, 15);
                    label1.Text = gpiomap.Params.MapName;
                    groupBox1.Controls.Add(label1);

                    MyLabel label2 = new MyLabel();
                    label2.BackColor = Color.LightGreen;
                    label2.Location = new Point(20 + 100, 20 + 30 * num2);
                    label2.Name = "lbl2" + num3;
                    label2.Size = new Size(80, 20);
                    label2.Tag = gpiomap;
                    labels.Add(label2);
                    groupBox1.Controls.Add(label2);
                    num2++;
                }
                else
                {
                    MyLabel label1 = new MyLabel();
                    label1.ForeColor = Color.Black;
                    label1.Location = new Point(20, 20 + 30 * num4);
                    label1.Name = "lbl3" + num3;
                    label1.Size = new Size(70, 15);
                    label1.Text = gpiomap.Params.MapName;
                    groupBox2.Controls.Add(label1);

                    MyLabel label2 = new MyLabel();
                    label2.BackColor = Color.LightGreen;
                    label2.Location = new Point(20 + 100, 20 + 30 * num4);
                    label2.Name = "lbl4" + num3;
                    label2.Size = new Size(80, 20);
                    label2.Tag = gpiomap;
                    labels.Add(label2);
                    groupBox2.Controls.Add(label2);
                    num4++;
                }

                if (!Cards.Contains(gpiomap.Params.CardIndex))
                {
                    Cards.Add(gpiomap.Params.CardIndex);
                }

                num3++;
            }
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool isInitialized = (MyApp.GetInstance().MotionSystem as MotionSystemManager).IsInitialized;
            if (isInitialized)
            {
                this.timer1.Interval = 200;
                this.timer1.Enabled = false;
                Dictionary<int, BitArray> dictionary = new Dictionary<int, BitArray>();
                Dictionary<int, BitArray> dictionary2 = new Dictionary<int, BitArray>();
                foreach (int key in this.Cards)
                {
                    dictionary[key] = (MyApp.GetInstance().MotionSystem as MotionSystemManager).CardMaps[key].InputIOValues;
                    dictionary2[key] = (MyApp.GetInstance().MotionSystem as MotionSystemManager).CardMaps[key].OutputIOValues;
                }
                foreach (var item in labels)
                {
                    var gpiomap = item.Tag as GPIOMap;
                    bool flag3 = gpiomap.Params.IOType == IOType.Input;
                    if (flag3)
                    {
                        item.State = dictionary[gpiomap.Params.CardIndex][gpiomap.Params.IOIndex];
                    }
                    else
                    {
                        item.State = dictionary2[gpiomap.Params.CardIndex][gpiomap.Params.IOIndex];
                    }


                }

              





                //if (MyApp.GetInstance().AlarmPublisher.IsAlarm)
                //{
                //    MyApp.TricolorLight.State = MachineStatus.Alarm;
                //    //ledGreen.State = false;
                //    //ledYellow.State = false;
                //    //ledRed.State = true;
                //}
                





                this.timer1.Enabled = true;
            }
        }
    }
}
