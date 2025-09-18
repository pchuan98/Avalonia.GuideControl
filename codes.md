# Avalonia.GuideControl 项目代码总结

这是一个用于 Avalonia UI 的引导控件库，提供交互式用户引导和教程功能。

## 🏗️ 项目架构

### 核心控件 (`Controls/`)
- **Mask.cs** - 主要遮罩控件，创建带透明 Hole 区域的全屏覆盖层，支持几何布尔运算和多 Hole 区域
- **GuideCard.cs** - 引导卡片控件，显示引导内容和导航按钮，基于 TemplatedControl 实现

### 数据模型 (`Models/`)
- **Guide.cs** - 完整引导流程的根容器，管理步骤资源池和执行序列
- **GuideStep.cs** - 单个引导步骤配置，包含控件路径、Hole 配置、验证方法等
- **GuideCardConfig.cs** - 引导卡片样式和内容配置，支持按钮和位置设置
- **ControlInfo.cs** - 控件位置和树结构信息，用于引导目标定位
- **Hole.cs** - 透明区域几何配置，支持圆角、边框和样式自定义
- **HolePadding.cs** - Hole 区域扩展边距配置
- **Offset.cs** - 2D 位置偏移量配置
- **CardPosition.cs** - 卡片相对位置枚举（使用位标志）
- **GuideOperation.cs** - 引导操作状态和结果枚举

### 工具类 (`AvaloniaUtils/`)
- **AvaloniaUtils.cs** - 窗口顶层管理、控件查找、层级排序等基础功能
- **AvaloniaUtils.VisualTree.cs** - 视觉树路径生成和解析，使用 `TypeName[index]` 格式
- **AvaloniaUtils.LogicTree.cs** - 逻辑树路径生成和解析，结构与视觉树相似
- **AvaloniaUtils.ControlInfo.cs** - 控件信息提取和匹配验证，支持多种验证模式
- **AvaloniaUtils.Measure.cs** - 控件尺寸测量工具，使用隐藏窗口避免界面干扰

### 扩展方法 (`Extensions/`)
- **MaskExtensions.cs** - 遮罩显示的简化方法，提供预配置的调试实例
- **GuideStepExtension.cs** - 步骤到 Hole 转换和显示，处理主 Hole 和附加 Holes
- **CardExtension.cs** - 卡片位置计算和创建，智能计算最佳显示位置

### 管理器
- **GuideManager.cs** - 引导流程执行和状态管理，支持异步执行和验证机制

