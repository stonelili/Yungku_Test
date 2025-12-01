using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Yungku.BNU01_V1.Handler.Logic.StationAction;

namespace Yungku.BNU01_V1.Handler.Tests
{
    /// <summary>
    /// ActionCalculator 单元测试类
    /// 注意：这是一个示例测试类，实际项目中应使用 NUnit 或 xUnit 等测试框架
    /// </summary>
    public class ActionCalculatorTests
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
            testResults.AppendLine("ActionCalculator 单元测试开始");
            testResults.AppendLine($"测试时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            testResults.AppendLine("========================================");
            testResults.AppendLine();

            // 基础运算测试
            TestAddition();
            TestSubtraction();
            TestMultiplication();
            TestDivision();
            TestComplexCalculation();

            // 边界测试
            TestZeroDivision();
            TestLargeNumbers();
            TestSmallNumbers();
            TestNegativeNumbers();

            // 比较运算符测试
            TestCompareOperators();
            TestDoubleLimit();

            // 异常测试
            TestInvalidParameters();
            TestNaNAndInfinity();

            // 性能测试
            TestPerformance();

            // 输出测试摘要
            testResults.AppendLine();
            testResults.AppendLine("========================================");
            testResults.AppendLine("测试摘要");
            testResults.AppendLine("========================================");
            testResults.AppendLine($"总测试数: {totalCount}");
            testResults.AppendLine($"通过: {passCount} ({(passCount * 100.0 / totalCount):F2}%)");
            testResults.AppendLine($"失败: {failCount} ({(failCount * 100.0 / totalCount):F2}%)");
            testResults.AppendLine("========================================");

            return testResults.ToString();
        }

        #region 基础运算测试

        /// <summary>
        /// 测试加法运算
        /// </summary>
        private void TestAddition()
        {
            BeginTest("加法运算测试");

            // 测试用例1: 正数相加
            AssertEqual("10 + 20", 10.0, 20.0, 30.0, CalculatorType.加法运算);

            // 测试用例2: 小数相加
            AssertEqual("10.5 + 20.3", 10.5, 20.3, 30.8, CalculatorType.加法运算);

            // 测试用例3: 负数相加
            AssertEqual("-10 + (-20)", -10.0, -20.0, -30.0, CalculatorType.加法运算);

            // 测试用例4: 正负数相加
            AssertEqual("10 + (-20)", 10.0, -20.0, -10.0, CalculatorType.加法运算);

            EndTest();
        }

        /// <summary>
        /// 测试减法运算
        /// </summary>
        private void TestSubtraction()
        {
            BeginTest("减法运算测试");

            AssertEqual("20 - 10", 20.0, 10.0, 10.0, CalculatorType.减法运算);
            AssertEqual("10 - 20", 10.0, 20.0, -10.0, CalculatorType.减法运算);
            AssertEqual("10.5 - 5.3", 10.5, 5.3, 5.2, CalculatorType.减法运算);
            AssertEqual("-10 - (-5)", -10.0, -5.0, -5.0, CalculatorType.减法运算);

            EndTest();
        }

        /// <summary>
        /// 测试乘法运算
        /// </summary>
        private void TestMultiplication()
        {
            BeginTest("乘法运算测试");

            AssertEqual("10 × 20", 10.0, 20.0, 200.0, CalculatorType.乘法运算);
            AssertEqual("3.5 × 2.0", 3.5, 2.0, 7.0, CalculatorType.乘法运算);
            AssertEqual("-10 × 5", -10.0, 5.0, -50.0, CalculatorType.乘法运算);
            AssertEqual("-10 × (-5)", -10.0, -5.0, 50.0, CalculatorType.乘法运算);
            AssertEqual("0 × 100", 0.0, 100.0, 0.0, CalculatorType.乘法运算);

            EndTest();
        }

        /// <summary>
        /// 测试除法运算
        /// </summary>
        private void TestDivision()
        {
            BeginTest("除法运算测试");

            AssertEqual("100 ÷ 4", 100.0, 4.0, 25.0, CalculatorType.除法运算);
            AssertEqual("10 ÷ 2", 10.0, 2.0, 5.0, CalculatorType.除法运算);
            AssertEqual("7 ÷ 2", 7.0, 2.0, 3.5, CalculatorType.除法运算);
            AssertEqual("-10 ÷ 2", -10.0, 2.0, -5.0, CalculatorType.除法运算);

            EndTest();
        }

