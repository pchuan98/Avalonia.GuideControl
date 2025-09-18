using System.Collections.ObjectModel;
using Avalonia.GuideControl.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.GuideControl.DevTools.Record;

/// <summary>
/// 编辑单个引导步骤的所有配置信息
/// </summary>
/// <remarks>包含位置配置、Hole设置和卡片内容等完整编辑功能</remarks>
/// <example>
/// var editor = new StepEditorViewModel(guideStep);
/// editor.StepAlias = "welcome-step";
/// editor.Update(ref step);
/// </example>
internal partial class StepEditorViewModel(GuideStep step) : ObservableObject
{
    public StepEditorViewModel() : this(new GuideStep()) { }

    #region prop

    /// <summary>
    /// 步骤的唯一标识符，自动生成不可修改
    /// </summary>
    [ObservableProperty]
    public partial string StepId { get; private set; } = step.Id.ToString("N");

    /// <summary>
    /// 步骤的友好名称，用于识别和管理
    /// </summary>
    /// <remarks>建议使用简短有意义的名称，如 "welcome-step"</remarks>
    /// <example>StepAlias = "create-project-step";</example>
    [ObservableProperty]
    public partial string StepAlias { get; set; } = step.Alias;

    /// <summary>
    /// 目标控件在界面树中的路径
    /// </summary>
    /// <remarks>格式: MainWindow.Panel.Button，用于定位要高亮的控件</remarks>
    /// <example>StepVisualTree = "MainWindow.MenuBar.FileMenu.NewButton";</example>
    [ObservableProperty]
    public partial string StepVisualTree { get; set; } = step.VisualTree;

    /// <summary>
    /// 额外添加的Holes区域
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<string> StepAdditionalHoles { get; set; } = new(step.AdditionalHoles);

    /// <summary>
    /// 步骤可以开始的验证方法名称
    /// </summary>
    /// <remarks>可为空，空值表示手动确认完成</remarks>
    /// <example>StepFinishMethod = "CheckUserLoginStatus";</example>
    [ObservableProperty]
    public partial string StepPreparedMethod { get; set; } = step.PreparedMethod ?? string.Empty;

    /// <summary>
    /// 步骤完成的验证方法名称
    /// </summary>
    /// <remarks>可为空，空值表示手动确认完成</remarks>
    /// <example>StepFinishMethod = "CheckUserLoginStatus";</example>
    [ObservableProperty]
    public partial string StepFinishMethod { get; set; } = step.FinishMethod ?? string.Empty;

    /// <summary>
    /// 引导卡片相对于目标控件的显示位置
    /// </summary>
    /// <remarks>支持位操作组合，如 Top | Left 表示左上角</remarks>
    /// <example>StepPosition = CardPosition.Top | CardPosition.Right;</example>
    [ObservableProperty]
    public partial CardPosition StepPosition { get; set; } = step.Config.Position;

    /// <summary>
    /// 引导卡片水平方向的微调偏移量
    /// </summary>
    /// <remarks>正值向右偏移，负值向左偏移，单位像素</remarks>
    /// <example>StepOffsetX = 10; // 向右偏移10像素</example>
    [ObservableProperty]
    public partial double StepOffsetX { get; set; } = step.Config.Offset.X;

    /// <summary>
    /// 引导卡片垂直方向的微调偏移量
    /// </summary>
    /// <remarks>正值向下偏移，负值向上偏移，单位像素</remarks>
    /// <example>StepOffsetY = -5; // 向上偏移5像素</example>
    [ObservableProperty]
    public partial double StepOffsetY { get; set; } = step.Config.Offset.Y;

    /// <summary>
    /// Hole区域左侧扩展的内边距
    /// </summary>
    /// <remarks>正值向左扩展Hole范围，使Hole大于控件边界</remarks>
    /// <example>HolePaddingLeft = 5; // 向左扩展5像素</example>
    [ObservableProperty]
    public partial double HolePaddingLeft { get; set; } = step.HolePadding.Left;