### 开发工具 (`DevTools/`)
- **Tools.cs** - F1 键触发的开发工具入口，附加到窗口
- **Record/RecordWindow.axaml.cs** - 可视化录制窗口，支持拖拽和透明度调整
- **Record/ViewModels/*.cs** - 录制功能的完整 MVVM 架构

## 🔧 核心功能详解

### 1. 遮罩系统 (Mask)
**文件**: `Controls/Mask.cs`
**功能**: 创建全屏遮罩层，支持多个透明 Hole 区域
```csharp
// 核心方法
void Show(Control? control = null)          // 显示遮罩，自动定位到顶层
void Hidden(Control? control = null)        // 隐藏遮罩，清理资源
void OnHoleItemsChanged()                   // Hole 列表变化时重新绘制几何图形
Geometry GetHoleGeometry(Panel panel)      // 使用布尔运算生成透明区域几何
```
**技术特点**:
- 使用几何布尔运算（Exclude）创建复杂透明区域
- 自动置于最顶层使用 Popup 容器（命名格式：`GuideTopmost_{timestamp}`）
- 实现 IDisposable 进行资源管理

### 2. 引导卡片 (GuideCard)
**文件**: `Controls/GuideCard.cs`
**功能**: 显示引导内容，包含标题、内容、导航按钮
```csharp
// 主要属性
string Header, Tips, Content                // 显示内容
string PreviousButtonText, NextButtonText, SkipButtonText  // 按钮文本
bool IsPreviousButtonVisible, IsNextButtonVisible, IsSkipButtonVisible  // 按钮显示状态
ICommand PreviousCommand, NextCommand, SkipCommand  // 按钮命令绑定

// 事件系统
event EventHandler<RoutedEventArgs> PreviousClick, NextClick, SkipClick
```

### 3. 控件定位系统 (AvaloniaUtils)
**文件**: `AvaloniaUtils/*.cs`
**功能**: 通过路径字符串精确定位界面控件，支持视觉树和逻辑树两种模式

#### 视觉树操作
```csharp
// AvaloniaUtils.VisualTree.cs
string VisualTreeString(Control control, Control scope)     // 生成视觉树路径
Control? FromVisualTree(string path, Control scope)        // 根据路径查找控件
// 路径格式：Button[0]/StackPanel[1]/TextBox[0]
```

#### 逻辑树操作
```csharp
// AvaloniaUtils.LogicTree.cs  
string LogicTreeString(Control control, Control scope)      // 生成逻辑树路径
Control? FromLogicTree(string path, Control scope)         // 根据路径查找控件
```

#### 控件信息提取
```csharp
// AvaloniaUtils.ControlInfo.cs
ControlInfo Info(Control control, Control scope)           // 提取控件完整信息
bool CheckInfoName(Control control, ControlInfo info)      // 验证控件名称匹配
bool CheckInfoVisualPath(Control control, ControlInfo info) // 验证视觉树路径
bool CheckInfoLogicPath(Control control, ControlInfo info)  // 验证逻辑树路径
```

#### 窗口层级管理
```csharp
// AvaloniaUtils.cs
void TopmostAppend(Window window, Control control)         // 添加到最顶层
void TopmostRemove(Window window, Control control)         // 从顶层移除
void ClearTopmost(Panel panel)                            // 清除所有引导层
IEnumerable<Control> FindControls(Control control, Point point) // 根据坐标查找控件
Control? Topmost(IEnumerable<Control> controls, Control? root)  // 获取最顶层控件
```

### 4. 位置计算系统 (CardExtension)
**文件**: `Extensions/CardExtension.cs`
**功能**: 智能计算引导卡片的最佳显示位置
```csharp
// 位置计算方法
Point? CalculatePosition(GuideCard card, Hole targetHole, CardPosition position, Offset offset, Control? root)
Point? CalculatePositionFromStep(GuideCard card, GuideStep step, Control? root)
GuideCard CreateCard(GuideStep step, GuideCardConfig? defaultConfig)
```
**算法特点**:
- 支持九宫格位置定义（Top、Bottom、Left、Right及其组合）
- 自动边界检测，防止卡片超出屏幕
- 考虑 Hole 区域和卡片尺寸的最优化布局

### 5. 引导步骤转换 (GuideStepExtension)
**文件**: `Extensions/GuideStepExtension.cs`
**功能**: 将引导步骤转换为可视化元素
```csharp
Hole? MainHole(GuideStep step, Control? root)              // 获取主 Hole（基于 VisualTree）
List<Hole> AddtioinalHoles(GuideStep step, Control? root)  // 获取附加 Holes
List<Hole> Holes(GuideStep step, Control? control)        // 获取所有 Holes 合集
Task<bool> Show(GuideStep step, Control? control, GuideCardConfig? config, CancellationToken? token)
```

### 6. 引导管理器 (GuideManager)
**文件**: `GuideManager.cs`
**功能**: 统一管理引导流程的执行和控制
```csharp
// 主要属性
GuideStep[] StepResource                    // 所有步骤资源池
Guid[] Steps                               // 当前执行步骤序列
Dictionary<string, Func<bool>> ValidMethods // 验证方法字典
GuideStep? CurrentStep                     // 当前正在执行的步骤
event EventHandler<string> OnValid         // 验证完成事件

// 核心方法
void MergeGuide(Guide? guide)              // 合并引导配置到管理器
Task<GuideOperation> TestRunStep(Guid id, int? timeout) // 异步执行单个步骤
void UpdateValid(string method)            // 手动触发验证完成
```
**执行流程**:
1. 检查准备条件（PreparedMethod）
2. 显示引导步骤界面
3. 等待用户操作或超时
4. 验证完成条件（FinishMethod）
5. 返回操作结果

## 📋 数据模型详解

### Guide - 引导流程容器
**文件**: `Models/Guide.cs`
```csharp
Guid Id                                    // 唯一标识符
string Alias, Description                 // 别名和详细描述
GuideCardConfig DefaultCardConfig         // 默认卡片配置（继承机制）
List<GuideStep>? StepResources           // 步骤资源池（可重用）
List<Guid>? GuidSteps                    // 执行步骤序列
```
**序列化支持**: 使用 `System.Text.Json` 源生成器，支持 AOT 编译

### GuideStep - 引导步骤
**文件**: `Models/GuideStep.cs`
```csharp
Guid Id                                    // 步骤唯一标识
string Alias                               // 步骤显示名称
string VisualTree                          // 主目标控件的视觉树路径
List<string> AdditionalHoles               // 附加 Hole 的控件路径列表
HolePadding HolePadding                    // Hole 区域扩展边距
Offset HoleOffset                          // Hole 位置偏移量
string? PreparedMethod, FinishMethod       // 准备和完成验证方法名
GuideCardConfig Config                     // 步骤专属卡片配置
```

### GuideCardConfig - 卡片配置
**文件**: `Models/GuideCardConfig.cs`
```csharp
string Header, Content, Tips               // 卡片显示内容
string? PreviousButtonText, SkipButtonText, NextButtonText  // 按钮文本自定义
bool? IsPreviousButtonVisible, IsSkipButtonVisible, IsNextButtonVisible  // 按钮显示控制
string? StyleClass                         // CSS 样式类名
CardPosition Position                      // 相对目标的位置（位标志枚举）
Offset Offset                             // 卡片位置微调偏移
```

### ControlInfo - 控件信息
**文件**: `Models/ControlInfo.cs`
```csharp
string TypeName                            // 控件类型全名
string? ElementName                        // 控件 Name 属性值
Control? Scope                            // 搜索作用域根控件
string? VisualTree, LogicTree             // 视觉树和逻辑树路径
Rect Location                             // 控件在屏幕上的绝对位置
Hole AsHole()                             // 转换为 Hole 对象的便捷方法
```

### Hole - 透明区域
**文件**: `Models/Hole.cs`
```csharp
Rect Bounds                               // 透明区域的矩形边界
CornerRadius CornerRadius                 // 圆角半径设置
IBrush? BorderBrush                       // 边框画刷（运行时）
string? BorderBrushColor                  // 边框颜色字符串（序列化友好）
double BorderThickness                    // 边框线条粗细
bool IsHitTestVisible                     // 是否允许鼠标事件穿透
```

## 🛠️ 开发工具系统

### 工具入口 (DevTools/Tools.cs)
```csharp
void AttachGuideDevTools(Window window)    // 附加开发工具到窗口
// 使用方式: window.AttachGuideDevTools()
// 快捷键: F1 打开录制窗口，实时调试引导流程
```

### 录制窗口系统 (DevTools/Record/)
**主窗口**: `RecordWindow.axaml.cs`
- 可拖拽的半透明工具窗口
- 文件打开/保存功能（JSON 格式）
- 实时预览和测试功能

**MVVM 架构**:
- **RecordViewModel.cs** - 主协调器，管理整体录制流程
- **BasicInfoViewModel.cs** - 基本信息编辑（ID、名称、描述）
- **DefaultCardViewModel.cs** - 默认卡片配置管理
- **StepEditorViewModel.cs** - 单步骤详细编辑器
- **StepsOrderViewModel.cs** - 步骤排序和执行序列管理

**交互机制**:
- **Alt + 鼠标悬停**: 选择和高亮目标控件
- **F4 键**: 快速添加附加 Hole 区域
- **拖拽排序**: 可视化调整步骤执行顺序

## 🎯 关键使用模式

### 1. 基础遮罩使用
```csharp
var mask = new Mask();
mask.HoleItems = [new Hole(new Rect(100, 100, 200, 150))];
mask.Show(targetWindow);  // 自动置顶显示
// 使用完毕后
mask.Hidden();  // 自动清理资源
```

### 2. 控件精确定位
```csharp
// 提取控件信息
var info = targetButton.Info(mainWindow);
// 生成路径字符串
var visualPath = targetButton.VisualTreeString(mainWindow);
var logicPath = targetButton.LogicTreeString(mainWindow);
// 根据路径查找控件
var foundControl = AvaloniaUtils.FromVisualTree(visualPath, mainWindow);
```

### 3. 完整引导流程执行
```csharp
var manager = new GuideManager(rootControl);
// 注册验证方法
manager.ValidMethods["user_clicked_button"] = () => buttonClicked;
// 加载引导配置
manager.MergeGuide(guide);
// 执行步骤
var result = await manager.TestRunStep(stepId, timeout: 30000);
```

### 4. 开发工具集成
```csharp
// 在 App.xaml.cs 或 MainWindow 构造函数中
protected override void OnOpened(EventArgs e) {
    base.OnOpened(e);
    this.AttachGuideDevTools();  // F1 激活录制工具
}
```

### 5. 自定义卡片位置
```csharp
var config = new GuideCardConfig {
    Position = CardPosition.Top | CardPosition.Right,  // 右上角
    Offset = new Offset(10, -5),  // 微调偏移
    Header = "操作提示",
    Content = "点击此按钮继续下一步操作"
};
```

## 🔧 技术架构特色

### 1. 几何计算引擎
- 使用 Avalonia 的 **PathGeometry** 和 **GeometryGroup**
- **CombineGeometry** 实现布尔运算（Exclude 操作）
- 支持复杂的多 Hole 区域组合

### 2. 坐标系统管理
- 全屏坐标与控件相对坐标的智能转换
- **TransformToVisual** 进行精确的坐标变换
- 支持多层 Panel 嵌套的坐标计算

### 3. 异步执行模型
- 基于 **Task** 和 **CancellationToken** 的现代异步模式
- 支持超时控制和取消操作
- 事件驱动的验证机制

### 4. 序列化架构
- 使用 **System.Text.Json** 源生成器
- 支持 AOT（Ahead-of-Time）编译
- 智能忽略不可序列化属性（如 IBrush）

### 5. 扩展性设计
- 基于扩展方法的 API 设计
- 策略模式的位置计算
- 工厂模式的控件创建

## 📁 文件定位快速索引

| 功能需求 | 主要文件 | 关键方法/属性 |
|---------|---------|------------|
| 创建遮罩层 | `Controls/Mask.cs` | `Show()`, `OnHoleItemsChanged()`, `GetHoleGeometry()` |
| 引导卡片显示 | `Controls/GuideCard.cs` | 依赖属性绑定, 事件处理器 |
| 控件路径定位 | `AvaloniaUtils/AvaloniaUtils.VisualTree.cs` | `VisualTreeString()`, `FromVisualTree()` |
| 逻辑树操作 | `AvaloniaUtils/AvaloniaUtils.LogicTree.cs` | `LogicTreeString()`, `FromLogicTree()` |
| 控件信息提取 | `AvaloniaUtils/AvaloniaUtils.ControlInfo.cs` | `Info()`, `CheckInfoName()` |
| 卡片位置计算 | `Extensions/CardExtension.cs` | `CalculatePositionFromStep()` |
| 步骤到视觉转换 | `Extensions/GuideStepExtension.cs` | `Holes()`, `Show()`, `MainHole()` |
| 引导流程管理 | `GuideManager.cs` | `MergeGuide()`, `TestRunStep()`, `UpdateValid()` |
| 开发工具入口 | `DevTools/Tools.cs` | `AttachGuideDevTools()` |
| 录制界面主窗口 | `DevTools/Record/RecordWindow.axaml.cs` | 文件操作, 实时预览, 拖拽支持 |
| 录制功能协调 | `DevTools/Record/ViewModels/RecordViewModel.cs` | MVVM 数据绑定, 序列化管理 |
| 步骤编辑器 | `DevTools/Record/ViewModels/StepEditorViewModel.cs` | 步骤配置, 控件选择 |
| 数据模型定义 | `Models/*.cs` | 各模型类的属性和方法定义 |

## 🔍 快速问题定位指南

- **遮罩不显示或层级错误**: 检查 `AvaloniaUtils.cs` 的 `TopmostAppend()` 和 Popup 命名机制
- **控件路径查找失败**: 检查 `AvaloniaUtils.VisualTree.cs` 的路径生成算法和索引计算
- **卡片位置计算错误**: 检查 `CardExtension.cs` 的坐标转换和边界检测逻辑
- **引导步骤执行异常**: 检查 `GuideManager.cs` 的验证方法注册和异步流程控制
- **几何图形绘制问题**: 检查 `Mask.cs` 的 `GetHoleGeometry()` 和布尔运算逻辑
- **开发工具交互异常**: 检查 `DevTools/` 目录下的事件绑定和 MVVM 数据流
- **JSON 序列化错误**: 检查模型类的 `JsonPropertyName` 属性和源生成器配置

## 📖 架构总结

这个引导控件库展现了现代 .NET/Avalonia 应用的最佳实践：

1. **清晰的职责分离**: 控件、模型、工具类、扩展方法各司其职
2. **优秀的可扩展性**: 基于接口和扩展方法的开放式设计
3. **完善的工具链**: 可视化开发工具大大提升开发效率
4. **现代化技术栈**: 异步编程、源生成器、AOT 支持等前沿特性
5. **用户友好**: 简单易用的 API 设计和丰富的配置选项

整个架构既保持了灵活性，又确保了易用性，是一个高质量的引导控件解决方案。