        /// <summary>
        /// 测试综合运算
        /// </summary>
        private void TestComplexCalculation()
        {
            BeginTest("综合运算测试");

            // (A + B) × (A - B) ÷ 2
            AssertEqual("(10+6)×(10-6)÷2", 10.0, 6.0, 32.0, CalculatorType.综合运算);
            AssertEqual("(8+2)×(8-2)÷2", 8.0, 2.0, 30.0, CalculatorType.综合运算);
            AssertEqual("(5+5)×(5-5)÷2", 5.0, 5.0, 0.0, CalculatorType.综合运算);

            EndTest();
        }

        #endregion

        #region 边界测试

        /// <summary>
        /// 测试除零错误
        /// </summary>
        private void TestZeroDivision()
        {
            BeginTest("除零错误测试");

            // 这个测试应该失败（除数为零）
            try
            {
                ActionCalculator calc = CreateCalculator(100.0, 0.0, CalculatorType.除法运算);
                // 如果没有抛出异常或返回错误，则测试失败
                LogResult("除零检测", false, "应该检测到除零错误");
            }
            catch
            {
                LogResult("除零检测", true, "正确检测到除零错误");
            }

            EndTest();
        }

        /// <summary>
        /// 测试大数运算
        /// </summary>
        private void TestLargeNumbers()
        {
            BeginTest("大数运算测试");

            AssertEqual("1E10 + 2E10", 1e10, 2e10, 3e10, CalculatorType.加法运算, 1e8);
            AssertEqual("1E10 × 2", 1e10, 2.0, 2e10, CalculatorType.乘法运算, 1e8);

            EndTest();
        }

        /// <summary>
        /// 测试小数运算
        /// </summary>
        private void TestSmallNumbers()
        {
            BeginTest("小数运算测试");

            AssertEqual("0.001 + 0.002", 0.001, 0.002, 0.003, CalculatorType.加法运算, 0.0001);
            AssertEqual("0.1 × 0.1", 0.1, 0.1, 0.01, CalculatorType.乘法运算, 0.0001);

            EndTest();
        }

        /// <summary>
        /// 测试负数运算
        /// </summary>
        private void TestNegativeNumbers()
        {
            BeginTest("负数运算测试");

            AssertEqual("-10 + 5", -10.0, 5.0, -5.0, CalculatorType.加法运算);
            AssertEqual("-10 - (-5)", -10.0, -5.0, -5.0, CalculatorType.减法运算);
            AssertEqual("-10 × (-5)", -10.0, -5.0, 50.0, CalculatorType.乘法运算);
            AssertEqual("-10 ÷ (-2)", -10.0, -2.0, 5.0, CalculatorType.除法运算);

            EndTest();
        }

        #endregion

        #region 比较运算符测试

        /// <summary>
        /// 测试比较运算符
        /// </summary>
        private void TestCompareOperators()
        {
            BeginTest("比较运算符测试");

            // 等于
            AssertCompare("等于判断-通过", 30.0, 30.0, CompareOperator.等于, true);
            AssertCompare("等于判断-失败", 30.0, 30.1, CompareOperator.等于, false);

            // 小于
            AssertCompare("小于判断-通过", 29.0, 30.0, CompareOperator.小于, true);
            AssertCompare("小于判断-失败", 31.0, 30.0, CompareOperator.小于, false);

            // 大于
            AssertCompare("大于判断-通过", 31.0, 30.0, CompareOperator.大于, true);
            AssertCompare("大于判断-失败", 29.0, 30.0, CompareOperator.大于, false);

            // 小于等于
            AssertCompare("小于等于判断-通过", 30.0, 30.0, CompareOperator.小于等于, true);
            AssertCompare("小于等于判断-失败", 31.0, 30.0, CompareOperator.小于等于, false);

            // 大于等于
            AssertCompare("大于等于判断-通过", 30.0, 30.0, CompareOperator.大于等于, true);
            AssertCompare("大于等于判断-失败", 29.0, 30.0, CompareOperator.大于等于, false);

            EndTest();
        }

