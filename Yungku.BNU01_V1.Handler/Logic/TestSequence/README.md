# TestSequence 流程配置分析和改进

## 概述

本文档总结了对新增 TestSequence 测试序列流程的全面分析，并列出了配置不足之处及改进措施。

## 现有架构分析

### ✅ 优点

1. **完善的序列执行引擎 (SequenceExecutor)**
   - 支持同步/异步执行
   - 支持暂停、恢复、停止控制
   - 支持单步调试模式
   - 事件驱动架构（步骤开始/完成、序列开始/完成事件）

2. **丰富的步骤类型**
   - `NumericTest` - 数值测试，支持限值比较
   - `StringTest` - 字符串测试，支持期望值匹配
   - `PassFail` - 布尔判断测试
   - `Action` - 动作执行
   - `ForLoop` / `WhileLoop` / `ForEachLoop` - 循环控制
   - `ConditionalBranch` - 条件分支
   - `SequenceGroup` - 步骤组
   - `SubSequenceCall` - 子序列调用

3. **三级变量作用域**
   - `Local` - 局部变量（步骤级别）
   - `Sequence` - 序列变量
   - `Global` - 全局变量（跨序列共享）

4. **XML序列化**
   - 配置文件可读性好
   - 支持序列化/反序列化

5. **测试方法注册表**
   - 自动扫描标记有 `[TestMethod]` 特性的方法
   - 通过反射调用测试方法

### ❌ 原有不足及改进

| 不足 | 改进措施 | 状态 |
|------|----------|------|
| 缺少示例XML配置文件 | 添加了 `SampleTestSequence.xml` 示例文件 | ✅ 已完成 |
| 缺少配置验证工具 | 添加了 `SequenceConfigValidator` 验证器 | ✅ 已完成 |
| 测试用例缺少验证器测试 | 添加了 `TestConfigValidator` 测试方法 | ✅ 已完成 |

## 新增文件

### 1. 示例配置文件
**路径:** `Config/SequenceConfig/SampleTestSequence.xml`

提供完整的XML配置示例，包含：
- 主测试序列（包含初始化、二维码读取、电气测试、光学测试、外观检测、清理等步骤）
- 快速测试序列（简化版）
- 循环测试示例（演示For循环和条件分支功能）

### 2. 配置验证器
**路径:** `Logic/TestSequence/Config/SequenceConfigValidator.cs`

提供全面的配置验证功能：
- 验证序列ID唯一性
- 验证步骤ID唯一性
- 验证目标方法是否存在
- 验证限值设置（下限不能大于上限）
- 验证循环配置（防止死循环）
- 验证条件表达式
- 最佳实践建议（初始化步骤、清理步骤、重试机制等）

## UI界面说明

### 测试时是否使用左右工位界面？

**回答：是的！**

测试执行时，结果会显示在 FormAuto 的"测试参数"标签页中，该标签页包含：
- `lvLeftStation` - 左工位测试数据列表
- `lvRightStation` - 右工位测试数据列表

### UI显示流程

1. `ActionSequenceTest` 执行测试序列
2. 通过事件 `StepCompleted` 获取步骤结果
3. 调用 `TestDisplayHelper` 更新对应工位的显示
4. 根据 `stationIndex` 区分左工位(0)和右工位(1)

### 列显示格式（6列）

| Order | Name | LowLimit | UpperLimit | Value | Test |
|-------|------|----------|------------|-------|------|
| 序号 | 测试名称 | 下限 | 上限 | 测试值 | 结果 |

## ⚠️ 多产品同时测试场景

### 当前设计限制

当前UI设计基于**一对一**模式（每个工位对应一个产品显示区域）。对于以下场景需要特别考虑：

| 测试场景 | 当前支持情况 | 建议处理方式 |
|----------|--------------|--------------|
| 单产品测试 | ✅ 完全支持 | 直接使用左/右工位显示 |
| 左右工位各一个产品 | ✅ 完全支持 | 左右工位分别显示 |
| 多个相同产品同时测试 | ⚠️ 需区分 | 测试名称添加产品索引前缀 |
| 多个不同产品同时测试 | ⚠️ 需区分 | 测试名称添加产品名称/类型前缀 |

### 多产品测试的UI区分方案

#### 方案1：测试名称添加产品标识（推荐）

在测试步骤名称中包含产品索引或标识，使测试结果能够区分不同产品：

```xml
<!-- 示例：为每个产品创建独立的测试步骤 -->
<Step ID="STEP_P0_VOLTAGE" Name="[P0] 电压测试" Type="NumericTest">
  <TargetMethod Class="CommonTestMethods" Method="TestVoltage"/>
  <Parameters>
    <Param Name="channel" Type="int" Value="0"/>
  </Parameters>
  <Limits Lower="2.8" Upper="3.8" Unit="V"/>
</Step>

<Step ID="STEP_P1_VOLTAGE" Name="[P1] 电压测试" Type="NumericTest">
  <TargetMethod Class="CommonTestMethods" Method="TestVoltage"/>
  <Parameters>
    <Param Name="channel" Type="int" Value="1"/>
  </Parameters>
  <Limits Lower="2.8" Upper="3.8" Unit="V"/>
</Step>
```

界面显示效果：
```
| Order | Name           | LowLimit | UpperLimit | Value | Test |
|-------|----------------|----------|------------|-------|------|
| 1     | [P0] 电压测试  | 2.800    | 3.800      | 3.250 | PASS |
| 2     | [P1] 电压测试  | 2.800    | 3.800      | 3.310 | PASS |
```

#### 方案2：使用循环和变量动态生成

