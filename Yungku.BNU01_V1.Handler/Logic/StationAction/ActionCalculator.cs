using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;
using YungkuSystem.Machine;
using YungkuSystem.Structs;
using YungkuSystem.TestFlow;
using YungkuSystem.ThreadMessage;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    /// <summary>
    /// 计算器操作类型枚举
    /// </summary>
    public enum CalculatorType
    {
        加法运算,
        减法运算,
        乘法运算,
        除法运算,
        综合运算,
        验证结果
    }

    /// <summary>
    /// 比较运算符枚举
    /// </summary>
    public enum CompareOperator
    {
        [Description("等于(=)")]
        等于,

        [Description("小于(<)")]
        小于,

        [Description("大于(>)")]
        大于,

        [Description("小于等于(<=)")]
        小于等于,

        [Description("大于等于(>=)")]
        大于等于,

        [Description("双限-小于等于(<=X<=)")]
        双限_小于等于,

        [Description("双限-小于(<X<)")]
        双限_小于,

        [Description("双限-大于等于(>=X>=)")]
        双限_大于等于,

        [Description("双限-大于(>X>)")]
        双限_大于,

        [Description("双限-小于大于(<X>)")]
        双限_小于大于,

        [Description("双限-大于小于(>X<)")]
        双限_大于小于
    }

    /// <summary>
    /// 计算器动作类 - 用于测试和验证数学运算
    /// </summary>
    public class ActionCalculator : ActionObject
    {
        #region 属性定义

        public override string ObjectClass
        {
            get { return "计算器"; }
        }

        public override bool IsContainner
        {
            get { return false; }
        }

        public override bool IsRunState
        {
            get { return true; }
        }

        [MyDisplayName("操作数A"), MyCategory("计算参数"), Description("第一个操作数")]
        public double OperandA { get; set; } = 0.0;

        [MyDisplayName("操作数B"), MyCategory("计算参数"), Description("第二个操作数")]
        public double OperandB { get; set; } = 0.0;

        [MyDisplayName("期望结果"), MyCategory("计算参数"), Description("用于验证的期望结果")]
        public double ExpectedResult { get; set; } = 0.0;

        [MyDisplayName("下限值"), MyCategory("计算参数"), Description("测试结果的下限值")]
        public double LowLimit { get; set; } = 0.0;

        private double upperLimit = 100.0;

        [MyDisplayName("上限值"), MyCategory("计算参数"), Description("测试结果的上限值（仅双限比较方式有效）")]
        public double UpperLimit
        {
            get { return upperLimit; }
            set
            {
                // 当选择单限比较时，不允许修改上限值
                if (IsUpperLimitEnabled())
                {
                    upperLimit = value;
                }
                else
                {
                    // 单限比较时，上限值无效，自动设置为下限值
                    upperLimit = LowLimit;
                }
            }
        }

        [MyDisplayName("比较方式"), MyCategory("计算参数"), Description("结果判断的比较运算符")]
        public CompareOperator CompareType { get; set; } = CompareOperator.双限_小于等于;

        [MyDisplayName("计算类型"), MyCategory("参数")]
        public CalculatorType CalculatorType { get; set; } = CalculatorType.加法运算;

        [MyDisplayName("是否验证结果"), MyCategory("计算参数")]
        public bool IsVerifyResult { get; set; } = true;

        [MyDisplayName("是否记录详细日志"), MyCategory("计算参数")]
        public bool IsDetailLog { get; set; } = true;

        [MyDisplayName("计算超时时间(ms)"), MyCategory("计算参数")]
        public int CalculationTimeout { get; set; } = 5000;

        public YesNo result = true;

        [MyDisplayName("执行结果"), MyCategory("监视信息")]
        public YesNo Result
        {
            get { return result; }
            set { result = value; }
        }

        // 内部变量
        private double calculatedResult = 0.0;
        private bool isCalculationValid = true;
        private string errorMessage = string.Empty;

        #endregion

        #region 方法重写

        /// <summary>
        /// 复制对象成员
        /// </summary>
        protected override void CloneMembers(YungkuSystem.Script.Core.Base dest)
        {
            base.CloneMembers(dest);
            ActionCalculator obj = dest as ActionCalculator;
            obj.OperandA = this.OperandA;
            obj.OperandB = this.OperandB;
            obj.ExpectedResult = this.ExpectedResult;
            obj.LowLimit = this.LowLimit;
            obj.UpperLimit = this.UpperLimit;
            obj.CompareType = this.CompareType;
            obj.CalculatorType = this.CalculatorType;
            obj.IsVerifyResult = this.IsVerifyResult;
            obj.IsDetailLog = this.IsDetailLog;
            obj.CalculationTimeout = this.CalculationTimeout;
        }

        public override void Binding()
        {
            base.Binding();
        }

        #endregion

        #region 主执行流程
        protected override void Execute()
        {
            base.Execute();

            Station st = Station.Station;
            int stationIndex = st.Turntable.Stations.IndexOf(st);
            string stationName = stationIndex == 0 ? "左工位" : "右工位";

            WriteRecord($"========== [{stationName}] ActionCalculator 启动检查 ==========");
            WriteRecord($"动作名称: {Name}");
            WriteRecord($"HasPass: {HasPass}");
            WriteRecord($"MustExecute: {MustExecute}");

            // 如果没有Pass产品，且不是强制执行，直接结束并跳到下一个动作
            if (!HasPass && !MustExecute)
            {
                WriteRecord($"[{stationName}][跳过] 无Pass产品且非强制执行，跳过本动作");
                WriteRecord($"===============================================");

                //使用 Finish() 结束当前动作，让状态机继续执行下一个动作
                Finish();
                return;
            }
            else if (!HasPass && MustExecute)
            {
                WriteRecord($"[{stationName}][强制执行模式] 即使所有产品Fail也继续执行");
            }
            else
            {
                WriteRecord($"[{stationName}] 存在Pass产品，正常执行");
            }

            // 检查是否为空
            if (IsEmpty)
            {
                WriteRecord($"[{stationName}][跳过] IsEmpty=true");
                Finish();
                return;
            }

            WriteRecord($"[{stationName}] 开始执行计算操作");
            WriteRecord($"===============================================");

            try
            {
                switch (StateIndex)
                {
                    case ACT_STATE_START:
                        #region 开始状态
                        Watcher.StopAllWatch();

                        if (MyApp.NeedReset || MyApp.ShareData.ishoming)
                        {
                            WriteRecord("设备正在复位或回原，跳过计算");
                            To(ACT_STATE_END);
                            break;
                        }

                        WriteRecord($"========================================");
                        WriteRecord($"开始计算器操作：{CalculatorType}");
                        WriteRecord($"操作数A = {OperandA}");
                        WriteRecord($"操作数B = {OperandB}");
                        if (IsVerifyResult)
                        {
                            WriteRecord($"比较方式 = {CompareType}");
                            WriteRecord($"下限值 = {LowLimit}");
                            if (IsUpperLimitEnabled())
                            {
                                WriteRecord($"上限值 = {UpperLimit}");
                            }
                            else
                            {
                                WriteRecord($"上限值 = (不使用)");
                            }
                        }
                        WriteRecord($"========================================");

                        if (!ValidateParameters())
                        {
                            To("参数验证失败");
                            break;
                        }

                        switch (CalculatorType)
                        {
                            case CalculatorType.加法运算:
                                To("执行加法运算");
                                break;
                            case CalculatorType.减法运算:
                                To("执行减法运算");
                                break;
                            case CalculatorType.乘法运算:
                                To("执行乘法运算");
                                break;
                            case CalculatorType.除法运算:
                                To("执行除法运算");
                                break;
                            case CalculatorType.综合运算:
                                To("执行综合运算");
                                break;
                            case CalculatorType.验证结果:
                                To("验证计算结果");
                                break;
                            default:
                                WriteRecord($"[错误] 未知的计算类型");
                                To("计算失败");
                                break;
                        }
                        #endregion
                        break;

                    case "执行加法运算":
                        #region 加法运算
                        if (PerformAddition())
                        {
                            WriteRecord($"加法运算完成: {OperandA} + {OperandB} = {calculatedResult}");
                            Watcher.StopWatch(StateIndex);

                            if (IsVerifyResult)
                            {
                                To("验证计算结果");
                            }
                            else
                            {
                                To("计算成功");
                            }
                        }
                        else if (Watcher.StartCheckIsTimeout(StateIndex, CalculationTimeout))
                        {
                            WriteRecord($"[测试失败] 加法运算超时");
                            To("计算失败");
                        }
                        #endregion
                        break;

                    case "执行减法运算":
                        #region 减法运算
                        if (PerformSubtraction())
                        {
                            WriteRecord($"减法运算完成: {OperandA} - {OperandB} = {calculatedResult}");
                            Watcher.StopWatch(StateIndex);

                            if (IsVerifyResult)
                            {
                                To("验证计算结果");
                            }
                            else
                            {
                                To("计算成功");
                            }
                        }
                        else if (Watcher.StartCheckIsTimeout(StateIndex, CalculationTimeout))
                        {
                            WriteRecord($"[测试失败] 减法运算超时");
                            To("计算失败");
                        }
                        #endregion
                        break;

                    case "执行乘法运算":
                        #region 乘法运算
                        if (PerformMultiplication())
                        {
                            WriteRecord($"乘法运算完成: {OperandA} × {OperandB} = {calculatedResult}");
                            Watcher.StopWatch(StateIndex);

                            if (IsVerifyResult)
                            {
                                To("验证计算结果");
                            }
                            else
                            {
                                To("计算成功");
                            }
                        }
                        else if (Watcher.StartCheckIsTimeout(StateIndex, CalculationTimeout))
                        {
                            WriteRecord($"[测试失败] 乘法运算超时");
                            To("计算失败");
                        }
                        #endregion
                        break;

                    case "执行除法运算":
                        #region 除法运算
                        if (PerformDivision())
                        {
                            WriteRecord($"除法运算完成: {OperandA} ÷ {OperandB} = {calculatedResult}");
                            Watcher.StopWatch(StateIndex);

                            if (IsVerifyResult)
                            {
                                To("验证计算结果");
                            }
                            else
                            {
                                To("计算成功");
                            }
                        }
                        else if (Watcher.StartCheckIsTimeout(StateIndex, CalculationTimeout))
                        {
                            WriteRecord($"[测试失败] 除法运算超时");
                            To("计算失败");
                        }
                        #endregion
                        break;

                    case "执行综合运算":
                        #region 综合运算
                        if (PerformComplexCalculation())
                        {
                            WriteRecord($"综合运算完成: 结果 = {calculatedResult}");

                            Watcher.StopWatch(StateIndex);

                            if (IsVerifyResult)
                            {
                                To("验证计算结果");
                            }
                            else
                            {
                                To("计算成功");
                            }
                        }
                        else if (Watcher.StartCheckIsTimeout(StateIndex, CalculationTimeout))
                        {
                            WriteRecord($"[测试失败] 综合运算超时");
                            To("计算失败");
                        }
                        #endregion
                        break;

                    case "验证计算结果":
                        #region 验证结果
                        bool verifyResult = VerifyResult();

                        if (verifyResult)
                        {
                            WriteRecord($"✓ 结果验证通过: {GetCompareDescription()}");

                            Watcher.StopWatch(StateIndex);
                            To("计算成功");
                        }
                        else
                        {
                            WriteRecord($"✗ 结果验证失败: {GetCompareDescription()}");
                            WriteRecord($"[测试失败] 计算结果验证失败，不触发报警");

                            To("计算失败");
                        }
                        #endregion
                        break;

                    case "参数验证失败":
                        #region 参数验证失败
                        WriteRecord($"参数验证失败: {errorMessage}");

                        if (stationIndex == 0)
                        {
                            TestDisplayHelper.UpdateLeftStation(Name,
                                LowLimit.ToString(),
                                IsUpperLimitEnabled() ? UpperLimit.ToString() : "",
                                "",
                                "FAIL");
                        }
                        else if (stationIndex == 1)
                        {
                            TestDisplayHelper.UpdateRightStation(Name,
                                LowLimit.ToString(),
                                IsUpperLimitEnabled() ? UpperLimit.ToString() : "",
                                "",
                                "FAIL");
                        }

                        WriteRecord($"[测试失败] 参数验证失败: {errorMessage}");

                        SetProductsResult(TestResult.Fail);
                        To(ACT_STATE_END);
                        #endregion
                        break;

                    case "计算失败":
                        #region 计算失败
                        WriteRecord($"计算失败: {errorMessage}");

                        //一次性传递完整的6列数据
                        if (stationIndex == 0)
                        {
                            TestDisplayHelper.UpdateLeftStation(Name,
                                LowLimit.ToString(),
                                IsUpperLimitEnabled() ? UpperLimit.ToString() : "",
                                FormatValue(calculatedResult),
                                "FAIL");
                        }
                        else if (stationIndex == 1)
                        {
                            TestDisplayHelper.UpdateRightStation(Name,
                                LowLimit.ToString(),
                                IsUpperLimitEnabled() ? UpperLimit.ToString() : "",
                                FormatValue(calculatedResult),
                                "FAIL");
                        }

                        WriteRecord($"[测试失败] 计算失败: {errorMessage}");

                        SetProductsResult(TestResult.Fail);
                        To(ACT_STATE_END);
                        #endregion
                        break;

                    case "计算成功":
                        #region 计算成功
                        WriteRecord($"✓ 计算操作成功完成");

                        // ✅ 一次性传递完整的6列数据
                        if (stationIndex == 0)
                        {
                            TestDisplayHelper.UpdateLeftStation(Name,
                                LowLimit.ToString(),
                                IsUpperLimitEnabled() ? UpperLimit.ToString() : "",
                                FormatValue(calculatedResult),
                                "PASS");
                        }
                        else if (stationIndex == 1)
                        {
                            TestDisplayHelper.UpdateRightStation(Name,
                                LowLimit.ToString(),
                                IsUpperLimitEnabled() ? UpperLimit.ToString() : "",
                                FormatValue(calculatedResult),
                                "PASS");
                        }

                        SetProductsResult(TestResult.Pass);
                        To(ACT_STATE_END);
                        #endregion
                        break;

                    case ACT_STATE_END:
                        #region 结束状态
                        WriteRecord($"========================================");
                        WriteRecord($"计算器操作完成");
                        WriteRecord($"最终结果: {calculatedResult}");
                        WriteRecord($"执行状态: {(isCalculationValid ? "成功" : "失败")}");
                        WriteRecord($"========================================");
                        Finish();
                        #endregion
                        break;

                    default:
                        #region 默认处理
                        WriteRecord($"[错误] 计算器Action程序逻辑错误");
                        State = YungkuSystem.Script.Core.ActionState.Error;
                        To(ACT_STATE_END);
                        #endregion
                        break;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                WriteRecord($"[异常] 计算器执行异常: {ex.Message}");
                WriteRecord($"异常堆栈: {ex.StackTrace}");

                // 异常时显示错误
                try
                {
                    if (stationIndex == 0)
                    {
                        TestDisplayHelper.UpdateLeftStation(Name, $"ERROR - {ex.Message}");
                    }
                    else if (stationIndex == 1)
                    {
                        TestDisplayHelper.UpdateRightStation(Name, $"ERROR - {ex.Message}");
                    }
                }
                catch { }
                SetProductsResult(TestResult.Error);
                To(ACT_STATE_END);
            }
        }

        #endregion

        #region 计算方法

        /// <summary>
        /// 参数有效性验证
        /// </summary>
        private bool ValidateParameters()
        {
            try
            {
                // 检查操作数是否为有效数字
                if (double.IsNaN(OperandA) || double.IsInfinity(OperandA))
                {
                    errorMessage = "操作数A不是有效数字";
                    return false;
                }

                if (double.IsNaN(OperandB) || double.IsInfinity(OperandB))
                {
                    errorMessage = "操作数B不是有效数字";
                    return false;
                }

                // 除法运算特殊检查
                if (CalculatorType == CalculatorType.除法运算 && Math.Abs(OperandB) < 1e-10)
                {
                    errorMessage = "除法运算的除数不能为零";
                    return false;
                }

                // 验证限制值
                if (IsVerifyResult)
                {
                    if (double.IsNaN(LowLimit) || double.IsInfinity(LowLimit))
                    {
                        errorMessage = "下限值不是有效数字";
                        return false;
                    }

                    if (IsUpperLimitEnabled())
                    {
                        if (double.IsNaN(UpperLimit) || double.IsInfinity(UpperLimit))
                        {
                            errorMessage = "上限值不是有效数字";
                            return false;
                        }

                        // 对于需要范围的比较方式，检查上下限关系
                        if ((CompareType == CompareOperator.双限_小于等于 ||
                             CompareType == CompareOperator.双限_小于 ||
                             CompareType == CompareOperator.双限_小于大于) &&
                            LowLimit >= UpperLimit)
                        {
                            errorMessage = "下限值必须小于上限值";
                            return false;
                        }
                    }
                }

                if (IsDetailLog)
                {
                    WriteRecord("✓ 参数验证通过");
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"参数验证异常: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// 执行加法运算: A + B
        /// </summary>
        private bool PerformAddition()
        {
            try
            {
                calculatedResult = OperandA + OperandB;
                isCalculationValid = true;

                if (IsDetailLog)
                {
                    WriteRecord($"  步骤1: 读取操作数A = {OperandA}");
                    WriteRecord($"  步骤2: 读取操作数B = {OperandB}");
                    WriteRecord($"  步骤3: 执行加法运算");
                    WriteRecord($"  步骤4: 得到结果 = {calculatedResult}");
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"加法运算异常: {ex.Message}";
                isCalculationValid = false;
                return false;
            }
        }

        /// <summary>
        /// 执行减法运算: A - B
        /// </summary>
        private bool PerformSubtraction()
        {
            try
            {
                calculatedResult = OperandA - OperandB;
                isCalculationValid = true;

                if (IsDetailLog)
                {
                    WriteRecord($"  步骤1: 读取被减数A = {OperandA}");
                    WriteRecord($"  步骤2: 读取减数B = {OperandB}");
                    WriteRecord($"  步骤3: 执行减法运算");
                    WriteRecord($"  步骤4: 得到结果 = {calculatedResult}");
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"减法运算异常: {ex.Message}";
                isCalculationValid = false;
                return false;
            }
        }

        /// <summary>
        /// 执行乘法运算: A × B
        /// </summary>
        private bool PerformMultiplication()
        {
            try
            {
                calculatedResult = OperandA * OperandB;
                isCalculationValid = true;

                if (IsDetailLog)
                {
                    WriteRecord($"  步骤1: 读取乘数A = {OperandA}");
                    WriteRecord($"  步骤2: 读取乘数B = {OperandB}");
                    WriteRecord($"  步骤3: 执行乘法运算");
                    WriteRecord($"  步骤4: 得到结果 = {calculatedResult}");
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"乘法运算异常: {ex.Message}";
                isCalculationValid = false;
                return false;
            }
        }

        /// <summary>
        /// 执行除法运算: A ÷ B
        /// </summary>
        private bool PerformDivision()
        {
            try
            {
                // 再次检查除数
                if (Math.Abs(OperandB) < 1e-10)
                {
                    errorMessage = "除数不能为零";
                    isCalculationValid = false;
                    return false;
                }

                calculatedResult = OperandA / OperandB;
                isCalculationValid = true;

                if (IsDetailLog)
                {
                    WriteRecord($"  步骤1: 读取被除数A = {OperandA}");
                    WriteRecord($"  步骤2: 读取除数B = {OperandB}");
                    WriteRecord($"  步骤3: 检查除数非零");
                    WriteRecord($"  步骤4: 执行除法运算");
                    WriteRecord($"  步骤5: 得到结果 = {calculatedResult}");
                }

                // 检查结果有效性
                if (double.IsNaN(calculatedResult) || double.IsInfinity(calculatedResult))
                {
                    errorMessage = "除法运算结果无效";
                    isCalculationValid = false;
                    return false;
                }

                return true;
            }
            catch (DivideByZeroException)
            {
                errorMessage = "除以零异常";
                isCalculationValid = false;
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"除法运算异常: {ex.Message}";
                isCalculationValid = false;
                return false;
            }
        }

        /// <summary>
        /// 执行综合运算: (A + B) × (A - B) ÷ 2
        /// </summary>
        private bool PerformComplexCalculation()
        {
            try
            {
                if (IsDetailLog)
                {
                    WriteRecord($"  综合运算公式: (A + B) × (A - B) ÷ 2");
                    WriteRecord($"  步骤1: 计算 A + B = {OperandA} + {OperandB}");
                }

                double sum = OperandA + OperandB;
                if (IsDetailLog)
                {
                    WriteRecord($"         结果 = {sum}");
                    WriteRecord($"  步骤2: 计算 A - B = {OperandA} - {OperandB}");
                }

                double diff = OperandA - OperandB;
                if (IsDetailLog)
                {
                    WriteRecord($"         结果 = {diff}");
                    WriteRecord($"  步骤3: 计算 ({sum}) × ({diff})");
                }

                double product = sum * diff;
                if (IsDetailLog)
                {
                    WriteRecord($"         结果 = {product}");
                    WriteRecord($"  步骤4: 计算 ({product}) ÷ 2");
                }

                calculatedResult = product / 2.0;
                isCalculationValid = true;

                if (IsDetailLog)
                {
                    WriteRecord($"         最终结果 = {calculatedResult}");
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"综合运算异常: {ex.Message}";
                isCalculationValid = false;
                return false;
            }
        }

        /// <summary>
        /// 验证计算结果
        /// </summary>
        private bool VerifyResult()
        {
            try
            {
                bool isPass = CompareResult(calculatedResult);

                if (IsDetailLog)
                {
                    WriteRecord($"  验证步骤:");
                    WriteRecord($"  - 计算结果: {calculatedResult}");
                    WriteRecord($"  - 比较方式: {CompareType}");
                    WriteRecord($"  - 下限值: {LowLimit}");

                    if (IsUpperLimitEnabled())
                    {
                        WriteRecord($"  - 上限值: {UpperLimit}");
                    }
                    else
                    {
                        WriteRecord($"  - 上限值: (不使用)");
                    }

                    WriteRecord($"  - 判断表达式: {GetCompareDescription()}");
                    WriteRecord($"  - 验证结果: {(isPass ? "通过" : "失败")}");
                }

                return isPass;
            }
            catch (Exception ex)
            {
                errorMessage = $"结果验证异常: {ex.Message}";
                return false;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 智能格式化数值 - 自动去除尾部无用的零
        /// </summary>
        private string FormatValue(double value)
        {
            // 如果是整数，直接返回整数格式
            if (Math.Abs(value - Math.Round(value)) < 1e-10)
            {
                return value.ToString("F0");
            }

            // 否则保留必要的小数位（最多4位），自动去除尾部的零
            string formatted = value.ToString("0.####");
            return formatted;
        }

        /// <summary>
        /// 判断当前比较方式是否需要使用上限
        /// </summary>
        private bool IsUpperLimitEnabled()
        {
            switch (CompareType)
            {
                case CompareOperator.双限_小于等于:
                case CompareOperator.双限_小于:
                case CompareOperator.双限_大于等于:
                case CompareOperator.双限_大于:
                case CompareOperator.双限_小于大于:
                case CompareOperator.双限_大于小于:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 根据比较运算符验证结果
        /// </summary>
        private bool CompareResult(double value)
        {
            switch (CompareType)
            {
                case CompareOperator.等于:
                    return Math.Abs(value - LowLimit) < 1e-10;

                case CompareOperator.小于:
                    return value < LowLimit;

                case CompareOperator.大于:
                    return value > LowLimit;

                case CompareOperator.小于等于:
                    return value <= LowLimit;

                case CompareOperator.大于等于:
                    return value >= LowLimit;

                case CompareOperator.双限_小于等于:
                    return value >= LowLimit && value <= UpperLimit;

                case CompareOperator.双限_小于:
                    return value > LowLimit && value < UpperLimit;

                case CompareOperator.双限_大于等于:
                    return value <= LowLimit || value >= UpperLimit;

                case CompareOperator.双限_大于:
                    return value < LowLimit || value > UpperLimit;

                case CompareOperator.双限_小于大于:
                    return value > LowLimit && value < UpperLimit;

                case CompareOperator.双限_大于小于:
                    return value < LowLimit || value > UpperLimit;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 获取比较方式的描述文本
        /// </summary>
        private string GetCompareDescription()
        {
            if (IsUpperLimitEnabled())
            {
                string op = GetOperatorSymbol();
                return $"{LowLimit} {op} {calculatedResult} {op} {UpperLimit}";
            }
            else
            {
                return $"{calculatedResult} {GetOperatorSymbol()} {LowLimit}";
            }
        }

        /// <summary>
        /// 获取运算符号
        /// </summary>
        private string GetOperatorSymbol()
        {
            switch (CompareType)
            {
                case CompareOperator.等于:
                    return "=";
                case CompareOperator.小于:
                case CompareOperator.双限_小于:
                case CompareOperator.双限_小于大于:
                    return "<";
                case CompareOperator.大于:
                case CompareOperator.双限_大于:
                case CompareOperator.双限_大于小于:
                    return ">";
                case CompareOperator.小于等于:
                case CompareOperator.双限_小于等于:
                    return "<=";
                case CompareOperator.大于等于:
                case CompareOperator.双限_大于等于:
                    return ">=";
                default:
                    return "?";
            }
        }

        /// <summary>
        /// 设置所有产品的测试结果
        /// </summary>
        private void SetProductsResult(TestResult result)
        {
            try
            {
                // 获取当前站位和对应转盘上的测试头
                Station st = Station.Station;
                Head hd = st.Turntable.GetHeadByStation(st);

                // 站位或测试头有一个被关闭时都不应该继续执行
                if (!st.Enabled || !hd.Enabled)
                    return;

                // 遍历测试头中的治具对象
                foreach (Jig jig in hd.TestItems)
                {
                    if (jig.Enabled)
                    {
                        foreach (Product product in jig.TestItems)
                        {
                            if (product.Enabled)
                            {
                                //只有当前结果是 Fail 或 Error 时才更新
                                // 如果产品已经是 Fail 或 Error，不应该被 Pass 覆盖
                                if (result == TestResult.Fail || result == TestResult.Error)
                                {
                                    product.Result = result;

                                    if (result == TestResult.Fail)
                                    {
                                        // 如果 ResultCode 已经有值，保留第一个失败的错误码
                                        if (string.IsNullOrEmpty(product.ResultCode) || product.ResultCode == "00")
                                        {
                                            product.ResultCode = "CALC_ERROR";
                                        }
                                    }
                                }
                                else if (result == TestResult.Pass)
                                {
                                    //只有当前产品还没有失败时，才设置为 Pass
                                    if (product.Result != TestResult.Fail && product.Result != TestResult.Error)
                                    {
                                        product.Result = TestResult.Pass;
                                        product.ResultCode = "00";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteRecord($"设置产品结果异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取计算器状态摘要
        /// </summary>
        public string GetStatusSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========== 计算器状态摘要 ==========");
            sb.AppendLine($"操作类型: {CalculatorType}");
            sb.AppendLine($"操作数A: {OperandA}");
            sb.AppendLine($"操作数B: {OperandB}");
            sb.AppendLine($"计算结果: {calculatedResult}");
            sb.AppendLine($"比较方式: {CompareType}");
            sb.AppendLine($"下限值: {LowLimit}");

            if (IsUpperLimitEnabled())
            {
                sb.AppendLine($"上限值: {UpperLimit}");
            }
            else
            {
                sb.AppendLine($"上限值: (不使用)");
            }

            sb.AppendLine($"判断表达式: {GetCompareDescription()}");
            sb.AppendLine($"结果有效: {isCalculationValid}");
            sb.AppendLine($"错误信息: {(string.IsNullOrEmpty(errorMessage) ? "无" : errorMessage)}");
            sb.AppendLine("===================================");
            return sb.ToString();
        }

        #endregion
    }
}