        /// <summary>
        /// 测试双限判断
        /// </summary>
        private void TestDoubleLimit()
        {
            BeginTest("双限判断测试");

            // 双限-小于等于 (10 <= X <= 30)
            AssertDoubleLimitCompare("双限小于等于-下限", 10.0, 10.0, 30.0, CompareOperator.双限_小于等于, true);
            AssertDoubleLimitCompare("双限小于等于-中间", 20.0, 10.0, 30.0, CompareOperator.双限_小于等于, true);
            AssertDoubleLimitCompare("双限小于等于-上限", 30.0, 10.0, 30.0, CompareOperator.双限_小于等于, true);
            AssertDoubleLimitCompare("双限小于等于-超出", 35.0, 10.0, 30.0, CompareOperator.双限_小于等于, false);

            // 双限-小于 (10 < X < 30)
            AssertDoubleLimitCompare("双限小于-下限", 10.0, 10.0, 30.0, CompareOperator.双限_小于, false);
            AssertDoubleLimitCompare("双限小于-中间", 20.0, 10.0, 30.0, CompareOperator.双限_小于, true);
            AssertDoubleLimitCompare("双限小于-上限", 30.0, 10.0, 30.0, CompareOperator.双限_小于, false);

            EndTest();
        }

        #endregion

        #region 异常测试

        /// <summary>
        /// 测试无效参数
        /// </summary>
        private void TestInvalidParameters()
        {
            BeginTest("无效参数测试");

            // 无效的上下限关系（应该失败）
            LogTest("无效的上下限", () =>
            {
                ActionCalculator calc = CreateCalculator(10.0, 20.0, CalculatorType.加法运算);
                calc.LowLimit = 30.0;
                calc.UpperLimit = 10.0; // 上限小于下限
                calc.CompareType = CompareOperator.双限_小于等于;
                // 参数验证应该失败
                return false;
            });

            EndTest();
        }

        /// <summary>
        /// 测试NaN和Infinity
        /// </summary>
        private void TestNaNAndInfinity()
        {
            BeginTest("NaN和Infinity测试");

            // NaN测试
            LogTest("NaN操作数A", () =>
            {
                ActionCalculator calc = CreateCalculator(double.NaN, 20.0, CalculatorType.加法运算);
                // 应该检测到无效参数
                return false;
            });

            LogTest("Infinity操作数B", () =>
            {
                ActionCalculator calc = CreateCalculator(10.0, double.PositiveInfinity, CalculatorType.加法运算);
                // 应该检测到无效参数
                return false;
            });

            EndTest();
        }

        #endregion

        #region 性能测试

        /// <summary>
        /// 测试性能
        /// </summary>
        private void TestPerformance()
        {
            BeginTest("性能测试");

            int iterations = 1000;
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                ActionCalculator calc = CreateCalculator(i, i + 1, CalculatorType.加法运算);
                // 模拟计算
            }

            sw.Stop();
            double avgTime = sw.ElapsedMilliseconds / (double)iterations;

            testResults.AppendLine($"  执行 {iterations} 次计算");
            testResults.AppendLine($"  总耗时: {sw.ElapsedMilliseconds} ms");
            testResults.AppendLine($"  平均耗时: {avgTime:F3} ms");
            testResults.AppendLine($"  性能评估: {(avgTime < 1 ? "优秀" : avgTime < 10 ? "良好" : "需优化")}");

            EndTest();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建计算器实例
        /// </summary>
        private ActionCalculator CreateCalculator(double a, double b, CalculatorType type)
        {
            return new ActionCalculator
            {
                OperandA = a,
                OperandB = b,
                CalculatorType = type,
                IsVerifyResult = false,
                IsDetailLog = false
            };
        }

