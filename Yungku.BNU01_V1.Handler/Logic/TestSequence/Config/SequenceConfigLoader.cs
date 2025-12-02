using System;
using System.IO;
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
    }
}