使用ForLoop循环遍历多个产品，结合变量生成带产品索引的测试名称：

```xml
<Step ID="MULTI_PRODUCT_TEST" Name="多产品电压测试" Type="ForLoop" 
      LoopStart="0" LoopEnd="3" LoopStep="1" LoopVariable="productIndex">
  <SubSteps>
    <Step ID="LOOP_VOLTAGE" Name="[P${productIndex}] 电压测试" Type="NumericTest">
      <TargetMethod Class="CommonTestMethods" Method="TestVoltage"/>
      <Parameters>
        <Param Name="channel" Type="int" Value="${productIndex}"/>
      </Parameters>
      <Limits Lower="2.8" Upper="3.8" Unit="V"/>
    </Step>
  </SubSteps>
</Step>
```

#### 方案3：产品分组显示（未来扩展）

如需要更清晰的多产品区分，可考虑以下UI扩展：
- 添加产品选择标签页（每个产品一个Tab）
- 使用TreeView按产品分组显示测试结果
- 在ListView中添加"产品"列

### 不同产品类型的处理

当同时测试不同类型的产品时，建议：

1. **配置多个序列** - 每种产品类型使用独立的测试序列
2. **使用不同的序列ID** - 在ActionSequenceTest中指定对应的SequenceId
3. **测试名称添加产品类型标识** - 例如 `[TypeA]`, `[TypeB]`

```xml
<!-- A类产品序列 -->
<Sequence ID="SEQ_TYPE_A" Name="A类产品测试">
  <Steps>
    <Step ID="A_INIT" Name="[TypeA] 初始化" .../>
    <Step ID="A_TEST1" Name="[TypeA] 电压测试" .../>
  </Steps>
</Sequence>

<!-- B类产品序列 -->
<Sequence ID="SEQ_TYPE_B" Name="B类产品测试">
  <Steps>
    <Step ID="B_INIT" Name="[TypeB] 初始化" .../>
    <Step ID="B_TEST1" Name="[TypeB] 功率测试" .../>
  </Steps>
</Sequence>
```

### 代码层面的产品区分

在 `TestDisplayHelper` 中，通过 `GetStationIndexByProduct` 方法根据产品索引确定显示工位：

```csharp
// 当前实现：根据产品索引返回工位索引
private static int GetStationIndexByProduct(Product product)
{
    try
    {
        // 方案1: 根据产品索引判断
        if (product.Jig != null && product.Jig.Head != null)
        {
            int productIndex = product.Jig.Head.ProductIndexOf(product);
            return productIndex; // 产品索引直接映射到工位索引
        }

        // 方案2: 根据产品名称判断（备用方案）
        if (product.Name.Contains("0"))
        {
            return 0; // 左工位
        }
        else if (product.Name.Contains("1"))
        {
            return 1; // 右工位
        }
    }
    catch (Exception ex)
    {
        MyApp.GetInstance().Logger.WriteError($"获取工位索引失败: {ex.Message}");
    }
    return 0; // 默认返回左工位
}
```

对于超过2个产品的情况，可扩展为动态创建多个显示区域或使用其他区分方式。

## 配置文件使用方法

### 在 ActionSequenceTest 中配置

1. 在 `SequenceFilePath` 属性中指定配置文件路径
2. （可选）在 `SequenceId` 属性中指定要执行的序列ID

```csharp
ActionSequenceTest action = new ActionSequenceTest();
action.SequenceFilePath = "Config/SequenceConfig/SampleTestSequence.xml";
action.SequenceId = "SEQ_MAIN";  // 可选，为空则执行第一个序列
action.DisplayToUI = true;       // 是否显示到界面
```

### 创建新的测试方法

1. 在 `CommonTestMethods` 类或新的类中添加方法
2. 使用 `[TestMethod]` 特性标记方法

```csharp
[TestMethod(Name = "自定义测试", Description = "测试描述", Category = "测试分类")]
public double CustomTest(int param1, string param2)
{
    // 测试逻辑
    return testResult;
}
```

### 在XML中引用测试方法

```xml
<Step ID="STEP_CUSTOM" Name="自定义测试" Type="NumericTest">
  <TargetMethod Class="CommonTestMethods" Method="CustomTest"/>
  <Parameters>
    <Param Name="param1" Type="int" Value="10"/>
    <Param Name="param2" Type="string" Value="test"/>
  </Parameters>
  <Limits Lower="0" Upper="100" Unit="单位"/>
</Step>
```

## 验证配置文件

```csharp
var loader = new SequenceConfigLoader();
var validator = new SequenceConfigValidator();

var config = loader.LoadConfig("path/to/config.xml");
var report = validator.ValidateConfig(config);

if (!report.IsValid)
{
    Console.WriteLine("配置验证失败:");
    foreach (var error in report.Errors)
    {
        Console.WriteLine($"  错误: {error}");
    }
}

// 查看建议
foreach (var suggestion in report.Suggestions)
{
    Console.WriteLine($"  建议: {suggestion}");
}
```

## 最佳实践建议

1. **始终包含初始化和清理步骤** - 确保测试环境正确设置和释放
2. **为关键步骤添加重试机制** - 设置 `RetryCount` 属性
3. **使用有意义的步骤ID** - 便于跳转和日志追踪
4. **合理设置超时时间** - 避免过短导致误判或过长影响效率
5. **使用变量存储中间结果** - 设置 `ResultVariable` 属性
6. **验证配置文件** - 使用 `SequenceConfigValidator` 在部署前验证

## 版本历史

- **v1.0** - 初始版本
  - 添加示例配置文件
  - 添加配置验证器
  - 添加配置验证测试用例
