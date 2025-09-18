using Avalonia.GuideControl.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.GuideControl.DevTools.Record;

/// <summary>
/// 管理引导卡片默认配置的视图模型，用于设置全局按钮显示状态和文本内容
/// </summary>
/// <remarks>修改这些属性会影响所有使用默认配置的引导卡片</remarks>
internal partial class DefaultCardViewModel(Guide guide) : ObservableObject
{
    public DefaultCardViewModel() : this(new Guide()) { }
    /// <summary>
    /// 控制"上一步"按钮是否显示
    /// </summary>
    /// <example>viewModel.DefaultIsPreviousVisible = false; // 隐藏上一步按钮</example>
    [ObservableProperty]
    public partial bool DefaultIsPreviousVisible { get; set; } = guide.DefaultCardConfig.IsPreviousButtonVisible ?? false;

    /// <summary>
    /// 控制"跳过"按钮是否显示
    /// </summary>
    /// <example>viewModel.DefaultIsSkipVisible = false; // 隐藏跳过按钮</example>
    [ObservableProperty]
    public partial bool DefaultIsSkipVisible { get; set; } = guide.DefaultCardConfig.IsSkipButtonVisible ?? false;

    /// <summary>
    /// 控制"下一步"按钮是否显示
    /// </summary>
    /// <example>viewModel.DefaultIsNextVisible = false; // 隐藏下一步按钮</example>
    [ObservableProperty]
    public partial bool DefaultIsNextVisible { get; set; } = guide.DefaultCardConfig.IsNextButtonVisible ?? false;

    /// <summary>
    /// 设置"上一步"按钮的显示文本
    /// </summary>
    /// <remarks>不能为null，建议保持简短文字</remarks>
    /// <example>viewModel.DefaultPreviousButtonText = "返回";</example>
    [ObservableProperty]
    public partial string DefaultPreviousButtonText { get; set; } = guide.DefaultCardConfig.PreviousButtonText ?? "Previous";

    /// <summary>
    /// 设置"跳过"按钮的显示文本
    /// </summary>
    /// <remarks>不能为null，建议保持简短文字</remarks>
    /// <example>viewModel.DefaultSkipButtonText = "忽略";</example>
    [ObservableProperty]
    public partial string DefaultSkipButtonText { get; set; } = guide.DefaultCardConfig.SkipButtonText ?? "Skip";

    /// <summary>
    /// 设置"下一步"按钮的显示文本
    /// </summary>
    /// <remarks>不能为null，建议保持简短文字</remarks>
    /// <example>viewModel.DefaultNextButtonText = "继续";</example>
    [ObservableProperty]
    public partial string DefaultNextButtonText { get; set; } = guide.DefaultCardConfig.NextButtonText ?? "Next";

    /// <summary>
    /// 设置卡片默认样式类名，用于应用不同的视觉风格
    /// </summary>
    /// <remarks>空字符串表示使用默认样式，"classic"表示经典样式</remarks>
    /// <example>viewModel.DefaultStyleClass = "classic";</example>
    [ObservableProperty]
    public partial string DefaultStyleClass { get; set; } = guide.DefaultCardConfig.StyleClass ?? "classic";

    /// <summary>
    /// 将当前视图模型的数据更新到Guide对象的默认卡片配置中
    /// </summary>
    /// <param name="guide">要更新的Guide对象</param>
    public void Update(ref Guide guide)
    {
        guide.DefaultCardConfig.IsPreviousButtonVisible = DefaultIsPreviousVisible;
        guide.DefaultCardConfig.IsSkipButtonVisible = DefaultIsSkipVisible;
        guide.DefaultCardConfig.IsNextButtonVisible = DefaultIsNextVisible;

        guide.DefaultCardConfig.PreviousButtonText = string.IsNullOrWhiteSpace(DefaultPreviousButtonText) ? null : DefaultPreviousButtonText;
        guide.DefaultCardConfig.SkipButtonText = string.IsNullOrWhiteSpace(DefaultSkipButtonText) ? null : DefaultSkipButtonText;
        guide.DefaultCardConfig.NextButtonText = string.IsNullOrWhiteSpace(DefaultNextButtonText) ? null : DefaultNextButtonText;

        guide.DefaultCardConfig.StyleClass = string.IsNullOrWhiteSpace(DefaultStyleClass) ? null : DefaultStyleClass;
    }
}