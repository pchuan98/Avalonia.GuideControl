using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.GuideControl;

/// <summary>
/// 包含引导控件主题的样式集合
/// </summary>
public class GuideTheme : Styles
{
    /// <summary>
    /// 初始化 GuideTheme 的新实例
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public GuideTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);
    }
}