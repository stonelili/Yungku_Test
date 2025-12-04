using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence.Config
{
    /// <summary>
    /// 序列配置加载器
    /// 从XML文件加载测试序列配置
    /// </summary>
    public class SequenceConfigLoader
    {
        /// <summary>
        /// 从XML文件加载序列配置
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>测试序列配置</returns>
        public TestSequenceConfig LoadConfig(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"配置文件不存在: {filePath}");

            try
            {
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    var serializer = new XmlSerializer(typeof(TestSequenceConfig));
                    return (TestSequenceConfig)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"加载配置文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从XML文件加载单个序列
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>测试序列</returns>
        public TestSequence LoadSequence(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"配置文件不存在: {filePath}");

            try
            {
                // 首先尝试作为TestSequenceConfig加载
                var config = LoadConfig(filePath);
                if (config.Sequences.Count > 0)
                {
                    return config.Sequences[0];
                }
            }
            catch
            {
                // 如果失败，尝试直接加载为TestSequence
            }

            try
            {
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    var serializer = new XmlSerializer(typeof(TestSequence));
                    return (TestSequence)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"加载序列文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从XML字符串加载序列配置
        /// </summary>
        /// <param name="xmlContent">XML内容</param>
        /// <returns>测试序列配置</returns>
        public TestSequenceConfig LoadConfigFromString(string xmlContent)
        {
            if (string.IsNullOrEmpty(xmlContent))
                throw new ArgumentNullException(nameof(xmlContent));

            try
            {
                using (var reader = new StringReader(xmlContent))
                {
                    var serializer = new XmlSerializer(typeof(TestSequenceConfig));
                    return (TestSequenceConfig)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"解析XML内容失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 验证配置文件
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        /// <returns>验证结果</returns>
        public (bool IsValid, string ErrorMessage) ValidateConfig(string filePath)
        {
            try
            {
                var config = LoadConfig(filePath);

                if (config.Sequences == null || config.Sequences.Count == 0)
                {
                    return (false, "配置文件中没有定义序列");
                }

                foreach (var sequence in config.Sequences)
                {
                    if (string.IsNullOrEmpty(sequence.ID))
                    {
                        return (false, $"序列 '{sequence.Name}' 缺少ID");
                    }

                    if (sequence.Steps == null || sequence.Steps.Count == 0)
                    {
                        return (false, $"序列 '{sequence.Name}' 没有定义步骤");
                    }

                    foreach (var step in sequence.Steps)
                    {
                        if (string.IsNullOrEmpty(step.ID))
                        {
                            return (false, $"步骤 '{step.Name}' 缺少ID");
                        }

                        // 验证测试方法（如果已注册）
                        if (step.TargetMethod != null &&
                            !string.IsNullOrEmpty(step.TargetMethod.ClassName) &&
                            !string.IsNullOrEmpty(step.TargetMethod.MethodName))
                        {
                            var registry = TestMethodRegistry.Instance;
                            if (registry.IsInitialized)
                            {
                                var validation = registry.ValidateMethodWithMessage(step.TargetMethod);
                                if (!validation.IsValid)
                                {
                                    return (false, $"步骤 '{step.Name}': {validation.ErrorMessage}");
                                }
                            }
                        }

                        // 验证限值
                        if (step.Type == StepType.NumericTest && step.Limits != null)
                        {
                            if (step.Limits.Lower > step.Limits.Upper)
                            {
                                return (false, $"步骤 '{step.Name}' 的下限大于上限");
                            }
                        }
                    }
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"验证失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 安全加载配置（不抛出异常）
        /// </summary>
        public (TestSequenceConfig Config, string ErrorMessage) TryLoadConfig(string filePath)
        {
            try
            {
                var config = LoadConfig(filePath);
                return (config, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        /// <summary>
        /// 创建默认配置
        /// </summary>
        public TestSequenceConfig CreateDefaultConfig()
        {
            return new TestSequenceConfig
            {
                Name = "默认测试序列配置",
                Version = "1.0",
                CreatedTime = DateTime.Now,
                Sequences = new System.Collections.Generic.List<TestSequence>
                {
                    new TestSequence
                    {
                        ID = "SEQ_DEFAULT",
                        Name = "默认测试序列",
                        Description = "自动生成的默认测试序列",
                        Enabled = true,
                        Version = "1.0"
                    }
                }
            };
        }

        /// <summary>
        /// 处理序列导入
        /// </summary>
        /// <param name="sequence">目标序列</param>
        /// <param name="basePath">配置文件的基础路径</param>
        public void ProcessImports(TestSequence sequence, string basePath = null)
        {
            if (sequence.Imports == null || sequence.Imports.Count == 0)
                return;

            foreach (var import in sequence.Imports)
            {
                try
                {
                    string importPath = import.File;
                    
                    // 处理相对路径
                    if (!Path.IsPathRooted(importPath) && !string.IsNullOrEmpty(basePath))
                    {
                        importPath = Path.Combine(Path.GetDirectoryName(basePath), importPath);
                    }

                    if (!File.Exists(importPath))
                    {
                        throw new FileNotFoundException($"导入文件不存在: {importPath}");
                    }

                    var importedConfig = LoadConfig(importPath);

                    switch (import.Type)
                    {
                        case ImportType.Variables:
                            ImportVariables(sequence, importedConfig, import);
                            break;

                        case ImportType.Sequences:
                            ImportSequences(importedConfig, import);
                            break;

                        case ImportType.All:
                            ImportVariables(sequence, importedConfig, import);
                            ImportSequences(importedConfig, import);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"处理导入失败 ({import.File}): {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// 导入变量定义
        /// </summary>
        private void ImportVariables(TestSequence targetSequence, TestSequenceConfig sourceConfig, SequenceImport import)
        {
            // 获取要导入的变量（根据过滤条件）
            var variableFilter = !string.IsNullOrEmpty(import.VariableFilter) 
                ? import.VariableFilter.Split(',').Select(v => v.Trim()).ToList() 
                : null;

            foreach (var sourceSequence in sourceConfig.Sequences)
            {
                foreach (var variable in sourceSequence.Variables)
                {
                    // 应用过滤条件
                    if (variableFilter != null && !variableFilter.Contains(variable.Name))
                        continue;

                    // 应用前缀
                    string targetName = !string.IsNullOrEmpty(import.Prefix) 
                        ? $"{import.Prefix}{variable.Name}" 
                        : variable.Name;

                    // 检查是否已存在
                    var existingVar = targetSequence.Variables.FirstOrDefault(v => v.Name == targetName);
                    if (existingVar != null)
                    {
                        if (import.OverwriteExisting)
                        {
                            targetSequence.Variables.Remove(existingVar);
                        }
                        else
                        {
                            continue; // 跳过已存在的变量
                        }
                    }

                    // 创建新的变量副本
                    var newVariable = new SequenceVariable
                    {
                        Name = targetName,
                        Type = variable.Type,
                        DefaultValue = variable.DefaultValue,
                        Description = variable.Description,
                        Scope = variable.Scope,
                        ArraySize = variable.ArraySize
                    };

                    targetSequence.Variables.Add(newVariable);
                }
            }
        }

        /// <summary>
        /// 导入序列定义（注册供子序列调用）
        /// </summary>
        private void ImportSequences(TestSequenceConfig sourceConfig, SequenceImport import)
        {
            foreach (var sequence in sourceConfig.Sequences)
            {
                // 应用前缀
                if (!string.IsNullOrEmpty(import.Prefix))
                {
                    sequence.ID = $"{import.Prefix}{sequence.ID}";
                }

                // 注册序列
                SequenceExecutor.RegisterSequence(sequence);
            }
        }

        /// <summary>
        /// 加载配置并处理导入
        /// </summary>
        public TestSequenceConfig LoadConfigWithImports(string filePath)
        {
            var config = LoadConfig(filePath);

            foreach (var sequence in config.Sequences)
            {
                ProcessImports(sequence, filePath);
            }

            return config;
        }
    }
}
