using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using YungkuSystem.Machine;
using YungkuSystem;
using YungkuSystem.Log;
using YungkuSystem.Statistical;
using YungkuSystem.Controls;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using YungkuSystem.Globalization;
using YungkuSystem.TestFlow;
using Yungku.BNU01_V1.Handler.Config;
using YungkuSystem.AlarmManage;
using YungkuSystem.Tray;
using YungkuSystem.ThreadMessage;
using LiveCharts.Wpf;
using LiveCharts;
using YungkuSystem.YKControls.ykProcess;
using YungkuSystem.Structs;
using YungkuSystem.ExternalControl.StatisticalForm;
using YungkuSystem.Tester;

namespace Yungku.BNU01_V1.Handler.Pages
{
    public partial class FormAuto : Form
    {
        private ListViewLogView listViewLogView = null;
        private Form frm;
        private ProductObject product = MyApp.GetInstance().Machine.TestItems[0].TestItems[0].TestItems[0].TestItems[0].BindingObject as ProductObject;

        public FormAuto()
        {
            InitializeComponent();
            InitializeTestParameterListViews();
            InitializeStatisticsPanel();
        }

        private void FormAuto_Load(object sender, EventArgs e)
        {
            listViewLogView = new ListViewLogView(lvLog);
            LogPublisher lp = (LogPublisher)MyApp.GetInstance().FW.LogService.CreateLogPublisher(Framework.KEY);
            lp.AddLogWriter(listViewLogView);
            lp.ContinuePublish();

            MyApp.GetInstance().AlarmPublisher.AlarmListChanged += AlarmPublisher_AlarmListChanged;
            MyApp.GetInstance().AlarmPublisher.AddAlarmWriter(new ListViewAlarmView(lvAlarms));
            //根据产品数量更新界面
            int productCount = MyApp.GetInstance().Machine.ProductCount;
            yieldGroup1.Count = productCount;
            labelGroup2.Count = (MyApp.GetInstance().Machine.TestItems[0] as Turntable).Stations.Count;
            Control.CheckForIllegalCrossThreadCalls = false;
            //statisticalState1.Statis = MyApp.GetInstance().Statistical;

            //InitClient();//软件开启建立连接

            // 添加统计面板到界面
            AddStatisticsPanelToForm();

            // ❌ 注释掉演示数据，使用真实测试数据
            // AddTestDataForDemo();

            // 订阅窗口大小改变事件以调整ListView列宽
            splitTestContent.SizeChanged += SplitTestContent_SizeChanged;

            // 初始化列宽（窗口句柄已创建）
            this.BeginInvoke(new Action(() => AdjustListViewColumnWidths()));
        }

        /// <summary>
        /// 当splitTestContent大小改变时调整ListView列宽
        /// </summary>
        private void SplitTestContent_SizeChanged(object sender, EventArgs e)
        {
            AdjustListViewColumnWidths();
        }

