using System;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence.Attributes
{
    /// <summary>
    /// 标记可作为测试方法的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestMethodAttribute : Attribute
    {
        /// <summary>
        /// 测试方法名称（用于显示）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 测试方法描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 测试方法分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 默认超时时间（毫秒）
        /// </summary>
        public int DefaultTimeout { get; set; } = 30000;

        /// <summary>
        /// 是否支持异步执行
        /// </summary>
        public bool IsAsync { get; set; } = false;

        public TestMethodAttribute()
        {
        }

        public TestMethodAttribute(string name)
        {
            Name = name;
        }

        public TestMethodAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
