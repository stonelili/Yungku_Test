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
        // 方案1: 根据产品索引判断（推荐）
        if (product.Jig != null && product.Jig.Head != null)
        {
            int productIndex = product.Jig.Head.ProductIndexOf(product);
            return productIndex; // 产品索引直接映射到工位索引
        }

        // 方案2: 根据产品名称判断（备用方案）
        // 注意：简单的substring匹配可能产生误判，建议使用更精确的模式匹配
        // 例如：使用正则表达式或更严格的命名规范（如 "Product0", "Product1"）
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

> ⚠️ **注意**：方案2中使用 `Contains()` 进行简单字符串匹配可能产生误判（如产品名"Product10"会同时匹配"0"和"1"）。建议使用更精确的匹配方式，如正则表达式 `Regex.Match(product.Name, @"Product(\d+)$")` 或严格的产品命名规范。

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

## 🏭 高级配置场景

### 多工位并行测试配置

对于拥有多个工位（如20个工位）同时测试多个产品的场景，系统支持以下配置：

#### 工位配置结构

系统通过 `Turntable -> Station` 层级结构管理工位：

```
Turntable0
├── Station0  --> Turntable0_Station0_SequenceFile.xml
├── Station1  --> Turntable0_Station1_SequenceFile.xml
├── Station2  --> Turntable0_Station2_SequenceFile.xml
...
└── Station7  --> Turntable0_Station7_SequenceFile.xml
```

每个工位可以独立配置其测试序列文件路径（在 `FileStore.cs` 中定义）。

#### 配对测试（2个产品同时测试）

对于20个工位但只测试2种产品的场景，建议配置方案：

| 工位编号 | 产品类型 | 序列文件 |
|---------|----------|---------|
| Station0, 2, 4... | 产品A | SEQ_PRODUCT_A |
| Station1, 3, 5... | 产品B | SEQ_PRODUCT_B |

```xml
<!-- 根据工位奇偶分配不同产品测试 -->
<Sequence ID="SEQ_PRODUCT_A" Name="A产品测试序列">
  <Variables>
    <Variable Name="ProductType" Type="string" DefaultValue="TypeA"/>
  </Variables>
  <Steps>
    <Step ID="A_TEST" Name="[TypeA] 电压测试" .../>
  </Steps>
</Sequence>

<Sequence ID="SEQ_PRODUCT_B" Name="B产品测试序列">
  <Variables>
    <Variable Name="ProductType" Type="string" DefaultValue="TypeB"/>
  </Variables>
  <Steps>
    <Step ID="B_TEST" Name="[TypeB] 电流测试" .../>
  </Steps>
</Sequence>
```

### 老化测试（多轮循环测试）

系统支持通过 `StartMode.Loop` 启动模式实现老化测试的多轮循环执行。

#### 配置方法

1. **在 GeneralSettings 中设置启动模式**：
   ```csharp
   GeneralSettings.StartMode = StartMode.Loop; // 循环跑机模式
   ```