        /// <summary>
        /// 调整ListView列宽以适应容器大小
        /// </summary>
        private void AdjustListViewColumnWidths()
        {
            try
            {
                // 获取左工位ListView的可用宽度（减去边框和滚动条）
                int leftWidth = lvLeftStation.ClientSize.Width - 5;
                int rightWidth = lvRightStation.ClientSize.Width - 5;

                // 按比例分配6列的宽度：Order(10%) Name(30%) LowLimit(12%) UpperLimit(12%) Value(18%) Test(18%)
                int leftOrder = (int)(leftWidth * 0.10);
                int leftName = (int)(leftWidth * 0.30);
                int leftLow = (int)(leftWidth * 0.12);
                int leftUpper = (int)(leftWidth * 0.12);
                int leftValue = (int)(leftWidth * 0.18);
                int leftTest = leftWidth - leftOrder - leftName - leftLow - leftUpper - leftValue;

                int rightOrder = (int)(rightWidth * 0.10);
                int rightName = (int)(rightWidth * 0.30);
                int rightLow = (int)(rightWidth * 0.12);
                int rightUpper = (int)(rightWidth * 0.12);
                int rightValue = (int)(rightWidth * 0.18);
                int rightTest = rightWidth - rightOrder - rightName - rightLow - rightUpper - rightValue;

                // 确保最小宽度
                leftOrder = Math.Max(leftOrder, 40);
                leftName = Math.Max(leftName, 100);
                leftLow = Math.Max(leftLow, 60);
                leftUpper = Math.Max(leftUpper, 60);
                leftValue = Math.Max(leftValue, 70);
                leftTest = Math.Max(leftTest, 70);

                rightOrder = Math.Max(rightOrder, 40);
                rightName = Math.Max(rightName, 100);
                rightLow = Math.Max(rightLow, 60);
                rightUpper = Math.Max(rightUpper, 60);
                rightValue = Math.Max(rightValue, 70);
                rightTest = Math.Max(rightTest, 70);

                // 调整左工位列宽
                if (lvLeftStation.Columns.Count >= 6)
                {
                    lvLeftStation.Columns[0].Width = leftOrder;
                    lvLeftStation.Columns[1].Width = leftName;
                    lvLeftStation.Columns[2].Width = leftLow;
                    lvLeftStation.Columns[3].Width = leftUpper;
                    lvLeftStation.Columns[4].Width = leftValue;
                    lvLeftStation.Columns[5].Width = leftTest;
                }

                // 调整右工位列宽
                if (lvRightStation.Columns.Count >= 6)
                {
                    lvRightStation.Columns[0].Width = rightOrder;
                    lvRightStation.Columns[1].Width = rightName;
                    lvRightStation.Columns[2].Width = rightLow;
                    lvRightStation.Columns[3].Width = rightUpper;
                    lvRightStation.Columns[4].Width = rightValue;
                    lvRightStation.Columns[5].Width = rightTest;
                }
            }
            catch (Exception ex)
            {
                MyApp.GetInstance().Logger.WriteError($"调整ListView列宽时发生异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 添加测试数据用于演示（仅用于测试）
        /// </summary>
        private void AddTestDataForDemo()
        {
            // 添加左工位测试数据示例
            UpdateLeftStationTest(1, "电压测试", "PASS");
            UpdateLeftStationTest(2, "电流测试", "PASS");
            UpdateLeftStationTest(3, "电阻测试", "FAIL");
            UpdateLeftStationTest(4, "温度测试", "PASS");

            // 添加右工位测试数据示例
            UpdateRightStationTest(1, "功率测试", "PASS");
            UpdateRightStationTest(2, "频率测试", "PASS");
            UpdateRightStationTest(3, "信号测试", "PASS");
            UpdateRightStationTest(4, "噪声测试", "FAIL");
        }

        private void AlarmPublisher_AlarmListChanged(object sender, Alarm e)
        {
            MyApp.RunOnUI(new System.Action(() =>
            {
                //转到报警列表
                if (MyApp.GetInstance().AlarmPublisher.IsAlarm)
                {
                    ykTabLog.SelectedIndex = 1;//Alarms view
                }
                else
                {
                    ykTabLog.SelectedIndex = 0;//Log view
                }
            }));
        }

        private void tmrUpdateUI_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                lblProductName.Text = G.Text(MyApp.Config.General.ProductName);
                ShowYield();

                int dogLiveDays = MyApp.SuperDog.GetDaysLeft();
                if (dogLiveDays == 365)
                {
                    lblSuperDogLiveDays.Visible = false;
                }
                else
                {
                    lblSuperDogLiveDays.Visible = true;
                    lblSuperDogLiveDays.Text = G.Text("加密狗剩余：" + dogLiveDays + "天");
                }
                UpdataTimeText();
            }
        }

        private void UpdataTimeText()
        {
            double tepCount;
            double avgCount;
            foreach (Turntable tt in MyApp.GetInstance().Machine.TestItems)//turntables
            {
                for (int i = 0; i < tt.Stations.Count; i++)
                {
                    tepCount = tt.Stations[i].Executor.Actions.Root.ExecutedAllTimes;
                    avgCount = tt.Stations[i].Executor.Actions.Root.ExecutedAgvTimes;
                    if (tepCount != -1 && avgCount != -1)
                    {
                        string stateStr = (tepCount / 1000).ToString("0") + "[Avg:" + (avgCount / 1000).ToString("0") + "]" + "ms" + "\r\n";
                        stateStr += tt.Stations[i].Name + "\\" + tt.Stations[i].Executor.CurrentStateIndex;
                        labelGroup2.LabelControls[i].Text = stateStr;
                    }
                }
            }

        }

        private void ShowYield()
        {
            IStatistical statis = MyApp.GetInstance().Statistical;
            //获取机台上所有治具的名称
            List<string> names = GetProductNames();
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                int passCount = statis.GetPassByJig(name);
                int failCount = statis.GetFailByItemName(name);
                int total = passCount + failCount;
                double yield = total == 0 ? 1 : (passCount * 1.0 / total);
                if (i < yieldGroup1.YieldControls.Count)
                {
                    YKProgressBar control = yieldGroup1.YieldControls[i];
                    control.Caption = name;
                    control.Value = yield;
                }

            }
        }

