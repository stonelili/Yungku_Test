using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence
{
    #region 枚举定义

    /// <summary>
    /// 测试步骤类型
    /// </summary>
    public enum StepType
    {
        /// <summary>
        /// 数值测试 - 返回数值并与限值比较
        /// </summary>
        NumericTest,

        /// <summary>
        /// 字符串测试 - 返回字符串并匹配
        /// </summary>
        StringTest,

        /// <summary>
        /// 逻辑判断 - 返回布尔值
        /// </summary>
        PassFail,

        /// <summary>
        /// 动作执行 - 无返回值的操作
        /// </summary>
        Action,

        /// <summary>
        /// 多值测试 - 返回多个数值
        /// </summary>
        MultiNumericTest,

        /// <summary>
        /// 回调测试 - 需要回调确认结果
        /// </summary>
        CallbackAction,

        /// <summary>
        /// 序列组 - 包含子步骤的组
        /// </summary>
        SequenceGroup
    }

    /// <summary>
    /// 测试步骤结果
    /// </summary>
    public enum StepResult
    {
        /// <summary>
        /// 未执行
        /// </summary>
        NotExecuted,

        /// <summary>
        /// 通过
        /// </summary>
        Pass,

        /// <summary>
        /// 失败
        /// </summary>
        Fail,

        /// <summary>
        /// 跳过
        /// </summary>
        Skipped,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 超时
        /// </summary>
        Timeout,

        /// <summary>
        /// 执行中
        /// </summary>
        Running
    }

    /// <summary>
    /// 失败处理策略
    /// </summary>
    public enum FailAction
    {
        /// <summary>
        /// 继续执行
        /// </summary>
        Continue,

        /// <summary>
        /// 中断序列
        /// </summary>
        Abort,

        /// <summary>
        /// 跳转到指定步骤
        /// </summary>
        GotoStep,

        /// <summary>
        /// 重试当前步骤
        /// </summary>
        Retry,

        /// <summary>
        /// 跳转到后置条件
        /// </summary>
        GotoPostCondition
    }

    /// <summary>
    /// 序列执行状态
    /// </summary>
    public enum SequenceState
    {
        /// <summary>
        /// 未执行
        /// </summary>
        NotExecuted,

        /// <summary>
        /// 执行中
        /// </summary>
        Running,

        /// <summary>
        /// 已完成（通过）
        /// </summary>
        Completed,

        /// <summary>
        /// 已完成（失败）
        /// </summary>
        Failed,

        /// <summary>
        /// 已中断
        /// </summary>
        Aborted,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused
    }

    #endregion

    /// <summary>
    /// 测试步骤参数
    /// </summary>
    [Serializable]
    public class StepParameter
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        [XmlAttribute]
        public string Type { get; set; } = "string";

        /// <summary>
        /// 参数值
        /// </summary>
        [XmlAttribute]
        public string Value { get; set; }

        /// <summary>
        /// 参数描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        public StepParameter()
        {
        }

        public StepParameter(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// 获取转换后的参数值
        /// </summary>
        public object GetTypedValue()
        {
            if (string.IsNullOrEmpty(Value))
                return null;

            switch (Type.ToLower())
            {
                case "int":
                case "int32":
                    return int.Parse(Value);
                case "long":
                case "int64":
                    return long.Parse(Value);
                case "double":
                    return double.Parse(Value);
                case "float":
                case "single":
                    return float.Parse(Value);
                case "bool":
                case "boolean":
                    return bool.Parse(Value);
                case "decimal":
                    return decimal.Parse(Value);
                case "string":
                default:
                    return Value;
            }
        }

        public override string ToString()
        {
            return $"{Name}({Type})={Value}";
        }
    }

    /// <summary>
    /// 序列级变量定义 - 类似TestStand的序列变量
    /// 可以在配置中定义变量，并在步骤参数中通过${变量名}引用
    /// </summary>
    [Serializable]
    public class SequenceVariable
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 变量类型 (string, int, double, bool, etc.)
        /// </summary>
        [XmlAttribute]
        public string Type { get; set; } = "string";

        /// <summary>
        /// 默认值
        /// </summary>
        [XmlAttribute]
        public string DefaultValue { get; set; }

        /// <summary>
        /// 变量描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 变量作用域 (Sequence=序列级, Step=步骤级, Global=全局)
        /// </summary>
        [XmlAttribute]
        public string Scope { get; set; } = "Sequence";

        /// <summary>
        /// 当前值（运行时）
        /// </summary>
        [XmlIgnore]
        public object CurrentValue { get; set; }

        public SequenceVariable()
        {
        }

        public SequenceVariable(string name, string type, string defaultValue = null)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// 获取转换后的默认值
        /// </summary>
        public object GetTypedDefaultValue()
        {
            if (string.IsNullOrEmpty(DefaultValue))
                return GetDefaultForType();

            return ConvertToType(DefaultValue);
        }

        /// <summary>
        /// 将字符串值转换为变量类型
        /// </summary>
        public object ConvertToType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return GetDefaultForType();

            switch (Type.ToLower())
            {
                case "int":
                case "int32":
                    return int.Parse(value);
                case "long":
                case "int64":
                    return long.Parse(value);
                case "double":
                    return double.Parse(value);
                case "float":
                case "single":
                    return float.Parse(value);
                case "bool":
                case "boolean":
                    return bool.Parse(value);
                case "decimal":
                    return decimal.Parse(value);
                case "string":
                default:
                    return value;
            }
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultForType()
        {
            switch (Type.ToLower())
            {
                case "int":
                case "int32":
                    return 0;
                case "long":
                case "int64":
                    return 0L;
                case "double":
                    return 0.0;
                case "float":
                case "single":
                    return 0f;
                case "bool":
                case "boolean":
                    return false;
                case "decimal":
                    return 0m;
                case "string":
                default:
                    return "";
            }
        }

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void Reset()
        {
            CurrentValue = GetTypedDefaultValue();
        }

        public override string ToString()
        {
            return $"{Name}({Type})={CurrentValue ?? DefaultValue}";
        }
    }

    /// <summary>
    /// 目标方法信息
    /// </summary>
    [Serializable]
    public class TargetMethodInfo
    {
        /// <summary>
        /// 目标类名
        /// </summary>
        [XmlAttribute("Class")]
        public string ClassName { get; set; }

        /// <summary>
        /// 目标方法名
        /// </summary>
        [XmlAttribute("Method")]
        public string MethodName { get; set; }

        /// <summary>
        /// 完整的类型名称（包含命名空间）
        /// </summary>
        [XmlAttribute]
        public string FullTypeName { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        [XmlAttribute]
        public string AssemblyName { get; set; }

        public TargetMethodInfo()
        {
        }

        public TargetMethodInfo(string className, string methodName)
        {
            ClassName = className;
            MethodName = methodName;
        }

        public override string ToString()
        {
            return $"{ClassName}.{MethodName}";
        }
    }

    /// <summary>
    /// 测试限值
    /// </summary>
    [Serializable]
    public class TestLimits
    {
        /// <summary>
        /// 下限值
        /// </summary>
        [XmlAttribute]
        public double Lower { get; set; } = double.MinValue;

        /// <summary>
        /// 上限值
        /// </summary>
        [XmlAttribute]
        public double Upper { get; set; } = double.MaxValue;

        /// <summary>
        /// 单位
        /// </summary>
        [XmlAttribute]
        public string Unit { get; set; } = "";

        /// <summary>
        /// 精度（小数位数）
        /// </summary>
        [XmlAttribute]
        public int Precision { get; set; } = 3;

        /// <summary>
        /// 比较模式 (GE_LE: >=下限且<=上限, GT_LT: >下限且<上限)
        /// </summary>
        [XmlAttribute]
        public string CompareMode { get; set; } = "GE_LE";

        public TestLimits()
        {
        }

        public TestLimits(double lower, double upper, string unit = "")
        {
            Lower = lower;
            Upper = upper;
            Unit = unit;
        }

        /// <summary>
        /// 检查值是否在限值范围内
        /// </summary>
        public bool IsWithinLimits(double value)
        {
            switch (CompareMode.ToUpper())
            {
                case "GT_LT":
                    return value > Lower && value < Upper;
                case "GE_LT":
                    return value >= Lower && value < Upper;
                case "GT_LE":
                    return value > Lower && value <= Upper;
                case "GE_LE":
                default:
                    return value >= Lower && value <= Upper;
            }
        }

        public override string ToString()
        {
            return $"[{Lower:F3} ~ {Upper:F3}] {Unit}";
        }
    }

    /// <summary>
    /// 测试步骤基类
    /// </summary>
    [Serializable]
    public class TestStep
    {
        #region 基本属性

        /// <summary>
        /// 步骤ID
        /// </summary>
        [XmlAttribute]
        public string ID { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 步骤描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 步骤类型
        /// </summary>
        [XmlAttribute]
        public StepType Type { get; set; } = StepType.Action;

        /// <summary>
        /// 是否启用
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>
        [XmlAttribute]
        public int Timeout { get; set; } = 30000;

        /// <summary>
        /// 重试次数
        /// </summary>
        [XmlAttribute]
        public int RetryCount { get; set; } = 0;

        #endregion

        #region 目标方法

        /// <summary>
        /// 目标方法信息
        /// </summary>
        [XmlElement("TargetMethod")]
        public TargetMethodInfo TargetMethod { get; set; } = new TargetMethodInfo();

        /// <summary>
        /// 参数列表
        /// </summary>
        [XmlArray("Parameters")]
        [XmlArrayItem("Param")]
        public List<StepParameter> Parameters { get; set; } = new List<StepParameter>();

        #endregion

        #region 结果存储

        /// <summary>
        /// 结果存储到变量名 - 将步骤执行结果存储到指定的序列变量中
        /// 类似TestStand的ResultVariable功能
        /// </summary>
        [XmlAttribute]
        public string ResultVariable { get; set; }

        #endregion

        #region 测试限值

        /// <summary>
        /// 测试限值
        /// </summary>
        [XmlElement("Limits")]
        public TestLimits Limits { get; set; }

        /// <summary>
        /// 期望的字符串值（用于字符串测试）
        /// </summary>
        [XmlAttribute]
        public string ExpectedString { get; set; }

        #endregion

        #region 失败处理

        /// <summary>
        /// 失败时的处理策略
        /// </summary>
        [XmlAttribute]
        public FailAction OnFail { get; set; } = FailAction.Continue;

        /// <summary>
        /// 跳转目标步骤ID（当OnFail为GotoStep时使用）
        /// </summary>
        [XmlAttribute]
        public string GotoStepID { get; set; }

        #endregion

        #region 执行结果（运行时状态，不序列化）

        /// <summary>
        /// 步骤执行结果
        /// </summary>
        [XmlIgnore]
        public StepResult Result { get; set; } = StepResult.NotExecuted;

        /// <summary>
        /// 实际测量值
        /// </summary>
        [XmlIgnore]
        public object ActualValue { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        [XmlIgnore]
        public string ResultMessage { get; set; }

        /// <summary>
        /// 执行开始时间
        /// </summary>
        [XmlIgnore]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 执行结束时间
        /// </summary>
        [XmlIgnore]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 执行耗时（毫秒）
        /// </summary>
        [XmlIgnore]
        public double ExecutionTime => (EndTime - StartTime).TotalMilliseconds;

        /// <summary>
        /// 实际重试次数
        /// </summary>
        [XmlIgnore]
        public int ActualRetryCount { get; set; }

        #endregion

        #region 子步骤（用于SequenceGroup类型）

        /// <summary>
        /// 子步骤列表
        /// </summary>
        [XmlArray("SubSteps")]
        [XmlArrayItem("Step")]
        public List<TestStep> SubSteps { get; set; }

        #endregion

        public TestStep()
        {
            ID = Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public TestStep(string id, string name, StepType type)
        {
            ID = id;
            Name = name;
            Type = type;
        }

        /// <summary>
        /// 重置执行状态
        /// </summary>
        public void Reset()
        {
            Result = StepResult.NotExecuted;
            ActualValue = null;
            ResultMessage = null;
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            ActualRetryCount = 0;

            if (SubSteps != null)
            {
                foreach (var subStep in SubSteps)
                {
                    subStep.Reset();
                }
            }
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        public void AddParameter(string name, string type, string value)
        {
            Parameters.Add(new StepParameter(name, type, value));
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object GetParameterValue(string name)
        {
            var param = Parameters.Find(p => p.Name == name);
            return param?.GetTypedValue();
        }

        public override string ToString()
        {
            return $"[{ID}] {Name} ({Type}) - {Result}";
        }
    }
}
