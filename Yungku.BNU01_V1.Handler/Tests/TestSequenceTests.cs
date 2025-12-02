using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yungku.BNU01_V1.Handler.Logic.TestSequence;
using Yungku.BNU01_V1.Handler.Logic.TestSequence.Config;
using Yungku.BNU01_V1.Handler.Logic.TestMethods;

namespace Yungku.BNU01_V1.Handler.Tests
{
    /// <summary>
    /// 测试序列系统单元测试类
    /// </summary>
    public class TestSequenceTests
    {
        private StringBuilder testResults = new StringBuilder();
        private int passCount = 0;
        private int failCount = 0;
        private int totalCount = 0;

        /// <summary>
        /// 运行所有测试
        /// </summary>
        public string RunAllTests()
        {
            testResults.Clear();
            passCount = 0;
            failCount = 0;
            totalCount = 0;

            testResults.AppendLine("========================================");
            testResults.AppendLine("测试序列系统单元测试");
            testResults.AppendLine($"测试时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            testResults.AppendLine("========================================");
            testResults.AppendLine();

            // 测试步骤类
            TestStepCreation();
            TestStepParameters();
            TestStepLimits();

            // 测试序列类
            TestSequenceCreation();
            TestSequenceStepManagement();

            // 测试方法注册表
            TestMethodRegistryInitialize();
            TestMethodRegistryScan();

            // 测试配置加载/保存
            TestConfigSaveAndLoad();

            // 测试执行器
            TestSequenceExecutorBasic();

            // 输出测试摘要
            testResults.AppendLine();
            testResults.AppendLine("========================================");
            testResults.AppendLine("测试摘要");
            testResults.AppendLine("========================================");
            testResults.AppendLine($"总测试数: {totalCount}");
            testResults.AppendLine($"通过: {passCount} ({(totalCount > 0 ? passCount * 100.0 / totalCount : 0):F2}%)");
            testResults.AppendLine($"失败: {failCount} ({(totalCount > 0 ? failCount * 100.0 / totalCount : 0):F2}%)");
            testResults.AppendLine("========================================");

            return testResults.ToString();
        }

        #region TestStep 测试

        private void TestStepCreation()
        {
            BeginTest("TestStep 创建测试");

            try
            {
                var step = new TestStep
                {
                    ID = "STEP_001",
                    Name = "测试步骤",
                    Description = "测试描述",
                    Type = StepType.NumericTest,
                    Enabled = true,
                    Timeout = 5000
                };

                AssertTrue("步骤ID设置正确", step.ID == "STEP_001");
                AssertTrue("步骤名称设置正确", step.Name == "测试步骤");
                AssertTrue("步骤类型设置正确", step.Type == StepType.NumericTest);
                AssertTrue("步骤启用状态正确", step.Enabled == true);
                AssertTrue("步骤超时设置正确", step.Timeout == 5000);
                AssertTrue("步骤默认结果是NotExecuted", step.Result == StepResult.NotExecuted);
            }
            catch (Exception ex)
            {
                LogResult("TestStep创建", false, ex.Message);
            }

            EndTest();
        }

        private void TestStepParameters()
        {
            BeginTest("TestStep 参数测试");

            try
            {
                var step = new TestStep();
                step.AddParameter("intParam", "int", "100");
                step.AddParameter("doubleParam", "double", "3.14");
                step.AddParameter("stringParam", "string", "hello");

                var intValue = step.GetParameterValue("intParam");
                var doubleValue = step.GetParameterValue("doubleParam");
                var stringValue = step.GetParameterValue("stringParam");

                AssertTrue("整数参数转换正确", intValue is int && (int)intValue == 100);
                AssertTrue("浮点参数转换正确", doubleValue is double && Math.Abs((double)doubleValue - 3.14) < 0.001);
                AssertTrue("字符串参数正确", stringValue is string && (string)stringValue == "hello");
            }
            catch (Exception ex)
            {
                LogResult("参数测试", false, ex.Message);
            }

            EndTest();
        }

        private void TestStepLimits()
        {
            BeginTest("TestStep 限值测试");

            try
            {
                var limits = new TestLimits(3.8, 4.2, "mm");

                AssertTrue("下限设置正确", Math.Abs(limits.Lower - 3.8) < 0.001);
                AssertTrue("上限设置正确", Math.Abs(limits.Upper - 4.2) < 0.001);
                AssertTrue("单位设置正确", limits.Unit == "mm");

                // 测试限值判断
                AssertTrue("4.0在限值范围内", limits.IsWithinLimits(4.0));
                AssertTrue("3.8在限值范围内", limits.IsWithinLimits(3.8));
                AssertTrue("4.2在限值范围内", limits.IsWithinLimits(4.2));
                AssertTrue("3.7不在限值范围内", !limits.IsWithinLimits(3.7));
                AssertTrue("4.3不在限值范围内", !limits.IsWithinLimits(4.3));
            }
            catch (Exception ex)
            {
                LogResult("限值测试", false, ex.Message);
            }

            EndTest();
        }

        #endregion

        #region TestSequence 测试

        private void TestSequenceCreation()
        {
            BeginTest("TestSequence 创建测试");

            try
            {
                var sequence = new TestSequence
                {
                    ID = "SEQ_001",
                    Name = "测试序列",
                    Description = "测试描述",
                    Enabled = true
                };

                AssertTrue("序列ID设置正确", sequence.ID == "SEQ_001");
                AssertTrue("序列名称设置正确", sequence.Name == "测试序列");
                AssertTrue("序列启用状态正确", sequence.Enabled == true);
                AssertTrue("序列默认状态是NotExecuted", sequence.State == SequenceState.NotExecuted);
                AssertTrue("序列步骤列表初始化", sequence.Steps != null);
            }
            catch (Exception ex)
            {
                LogResult("序列创建", false, ex.Message);
            }

            EndTest();
        }

        private void TestSequenceStepManagement()
        {
            BeginTest("TestSequence 步骤管理测试");

            try
            {
                var sequence = new TestSequence { ID = "SEQ_001", Name = "测试序列" };

                // 添加步骤
                var step1 = new TestStep { ID = "1", Name = "步骤1" };
                var step2 = new TestStep { ID = "2", Name = "步骤2" };
                var step3 = new TestStep { ID = "3", Name = "步骤3" };

                sequence.AddStep(step1);
                sequence.AddStep(step2);
                sequence.AddStep(step3);

                AssertTrue("步骤数量正确", sequence.Steps.Count == 3);

                // 获取步骤
                var foundStep = sequence.GetStepById("2");
                AssertTrue("根据ID获取步骤正确", foundStep != null && foundStep.Name == "步骤2");

                // 获取索引
                int index = sequence.GetStepIndex(step2);
                AssertTrue("获取步骤索引正确", index == 1);

                // 删除步骤
                sequence.RemoveStepById("2");
                AssertTrue("删除步骤后数量正确", sequence.Steps.Count == 2);

                // 插入步骤
                var step4 = new TestStep { ID = "4", Name = "步骤4" };
                sequence.InsertStep(1, step4);
                AssertTrue("插入步骤后数量正确", sequence.Steps.Count == 3);
                AssertTrue("插入位置正确", sequence.Steps[1].ID == "4");
            }
            catch (Exception ex)
            {
                LogResult("步骤管理", false, ex.Message);
            }

            EndTest();
        }

        #endregion

        #region TestMethodRegistry 测试

        private void TestMethodRegistryInitialize()
        {
            BeginTest("TestMethodRegistry 初始化测试");

            try
            {
                TestMethodRegistry.Reset();
                var registry = TestMethodRegistry.Instance;
                registry.Initialize();

                AssertTrue("注册表已初始化", registry.IsInitialized);
                AssertTrue("注册了测试方法", registry.MethodCount > 0);
            }
            catch (Exception ex)
            {
                LogResult("注册表初始化", false, ex.Message);
            }

            EndTest();
        }

        private void TestMethodRegistryScan()
        {
            BeginTest("TestMethodRegistry 扫描测试");

            try
            {
                var registry = TestMethodRegistry.Instance;
                if (!registry.IsInitialized)
                {
                    registry.Initialize();
                }

                // 检查 CommonTestMethods 是否被注册
                var method = registry.GetMethod("CommonTestMethods", "Initialize");
                AssertTrue("找到Initialize方法", method != null);

                method = registry.GetMethod("CommonTestMethods", "TestFocalLength");
                AssertTrue("找到TestFocalLength方法", method != null);

                method = registry.GetMethod("CommonTestMethods", "TestColorTemperature");
                AssertTrue("找到TestColorTemperature方法", method != null);

                // 检查分类
                var categories = registry.GetAllCategories();
                AssertTrue("获取到分类列表", categories != null);
            }
            catch (Exception ex)
            {
                LogResult("注册表扫描", false, ex.Message);
            }

            EndTest();
        }

        #endregion

        #region 配置加载保存测试

        private void TestConfigSaveAndLoad()
        {
            BeginTest("配置保存和加载测试");

            string tempFile = Path.Combine(Path.GetTempPath(), $"test_sequence_{Guid.NewGuid():N}.xml");

            try
            {
                // 创建测试配置
                var config = new TestSequenceConfig
                {
                    Name = "测试配置",
                    Version = "1.0",
                    Sequences = new List<TestSequence>
                    {
                        new TestSequence
                        {
                            ID = "SEQ_TEST",
                            Name = "测试序列",
                            Steps = new List<TestStep>
                            {
                                new TestStep
                                {
                                    ID = "1",
                                    Name = "测试步骤",
                                    Type = StepType.NumericTest,
                                    TargetMethod = new TargetMethodInfo("CommonTestMethods", "TestFocalLength"),
                                    Limits = new TestLimits(3.8, 4.2, "mm")
                                }
                            }
                        }
                    }
                };

                // 保存配置
                var saver = new SequenceConfigSaver();
                saver.SaveConfig(config, tempFile);
                AssertTrue("配置文件已创建", File.Exists(tempFile));

                // 加载配置
                var loader = new SequenceConfigLoader();
                var loadedConfig = loader.LoadConfig(tempFile);

                AssertTrue("加载配置成功", loadedConfig != null);
                AssertTrue("序列数量正确", loadedConfig.Sequences.Count == 1);
                AssertTrue("序列ID正确", loadedConfig.Sequences[0].ID == "SEQ_TEST");
                AssertTrue("步骤数量正确", loadedConfig.Sequences[0].Steps.Count == 1);
                AssertTrue("步骤限值正确", loadedConfig.Sequences[0].Steps[0].Limits != null);
            }
            catch (Exception ex)
            {
                LogResult("配置保存加载", false, ex.Message);
            }
            finally
            {
                // 清理临时文件
                if (File.Exists(tempFile))
                {
                    try { File.Delete(tempFile); } catch { }
                }
            }

            EndTest();
        }

        #endregion

        #region 执行器测试

        private void TestSequenceExecutorBasic()
        {
            BeginTest("SequenceExecutor 基础测试");

            try
            {
                var executor = new SequenceExecutor();

                // 创建简单序列
                var sequence = new TestSequence
                {
                    ID = "SEQ_EXEC_TEST",
                    Name = "执行测试序列",
                    Steps = new List<TestStep>
                    {
                        new TestStep
                        {
                            ID = "1",
                            Name = "初始化",
                            Type = StepType.Action,
                            TargetMethod = new TargetMethodInfo("CommonTestMethods", "Initialize"),
                            Timeout = 5000
                        },
                        new TestStep
                        {
                            ID = "2",
                            Name = "焦距测试",
                            Type = StepType.NumericTest,
                            TargetMethod = new TargetMethodInfo("CommonTestMethods", "TestFocalLength"),
                            Limits = new TestLimits(0, 10, "mm"), // 宽松的限值确保通过
                            Timeout = 5000
                        }
                    }
                };

                // 执行序列
                var result = executor.Execute(sequence);

                AssertTrue("序列执行完成", result != null);
                AssertTrue("序列状态正确", sequence.State == SequenceState.Completed || sequence.State == SequenceState.Failed);
                AssertTrue("步骤已执行", sequence.Steps[0].Result != StepResult.NotExecuted);
            }
            catch (Exception ex)
            {
                LogResult("执行器基础测试", false, ex.Message);
            }

            EndTest();
        }

        #endregion

        #region 辅助方法

        private void BeginTest(string testName)
        {
            testResults.AppendLine($"\n{testName}");
            testResults.AppendLine(new string('-', 40));
        }

        private void EndTest()
        {
            testResults.AppendLine();
        }

        private void AssertTrue(string testName, bool condition)
        {
            totalCount++;
            if (condition)
            {
                passCount++;
                testResults.AppendLine($"  ✓ PASS: {testName}");
            }
            else
            {
                failCount++;
                testResults.AppendLine($"  ✗ FAIL: {testName}");
            }
        }

        private void LogResult(string testName, bool pass, string message)
        {
            totalCount++;
            if (pass)
            {
                passCount++;
                testResults.AppendLine($"  ✓ PASS: {testName} - {message}");
            }
            else
            {
                failCount++;
                testResults.AppendLine($"  ✗ FAIL: {testName} - {message}");
            }
        }

        #endregion

        /// <summary>
        /// 保存测试报告
        /// </summary>
        public void SaveTestReport(string filePath)
        {
            try
            {
                string report = RunAllTests();
                File.WriteAllText(filePath, report, Encoding.UTF8);
                testResults.AppendLine($"\n测试报告已保存到: {filePath}");
            }
            catch (Exception ex)
            {
                testResults.AppendLine($"\n保存测试报告失败: {ex.Message}");
            }
        }
    }
}
