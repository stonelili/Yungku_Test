using System;
using Yungku.BNU01_V1.Handler.Logic;
using YungkuSystem.Controls;
using YungkuSystem.Script.Core;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    /// <summary>
    /// 示例：在 Station 的测试 Action 中更新测试数据显示
    /// </summary>
    public class ExampleStationAction : ActionObject
    {
        public override string ObjectClass
        {
            get { return "测试示例"; }
        }

        public override bool IsContainner
        {
            get { return false; }
        }

        public override bool IsRunState
        {
            get { return true; }
        }

        protected override void Execute()
        {
            try
            {
                switch (StateIndex)
                {
                    case ACT_STATE_START:
                        #region 测试开始

                        // ? 步骤1：清空上一次的测试数据
                        TestDisplayHelper.ClearAllStations();
                        TestDisplayHelper.ResetCounters();

                        // ? 步骤2：显示测试开始状态
                        TestDisplayHelper.UpdateLeftStation("连接测试", "测试中...");
                        TestDisplayHelper.UpdateRightStation("连接测试", "测试中...");

                        To("执行测试");

                        #endregion
                        break;

                    case "执行测试":
                        #region 执行测试

                        // 这里执行实际的测试逻辑
                        // 例如：发送测试命令、读取传感器数据等

                        // 模拟测试过程
                        WriteInfo("开始执行测试...");

                        // ? 步骤3：更新测试进度
                        TestDisplayHelper.UpdateLeftStation("电压测试", "PASS");
                        TestDisplayHelper.UpdateLeftStation("电流测试", "PASS");
                        TestDisplayHelper.UpdateLeftStation("电阻测试", "测试中...");

                        TestDisplayHelper.UpdateRightStation("功率测试", "PASS");
                        TestDisplayHelper.UpdateRightStation("频率测试", "PASS");
                        TestDisplayHelper.UpdateRightStation("信号测试", "测试中...");

                        To("等待测试完成");

                        #endregion
                        break;

                    case "等待测试完成":
                        #region 等待测试完成

                        // 这里等待测试完成
                        // 例如：等待测试设备返回结果

                        // ? 步骤4：更新最终测试结果
                        TestDisplayHelper.UpdateLeftStation("电阻测试", "PASS");
                        TestDisplayHelper.UpdateLeftStation("温度测试", "PASS");

                        TestDisplayHelper.UpdateRightStation("信号测试", "PASS");
                        TestDisplayHelper.UpdateRightStation("噪声测试", "FAIL");

                        WriteInfo("测试完成");

                        To(ACT_STATE_END);

                        #endregion
                        break;

                    case ACT_STATE_END:
                        #region 测试结束

                        WriteInfo("测试流程结束");
                        Finish();

                        #endregion
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteInfo($"测试异常: {ex.Message}", true);
                OnAlarm($"测试异常: {ex.Message}");
            }
        }
    }
}
