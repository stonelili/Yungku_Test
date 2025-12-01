using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yungku.BNU01_V1.Handler.Config;
using Yungku.BNU01_V1.Handler.Logic.Actions;
using Yungku.BNU01_V1.Handler.Logic.Objects;
using Yungku.BNU01_V1.Handler.Pages;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Machine;
using YungkuSystem.Statistical;
using YungkuSystem.Tester;
using YungkuSystem.TestFlow;
using YungkuSystem.ThreadMessage;

namespace Yungku.BNU01_V1.Handler.Logic
{
    internal class ActionMain : LogicObject
    {
        private Product prod = null;
        private ProductObject prodObj
        {
            get
            {
                if (prod != null)
                {
                    return prod.BindingObject as ProductObject;
                }
                return null;
            }
        }
        private IModuleGroup curTestClient = null;


        public ActionMain()
        {
            this.Name = "主控流程";
            MyApp.MainAction = this;
            try
            {
                // 正确的 Machine 层次结构（根据 MyApp.Initialize 中的初始化结果弹窗代码）：
                // machine.TestItems[0].TestItems[0].TestItems[0].TestItems.Count
                // 表示：Machine → TestItems[0] (Turntable) → TestItems[0] (Head?) → TestItems[0] (Jig?) → TestItems[0] (Product)

                if (MyApp.GetInstance()?.Machine?.TestItems != null &&
                MyApp.GetInstance().Machine.TestItems.Count > 0)
                {
                    // 获取第一个 Turntable
                    Turntable turntable = MyApp.GetInstance().Machine.TestItems[0] as Turntable;

                    if (turntable?.Stations != null && turntable.Stations.Count > 0)
                    {
                        // 获取第一个 Station
                        Station station = turntable.Stations[0];

                        // 尝试从 Turntable.TestItems 获取 Head（因为初始化结果弹窗代码使用 Machine.TestItems[0].TestItems[0]...）
                        if (turntable.TestItems != null && turntable.TestItems.Count > 0)
                        {
                            // 获取第一个 Head
                            Head head = turntable.TestItems[0] as Head;

                            if (head?.TestItems != null && head.TestItems.Count > 0)
                            {
                                // 获取第一个 Jig
                                Jig jig = head.TestItems[0] as Jig;

                                if (jig?.TestItems != null && jig.TestItems.Count > 0)
                                {
                                    // 获取第一个 Product
                                    prod = jig.TestItems[0] as Product;

                                    if (prod != null)
                                    {
                                        // 安全获取 prodObj 和 curTestClient
                                        var productObj = prod.BindingObject as ProductObject;
                                        if (productObj != null)
                                        {
                                            curTestClient = productObj.TestClient;
                                            MyApp.GetInstance()?.Logger?.WriteRecord($"[ActionMain构造]:成功初始化 - Product:{prod.Name}");
                                        }
                                        else
                                        {
                                            MyApp.GetInstance()?.Logger?.WriteError("[ActionMain构造]:Product.BindingObject 不是 ProductObject 类型");
                                        }
                                    }
                                    else
                                    {
                                        MyApp.GetInstance()?.Logger?.WriteError("[ActionMain构造]:Jig.TestItems[0] 不是 Product 类型");
                                    }
                                }
                                else
                                {
                                    MyApp.GetInstance()?.Logger?.WriteError("[ActionMain构造]:Jig.TestItems 为空或数量不足");
                                }
                            }
                            else
                            {
                                MyApp.GetInstance()?.Logger?.WriteError("[ActionMain构造]:Head.TestItems 为空或数量不足");
                            }
                        }
                        else
                        {
                            MyApp.GetInstance()?.Logger?.WriteError("[ActionMain构造]:Turntable.TestItems 为空或数量不足");
                        }
                    }
                    else
                    {
                        MyApp.GetInstance()?.Logger?.WriteError("[ActionMain构造]:Turntable.Stations 为空或数量不足");
                    }
                }
                else
                {
                    MyApp.GetInstance()?.Logger?.WriteError("[ActionMain构造]:Machine.TestItems (Turntable) 为空");
                }
            }
            catch (Exception ex)
            {
                MyApp.GetInstance()?.Logger?.WriteError($"[ActionMain构造]:初始化异常 - {ex.Message}\n堆栈：{ex.StackTrace}");
            }
            //MyApp.GetInstance()?.Logger?.WriteRecord("[ActionMain构造]:MainAction 已设置到 MyApp.MainAction");

        }