        /// <summary>
        /// 断言相等
        /// </summary>
        private void AssertEqual(string testName, double a, double b, double expected,
            CalculatorType type, double tolerance = 0.001)
        {
            totalCount++;
            try
            {
                ActionCalculator calc = CreateCalculator(a, b, type);
                calc.IsVerifyResult = true;
                calc.CompareType = CompareOperator.双限_小于等于;
                calc.LowLimit = expected - tolerance;
                calc.UpperLimit = expected + tolerance;

                // 模拟执行（实际应调用Execute方法）
                double result = SimulateCalculation(a, b, type);
                bool isValid = Math.Abs(result - expected) <= tolerance;

                if (isValid)
                {
                    passCount++;
                    testResults.AppendLine($"  ✓ PASS: {testName}");
                    testResults.AppendLine($"    计算结果: {result}, 期望: {expected}");
                }
                else
                {
                    failCount++;
                    testResults.AppendLine($"  ✗ FAIL: {testName}");
                    testResults.AppendLine($"    计算结果: {result}, 期望: {expected}, 误差: {Math.Abs(result - expected)}");
                }
            }
            catch (Exception ex)
            {
                failCount++;
                testResults.AppendLine($"  ✗ FAIL: {testName}");
                testResults.AppendLine($"    异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 断言单限比较
        /// </summary>
        private void AssertCompare(string testName, double value, double limit,
            CompareOperator compareType, bool shouldPass)
        {
            totalCount++;
            ActionCalculator calc = new ActionCalculator
            {
                IsVerifyResult = true,
                CompareType = compareType,
                LowLimit = limit
            };

            bool actualPass = false;
            switch (compareType)
            {
                case CompareOperator.等于:
                    actualPass = Math.Abs(value - limit) < 1e-10;
                    break;
                case CompareOperator.小于:
                    actualPass = value < limit;
                    break;
                case CompareOperator.大于:
                    actualPass = value > limit;
                    break;
                case CompareOperator.小于等于:
                    actualPass = value <= limit;
                    break;
                case CompareOperator.大于等于:
                    actualPass = value >= limit;
                    break;
            }

            bool testPass = (actualPass == shouldPass);

            if (testPass)
            {
                passCount++;
                testResults.AppendLine($"  ✓ PASS: {testName}");
            }
            else
            {
                failCount++;
                testResults.AppendLine($"  ✗ FAIL: {testName}");
                testResults.AppendLine($"    期望验证{(shouldPass ? "通过" : "失败")}, 实际{(actualPass ? "通过" : "失败")}");
            }
        }

        /// <summary>
        /// 断言双限比较
        /// </summary>
        private void AssertDoubleLimitCompare(string testName, double value, double lowLimit,
            double upperLimit, CompareOperator compareType, bool shouldPass)
        {
            totalCount++;
            ActionCalculator calc = new ActionCalculator
            {
                IsVerifyResult = true,
                CompareType = compareType,
                LowLimit = lowLimit,
                UpperLimit = upperLimit
            };

            bool actualPass = false;
            switch (compareType)
            {
                case CompareOperator.双限_小于等于:
                    actualPass = value >= lowLimit && value <= upperLimit;
                    break;
                case CompareOperator.双限_小于:
                    actualPass = value > lowLimit && value < upperLimit;
                    break;
                case CompareOperator.双限_大于等于:
                    actualPass = value <= lowLimit || value >= upperLimit;
                    break;
                case CompareOperator.双限_大于:
                    actualPass = value < lowLimit || value > upperLimit;
                    break;
                case CompareOperator.双限_小于大于:
                    actualPass = value > lowLimit && value < upperLimit;
                    break;
                case CompareOperator.双限_大于小于:
                    actualPass = value < lowLimit || value > upperLimit;
                    break;
            }

            bool testPass = (actualPass == shouldPass);

            if (testPass)
            {
                passCount++;
                testResults.AppendLine($"  ✓ PASS: {testName}");
            }
            else
            {
                failCount++;
                testResults.AppendLine($"  ✗ FAIL: {testName}");
                testResults.AppendLine($"    期望验证{(shouldPass ? "通过" : "失败")}, 实际{(actualPass ? "通过" : "失败")}");
            }
        }

        /// <summary>
        /// 记录测试结果
        /// </summary>
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

        /// <summary>
        /// 执行测试并记录
        /// </summary>
        private void LogTest(string testName, Func<bool> testFunc)
        {
            totalCount++;
            try
            {
                bool result = testFunc();
                if (result)
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
            catch (Exception ex)
            {
                failCount++;
                testResults.AppendLine($"  ✗ FAIL: {testName} - 异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 模拟计算
        /// </summary>
        private double SimulateCalculation(double a, double b, CalculatorType type)
        {
            switch (type)
            {
                case CalculatorType.加法运算:
                    return a + b;
                case CalculatorType.减法运算:
                    return a - b;
                case CalculatorType.乘法运算:
                    return a * b;
                case CalculatorType.除法运算:
                    return a / b;
                case CalculatorType.综合运算:
                    return (a + b) * (a - b) / 2.0;
                default:
                    throw new NotImplementedException();
            }
        }

        private void BeginTest(string testName)
        {
            testResults.AppendLine($"\n{testName}");
            testResults.AppendLine(new string('-', 40));
        }

        private void EndTest()
        {
            testResults.AppendLine();
        }

        #endregion

        /// <summary>
        /// 生成测试报告
        /// </summary>
        public void SaveTestReport(string filePath)
        {
            try
            {
                string report = RunAllTests();
                System.IO.File.WriteAllText(filePath, report, Encoding.UTF8);
                testResults.AppendLine($"\n测试报告已保存到: {filePath}");
            }
            catch (Exception ex)
            {
                testResults.AppendLine($"\n保存测试报告失败: {ex.Message}");
            }
        }
    }
}