2. **序列内使用循环步骤**：
   ```xml
   <!-- 老化测试序列 - 支持多轮测试 -->
   <Sequence ID="SEQ_AGING" Name="老化测试序列">
     <Variables>
       <Variable Name="TestRound" Type="int" DefaultValue="0" Description="当前测试轮次"/>
       <Variable Name="MaxRounds" Type="int" DefaultValue="100" Description="最大测试轮次"/>
     </Variables>
     
     <Steps>
       <!-- 外层循环：控制测试轮次 -->
       <Step ID="AGING_LOOP" Name="老化循环" Type="WhileLoop" 
             WhileCondition="${TestRound} &lt; ${MaxRounds}" MaxIterations="1000">
         <SubSteps>
           <!-- 更新轮次计数 -->
           <Step ID="UPDATE_ROUND" Name="更新轮次" Type="Action">
             <Expression>${TestRound} = ${TestRound} + 1</Expression>
           </Step>
           
           <!-- 记录当前轮次 -->
           <Step ID="LOG_ROUND" Name="[Round${TestRound}] 轮次开始" Type="Action">
             <TargetMethod Class="CommonTestMethods" Method="LogMessage"/>
             <Parameters>
               <Param Name="message" Value="开始第${TestRound}轮老化测试"/>
             </Parameters>
           </Step>
           
           <!-- 实际测试步骤 -->
           <Step ID="AGING_TEST1" Name="[Round${TestRound}] 电压测试" Type="NumericTest">
             <TargetMethod Class="CommonTestMethods" Method="TestVoltage"/>
             <Limits Lower="2.8" Upper="3.8" Unit="V"/>
           </Step>
           
           <!-- 延时等待下一轮 -->
           <Step ID="WAIT_NEXT" Name="等待下一轮" Type="Action">
             <TargetMethod Class="CommonTestMethods" Method="Delay"/>
             <Parameters>
               <Param Name="milliseconds" Type="int" Value="60000"/>
             </Parameters>
           </Step>
         </SubSteps>
       </Step>
     </Steps>
   </Sequence>
   ```

3. **老化测试界面显示**：
   - 每轮测试结果会带有 `[RoundN]` 前缀
   - 统计面板累计显示所有轮次的 Pass/Fail 计数

### 产品类型选择

当一台机器需要测试多种产品类型时，可通过以下方式实现产品选择：

#### 方案1：通过工单设置选择产品

在 `GeneralSettings.ProductName` 中设置当前生产的机种名称，测试序列根据此设置加载对应的配置：

```csharp
// 读取当前产品类型
string productType = MyApp.Config.GeneralSettings.ProductName;

// 根据产品类型选择序列ID
string sequenceId = productType switch
{
    "ProductA" => "SEQ_PRODUCT_A",
    "ProductB" => "SEQ_PRODUCT_B",
    "ProductC" => "SEQ_PRODUCT_C",
    _ => "SEQ_DEFAULT"
};

actionSequenceTest.SequenceId = sequenceId;
```

#### 方案2：XML配置中使用条件分支

```xml
<!-- 根据产品类型执行不同测试 -->
<Step ID="PRODUCT_SELECT" Name="产品类型判断" Type="ConditionalBranch">
  <BranchCondition>${ProductType} == "TypeA"</BranchCondition>
  <TrueSteps>
    <Step ID="TYPE_A_TESTS" Name="[TypeA] 完整测试" Type="SubSequenceCall">
      <SubSequenceId>SEQ_TYPE_A</SubSequenceId>
    </Step>
  </TrueSteps>
  <FalseSteps>
    <Step ID="TYPE_B_TESTS" Name="[TypeB] 完整测试" Type="SubSequenceCall">
      <SubSequenceId>SEQ_TYPE_B</SubSequenceId>
    </Step>
  </FalseSteps>
</Step>
```

#### 方案3：界面产品选择器（建议扩展）

如需更直观的产品选择功能，建议在 FormAuto 或 FormSetting 中添加产品选择下拉框：

1. 读取可用的产品配置列表
2. 用户选择产品类型
3. 系统自动加载对应的测试序列

### 配置汇总表

| 测试场景 | 配置方式 | 关键设置 |
|----------|----------|----------|
| 单产品单工位 | 默认配置 | `SequenceId` |
| 左右工位各一产品 | 工位独立配置 | `stationIndex` |
| 多工位配对测试 | 工位序列分配 | 每个 Station 独立 Sequence 文件 |
| 老化循环测试 | WhileLoop + StartMode.Loop | `MaxIterations`, `WhileCondition` |
| 多产品类型选择 | 条件分支 / 工单设置 | `BranchCondition`, `ProductName` |

## 版本历史

- **v1.1** - 高级配置场景
  - 添加多工位并行测试说明
  - 添加老化测试配置示例
  - 添加产品类型选择方案
  
- **v1.0** - 初始版本
  - 添加示例配置文件
  - 添加配置验证器
  - 添加配置验证测试用例
