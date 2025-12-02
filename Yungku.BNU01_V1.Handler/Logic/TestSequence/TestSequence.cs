using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence
{
    /// <summary>
    /// 测试序列基类
    /// </summary>
    [Serializable]
    [XmlRoot("Sequence")]
    public class TestSequence
    {
        #region 基本属性

        /// <summary>
        /// 序列ID
        /// </summary>
        [XmlAttribute]
        public string ID { get; set; }

        /// <summary>
        /// 序列名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 序列描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 版本号
        /// </summary>
        [XmlAttribute]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 作者
        /// </summary>
        [XmlAttribute]
        public string Author { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlAttribute]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [XmlAttribute]
        public DateTime ModifiedTime { get; set; } = DateTime.Now;

        #endregion

        #region 条件表达式

        /// <summary>
        /// 前置条件表达式
        /// </summary>
        [XmlElement]
        public string PreCondition { get; set; }

        /// <summary>
        /// 后置条件表达式
        /// </summary>
        [XmlElement]
        public string PostCondition { get; set; }

        #endregion

        #region 步骤列表

        /// <summary>
        /// 测试步骤列表
        /// </summary>
        [XmlArray("Steps")]
        [XmlArrayItem("Step")]
        public List<TestStep> Steps { get; set; } = new List<TestStep>();

        #endregion

        #region 运行时状态（不序列化）

        /// <summary>
        /// 序列执行状态
        /// </summary>
        [XmlIgnore]
        public SequenceState State { get; set; } = SequenceState.NotExecuted;

        /// <summary>
        /// 当前执行的步骤索引
        /// </summary>
        [XmlIgnore]
        public int CurrentStepIndex { get; set; } = -1;

        /// <summary>
        /// 当前执行的步骤
        /// </summary>
        [XmlIgnore]
        public TestStep CurrentStep
        {
            get
            {
                if (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Count)
                    return Steps[CurrentStepIndex];
                return null;
            }
        }

        /// <summary>
        /// 序列开始时间
        /// </summary>
        [XmlIgnore]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 序列结束时间
        /// </summary>
        [XmlIgnore]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 执行耗时（毫秒）
        /// </summary>
        [XmlIgnore]
        public double ExecutionTime => (EndTime - StartTime).TotalMilliseconds;

        /// <summary>
        /// 执行错误信息
        /// </summary>
        [XmlIgnore]
        public string ErrorMessage { get; set; }

        #endregion

        #region 统计信息（不序列化）

        /// <summary>
        /// 通过的步骤数量
        /// </summary>
        [XmlIgnore]
        public int PassCount => Steps.Count(s => s.Result == StepResult.Pass);

        /// <summary>
        /// 失败的步骤数量
        /// </summary>
        [XmlIgnore]
        public int FailCount => Steps.Count(s => s.Result == StepResult.Fail);

        /// <summary>
        /// 跳过的步骤数量
        /// </summary>
        [XmlIgnore]
        public int SkipCount => Steps.Count(s => s.Result == StepResult.Skipped);

        /// <summary>
        /// 错误的步骤数量
        /// </summary>
        [XmlIgnore]
        public int ErrorCount => Steps.Count(s => s.Result == StepResult.Error);

        /// <summary>
        /// 总体结果是否通过
        /// </summary>
        [XmlIgnore]
        public bool OverallPass => State == SequenceState.Completed && FailCount == 0 && ErrorCount == 0;

        #endregion

        public TestSequence()
        {
            ID = "SEQ_" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        public TestSequence(string id, string name)
        {
            ID = id;
            Name = name;
        }

        #region 步骤管理方法

        /// <summary>
        /// 添加步骤
        /// </summary>
        public void AddStep(TestStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            Steps.Add(step);
            ModifiedTime = DateTime.Now;
        }

        /// <summary>
        /// 插入步骤到指定位置
        /// </summary>
        public void InsertStep(int index, TestStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            if (index < 0 || index > Steps.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            Steps.Insert(index, step);
            ModifiedTime = DateTime.Now;
        }

        /// <summary>
        /// 删除步骤
        /// </summary>
        public bool RemoveStep(TestStep step)
        {
            var result = Steps.Remove(step);
            if (result)
                ModifiedTime = DateTime.Now;
            return result;
        }

        /// <summary>
        /// 根据ID删除步骤
        /// </summary>
        public bool RemoveStepById(string stepId)
        {
            var step = Steps.FirstOrDefault(s => s.ID == stepId);
            if (step != null)
            {
                return RemoveStep(step);
            }
            return false;
        }

        /// <summary>
        /// 移动步骤位置
        /// </summary>
        public void MoveStep(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= Steps.Count)
                throw new ArgumentOutOfRangeException(nameof(fromIndex));

            if (toIndex < 0 || toIndex >= Steps.Count)
                throw new ArgumentOutOfRangeException(nameof(toIndex));

            var step = Steps[fromIndex];
            Steps.RemoveAt(fromIndex);
            Steps.Insert(toIndex, step);
            ModifiedTime = DateTime.Now;
        }

        /// <summary>
        /// 根据ID获取步骤
        /// </summary>
        public TestStep GetStepById(string stepId)
        {
            return Steps.FirstOrDefault(s => s.ID == stepId);
        }

        /// <summary>
        /// 根据名称获取步骤
        /// </summary>
        public TestStep GetStepByName(string name)
        {
            return Steps.FirstOrDefault(s => s.Name == name);
        }

        /// <summary>
        /// 获取步骤索引
        /// </summary>
        public int GetStepIndex(TestStep step)
        {
            return Steps.IndexOf(step);
        }

        /// <summary>
        /// 获取步骤索引（根据ID）
        /// </summary>
        public int GetStepIndexById(string stepId)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].ID == stepId)
                    return i;
            }
            return -1;
        }

        #endregion

        #region 状态管理方法

        /// <summary>
        /// 重置序列状态
        /// </summary>
        public void Reset()
        {
            State = SequenceState.NotExecuted;
            CurrentStepIndex = -1;
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            ErrorMessage = null;

            foreach (var step in Steps)
            {
                step.Reset();
            }
        }

        /// <summary>
        /// 获取所有启用的步骤
        /// </summary>
        public IEnumerable<TestStep> GetEnabledSteps()
        {
            return Steps.Where(s => s.Enabled);
        }

        /// <summary>
        /// 获取下一个要执行的步骤
        /// </summary>
        public TestStep GetNextStep()
        {
            for (int i = CurrentStepIndex + 1; i < Steps.Count; i++)
            {
                if (Steps[i].Enabled)
                {
                    CurrentStepIndex = i;
                    return Steps[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 克隆序列
        /// </summary>
        public TestSequence Clone()
        {
            var clone = new TestSequence
            {
                ID = this.ID + "_Clone",
                Name = this.Name + " (Copy)",
                Description = this.Description,
                Enabled = this.Enabled,
                Version = this.Version,
                Author = this.Author,
                PreCondition = this.PreCondition,
                PostCondition = this.PostCondition,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now
            };

            // 深拷贝步骤需要使用序列化，这里简化处理
            foreach (var step in Steps)
            {
                clone.Steps.Add(new TestStep
                {
                    ID = step.ID + "_Clone",
                    Name = step.Name,
                    Description = step.Description,
                    Type = step.Type,
                    Enabled = step.Enabled,
                    Timeout = step.Timeout,
                    RetryCount = step.RetryCount,
                    TargetMethod = new TargetMethodInfo
                    {
                        ClassName = step.TargetMethod?.ClassName,
                        MethodName = step.TargetMethod?.MethodName,
                        FullTypeName = step.TargetMethod?.FullTypeName,
                        AssemblyName = step.TargetMethod?.AssemblyName
                    },
                    Limits = step.Limits != null ? new TestLimits
                    {
                        Lower = step.Limits.Lower,
                        Upper = step.Limits.Upper,
                        Unit = step.Limits.Unit,
                        Precision = step.Limits.Precision,
                        CompareMode = step.Limits.CompareMode
                    } : null,
                    ExpectedString = step.ExpectedString,
                    OnFail = step.OnFail,
                    GotoStepID = step.GotoStepID,
                    Parameters = step.Parameters.Select(p => new StepParameter
                    {
                        Name = p.Name,
                        Type = p.Type,
                        Value = p.Value,
                        Description = p.Description
                    }).ToList()
                });
            }

            return clone;
        }

        #endregion

        public override string ToString()
        {
            return $"[{ID}] {Name} ({Steps.Count} steps) - {State}";
        }
    }

    /// <summary>
    /// 测试序列配置（根元素）
    /// </summary>
    [Serializable]
    [XmlRoot("TestSequenceConfig")]
    public class TestSequenceConfig
    {
        /// <summary>
        /// 配置版本
        /// </summary>
        [XmlAttribute]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 配置名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlAttribute]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 序列列表
        /// </summary>
        [XmlElement("Sequence")]
        public List<TestSequence> Sequences { get; set; } = new List<TestSequence>();

        /// <summary>
        /// 获取序列
        /// </summary>
        public TestSequence GetSequence(string id)
        {
            return Sequences.FirstOrDefault(s => s.ID == id);
        }

        /// <summary>
        /// 获取启用的序列
        /// </summary>
        public IEnumerable<TestSequence> GetEnabledSequences()
        {
            return Sequences.Where(s => s.Enabled);
        }
    }
}
