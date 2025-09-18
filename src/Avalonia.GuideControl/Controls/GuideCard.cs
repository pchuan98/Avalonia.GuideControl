using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using DependencyPropertyGenerator;
using System.Windows.Input;

namespace Avalonia.GuideControl.Controls;

/// <summary>
/// 引导卡片控件，显示引导内容、标题、提示和三个导航按钮（上一步、下一步、跳过）
/// </summary>
/// <remarks>
/// 这是一个基于模板的控件，通过依赖属性支持数据绑定和命令模式。
/// 按钮的显示状态和文本内容都可以通过属性进行自定义配置。
/// </remarks>
/// <example>
/// var card = new GuideCard { Header = "步骤1", Content = "点击按钮继续" };
/// card.NextClick += (s, e) => ShowNextStep();
/// </example>
[DependencyProperty<string>("Header")]
[DependencyProperty<string>("Tips")]
[DependencyProperty<object>("Content")]
[DependencyProperty<string>("PreviousButtonText", DefaultValue = "上一步")]
[DependencyProperty<string>("NextButtonText", DefaultValue = "下一步")]
[DependencyProperty<string>("SkipButtonText", DefaultValue = "跳过")]
[DependencyProperty<bool>("IsPreviousButtonVisible", DefaultValue = true)]
[DependencyProperty<bool>("IsNextButtonVisible", DefaultValue = true)]
[DependencyProperty<bool>("IsSkipButtonVisible", DefaultValue = true)]
[DependencyProperty<ICommand>("PreviousCommand")]
[DependencyProperty<ICommand>("NextCommand")]
[DependencyProperty<ICommand>("SkipCommand")]
public partial class GuideCard : TemplatedControl
{
    /// <summary>上一步按钮的模板部件名称</summary>
    private const string PART_PreviousButton = "PART_PreviousButton";
    /// <summary>下一步按钮的模板部件名称</summary>
    private const string PART_NextButton = "PART_NextButton";
    /// <summary>跳过按钮的模板部件名称</summary>
    private const string PART_SkipButton = "PART_SkipButton";

    /// <summary>上一步按钮的实例引用，在模板应用时获取</summary>
    private Button? _previousButton;
    /// <summary>下一步按钮的实例引用，在模板应用时获取</summary>
    private Button? _nextButton;
    /// <summary>跳过按钮的实例引用，在模板应用时获取</summary>
    private Button? _skipButton;

    /// <summary>
    /// 上一步按钮点击事件，在执行 PreviousCommand 之后触发
    /// </summary>
    /// <example>card.PreviousClick += (s, e) => NavigateToPrevious();</example>
    public event EventHandler<RoutedEventArgs>? PreviousClick;
    
    /// <summary>
    /// 下一步按钮点击事件，在执行 NextCommand 之后触发
    /// </summary>
    /// <example>card.NextClick += (s, e) => NavigateToNext();</example>
    public event EventHandler<RoutedEventArgs>? NextClick;
    
    /// <summary>
    /// 跳过按钮点击事件，在执行 SkipCommand 之后触发
    /// </summary>
    /// <example>card.SkipClick += (s, e) => SkipGuide();</example>
    public event EventHandler<RoutedEventArgs>? SkipClick;

    /// <summary>
    /// 应用控件模板时获取按钮实例并绑定事件处理器
    /// </summary>
    /// <param name="e">模板应用事件参数，包含命名作用域用于查找模板部件</param>
    /// <remarks>
    /// 此方法会先解除旧按钮的事件绑定，然后重新获取按钮实例并绑定新的事件处理器。
    /// 如果模板中缺少某个按钮部件，对应的字段将为 null，不会影响其他按钮的正常工作。
    /// </remarks>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_previousButton != null)
            _previousButton.Click -= OnPreviousButtonClick;
        if (_nextButton != null)
            _nextButton.Click -= OnNextButtonClick;
        if (_skipButton != null)
            _skipButton.Click -= OnSkipButtonClick;

        _previousButton = e.NameScope.Get<Button>(PART_PreviousButton);
        _nextButton = e.NameScope.Get<Button>(PART_NextButton);
        _skipButton = e.NameScope.Get<Button>(PART_SkipButton);

        if (_previousButton != null)
            _previousButton.Click += OnPreviousButtonClick;
        if (_nextButton != null)
            _nextButton.Click += OnNextButtonClick;
        if (_skipButton != null)
            _skipButton.Click += OnSkipButtonClick;
    }

    /// <summary>
    /// 处理上一步按钮的点击事件，先执行命令再触发公共事件
    /// </summary>
    /// <param name="sender">事件发送者，通常是按钮实例</param>
    /// <param name="e">路由事件参数</param>
    private void OnPreviousButtonClick(object? sender, RoutedEventArgs e)
    {
        PreviousCommand?.Execute(null);
        PreviousClick?.Invoke(this, e);
    }

    /// <summary>
    /// 处理下一步按钮的点击事件，先执行命令再触发公共事件
    /// </summary>
    /// <param name="sender">事件发送者，通常是按钮实例</param>
    /// <param name="e">路由事件参数</param>
    private void OnNextButtonClick(object? sender, RoutedEventArgs e)
    {
        NextCommand?.Execute(null);
        NextClick?.Invoke(this, e);
    }

    /// <summary>
    /// 处理跳过按钮的点击事件，先执行命令再触发公共事件
    /// </summary>
    /// <param name="sender">事件发送者，通常是按钮实例</param>
    /// <param name="e">路由事件参数</param>
    private void OnSkipButtonClick(object? sender, RoutedEventArgs e)
    {
        SkipCommand?.Execute(null);
        SkipClick?.Invoke(this, e);
    }
}

