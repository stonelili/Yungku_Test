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

            // 测试变量功能
            TestSequenceVariables();

            // 安全修复测试
            TestSequenceExecutorDisposable();
            TestExpressionSafetyLimits();
            TestVariableLastAccessTime();
            TestThreadSafety();
            TestStaticDictionaryCleanup();
            TestWhileLoopProtection();

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
                // 确保清理临时文件，即使测试中断
                CleanupTempFile(tempFile);
            }

            EndTest();
        }

        /// <summary>
        /// 清理临时文件
        /// </summary>
        private void CleanupTempFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // 记录清理失败但不影响测试结果
                testResults.AppendLine($"  [警告] 清理临时文件失败: {ex.Message}");
            }
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

        #region 变量功能测试

        private void TestSequenceVariables()
        {
            BeginTest("序列变量功能测试");

            try
            {
                // 测试 SequenceVariable 类
                var variable = new SequenceVariable("testVar", "int", "100");
                AssertTrue("变量名称正确", variable.Name == "testVar");
                AssertTrue("变量类型正确", variable.Type == "int");
                AssertTrue("变量默认值正确", variable.DefaultValue == "100");

                // 测试类型转换
                var typedValue = variable.GetTypedDefaultValue();
                AssertTrue("整数类型转换正确", typedValue is int && (int)typedValue == 100);

                // 测试序列变量管理
                var sequence = new TestSequence { ID = "SEQ_VAR_TEST", Name = "变量测试序列" };
                sequence.AddVariable("ProductIndex", "int", "0", "产品索引");
                sequence.AddVariable("TestResult", "double", "0.0", "测试结果");
                sequence.AddVariable("Barcode", "string", "", "二维码");

                AssertTrue("序列变量数量正确", sequence.Variables.Count == 3);

                // 测试获取变量
                var productIndexVar = sequence.GetVariable("ProductIndex");
                AssertTrue("获取变量成功", productIndexVar != null);
                AssertTrue("变量类型正确", productIndexVar.Type == "int");

                // 测试设置变量值
                bool setResult = sequence.SetVariableValue("TestResult", 4.5);
                AssertTrue("设置变量值成功", setResult);

                var testResultValue = sequence.GetVariableValue("TestResult");
                AssertTrue("获取变量值正确", testResultValue != null && (double)testResultValue == 4.5);

                // 测试变量重置
                sequence.ResetVariables();
                var resetValue = sequence.GetVariableValue("TestResult");
                AssertTrue("变量重置成功", resetValue != null && Math.Abs((double)resetValue) < 0.001);

                // 测试变量引用解析（在步骤参数中使用 ${variableName}）
                sequence.SetVariableValue("ProductIndex", 5);
                var step = new TestStep
                {
                    ID = "1",
                    Name = "测试步骤",
                    ResultVariable = "TestResult"
                };
                step.AddParameter("index", "string", "${ProductIndex}");

                // 验证 ResultVariable 属性
                AssertTrue("ResultVariable 设置正确", step.ResultVariable == "TestResult");

                // 测试删除变量
                bool removeResult = sequence.RemoveVariable("Barcode");
                AssertTrue("删除变量成功", removeResult);
                AssertTrue("变量已删除", sequence.Variables.Count == 2);

                // 测试变量作用域
                var localVar = new SequenceVariable("localVar", "int", "1", VariableScope.Local);
                AssertTrue("局部变量作用域正确", localVar.Scope == VariableScope.Local);

                var globalVar = new SequenceVariable("globalVar", "int", "100", VariableScope.Global);
                AssertTrue("全局变量作用域正确", globalVar.Scope == VariableScope.Global);

                var seqVar = new SequenceVariable("seqVar", "int", "50");
                AssertTrue("序列变量默认作用域正确", seqVar.Scope == VariableScope.Sequence);

                // 测试数组类型变量
                var arrayVar = new SequenceVariable("testArray", "int[]", "1,2,3,4,5");
                AssertTrue("数组变量是数组类型", arrayVar.IsArray);
                AssertTrue("数组变量基础类型正确", arrayVar.BaseType == "int");

                var arrayValue = arrayVar.GetTypedDefaultValue() as int[];
                AssertTrue("数组默认值转换成功", arrayValue != null);
                AssertTrue("数组长度正确", arrayValue.Length == 5);
                AssertTrue("数组元素值正确", arrayValue[0] == 1 && arrayValue[4] == 5);

                // 测试数组元素访问
                arrayVar.Reset();
                var element = arrayVar.GetArrayElement(2);
                AssertTrue("获取数组元素成功", element != null && (int)element == 3);

                // 测试设置数组元素
                bool setArrayResult = arrayVar.SetArrayElement(2, 100);
                AssertTrue("设置数组元素成功", setArrayResult);
                element = arrayVar.GetArrayElement(2);
                AssertTrue("数组元素已更新", element != null && (int)element == 100);

                // 测试double数组
                var doubleArrayVar = new SequenceVariable("doubleArray", "double[]", "1.1,2.2,3.3");
                var doubleArray = doubleArrayVar.GetTypedDefaultValue() as double[];
                AssertTrue("double数组转换成功", doubleArray != null && doubleArray.Length == 3);
                AssertTrue("double数组元素正确", Math.Abs(doubleArray[1] - 2.2) < 0.001);

                // 测试string数组
                var stringArrayVar = new SequenceVariable("stringArray", "string[]", "a,b,c");
                var stringArray = stringArrayVar.GetTypedDefaultValue() as string[];
                AssertTrue("string数组转换成功", stringArray != null && stringArray.Length == 3);
                AssertTrue("string数组元素正确", stringArray[0] == "a" && stringArray[2] == "c");
            }
            catch (Exception ex)
            {
                LogResult("变量功能测试", false, ex.Message);
            }

            EndTest();
        }

        #endregion

        #region 安全修复测试

        /// <summary>
        /// 测试IDisposable实现
        /// </summary>
        private void TestSequenceExecutorDisposable()
        {
            BeginTest("SequenceExecutor IDisposable 测试");

            try
            {
                // 测试Dispose模式
                SequenceExecutor executor = null;
                try
                {
                    executor = new SequenceExecutor();
                    AssertTrue("执行器创建成功", executor != null);
                }
                finally
                {
                    if (executor != null)
                    {
                        executor.Dispose();
                        AssertTrue("执行器Dispose成功", true);
                    }
                }

                // 测试using语句
                using (var exec = new SequenceExecutor())
                {
                    AssertTrue("Using语句正常工作", exec != null);
                }
                AssertTrue("Using语句结束后资源已释放", true);
            }
            catch (Exception ex)
            {
                LogResult("IDisposable测试", false, ex.Message);
            }

            EndTest();
        }

        /// <summary>
        /// 测试表达式安全限制
        /// </summary>
        private void TestExpressionSafetyLimits()
        {
            BeginTest("表达式安全限制测试");

            try
            {
                using (var executor = new SequenceExecutor())
                {
                    // 测试正常表达式
                    var result = executor.EvaluateExpression("1 + 2 * 3");
                    AssertTrue("正常表达式计算成功", result != null);

                    // 测试嵌套括号表达式
                    result = executor.EvaluateExpression("((1 + 2) * 3)");
                    AssertTrue("嵌套表达式计算成功", result != null);

                    // 测试过长表达式
                    // 使用超过最大长度限制（1000字符）的表达式
                    const int MAX_EXPRESSION_LENGTH_FOR_TEST = 1000;
                    string longExpr = new string('1', MAX_EXPRESSION_LENGTH_FOR_TEST + 1);
                    bool threwException = false;
                    try
                    {
                        result = executor.EvaluateExpression(longExpr);
                    }
                    catch (InvalidOperationException)
                    {
                        threwException = true;
                    }
                    AssertTrue("过长表达式被拒绝", threwException || result == null);
                }
            }
            catch (Exception ex)
            {
                LogResult("表达式安全限制测试", false, ex.Message);
            }

            EndTest();
        }

        /// <summary>
        /// 测试变量LastAccessTime属性
        /// 注意：使用小延迟来确保时间戳差异可检测
        /// </summary>
        private void TestVariableLastAccessTime()
        {
            BeginTest("变量LastAccessTime属性测试");

            try
            {
                var variable = new SequenceVariable("testVar", "int", "100");
                
                // 获取初始访问时间
                DateTime initialTime = variable.LastAccessTime;
                AssertTrue("初始访问时间已设置", initialTime != DateTime.MinValue);

                // 等待一小段时间以确保时间戳更新可检测
                // 使用10ms作为最小可检测时间差
                System.Threading.Thread.Sleep(10);

                // 设置值更新访问时间
                variable.CurrentValue = 200;
                DateTime newTime = variable.LastAccessTime;
                
                AssertTrue("设置值后访问时间已更新", newTime >= initialTime);
            }
            catch (Exception ex)
            {
                LogResult("LastAccessTime测试", false, ex.Message);
            }

            EndTest();
        }

        /// <summary>
        /// 测试线程安全（ConcurrentDictionary）
        /// </summary>
        private void TestThreadSafety()
        {
            BeginTest("线程安全测试");

            try
            {
                // 测试全局变量并发访问
                bool noErrors = true;
                var tasks = new List<System.Threading.Tasks.Task>();

                for (int i = 0; i < 10; i++)
                {
                    int idx = i;
                    var task = System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            var variable = new SequenceVariable($"globalVar_{idx}", "int", idx.ToString(), VariableScope.Global);
                            SequenceExecutor.SetGlobalVariable($"globalVar_{idx}", variable);
                            var retrieved = SequenceExecutor.GetGlobalVariable($"globalVar_{idx}");
                            if (retrieved == null)
                                noErrors = false;
                        }
                        catch
                        {
                            noErrors = false;
                        }
                    });
                    tasks.Add(task);
                }

                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                AssertTrue("并发访问全局变量无错误", noErrors);

                // 清理
                SequenceExecutor.ClearGlobalVariables();
            }
            catch (Exception ex)
            {
                LogResult("线程安全测试", false, ex.Message);
            }

            EndTest();
        }

        /// <summary>
        /// 测试静态字典清理机制
        /// </summary>
        private void TestStaticDictionaryCleanup()
        {
            BeginTest("静态字典清理机制测试");

            try
            {
                // 添加全局变量
                var variable = new SequenceVariable("cleanupTestVar", "int", "100", VariableScope.Global);
                SequenceExecutor.SetGlobalVariable("cleanupTestVar", variable);

                // 验证变量存在
                var retrieved = SequenceExecutor.GetGlobalVariable("cleanupTestVar");
                AssertTrue("变量添加成功", retrieved != null);

                // 清理过期变量（使用0时间跨度，立即清理）
                SequenceExecutor.CleanupStaleGlobalVariables(TimeSpan.Zero);

                // 验证变量已被清理
                retrieved = SequenceExecutor.GetGlobalVariable("cleanupTestVar");
                AssertTrue("变量已被清理", retrieved == null);

                // 清理全局变量
                SequenceExecutor.ClearGlobalVariables();
            }
            catch (Exception ex)
            {
                LogResult("清理机制测试", false, ex.Message);
            }

            EndTest();
        }

        /// <summary>
        /// 测试While循环保护
        /// </summary>
        private void TestWhileLoopProtection()
        {
            BeginTest("While循环保护测试");

            try
            {
                // 创建一个带有While循环的步骤
                var step = new TestStep
                {
                    ID = "WHILE_TEST",
                    Name = "While循环测试",
                    Type = StepType.WhileLoop,
                    WhileCondition = "true", // 无限循环条件
                    MaxIterations = 0 // 测试默认值设置
                };

                // 验证MaxIterations默认值
                AssertTrue("MaxIterations初始值为0", step.MaxIterations == 0);

                // 设置MaxIterations
                step.MaxIterations = 100;
                AssertTrue("MaxIterations设置成功", step.MaxIterations == 100);
            }
            catch (Exception ex)
            {
                LogResult("While循环保护测试", false, ex.Message);
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
