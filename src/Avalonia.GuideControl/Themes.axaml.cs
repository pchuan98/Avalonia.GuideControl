using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.GuideControl;

/// <summary>
/// ���������ؼ��������ʽ����
/// </summary>
public class GuideTheme : Styles
{
    /// <summary>
    /// ��ʼ�� GuideTheme ����ʵ��
    /// </summary>
    /// <param name="serviceProvider">�����ṩ��</param>
    public GuideTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);
    }
}