        private List<string> GetProductNames()
        {
            return ProductObject.GetNames(MyApp.GetInstance().Machine);
        }

        private void FormAuto_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                MyApp.GetInstance().Logger.WriteRecord(G.Text("切换到自动画面"));

            }
        }

        private FormDetail fd = new FormDetail();
        private void yieldGroup1_DoubleClick(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().Statistical != null && MyApp.GetInstance().Statistical is StatisticalDefaultImpl)
                fd.Statistical = MyApp.GetInstance().Statistical as StatisticalDefaultImpl;
            if (!fd.Visible)
                fd.Show();
        }

        private void 显示统计信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().Statistical != null && MyApp.GetInstance().Statistical is StatisticalDefaultImpl)
                fd.Statistical = MyApp.GetInstance().Statistical as StatisticalDefaultImpl;
            if (!fd.Visible)
                fd.Show();
        }

        private void 显示不良明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyApp.GetInstance().Statistical != null && MyApp.GetInstance().Statistical is StatisticalDefaultImpl)
                fd.Statistical = MyApp.GetInstance().Statistical as StatisticalDefaultImpl;
            if (!fd.Visible)
                fd.Show();
        }

        private void nG代码管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyApp.GetInstance().Statistical.GetCodeNameEditor().Show();
        }

        private void tsmiClearLogs_Click(object sender, EventArgs e)
        {
            lvLog.Items.Clear();
        }

        private void tsmiSelectLogs_Click(object sender, EventArgs e)
        {
            FormLogViewQuery frm = new FormLogViewQuery();
            frm.Show();
        }

        private void 当前另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewLogView.CurrentSaveAs();
        }

        private void FormAuto_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
            SolidBrush br = new SolidBrush(this.BackColor);
            e.Graphics.FillRectangle(br, rect);
            br.Dispose();
            e.Graphics.DrawRectangle(Pens.Blue, rect);
        }

        private void FormAuto_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void FormAuto_Resize(object sender, EventArgs e)
        {
            splitfunction.SplitterDistance = 357;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(G.Text("确定清除统计数据吗？"), G.Text("清楚统计数据"),
             MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (MyApp.GetInstance().Statistical != null)
                {
                    MyApp.GetInstance().Statistical.Clear();
                }
            }
        }

        //双击连接线体
        //private void ledTestConnect_DoubleClick(object sender, EventArgs e)
        //{
        //    MyApp.RunOnUIThread(new Action(() =>
        //    {
        //        try
        //        {
        //            InitClient();
        //            Tip.ShowWarning(G.Text("正在重连"));
        //        }
        //        catch (Exception ex)
        //        {
        //            Tip.ShowError(G.Text($"重连失败: {ex.Message}"));
        //            MyApp.GetInstance().Logger.WriteError($"重连客户端失败: {ex}");
        //        }
        //    }));
        //}

        private void tmrConnect_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.tmrConnect.Enabled = false;

                //// 添加空引用检查
                //if (MyApp.GetInstance().TestSoftwareClient != null)
                //{
                //    if (MyApp.GetInstance().TestSoftwareClient.RealConnectionState == YungkuSystem.Tester.ConnectionState.Connected)
                //    {
                //        ledTestConnect.State = true;
                //        lblledTestConnect_C.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));

                //        lblledTestConnect_E.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
                //    }
                //    else
                //    {
                //        ledTestConnect.State = false;
                //        lblledTestConnect_C.BackColor = Color.Red;
                //        lblledTestConnect_E.BackColor = Color.Red;
                //    }
                //}
                //else
                //{
                //    // TestSoftwareClient 未初始化
                //    ledTestConnect.State = false;
                //    lblledTestConnect_C.BackColor = Color.Red;
                //    lblledTestConnect_E.BackColor = Color.Red;
                //}
                ledTestConnect.State = false;
                // 更新MES连接状态
                UpdateMESConnectionStatus();

                this.tmrConnect.Enabled = true;
            }
        }

        //public void InitClient()
        //{
        //    MyApp.GetInstance().TestSoftwareClient = product.TestClient;

        //    if (MyApp.GetInstance().TestSoftwareClient.RealConnectionState != YungkuSystem.Tester.ConnectionState.Connected)
        //    {
        //        MyApp.GetInstance().TestSoftwareClient.Uinit();
        //        Thread.Sleep(100);
        //        MyApp.GetInstance().TestSoftwareClient.Init();
        //    }
        //    MyApp.GetInstance().TestSoftwareClient.OnReceiveCompleted += Client_OnReceiveCompleted;//完成接收 
        //}

        private void Client_OnReceiveCompleted(object sender, SocketDatagramReceivedEventArgs<Dictionary<string, string>> e)
        {
            Dictionary<string, string> data = e.Datagram;
            MyApp.GetInstance().CurrentCMD = data["Product0"].ToString(); ;
        }
        /// <summary>
        /// 切换到测试参数显示页面（如果有TabControl）
        /// </summary>
        public void SwitchToTestParameterView()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SwitchToTestParameterView()));
                return;
            }

            try
            {
                if (ykTabLog != null)
                {
                    ykTabLog.SelectedIndex = 2; // 切换到测试参数标签页
                    MyApp.GetInstance().Logger.WriteRecord("[FormAuto]:已切换到测试参数显示页面");
                }
                else
                {
                    MyApp.GetInstance().Logger.WriteError("[FormAuto]:ykTabLog 控件未初始化");
                }
            }
            catch (Exception ex)
            {
                MyApp.GetInstance().Logger.WriteError($"切换到测试参数页面失败: {ex.Message}");
            }
        }
        //private void tmrAutoConnect_Tick(object sender, EventArgs e)
        //{
        //    AutoConnect();
        //}

        //private void AutoConnect()
        //{
        //    // 添加空引用检查
        //    if (MyApp.GetInstance().TestSoftwareClient != null &&
        //    MyApp.GetInstance().TestSoftwareClient.RealConnectionState != YungkuSystem.Tester.ConnectionState.Connected)
        //    {
        //        MyApp.GetInstance().TestSoftwareClient.Uinit();
        //        Thread.Sleep(100);
        //        MyApp.GetInstance().TestSoftwareClient.Init();
        //        MyApp.GetInstance().TestSoftwareClient.OnReceiveCompleted -= Client_OnReceiveCompleted;
        //        MyApp.GetInstance().TestSoftwareClient.OnReceiveCompleted += Client_OnReceiveCompleted;//完成接收      
        //    }
        //}

        /// <summary>
        /// 统一的工位测试数据更新方法（支持左右工位独立统计）- 6列格式
        /// </summary>
        /// <param name="stationIndex">工位索引（0-左工位，1-右工位）</param>
        /// <param name="index">测试序号</param>
        /// <param name="testName">测试名称</param>
        /// <param name="lowLimit">下限值</param>
        /// <param name="upperLimit">上限值</param>
        /// <param name="testValue">测试值</param>
        /// <param name="testResult">测试结果</param>
        /// <param name="shouldUpdateStats">是否更新统计数据</param>
        public void UpdateStationTest(int stationIndex, int index, string testName, string lowLimit, string upperLimit, string testValue, string testResult, bool shouldUpdateStats)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateStationTest(stationIndex, index, testName, lowLimit, upperLimit, testValue, testResult, shouldUpdateStats)));
                return;
            }

            try
            {
                // 根据 stationIndex 选择正确的 ListView 和统计变量
                YSListView targetListView = (stationIndex == 0) ? lvLeftStation : lvRightStation;
                string stationName = (stationIndex == 0) ? "左工位" : "右工位";

                // 创建6列数据
                ListViewItem item = new ListViewItem(index.ToString()); // Order
                item.SubItems.Add(testName);                            // Name
                item.SubItems.Add(lowLimit);                            // LowLimit
                item.SubItems.Add(upperLimit);                          // UpperLimit
                item.SubItems.Add(testValue);                           // Value
                item.SubItems.Add(testResult);                          // Test

                // 根据测试结果设置颜色 - 按照图片样式
                if (testResult.Contains("PASS") || testResult.Contains("通过") || testResult.Contains("OK"))
                {
                    item.BackColor = Color.FromArgb(0, 200, 0);  // 绿色背景
                    item.ForeColor = Color.Black;                 // 白色字体
                }
                else if (testResult.Contains("FAIL") || testResult.Contains("失败") || testResult.Contains("NG"))
                {
                    item.BackColor = Color.FromArgb(255, 0, 0);  // 红色背景
                    item.ForeColor = Color.Black;                 // 白色字体
                }
                else
                {
                    item.BackColor = Color.White;
                    item.ForeColor = Color.Black;
                }

                targetListView.Items.Add(item);

                // 自动滚动到最新项
                if (targetListView.Items.Count > 0)
                {
                    targetListView.Items[targetListView.Items.Count - 1].EnsureVisible();
                }

                // 只在需要时更新统计数据，且区分左右工位
                if (shouldUpdateStats)
                {
                    if (stationIndex == 0) // 左工位
                    {
                        leftStationTotal++;
                        lblLeftTotalValue.Text = leftStationTotal.ToString();

                        if (testResult.Contains("PASS"))
                        {
                            leftStationPass++;
                            lblLeftPassValue.Text = leftStationPass.ToString();
                        }
                        else if (testResult.Contains("FAIL"))
                        {
                            leftStationFail++;
                            lblLeftFailValue.Text = leftStationFail.ToString();
                        }

                        // 更新左工位良率
                        double leftYield = leftStationTotal > 0 ? (leftStationPass * 100.0 / leftStationTotal) : 0;
                        lblLeftYieldValue.Text = leftYield.ToString("F2") + "%";
                    }
                    else if (stationIndex == 1) // 右工位
                    {
                        rightStationTotal++;
                        lblRightTotalValue.Text = rightStationTotal.ToString();

                        if (testResult.Contains("PASS"))
                        {
                            rightStationPass++;
                            lblRightPassValue.Text = rightStationPass.ToString();
                        }
                        else if (testResult.Contains("FAIL"))
                        {
                            rightStationFail++;
                            lblRightFailValue.Text = rightStationFail.ToString();
                        }

                        // 更新右工位良率
                        double rightYield = rightStationTotal > 0 ? (rightStationPass * 100.0 / rightStationTotal) : 0;
                        lblRightYieldValue.Text = rightYield.ToString("F2") + "%";
                    }

                    MyApp.GetInstance().Logger.WriteRecord($"[{stationName}统计] 总数:{(stationIndex == 0 ? leftStationTotal : rightStationTotal)}, 良品:{(stationIndex == 0 ? leftStationPass : rightStationPass)}, 不良:{(stationIndex == 0 ? leftStationFail : rightStationFail)}");
                }

                // 限制最大显示条数（可选，避免数据过多）
                if (targetListView.Items.Count > 100)
                {
                    targetListView.Items.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                MyApp.GetInstance().Logger.WriteError($"更新{(stationIndex == 0 ? "左" : "右")}工位测试数据失败: {ex.Message}");
            }
        }

        /// <summary>
        ///单独更新某行的 Value 列（不影响其他列，不更新统计）
        /// </summary>
        /// <param name="stationIndex">工位索引（0-左工位，1-右工位）</param>
        /// <param name="testName">测试名称（用于查找对应行）</param>
        /// <param name="testValue">新的测试值</param>
        public void UpdateStationTestValue(int stationIndex, string testName, string testValue)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateStationTestValue(stationIndex, testName, testValue)));
                return;
            }

            try
            {
                // 选择对应工位的 ListView
                YSListView targetListView = (stationIndex == 0) ? lvLeftStation : lvRightStation;
                string stationName = (stationIndex == 0) ? "左工位" : "右工位";

                // 查找具有该测试名称的行
                ListViewItem targetItem = null;
                foreach (ListViewItem item in targetListView.Items)
                {
                    // Name 列在 SubItems[1]
                    if (item.SubItems.Count > 1 && item.SubItems[1].Text == testName)
                    {
                        targetItem = item;
                        break;
                    }
                }

                if (targetItem != null && targetItem.SubItems.Count >= 5)
                {
                    // Value 列在 SubItems[4]
                    targetItem.SubItems[4].Text = testValue;
                    MyApp.GetInstance().Logger.WriteRecord($"[{stationName}] 已更新 {testName} 的 Value 为: {testValue}");
                }
                else
                {
                    MyApp.GetInstance().Logger.WriteError($"[{stationName}] 未找到测试项: {testName}，无法更新 Value");
                }
            }
            catch (Exception ex)
            {
                MyApp.GetInstance().Logger.WriteError($"更新{(stationIndex == 0 ? "左" : "右")}工位 Value 列失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 初始化测试参数ListView的列 - 按照图片样式
        /// </summary>
        private void InitializeTestParameterListViews()
        {
            // 初始化左工位ListView - 6列格式
            lvLeftStation.Columns.Clear();
            lvLeftStation.Columns.Add("Order", 50, HorizontalAlignment.Center);        // 序号
            lvLeftStation.Columns.Add("Name", 150, HorizontalAlignment.Left);          // 测试名称
            lvLeftStation.Columns.Add("LowLimit", 80, HorizontalAlignment.Center);     // 下限
            lvLeftStation.Columns.Add("UpperLimit", 80, HorizontalAlignment.Center);   // 上限
            lvLeftStation.Columns.Add("Value", 80, HorizontalAlignment.Center);        // 测试值
            lvLeftStation.Columns.Add("Test", 80, HorizontalAlignment.Center);         // 测试结果

            // 初始化右工位ListView - 6列格式
            lvRightStation.Columns.Clear();
            lvRightStation.Columns.Add("Order", 50, HorizontalAlignment.Center);       // 序号
            lvRightStation.Columns.Add("Name", 150, HorizontalAlignment.Left);         // 测试名称
            lvRightStation.Columns.Add("LowLimit", 80, HorizontalAlignment.Center);    // 下限
            lvRightStation.Columns.Add("UpperLimit", 80, HorizontalAlignment.Center);  // 上限
            lvRightStation.Columns.Add("Value", 80, HorizontalAlignment.Center);       // 测试值
            lvRightStation.Columns.Add("Test", 80, HorizontalAlignment.Center);        // 测试结果

            Font modernFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);


            // 设置左工位ListView样式
            lvLeftStation.FullRowSelect = true;
            lvLeftStation.GridLines = true;
            lvLeftStation.View = View.Details;
            lvLeftStation.Font = modernFont;  
            lvLeftStation.BackColor = Color.WhiteSmoke;

            // 设置右工位ListView样式（与左工位完全一致）
            lvRightStation.FullRowSelect = true;
            lvRightStation.GridLines = true;
            lvRightStation.View = View.Details;
            lvRightStation.Font = modernFont;  
            lvRightStation.BackColor = Color.WhiteSmoke;

        }

        /// <summary>
        /// 更新左工位测试数据
        /// </summary>
        /// <param name="index">测试序号</param>
        /// <param name="testName">测试名称</param>
        /// <param name="testResult">测试结果</param>
        public void UpdateLeftStationTest(int index, string testName, string testResult)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateLeftStationTest(index, testName, testResult)));
                return;
            }

            ListViewItem item = new ListViewItem(index.ToString());
            item.SubItems.Add(testName);
            item.SubItems.Add(testResult);

            // 根据测试结果设置颜色
            if (testResult.Contains("PASS") || testResult.Contains("通过") || testResult.Contains("OK"))
            {
                item.BackColor = Color.LightGreen;
                item.ForeColor = Color.Black;
            }
            else if (testResult.Contains("FAIL") || testResult.Contains("失败") || testResult.Contains("NG"))
            {
                item.BackColor = Color.LightPink;
                item.ForeColor = Color.Black;
            }
            else
            {
                item.BackColor = Color.White;
                item.ForeColor = Color.Black;
            }

            lvLeftStation.Items.Add(item);

            // 自动滚动到最新项
            if (lvLeftStation.Items.Count > 0)
            {
                lvLeftStation.Items[lvLeftStation.Items.Count - 1].EnsureVisible();
            }

            // 限制最大显示条数（可选，避免数据过多）
            if (lvLeftStation.Items.Count > 100)
            {
                lvLeftStation.Items.RemoveAt(0);
            }
        }

        /// <summary>
        /// 更新右工位测试数据
        /// </summary>
        /// <param name="index">测试序号</param>
        /// <param name="testName">测试名称</param>
        /// <param name="testResult">测试结果</param>
        public void UpdateRightStationTest(int index, string testName, string testResult)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateRightStationTest(index, testName, testResult)));
                return;
            }

            ListViewItem item = new ListViewItem(index.ToString());
            item.SubItems.Add(testName);
            item.SubItems.Add(testResult);

            // 根据测试结果设置颜色
            if (testResult.Contains("PASS") || testResult.Contains("通过") || testResult.Contains("OK"))
            {
                item.BackColor = Color.LightGreen;
                item.ForeColor = Color.Black;
            }
            else if (testResult.Contains("FAIL") || testResult.Contains("失败") || testResult.Contains("NG"))
            {
                item.BackColor = Color.LightPink;
                item.ForeColor = Color.Black;
            }
            else
            {
                item.BackColor = Color.White;
                item.ForeColor = Color.Black;
            }

            lvRightStation.Items.Add(item);

            // 自动滚动到最新项
            if (lvRightStation.Items.Count > 0)
            {
                lvRightStation.Items[lvRightStation.Items.Count - 1].EnsureVisible();
            }

            // 限制最大显示条数（可选，避免数据过多）
            if (lvRightStation.Items.Count > 100)
            {
                lvRightStation.Items.RemoveAt(0);
            }
        }

        /// <summary>
        /// 清空左工位测试数据
        /// </summary>
        public void ClearLeftStationTests()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ClearLeftStationTests()));
                return;
            }
            lvLeftStation.Items.Clear();
        }

        /// <summary>
        /// 清空右工位测试数据
        /// </summary>
        public void ClearRightStationTests()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ClearRightStationTests()));
                return;
            }
            lvRightStation.Items.Clear();
        }

        /// <summary>
        /// 更新MES连接状态
        /// </summary>
        private void UpdateMESConnectionStatus()
        {
            // TODO: 当MES协议更新后，这里需要实现实际的MES连接状态检查
            // 目前MES协议还没有更新，暂时显示未连接状态

            // 示例代码（需要根据实际MES客户端实现）:
            // if (MyApp.GetInstance().MESClient != null)
            // {
            //     if (MyApp.GetInstance().MESClient.IsConnected)
            //     {
            //  ledMESConnect.State = true;
            //lblledMESConnect_C.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //      lblledMESConnect_E.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //     }
            //     else
            //     {
            //      ledMESConnect.State = false;
            //         lblledMESConnect_C.BackColor = Color.Red;
            //    lblledMESConnect_E.BackColor = Color.Red;
            //     }
            // }
            // else
            // {
            //     ledMESConnect.State = false;
            //  lblledMESConnect_C.BackColor = Color.Red;
            //     lblledMESConnect_E.BackColor = Color.Red;
            // }

            // 临时显示：MES未连接（灰色状态，表示功能未启用）
            ledMESConnect.State = false;
            lblledMESConnect_C.BackColor = System.Drawing.Color.Gray;
            lblledMESConnect_E.BackColor = System.Drawing.Color.Gray;
        }

        /// <summary>
        /// MES连接LED双击事件 - 尝试重连MES
        /// </summary>
        private void ledMESConnect_DoubleClick(object sender, EventArgs e)
        {
            MyApp.RunOnUIThread(new Action(() =>
        {
            try
            {
                // TODO: 当MES协议更新后，实现MES重连逻辑
                Tip.ShowWarning(G.Text("MES协议尚未实现，功能开发中..."));
                MyApp.GetInstance().Logger.WriteInfo("MES连接功能尚未实现");

                // 示例代码（需要根据实际MES客户端实现）:
                // InitMESClient();
                // Tip.ShowWarning(G.Text("正在重连MES"));
            }
            catch (Exception ex)
            {
                Tip.ShowError(G.Text($"重连MES失败: {ex.Message}"));
                MyApp.GetInstance().Logger.WriteError($"重连MES客户端失败: {ex}");
            }
        }));
        }
    }
}