        private IModuleGroup GetTestClient()
        {
            if (curTestClient == null && prodObj != null)
            {
                curTestClient = prodObj.TestClient;
            }
            return curTestClient;
        }

       
        // 在 ActionMain 类中添加工位启动控制字段
        private bool leftStationReady = false;  // 左工位准备启动标志
        private bool rightStationReady = false; // 右工位准备启动标志
        private bool waitingForStationStart = false; // 等待工位启动标志

        //记录实际启动的工位列表
        private List<Station> runningStations = new List<Station>();

        // 添加公共方法供外部调用
        public void StartLeftStation()
        {
            leftStationReady = true;
            MyApp.GetInstance()?.Logger?.WriteRecord("用户点击左工位启动按钮");
        }

        public void StartRightStation()
        {
            rightStationReady = true;
            MyApp.GetInstance()?.Logger?.WriteRecord("用户点击右工位启动按钮");
        }
        public void ResetStationFlags()
        {
            leftStationReady = false;
            rightStationReady = false;
            waitingForStationStart = false;
            runningStations.Clear(); //
        }
        /// <summary>
        /// 主流程
        /// </summary>
        protected override void Handle()
        {
            if (MyApp.NeedReset || MyApp.ShareData.ishoming)
            {
                OnAlarm(G.Text("设备未复位或者在回原状态中，请检查！"));
                return;
            }
            try
            {
                switch (StateIndex)
                {
                    case ActionBase.ACT_STATE_START:
                        #region
                        Watcher.StopAllWatch();
                        CurrentHeadObject.SetModudeState(true);
                        prod.State = TestState.Testing;
                        RestProdutCode();

                        // 清空上一次测试的数据显示
                        TestDisplayHelper.ClearAllStations();
                        TestDisplayHelper.ResetCounters();

                        // 重置工位启动标志
                        ResetStationFlags();
                        To("等待工位启动");
                        #endregion
                        break;

                    case "等待工位启动":
                        #region
                        if (!waitingForStationStart)
                        {
                            waitingForStationStart = true;
                            WriteRecord("========================================");
                            WriteRecord("系统已就绪，等待启动工位测试");
                            WriteRecord("请在主界面点击【左工位启动】或【右工位启动】按钮");
                            WriteRecord("========================================");
                        }

                        // 检查是否有工位准备启动
                        if (leftStationReady || rightStationReady)
                        {
                            To("启动选定工位测试");
                        }
                        #endregion
                        break;

                    case "启动选定工位测试":
                        #region
                        // 自动切换到自动测试界面
                        MyApp.RunOnUI(new System.Action(() =>
                        {
                            try
                            {
                                FormMain formMain = MyApp.GetInstance().MainForm as FormMain;
                                if (formMain != null)
                                {
                                    // 获取 FormAuto 实例
                                    FormAuto formAuto = FormMain.GetFormAuto();
                                    if (formAuto != null)
                                    {
                                        // 切换到自动测试页面
                                        formMain.DockToMain(formAuto);
                                        WriteRecord("已自动切换到测试界面");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteError($"自动切换界面失败: {ex.Message}");
                            }
                        }));

                        StartSelectedStationTests();
                        To("等待测试完成");
                        #endregion
                        break;

                    case "等待测试完成":
                        #region
                        if (WaitAllStationTestsComplete())
                        {
                            WriteRecord("所有测试完成");
                            UpdateTestResultsToDisplay();
                            To("保存测试数据");
                        }
                        #endregion
                        break;

                    case "保存测试数据":
                        #region
                        WriteRecord("测试完成");

                        //重新启用已完成测试的工位按钮
                        foreach (Station station in runningStations)
                        {
                            int stationIndex = Turntable.Stations.IndexOf(station);
                            string stationName = stationIndex == 0 ? "左工位" : "右工位";

                            WriteRecord($"{stationName} 测试已完成，可以重新启动");

                            // 或者在这里重新设置标志为 ready 状态
                        }

                        // 清空运行工位列表（但不重置按钮标志）
                        runningStations.Clear();

                        waitingForStationStart = false; // 重置等待标志，允许重新输出提示信息

                        WriteRecord("========================================");
                        WriteRecord("本次测试已完成，系统准备下一次测试");
                        WriteRecord("========================================");

                        // 循环回到"等待工位启动"，而不是结束
                        To("等待工位启动");
                        Watcher.StopWatch(StateIndex);
                        #endregion
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(string.Format("{0}-{1}Exception:{2}", this.Name, StateIndex, ex.Message));
            }
        }

        /// <summary>
        /// 启动所有 Station 的测试并显示初始状态
        /// </summary>
        private void StartSelectedStationTests()
        {
            try
            {
                WriteRecord("========================================");
                WriteRecord("开始启动选定的工位测试");
                WriteRecord("========================================");

                // 清空运行工位列表
                runningStations.Clear();

                for (int i = 0; i < Turntable.Stations.Count; i++)
                {
                    Station station = Turntable.Stations[i];
                    string stationName = i == 0 ? "左工位" : "右工位";

                    // 检查该工位是否被用户选择启动
                    bool shouldStart = false;
                    if (i == 0 && leftStationReady)
                    {
                        shouldStart = true;
                        leftStationReady = false; // 重置标志
                        WriteRecord($"检测到左工位启动信号");
                    }
                    else if (i == 1 && rightStationReady)
                    {
                        shouldStart = true;
                        rightStationReady = false; // 重置标志
                        WriteRecord($"检测到右工位启动信号");
                    }

                    if (!shouldStart)
                    {
                        WriteRecord($"跳过 {stationName}(用户未选择启动)");
                        continue;
                    }

                    if (!station.Enabled)
                    {
                        WriteRecord($"{stationName} 未启用,跳过测试");
                        continue;
                    }

                    try
                    {
                        WriteRecord($"【{stationName}】 准备启动测试脚本...");

                        // ⭐ 关键修复1: 获取该工位对应的Head,用于重置产品状态
                        Head head = Turntable.GetHeadByStation(station);
                        if (head != null)
                        {
                            HeadObject headObj = head.BindingObject as HeadObject;

                            // 重置该工位下所有产品的状态
                            foreach (Jig jig in head.TestItems)
                            {
                                foreach (Product product in jig.TestItems)
                                {
                                    ProductObject prodObj = product.BindingObject as ProductObject;
                                    if (prodObj != null && prodObj.OwnerEnabled)
                                    {
                                        // 重置产品状态为Pass
                                        if (product.Result != TestResult.Closed)
                                        {
                                            product.Result = headObj.HasModudeState ? TestResult.Pass : TestResult.Empty;
                                            product.ResultCode = "00";
                                            WriteRecord($"  → 重置产品 [{product.Name}] 状态: {product.Result}");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            WriteRecord($"⚠️ {stationName} 未找到对应的 Head,无法重置产品状态");
                        }

                        // 清空该工位的上一次测试数据显示
                        if (i == 0)
                        {
                            TestDisplayHelper.ClearLeftStation();
                            WriteRecord("已清空左工位上一次测试数据显示");
                        }
                        else if (i == 1)
                        {
                            TestDisplayHelper.ClearRightStation();
                            WriteRecord("已清空右工位上一次测试数据显示");
                        }

                        if (station.Executor != null && station.Executor.Actions.Root != null)
                        {
                            // 确保之前的测试已停止
                            if (station.Executor.IsRunning)
                            {
                                WriteRecord($"{stationName} 正在运行,先停止");
                                station.Executor.Stop();
                                System.Threading.Thread.Sleep(100);
                            }

                            // 启动测试
                            WriteRecord($"启动 {stationName} 测试脚本...");
                            station.Executor.Run(0, station.Name);

                            // 将启动的工位添加到列表
                            runningStations.Add(station);

                            WriteRecord($"✅ {stationName} 测试脚本已启动");
                        }
                        else
                        {
                            WriteRecord($"❌ {stationName} Executor 配置异常,无法启动测试!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteError($"启动{stationName}测试失败: {ex.Message}");
                        Logger.WriteError($"异常堆栈: {ex.StackTrace}");
                    }
                }

                WriteRecord($"已启动 {runningStations.Count} 个工位的测试");
                WriteRecord("========================================");
            }
            catch (Exception ex)
            {
                Logger.WriteError($"启动测试失败: {ex.Message}");
                Logger.WriteError($"异常堆栈: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 等待测试完成
        /// </summary>
        private bool WaitAllStationTestsComplete()
        {
            bool allComplete = true;

            foreach (Station station in runningStations)
            {
                int stationIndex = Turntable.Stations.IndexOf(station);
                string stationName = stationIndex == 0 ? "左工位" : "右工位";

                if (station.Executor.IsRunning)
                {
                    allComplete = false;
                    break;
                }
                else
                {
                    //只检查 Executor 的最终状态
                    if (station.Executor.Actions != null &&
                        station.Executor.Actions.Root != null)
                    {
                        // 获取根动作的状态
                        var rootAction = station.Executor.Actions.Root;

                        // 如果根动作未完成且 Executor 已停止，可能是异常中断
                        if (rootAction.StateIndex != ActionBase.ACT_STATE_END &&
                            !station.Executor.IsRunning)
                        {
                            WriteRecord($"⚠️ {stationName} 可能异常中断");
                            WriteRecord($"  - 根动作: {rootAction.Name}");
                            WriteRecord($"  - 当前状态: {rootAction.StateIndex}");

                            Logger.WriteError($"[{stationName}] 测试流程可能异常中断");
                        }
                    }
                }
            }

            return allComplete;
        }
        /// <summary>
        /// 产品置位良品
        /// </summary>
        public void RestProdutCode()
        {
            foreach (Jig jig in CurrentHead.TestItems)
            {
                foreach (Product product in jig.TestItems)
                {
                    if (product.Result != TestResult.Closed)
                        product.Result = CurrentHeadObject.HasModudeState ? TestResult.Pass : TestResult.Empty;
                    product.ResultCode = "00";
                }
            }
        }

        /// <summary>
        ///只更新用户启动的工位的测试结果
        /// </summary>
        private void UpdateTestResultsToDisplay()
        {
            try
            {
                WriteRecord("========================================");
                WriteRecord("开始更新测试结果到界面");
                WriteRecord($"需要更新的工位数量: {runningStations.Count}");
                WriteRecord("========================================");

                //只处理实际启动的工位
                foreach (Station station in runningStations)
                {
                    // 获取工位索引
                    int stationIndex = Turntable.Stations.IndexOf(station);
                    string stationName = stationIndex == 0 ? "左工位" : "右工位";

                    WriteRecord($"正在更新 {stationName} 的测试结果...");

                    if (!station.Enabled)
                    {
                        WriteRecord($"{stationName} 未启用，跳过");
                        continue;
                    }

                    Head head = Turntable.GetHeadByStation(station);
                    if (head != null)
                    {
                        foreach (Jig jig in head.TestItems)
                        {
                            foreach (Product product in jig.TestItems)
                            {
                                ProductObject prodObj = product.BindingObject as ProductObject;
                                if (prodObj != null && prodObj.OwnerEnabled)
                                {
                                    string testName = product.Name;

                                    if (testName != null && testName.StartsWith("Product", StringComparison.OrdinalIgnoreCase))
                                    {
                                        string result = product.Result == TestResult.Pass ? "PASS" :
                                                       product.Result == TestResult.Fail ? string.Format("FAIL ({0})", product.ResultCode) :
                                                       product.Result.ToString();

                                        //根据实际工位索引更新
                                        if (stationIndex == 0)
                                        {
                                            TestDisplayHelper.UpdateLeftStation(testName, result);
                                            WriteRecord(string.Format("[左工位] {0}: {1}", testName, result));
                                        }
                                        else if (stationIndex == 1)
                                        {
                                            TestDisplayHelper.UpdateRightStation(testName, result);
                                            WriteRecord(string.Format("[右工位] {0}: {1}", testName, result));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        WriteRecord($"{stationName} 未找到对应的 Head");
                    }
                }

                WriteRecord("========================================");
                WriteRecord("测试结果更新完成");
                WriteRecord("========================================");
            }
            catch (Exception ex)
            {
                Logger.WriteError(string.Format("更新测试结果到界面失败: {0}", ex.Message));
            }
        }
    }
}