using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yungku.BNU01_V1.Handler.Logic.TestSequence.Attributes;

namespace Yungku.BNU01_V1.Handler.Logic.TestSequence
{
    /// <summary>
    /// 已注册的测试方法信息
    /// </summary>
    public class RegisteredTestMethod
    {
        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 方法所属类型
        /// </summary>
        public Type DeclaringType { get; set; }

        /// <summary>
        /// 测试方法特性
        /// </summary>
        public TestMethodAttribute Attribute { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName => Attribute?.Name ?? Method?.Name;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description => Attribute?.Description;

        /// <summary>
        /// 分类
        /// </summary>
        public string Category => Attribute?.Category ?? DeclaringType?.Name;

        /// <summary>
        /// 完整方法签名
        /// </summary>
        public string FullSignature => $"{DeclaringType?.Name}.{Method?.Name}";

        /// <summary>
        /// 返回类型
        /// </summary>
        public Type ReturnType => Method?.ReturnType;

        /// <summary>
        /// 参数信息
        /// </summary>
        public ParameterInfo[] Parameters => Method?.GetParameters();

        /// <summary>
        /// 是否为静态方法
        /// </summary>
        public bool IsStatic => Method?.IsStatic ?? false;

        /// <summary>
        /// 实例对象（用于非静态方法）
        /// </summary>
        public object Instance { get; set; }

        public override string ToString()
        {
            return $"{FullSignature} - {DisplayName}";
        }
    }

    /// <summary>
    /// 测试方法注册表
    /// 负责扫描、注册和管理项目中的测试方法
    /// </summary>
    public class TestMethodRegistry
    {
        private static TestMethodRegistry _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// 已注册的测试方法字典
        /// Key: 类名.方法名
        /// </summary>
        private Dictionary<string, RegisteredTestMethod> _registeredMethods = new Dictionary<string, RegisteredTestMethod>();