    /// <summary>
    /// Hole区域顶部扩展的内边距
    /// </summary>
    /// <remarks>正值向上扩展Hole范围，使Hole大于控件边界</remarks>
    /// <example>HolePaddingTop = 5; // 向上扩展5像素</example>
    [ObservableProperty]
    public partial double HolePaddingTop { get; set; } = step.HolePadding.Top;

    /// <summary>
    /// Hole区域右侧扩展的内边距
    /// </summary>
    /// <remarks>正值向右扩展Hole范围，使Hole大于控件边界</remarks>
    /// <example>HolePaddingRight = 5; // 向右扩展5像素</example>
    [ObservableProperty]
    public partial double HolePaddingRight { get; set; } = step.HolePadding.Right;

    /// <summary>
    /// Hole区域底部扩展的内边距
    /// </summary>
    /// <remarks>正值向下扩展Hole范围，使Hole大于控件边界</remarks>
    /// <example>HolePaddingBottom = 5; // 向下扩展5像素</example>
    [ObservableProperty]
    public partial double HolePaddingBottom { get; set; } = step.HolePadding.Bottom;

    /// <summary>
    /// Hole区域水平方向的位置偏移量
    /// </summary>
    /// <remarks>正值向右偏移，负值向左偏移，基于控件中心计算</remarks>
    /// <example>HoleOffsetX = 10; // Hole向右偏移10像素</example>
    [ObservableProperty]
    public partial double HoleOffsetX { get; set; } = step.HoleOffset.X;

    /// <summary>
    /// Hole区域垂直方向的位置偏移量
    /// </summary>
    /// <remarks>正值向下偏移，负值向上偏移，基于控件中心计算</remarks>
    /// <example>HoleOffsetY = -2; // Hole向上偏移2像素</example>
    [ObservableProperty]
    public partial double HoleOffsetY { get; set; } = step.HoleOffset.Y;

    // Card config properties
    /// <summary>
    /// 引导卡片的主标题，显示在卡片顶部
    /// </summary>
    /// <remarks>空值时不显示标题区域</remarks>
    /// <example>StepHeader = "欢迎使用";</example>
    [ObservableProperty]
    public partial string StepHeader { get; set; } = step.Config.Header;

    /// <summary>
    /// 引导卡片的主要内容文本
    /// </summary>
    /// <remarks>支持多行文本，建议保持简洁明了</remarks>
    /// <example>StepContent = "点击此按钮创建新项目";</example>
    [ObservableProperty]
    public partial string StepContent { get; set; } = step.Config.Content;

    /// <summary>
    /// 引导卡片右上角的提示信息
    /// </summary>
    /// <remarks>通常用于显示进度信息，如 "1/5"</remarks>
    /// <example>StepTips = "第1步";</example>
    [ObservableProperty]
    public partial string StepTips { get; set; } = step.Config.Tips;

    /// <summary>
    /// 上一步按钮的显示文本
    /// </summary>
    /// <remarks>null值使用默认配置，空字符串隐藏按钮</remarks>
    /// <example>StepPreviousButtonText = "返回";</example>
    [ObservableProperty]
    public partial string? StepPreviousButtonText { get; set; } = step.Config.PreviousButtonText;

    /// <summary>
    /// 跳过按钮的显示文本
    /// </summary>
    /// <remarks>null值使用默认配置，空字符串隐藏按钮</remarks>
    /// <example>StepSkipButtonText = "忽略";</example>
    [ObservableProperty]
    public partial string? StepSkipButtonText { get; set; } = step.Config.SkipButtonText;

    /// <summary>
    /// 下一步按钮的显示文本
    /// </summary>
    /// <remarks>null值使用默认配置，最后一步建议设为"完成"</remarks>
    /// <example>StepNextButtonText = "继续";</example>
    [ObservableProperty]
    public partial string? StepNextButtonText { get; set; } = step.Config.NextButtonText;

