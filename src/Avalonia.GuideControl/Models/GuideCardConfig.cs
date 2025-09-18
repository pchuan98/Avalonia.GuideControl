namespace Avalonia.GuideControl.Models;

/// <summary>
/// 引导卡片的配置信息，定义卡片的显示内容、样式和交互行为
/// </summary>
/// <remarks>
/// 配置引导卡片的标题、内容、按钮文本以及显示样式等属性
/// </remarks>
/// <example>
/// var config = new GuidCardConfig
/// {
///     Header = "欢迎使用",
///     Content = "这是您的第一个项目",
///     NextButtonText = "下一步",
///     IsSkipButtonVisible = true
/// };
/// </example>
public class GuideCardConfig
{
    /// <summary>
    /// 卡片标题文本，显示在卡片顶部
    /// </summary>
    /// <remarks>为空时不显示标题区域</remarks>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// 卡片主要内容，可以是纯文本或包含格式的内容
    /// </summary>
    /// <remarks>支持多行文本，建议保持简洁明了</remarks>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 右上角提示文本，通常用于显示进度信息
    /// </summary>
    /// <remarks>如 "1/5"、"第一步" 等进度提示</remarks>
    public string Tips { get; set; } = string.Empty;

    /// <summary>
    /// "上一步" 按钮的显示文本
    /// </summary>
    public string? PreviousButtonText { get; set; }

    /// <summary>
    /// "跳过" 按钮的显示文本
    /// </summary>
    public string? SkipButtonText { get; set; }

    /// <summary>
    /// "下一步" 按钮的显示文本
    /// </summary>
    public string? NextButtonText { get; set; }

    /// <summary>
    /// 是否显示 "上一步" 按钮
    /// </summary>
    /// <remarks>通常第一步不显示此按钮</remarks>
    public bool? IsPreviousButtonVisible { get; set; }

    /// <summary>
    /// 是否显示 "跳过" 按钮
    /// </summary>
    /// <remarks>允许用户跳过当前引导步骤</remarks>
    public bool? IsSkipButtonVisible { get; set; }

    /// <summary>
    /// 是否显示 "下一步" 按钮
    /// </summary>
    /// <remarks>通常最后一步显示为 "完成"</remarks>
    public bool? IsNextButtonVisible { get; set; }

    /// <summary>
    /// 卡片的样式类名，用于应用不同的视觉样式
    /// </summary>
    /// <remarks>如 "classic" 应用经典样式，为空则使用默认样式</remarks>
    /// <example>StyleClass = "classic"</example>
    public string? StyleClass { get; set; }

    /// <summary>
    /// 引导卡片相对于目标控件的位置
    /// </summary>
    /// <remarks>
    /// 支持位置组合，Center表示屏幕中央，其他位置可以使用位操作符组合
    /// </remarks>
    /// <example>Position = CardPosition.Top | CardPosition.Left</example>
    public CardPosition Position { get; set; } = CardPosition.Bottom;

    /// <summary>
    /// 引导卡片相对于计算位置的偏移量
    /// </summary>
    /// <remarks>用于微调卡片位置，X为水平偏移，Y为垂直偏移</remarks>
    /// <example>Offset = new Offset { X = 10, Y = -5 }</example>
    public Offset Offset { get; set; } = new();

    public static GuideCardConfig Default => new()
    {
        PreviousButtonText = "Previous",
        SkipButtonText = "Skip",
        NextButtonText = "Next",
        IsPreviousButtonVisible = false,
        IsSkipButtonVisible = false,
        IsNextButtonVisible = false,
        StyleClass = "classic",
        Position = CardPosition.Bottom,
        Offset = new Offset()
    };

    public bool IsInheritDefault()
        => PreviousButtonText is null
           && SkipButtonText is null
           && NextButtonText is null
           && IsPreviousButtonVisible is null
           && IsSkipButtonVisible is null
           && IsNextButtonVisible is null
           && StyleClass is null;
}