        /// <summary>
        /// 已实例化的测试类对象缓存
        /// </summary>
        private Dictionary<Type, object> _instanceCache = new Dictionary<Type, object>();

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 已注册方法数量
        /// </summary>
        public int MethodCount => _registeredMethods.Count;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static TestMethodRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TestMethodRegistry();
                        }
                    }
                }
                return _instance;
            }
        }

        private TestMethodRegistry()
        {
        }

        /// <summary>
        /// 初始化注册表，扫描并注册所有测试方法
        /// </summary>
        /// <param name="assemblies">要扫描的程序集列表，为空则扫描当前程序集</param>
        public void Initialize(params Assembly[] assemblies)
        {
            lock (_lock)
            {
                _registeredMethods.Clear();
                _instanceCache.Clear();

                if (assemblies == null || assemblies.Length == 0)
                {
                    assemblies = new[] { Assembly.GetExecutingAssembly() };
                }

                foreach (var assembly in assemblies)
                {
                    ScanAssembly(assembly);
                }

                IsInitialized = true;
            }
        }

        /// <summary>
        /// 扫描程序集中的测试方法
        /// </summary>
        private void ScanAssembly(Assembly assembly)
        {
            try
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    // 跳过抽象类和接口
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    ScanType(type);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 处理某些类型无法加载的情况 - 记录到调试输出
                System.Diagnostics.Debug.WriteLine($"[TestMethodRegistry] ReflectionTypeLoadException: {ex.Message}");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    if (loaderException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Loader exception: {loaderException.Message}");
                    }
                }

                foreach (var type in ex.Types)
                {
                    if (type != null && !type.IsAbstract && !type.IsInterface)
                    {
                        try
                        {
                            ScanType(type);
                        }
                        catch (Exception typeEx)
                        {
                            // 记录无法扫描的类型
                            System.Diagnostics.Debug.WriteLine($"[TestMethodRegistry] Failed to scan type {type.Name}: {typeEx.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录程序集扫描错误
                System.Diagnostics.Debug.WriteLine($"[TestMethodRegistry] Assembly scan error: {ex.Message}");
            }
        }

        /// <summary>
        /// 扫描类型中的测试方法
        /// </summary>
        private void ScanType(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<TestMethodAttribute>();
                if (attribute != null)
                {
                    RegisterMethod(type, method, attribute);
                }
            }
        }

        /// <summary>
        /// 注册测试方法
        /// </summary>
        private void RegisterMethod(Type type, MethodInfo method, TestMethodAttribute attribute)
        {
            string key = $"{type.Name}.{method.Name}";

            var registeredMethod = new RegisteredTestMethod
            {
                Method = method,
                DeclaringType = type,
                Attribute = attribute
            };

            _registeredMethods[key] = registeredMethod;
        }

        /// <summary>
        /// 手动注册测试方法
        /// </summary>
        public void RegisterMethod(Type type, string methodName, TestMethodAttribute attribute = null)
        {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (method == null)
                throw new ArgumentException($"Method {methodName} not found in type {type.Name}");

            if (attribute == null)
            {
                attribute = method.GetCustomAttribute<TestMethodAttribute>() ?? new TestMethodAttribute(methodName);
            }

            RegisterMethod(type, method, attribute);
        }

        /// <summary>
        /// 获取测试方法
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        public RegisteredTestMethod GetMethod(string className, string methodName)
        {
            string key = $"{className}.{methodName}";
            _registeredMethods.TryGetValue(key, out var method);
            return method;
        }

        /// <summary>
        /// 获取测试方法（通过完整签名）
        /// </summary>
        public RegisteredTestMethod GetMethod(string fullSignature)
        {
            _registeredMethods.TryGetValue(fullSignature, out var method);
            return method;
        }

        /// <summary>
        /// 获取所有已注册的测试方法
        /// </summary>
        public IEnumerable<RegisteredTestMethod> GetAllMethods()
        {
            return _registeredMethods.Values;
        }

        /// <summary>
        /// 获取指定分类的测试方法
        /// </summary>
        public IEnumerable<RegisteredTestMethod> GetMethodsByCategory(string category)
        {
            return _registeredMethods.Values.Where(m => m.Category == category);
        }

        /// <summary>
        /// 获取指定类的测试方法
        /// </summary>
        public IEnumerable<RegisteredTestMethod> GetMethodsByClass(string className)
        {
            return _registeredMethods.Values.Where(m => m.DeclaringType.Name == className);
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        public IEnumerable<string> GetAllCategories()
        {
            return _registeredMethods.Values.Select(m => m.Category).Distinct().Where(c => !string.IsNullOrEmpty(c));
        }

        /// <summary>
        /// 获取所有类名
        /// </summary>
        public IEnumerable<string> GetAllClassNames()
        {
            return _registeredMethods.Values.Select(m => m.DeclaringType.Name).Distinct();
        }

        /// <summary>
        /// 获取或创建类实例
        /// </summary>
        public object GetOrCreateInstance(Type type)
        {
            if (!_instanceCache.TryGetValue(type, out var instance))
            {
                lock (_lock)
                {
                    if (!_instanceCache.TryGetValue(type, out instance))
                    {
                        try
                        {
                            instance = Activator.CreateInstance(type);
                            _instanceCache[type] = instance;
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Cannot create instance of type {type.Name}: {ex.Message}", ex);
                        }
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// 设置类实例
        /// </summary>
        public void SetInstance(Type type, object instance)
        {
            if (instance != null && !type.IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentException($"Instance is not of type {type.Name}");
            }
            _instanceCache[type] = instance;
        }

        /// <summary>
        /// 清除实例缓存
        /// </summary>
        public void ClearInstanceCache()
        {
            lock (_lock)
            {
                _instanceCache.Clear();
            }
        }

        /// <summary>
        /// 验证测试方法是否存在
        /// </summary>
        public bool ValidateMethod(string className, string methodName)
        {
            return GetMethod(className, methodName) != null;
        }

        /// <summary>
        /// 验证测试方法并返回验证结果
        /// </summary>
        public (bool IsValid, string ErrorMessage) ValidateMethodWithMessage(TargetMethodInfo targetMethod)
        {
            if (targetMethod == null)
                return (false, "目标方法信息为空");

            if (string.IsNullOrEmpty(targetMethod.ClassName))
                return (false, "类名不能为空");

            if (string.IsNullOrEmpty(targetMethod.MethodName))
                return (false, "方法名不能为空");

            var method = GetMethod(targetMethod.ClassName, targetMethod.MethodName);
            if (method == null)
                return (false, $"未找到方法 {targetMethod.ClassName}.{targetMethod.MethodName}");

            return (true, null);
        }

        /// <summary>
        /// 刷新注册表
        /// </summary>
        public void Refresh()
        {
            Initialize(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 重置注册表
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }
    }
}