    /// <summary>
    /// 卡片的样式类名，用于应用不同的视觉风格
    /// </summary>
    /// <remarks>null使用默认样式，"classic"应用经典风格</remarks>
    /// <example>StepStyleClass = "classic";</example>
    [ObservableProperty]
    public partial string? StepStyleClass { get; set; } = step.Config.StyleClass;

    /// <summary>
    /// 控制上一步按钮的显示状态
    /// </summary>
    /// <remarks>null值使用默认配置，通常第一步设为false</remarks>
    /// <example>StepIsPreviousVisible = false; // 第一步隐藏上一步按钮</example>
    [ObservableProperty]
    public partial bool? StepIsPreviousVisible { get; set; } = step.Config.IsPreviousButtonVisible;

    /// <summary>
    /// 控制跳过按钮的显示状态
    /// </summary>
    /// <remarks>null值使用默认配置，设为false隐藏跳过功能</remarks>
    /// <example>StepIsSkipVisible = false; // 强制用户完成此步骤</example>
    [ObservableProperty]
    public partial bool? StepIsSkipVisible { get; set; } = step.Config.IsSkipButtonVisible;

    /// <summary>
    /// 控制下一步按钮的显示状态
    /// </summary>
    /// <remarks>null值使用默认配置，通常都应该显示</remarks>
    /// <example>StepIsNextVisible = true; // 显示下一步按钮</example>
    [ObservableProperty]
    public partial bool? StepIsNextVisible { get; set; } = step.Config.IsNextButtonVisible;

    #endregion

    [ObservableProperty]
    public partial bool IsInheritDefaultConfig { get; set; } = step.Config.IsInheritDefault();

    /// <summary>
    /// 将当前视图模型的数据更新到GuideStep对象中
    /// </summary>
    /// <param name="step">要更新的GuideStep对象</param>
    /// <remarks>调用此方法将所有编辑的数据同步到传入的步骤对象</remarks>
    /// <example>viewModel.Update(ref currentStep);</example>
    public void Update(ref GuideStep step)
    {
        step.Alias = StepAlias;
        step.VisualTree = StepVisualTree;
        step.PreparedMethod = string.IsNullOrWhiteSpace(StepPreparedMethod) ? null : StepPreparedMethod;
        step.FinishMethod = string.IsNullOrWhiteSpace(StepFinishMethod) ? null : StepFinishMethod;

        // Update position and offset
        step.Config.Position = StepPosition;
        step.Config.Offset = new Offset(StepOffsetX, StepOffsetY);

        // Update hole padding
        step.HolePadding = new HolePadding(HolePaddingLeft, HolePaddingTop, HolePaddingRight, HolePaddingBottom);

        // Update hole offset
        step.HoleOffset = new Offset(HoleOffsetX, HoleOffsetY);

        step.AdditionalHoles = new List<string>(StepAdditionalHoles);

        var config = step.Config;
        config.Header = StepHeader;
        config.Content = StepContent;
        config.Tips = StepTips;

        // inherit
        config.PreviousButtonText = IsInheritDefaultConfig ? null : string.IsNullOrWhiteSpace(StepPreviousButtonText) ? null : StepPreviousButtonText;
        config.SkipButtonText = IsInheritDefaultConfig ? null : string.IsNullOrWhiteSpace(StepSkipButtonText) ? null : StepSkipButtonText;
        config.NextButtonText = IsInheritDefaultConfig ? null : string.IsNullOrWhiteSpace(StepNextButtonText) ? null : StepNextButtonText;
        config.StyleClass = IsInheritDefaultConfig ? null : string.IsNullOrWhiteSpace(StepStyleClass) ? null : StepStyleClass;
        config.IsPreviousButtonVisible = IsInheritDefaultConfig ? null : StepIsPreviousVisible;
        config.IsSkipButtonVisible = IsInheritDefaultConfig ? null : StepIsSkipVisible;
        config.IsNextButtonVisible = IsInheritDefaultConfig ? null : StepIsNextVisible;
    }
}