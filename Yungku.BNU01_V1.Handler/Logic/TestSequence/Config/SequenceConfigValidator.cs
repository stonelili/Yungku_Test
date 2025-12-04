using System;
using System.Collections.Generic;
using System.Linq;
using Yungku.BNU01_V1.Handler.Logic.TestSequence;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence.Config
{
    /// <summary>
    /// 序列配置验证报告
    /// </summary>
    public class ConfigValidationReport
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// 错误列表
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// 警告列表
        /// </summary>
        public List<string> Warnings { get; set; } = new List<string>();

        /// <summary>
        /// 建议列表
        /// </summary>
        public List<string> Suggestions { get; set; } = new List<string>();

        /// <summary>
        /// 添加错误
        /// </summary>
        public void AddError(string message)
        {
            Errors.Add(message);
            IsValid = false;
        }

        /// <summary>
        /// 添加警告
        /// </summary>
        public void AddWarning(string message)
        {
            Warnings.Add(message);
        }

        /// <summary>
        /// 添加建议
        /// </summary>
        public void AddSuggestion(string message)
        {
            Suggestions.Add(message);
        }

        /// <summary>
        /// 获取摘要
        /// </summary>
        public string GetSummary()
        {
            var lines = new List<string>();
            lines.Add($"验证结果: {(IsValid ? "通过" : "失败")}");
            lines.Add($"错误: {Errors.Count}, 警告: {Warnings.Count}, 建议: {Suggestions.Count}");

            if (Errors.Count > 0)
            {
                lines.Add("\n错误:");
                foreach (var error in Errors)
                {
                    lines.Add($"  ✗ {error}");
                }
            }

            if (Warnings.Count > 0)
            {
                lines.Add("\n警告:");
                foreach (var warning in Warnings)
                {
                    lines.Add($"  ⚠ {warning}");
                }
            }

            if (Suggestions.Count > 0)
            {
                lines.Add("\n建议:");
                foreach (var suggestion in Suggestions)
                {
                    lines.Add($"  ℹ {suggestion}");
                }
            }

            return string.Join("\n", lines);
        }
    }

    /// <summary>
    /// 序列配置验证器
    /// 提供全面的配置验证功能
    /// </summary>
    public class SequenceConfigValidator
    {
        private readonly TestMethodRegistry _registry;

        public SequenceConfigValidator()
        {
            _registry = TestMethodRegistry.Instance;
            if (!_registry.IsInitialized)
            {
                _registry.Initialize();
            }
        }

        /// <summary>
        /// 验证完整配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>验证报告</returns>
        public ConfigValidationReport ValidateConfig(TestSequenceConfig config)
        {
            var report = new ConfigValidationReport();

            if (config == null)
            {
                report.AddError("配置对象为空");
                return report;
            }

            // 验证配置基本属性
            if (string.IsNullOrEmpty(config.Name))
            {
                report.AddWarning("配置名称为空，建议添加配置名称");
            }

            if (string.IsNullOrEmpty(config.Version))
            {
                report.AddWarning("配置版本为空，建议添加版本号");
            }

            // 验证序列列表
            if (config.Sequences == null || config.Sequences.Count == 0)
            {
                report.AddError("配置中没有定义任何序列");
                return report;
            }

            // 检查序列ID唯一性
            var duplicateIds = config.Sequences
                .GroupBy(s => s.ID)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var duplicateId in duplicateIds)
            {
                report.AddError($"序列ID重复: '{duplicateId}'");
            }

            // 验证每个序列
            foreach (var sequence in config.Sequences)
            {
                ValidateSequence(sequence, report);
            }

            // 添加最佳实践建议
            AddBestPracticeSuggestions(config, report);

            return report;
        }

        /// <summary>
        /// 验证单个序列
        /// </summary>
        private void ValidateSequence(TestSequence sequence, ConfigValidationReport report)
        {
            string seqPrefix = $"序列 '{sequence.Name ?? sequence.ID}'";

            // 基本验证
            if (string.IsNullOrEmpty(sequence.ID))
            {
                report.AddError($"{seqPrefix}: 缺少序列ID");
            }

            if (string.IsNullOrEmpty(sequence.Name))
            {
                report.AddWarning($"{seqPrefix}: 缺少序列名称");
            }

            if (!sequence.Enabled)
            {
                report.AddWarning($"{seqPrefix}: 序列已禁用");
            }

            // 步骤验证
            if (sequence.Steps == null || sequence.Steps.Count == 0)
            {
                report.AddWarning($"{seqPrefix}: 没有定义任何步骤");
                return;
            }

            // 检查步骤ID唯一性
            var duplicateStepIds = sequence.Steps
                .GroupBy(s => s.ID)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var duplicateId in duplicateStepIds)
            {
                report.AddError($"{seqPrefix}: 步骤ID重复: '{duplicateId}'");
            }

            // 验证每个步骤
            foreach (var step in sequence.Steps)
            {
                ValidateStep(step, sequence, report, seqPrefix);
            }

            // 验证变量
            ValidateVariables(sequence, report, seqPrefix);

            // 验证跳转目标
            ValidateGotoTargets(sequence, report, seqPrefix);
        }

        /// <summary>
        /// 验证单个步骤
        /// </summary>
        private void ValidateStep(TestStep step, TestSequence sequence, ConfigValidationReport report, string seqPrefix)
        {
            string stepPrefix = $"{seqPrefix} → 步骤 '{step.Name ?? step.ID}'";

            // 基本验证
            if (string.IsNullOrEmpty(step.ID))
            {
                report.AddError($"{stepPrefix}: 缺少步骤ID");
            }

            if (string.IsNullOrEmpty(step.Name))
            {
                report.AddWarning($"{stepPrefix}: 缺少步骤名称");
            }

            if (!step.Enabled)
            {
                report.AddWarning($"{stepPrefix}: 步骤已禁用");
            }

            // 超时验证
            if (step.Timeout <= 0)
            {
                report.AddWarning($"{stepPrefix}: 超时时间未设置或为0，将使用默认值");
            }
            else if (step.Timeout < 100)
            {
                report.AddWarning($"{stepPrefix}: 超时时间过短 ({step.Timeout}ms)，可能导致测试失败");
            }
            else if (step.Timeout > 300000)
            {
                report.AddWarning($"{stepPrefix}: 超时时间过长 ({step.Timeout}ms)，可能影响测试效率");
            }

            // 根据步骤类型验证
            switch (step.Type)
            {
                case StepType.NumericTest:
                    ValidateNumericTestStep(step, report, stepPrefix);
                    break;

                case StepType.StringTest:
                    ValidateStringTestStep(step, report, stepPrefix);
                    break;

                case StepType.ForLoop:
                    ValidateForLoopStep(step, report, stepPrefix);
                    break;

                case StepType.WhileLoop:
                    ValidateWhileLoopStep(step, report, stepPrefix);
                    break;

                case StepType.ForEachLoop:
                    ValidateForEachLoopStep(step, report, stepPrefix);
                    break;

                case StepType.ConditionalBranch:
                    ValidateConditionalBranchStep(step, report, stepPrefix);
                    break;

                case StepType.SubSequenceCall:
                    ValidateSubSequenceCallStep(step, report, stepPrefix);
                    break;
            }

            // 验证目标方法（对于非控制流步骤）
            if (step.Type != StepType.ForLoop && step.Type != StepType.WhileLoop &&
                step.Type != StepType.ForEachLoop && step.Type != StepType.ConditionalBranch &&
                step.Type != StepType.SequenceGroup && step.Type != StepType.SubSequenceCall)
            {
                ValidateTargetMethod(step, report, stepPrefix);
            }

            // 验证子步骤
            if (step.SubSteps != null && step.SubSteps.Count > 0)
            {
                foreach (var subStep in step.SubSteps)
                {
                    ValidateStep(subStep, sequence, report, stepPrefix);
                }
            }

            // 验证条件分支步骤
            if (step.TrueSteps != null)
            {
                foreach (var trueStep in step.TrueSteps)
                {
                    ValidateStep(trueStep, sequence, report, $"{stepPrefix}[True]");
                }
            }

            if (step.FalseSteps != null)
            {
                foreach (var falseStep in step.FalseSteps)
                {
                    ValidateStep(falseStep, sequence, report, $"{stepPrefix}[False]");
                }
            }
        }

        /// <summary>
        /// 验证数值测试步骤
        /// </summary>
        private void ValidateNumericTestStep(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            if (step.Limits == null)
            {
                report.AddWarning($"{stepPrefix}: 数值测试未设置限值，将无法判断Pass/Fail");
            }
            else
            {
                if (step.Limits.Lower > step.Limits.Upper)
                {
                    report.AddError($"{stepPrefix}: 限值下限({step.Limits.Lower})大于上限({step.Limits.Upper})");
                }

                if (step.Limits.Lower == step.Limits.Upper)
                {
                    report.AddWarning($"{stepPrefix}: 限值上下限相等，容差为0");
                }

                if (string.IsNullOrEmpty(step.Limits.Unit))
                {
                    report.AddSuggestion($"{stepPrefix}: 建议为数值测试添加单位");
                }
            }
        }

        /// <summary>
        /// 验证字符串测试步骤
        /// </summary>
        private void ValidateStringTestStep(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            // 字符串测试可以不设置期望值（只要读取成功就算通过）
            if (!string.IsNullOrEmpty(step.ExpectedString))
            {
                report.AddSuggestion($"{stepPrefix}: 已设置期望字符串，将进行精确匹配");
            }
        }

        /// <summary>
        /// 验证For循环步骤
        /// </summary>
        private void ValidateForLoopStep(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            if (step.SubSteps == null || step.SubSteps.Count == 0)
            {
                report.AddWarning($"{stepPrefix}: For循环没有子步骤");
            }

            if (step.LoopStep == 0)
            {
                report.AddError($"{stepPrefix}: For循环步进值为0，将导致死循环");
            }

            // 检查循环次数是否合理
            int iterations = 0;
            if (step.LoopStep > 0)
            {
                iterations = (step.LoopEnd - step.LoopStart) / step.LoopStep + 1;
            }
            else if (step.LoopStep < 0)
            {
                iterations = (step.LoopStart - step.LoopEnd) / (-step.LoopStep) + 1;
            }

            if (iterations > 1000)
            {
                report.AddWarning($"{stepPrefix}: For循环迭代次数较多({iterations}次)，可能影响性能");
            }

            if (string.IsNullOrEmpty(step.LoopVariable))
            {
                report.AddSuggestion($"{stepPrefix}: 建议设置循环变量名称，便于在子步骤中引用");
            }
        }

        /// <summary>
        /// 验证While循环步骤
        /// </summary>
        private void ValidateWhileLoopStep(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            if (step.SubSteps == null || step.SubSteps.Count == 0)
            {
                report.AddWarning($"{stepPrefix}: While循环没有子步骤");
            }

            if (string.IsNullOrEmpty(step.WhileCondition))
            {
                report.AddError($"{stepPrefix}: While循环缺少条件表达式");
            }

            if (step.MaxIterations <= 0)
            {
                report.AddWarning($"{stepPrefix}: While循环未设置最大迭代次数，将使用默认值");
            }
            else if (step.MaxIterations > 10000)
            {
                report.AddWarning($"{stepPrefix}: While循环最大迭代次数较大({step.MaxIterations})");
            }
        }

        /// <summary>
        /// 验证ForEach循环步骤
        /// </summary>
        private void ValidateForEachLoopStep(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            if (step.SubSteps == null || step.SubSteps.Count == 0)
            {
                report.AddWarning($"{stepPrefix}: ForEach循环没有子步骤");
            }

            if (string.IsNullOrEmpty(step.ArrayVariable))
            {
                report.AddError($"{stepPrefix}: ForEach循环缺少数组变量名");
            }

            if (string.IsNullOrEmpty(step.ElementVariable))
            {
                report.AddSuggestion($"{stepPrefix}: 建议设置元素变量名称，便于在子步骤中引用");
            }
        }

        /// <summary>
        /// 验证条件分支步骤
        /// </summary>
        private void ValidateConditionalBranchStep(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            if (string.IsNullOrEmpty(step.BranchCondition))
            {
                report.AddError($"{stepPrefix}: 条件分支缺少条件表达式");
            }

            if ((step.TrueSteps == null || step.TrueSteps.Count == 0) &&
                (step.FalseSteps == null || step.FalseSteps.Count == 0))
            {
                report.AddWarning($"{stepPrefix}: 条件分支没有定义任何分支步骤");
            }
        }

        /// <summary>
        /// 验证子序列调用步骤
        /// </summary>
        private void ValidateSubSequenceCallStep(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            if (string.IsNullOrEmpty(step.SubSequenceId) && string.IsNullOrEmpty(step.SubSequenceFile))
            {
                report.AddError($"{stepPrefix}: 子序列调用缺少序列ID或文件路径");
            }
        }

        /// <summary>
        /// 验证目标方法
        /// </summary>
        private void ValidateTargetMethod(TestStep step, ConfigValidationReport report, string stepPrefix)
        {
            if (step.TargetMethod == null)
            {
                // Action类型可以没有目标方法
                if (step.Type != StepType.Action)
                {
                    report.AddWarning($"{stepPrefix}: 未配置目标方法");
                }
                return;
            }

            if (string.IsNullOrEmpty(step.TargetMethod.ClassName))
            {
                report.AddError($"{stepPrefix}: 目标方法缺少类名");
                return;
            }

            if (string.IsNullOrEmpty(step.TargetMethod.MethodName))
            {
                report.AddError($"{stepPrefix}: 目标方法缺少方法名");
                return;
            }

            // 验证方法是否在注册表中
            var registeredMethod = _registry.GetMethod(step.TargetMethod.ClassName, step.TargetMethod.MethodName);
            if (registeredMethod == null)
            {
                report.AddWarning($"{stepPrefix}: 目标方法 '{step.TargetMethod.ClassName}.{step.TargetMethod.MethodName}' 未在注册表中找到");
            }
        }

        /// <summary>
        /// 验证变量定义
        /// </summary>
        private void ValidateVariables(TestSequence sequence, ConfigValidationReport report, string seqPrefix)
        {
            if (sequence.Variables == null || sequence.Variables.Count == 0)
            {
                return;
            }

            // 检查变量名唯一性
            var duplicateVars = sequence.Variables
                .GroupBy(v => v.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var duplicateVar in duplicateVars)
            {
                report.AddError($"{seqPrefix}: 变量名重复: '{duplicateVar}'");
            }

            // 验证变量类型
            foreach (var variable in sequence.Variables)
            {
                if (string.IsNullOrEmpty(variable.Name))
                {
                    report.AddError($"{seqPrefix}: 变量缺少名称");
                }

                if (string.IsNullOrEmpty(variable.Type))
                {
                    report.AddWarning($"{seqPrefix}: 变量 '{variable.Name}' 未指定类型，将使用默认类型(string)");
                }
            }
        }

        /// <summary>
        /// 验证跳转目标
        /// </summary>
        private void ValidateGotoTargets(TestSequence sequence, ConfigValidationReport report, string seqPrefix)
        {
            var allStepIds = GetAllStepIds(sequence.Steps);

            foreach (var step in sequence.Steps)
            {
                ValidateStepGotoTarget(step, allStepIds, report, seqPrefix);
            }
        }

        /// <summary>
        /// 获取所有步骤ID（包括子步骤）
        /// </summary>
        private HashSet<string> GetAllStepIds(List<TestStep> steps)
        {
            var ids = new HashSet<string>();
            if (steps == null) return ids;

            foreach (var step in steps)
            {
                if (!string.IsNullOrEmpty(step.ID))
                {
                    ids.Add(step.ID);
                }

                if (step.SubSteps != null)
                {
                    foreach (var id in GetAllStepIds(step.SubSteps))
                    {
                        ids.Add(id);
                    }
                }

                if (step.TrueSteps != null)
                {
                    foreach (var id in GetAllStepIds(step.TrueSteps))
                    {
                        ids.Add(id);
                    }
                }

                if (step.FalseSteps != null)
                {
                    foreach (var id in GetAllStepIds(step.FalseSteps))
                    {
                        ids.Add(id);
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// 验证步骤的跳转目标
        /// </summary>
        private void ValidateStepGotoTarget(TestStep step, HashSet<string> allStepIds, ConfigValidationReport report, string seqPrefix)
        {
            if (step.OnFail == FailAction.GotoStep && !string.IsNullOrEmpty(step.GotoStepID))
            {
                if (!allStepIds.Contains(step.GotoStepID))
                {
                    report.AddError($"{seqPrefix} → 步骤 '{step.Name}': 跳转目标步骤ID '{step.GotoStepID}' 不存在");
                }
            }

            // 递归验证子步骤
            if (step.SubSteps != null)
            {
                foreach (var subStep in step.SubSteps)
                {
                    ValidateStepGotoTarget(subStep, allStepIds, report, seqPrefix);
                }
            }

            if (step.TrueSteps != null)
            {
                foreach (var trueStep in step.TrueSteps)
                {
                    ValidateStepGotoTarget(trueStep, allStepIds, report, seqPrefix);
                }
            }

            if (step.FalseSteps != null)
            {
                foreach (var falseStep in step.FalseSteps)
                {
                    ValidateStepGotoTarget(falseStep, allStepIds, report, seqPrefix);
                }
            }
        }

        /// <summary>
        /// 添加最佳实践建议
        /// </summary>
        private void AddBestPracticeSuggestions(TestSequenceConfig config, ConfigValidationReport report)
        {
            // 检查是否有初始化和清理步骤
            foreach (var sequence in config.Sequences)
            {
                if (sequence.Steps == null || sequence.Steps.Count == 0)
                    continue;

                var firstStep = sequence.Steps.FirstOrDefault();
                var lastStep = sequence.Steps.LastOrDefault();

                bool hasInitStep = sequence.Steps.Any(s =>
                    s.Name?.Contains("初始化") == true ||
                    s.Name?.ToLower().Contains("init") == true ||
                    s.TargetMethod?.MethodName?.ToLower().Contains("init") == true);

                bool hasCleanupStep = sequence.Steps.Any(s =>
                    s.Name?.Contains("清理") == true ||
                    s.Name?.ToLower().Contains("cleanup") == true ||
                    s.TargetMethod?.MethodName?.ToLower().Contains("cleanup") == true);

                if (!hasInitStep)
                {
                    report.AddSuggestion($"序列 '{sequence.Name}': 建议添加初始化步骤");
                }

                if (!hasCleanupStep)
                {
                    report.AddSuggestion($"序列 '{sequence.Name}': 建议添加清理步骤");
                }

                // 检查是否有使用重试机制
                bool anyRetry = sequence.Steps.Any(s => s.RetryCount > 0);
                if (!anyRetry)
                {
                    report.AddSuggestion($"序列 '{sequence.Name}': 考虑为关键步骤添加重试机制");
                }

                // 检查是否有使用结果存储
                bool anyResultStorage = sequence.Steps.Any(s => !string.IsNullOrEmpty(s.ResultVariable));
                if (!anyResultStorage && sequence.Variables?.Count > 0)
                {
                    report.AddSuggestion($"序列 '{sequence.Name}': 已定义变量但未使用ResultVariable存储结果");
                }
            }
        }
    }
}
