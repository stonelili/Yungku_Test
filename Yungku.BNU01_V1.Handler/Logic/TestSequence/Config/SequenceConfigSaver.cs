using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence.Config
{
    /// <summary>
    /// 序列配置保存器
    /// 将测试序列配置保存到XML文件
    /// </summary>
    public class SequenceConfigSaver
    {
        /// <summary>
        /// 是否格式化输出
        /// </summary>
        public bool FormatOutput { get; set; } = true;

        /// <summary>
        /// 是否包含XML声明
        /// </summary>
        public bool IncludeXmlDeclaration { get; set; } = true;

        /// <summary>
        /// 缩进字符
        /// </summary>
        public string IndentChars { get; set; } = "  ";

        /// <summary>
        /// 保存序列配置到XML文件
        /// </summary>
        /// <param name="config">序列配置</param>
        /// <param name="filePath">目标文件路径</param>
        public void SaveConfig(TestSequenceConfig config, string filePath)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            try
            {
                // 确保目录存在
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = FormatOutput,
                    IndentChars = IndentChars,
                    OmitXmlDeclaration = !IncludeXmlDeclaration
                };

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", ""); // 移除默认命名空间

                using (var writer = XmlWriter.Create(filePath, settings))
                {
                    var serializer = new XmlSerializer(typeof(TestSequenceConfig));
                    serializer.Serialize(writer, config, namespaces);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"保存配置文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 保存单个序列到XML文件
        /// </summary>
        /// <param name="sequence">测试序列</param>
        /// <param name="filePath">目标文件路径</param>
        public void SaveSequence(TestSequence sequence, string filePath)
        {
            var config = new TestSequenceConfig
            {
                Name = sequence.Name,
                Version = "1.0",
                CreatedTime = DateTime.Now,
                Sequences = new System.Collections.Generic.List<TestSequence> { sequence }
            };

            SaveConfig(config, filePath);
        }

        /// <summary>
        /// 将配置序列化为XML字符串
        /// </summary>
        /// <param name="config">序列配置</param>
        /// <returns>XML字符串</returns>
        public string SerializeToString(TestSequenceConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            try
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = FormatOutput,
                    IndentChars = IndentChars,
                    OmitXmlDeclaration = !IncludeXmlDeclaration
                };

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                using (var stringWriter = new StringWriter())
                {
                    using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        var serializer = new XmlSerializer(typeof(TestSequenceConfig));
                        serializer.Serialize(xmlWriter, config, namespaces);
                    }
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"序列化配置失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 将单个序列序列化为XML字符串
        /// </summary>
        /// <param name="sequence">测试序列</param>
        /// <returns>XML字符串</returns>
        public string SerializeSequenceToString(TestSequence sequence)
        {
            var config = new TestSequenceConfig
            {
                Name = sequence.Name,
                Version = "1.0",
                Sequences = new System.Collections.Generic.List<TestSequence> { sequence }
            };

            return SerializeToString(config);
        }

        /// <summary>
        /// 备份现有配置文件
        /// </summary>
        /// <param name="filePath">原文件路径</param>
        /// <returns>备份文件路径</returns>
        public string BackupConfig(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                var backupPath = Path.Combine(
                    Path.GetDirectoryName(filePath),
                    $"{Path.GetFileNameWithoutExtension(filePath)}_backup_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(filePath)}"
                );

                File.Copy(filePath, backupPath, true);
                return backupPath;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"备份配置文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 保存配置并创建备份
        /// </summary>
        /// <param name="config">序列配置</param>
        /// <param name="filePath">目标文件路径</param>
        /// <param name="createBackup">是否创建备份</param>
        /// <returns>备份文件路径（如果创建了备份）</returns>
        public string SaveConfigWithBackup(TestSequenceConfig config, string filePath, bool createBackup = true)
        {
            string backupPath = null;

            if (createBackup && File.Exists(filePath))
            {
                backupPath = BackupConfig(filePath);
            }

            SaveConfig(config, filePath);

            return backupPath;
        }

        /// <summary>
        /// 安全保存配置（不抛出异常）
        /// </summary>
        public (bool Success, string ErrorMessage) TrySaveConfig(TestSequenceConfig config, string filePath)
        {
            try
            {
                SaveConfig(config, filePath);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// 创建示例配置文件
        /// </summary>
        public void CreateSampleConfig(string filePath)
        {
            var config = new TestSequenceConfig
            {
                Name = "示例测试序列配置",
                Version = "1.0",
                CreatedTime = DateTime.Now,
                Sequences = new System.Collections.Generic.List<TestSequence>
                {
                    new TestSequence
                    {
                        ID = "SEQ_LEFT_STATION",
                        Name = "左工位测试序列",
                        Description = "左工位的完整测试流程",
                        Enabled = true,
                        Version = "1.0",
                        Author = "System",
                        PreCondition = "HasProduct",
                        PostCondition = "SaveResults",
                        Steps = new System.Collections.Generic.List<TestStep>
                        {
                            new TestStep
                            {
                                ID = "1",
                                Name = "初始化测试",
                                Description = "初始化测试环境",
                                Type = StepType.Action,
                                Enabled = true,
                                Timeout = 5000,
                                TargetMethod = new TargetMethodInfo("CommonTestMethods", "Initialize"),
                                OnFail = FailAction.Abort,
                                Parameters = new System.Collections.Generic.List<StepParameter>
                                {
                                    new StepParameter("timeout", "int", "5000")
                                }
                            },
                            new TestStep
                            {
                                ID = "2",
                                Name = "焦距测试",
                                Description = "测试模组焦距是否在规定范围内",
                                Type = StepType.NumericTest,
                                Enabled = true,
                                Timeout = 10000,
                                TargetMethod = new TargetMethodInfo("CommonTestMethods", "TestFocalLength"),
                                Limits = new TestLimits(3.8, 4.2, "mm"),
                                OnFail = FailAction.Continue,
                                Parameters = new System.Collections.Generic.List<StepParameter>
                                {
                                    new StepParameter("productIndex", "int", "0")
                                }
                            },
                            new TestStep
                            {
                                ID = "3",
                                Name = "色温测试",
                                Description = "测试色温是否在规定范围内",
                                Type = StepType.NumericTest,
                                Enabled = true,
                                Timeout = 10000,
                                TargetMethod = new TargetMethodInfo("CommonTestMethods", "TestColorTemperature"),
                                Limits = new TestLimits(2700, 6500, "K"),
                                OnFail = FailAction.Continue
                            },
                            new TestStep
                            {
                                ID = "4",
                                Name = "二维码读取",
                                Description = "读取产品二维码",
                                Type = StepType.StringTest,
                                Enabled = true,
                                Timeout = 5000,
                                TargetMethod = new TargetMethodInfo("CommonTestMethods", "ReadBarcode"),
                                OnFail = FailAction.Continue
                            }
                        }
                    }
                }
            };

            SaveConfig(config, filePath);
        }
    }
}
