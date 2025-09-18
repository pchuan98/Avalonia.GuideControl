using Avalonia.Controls;

namespace Avalonia.GuideControl;

public static partial class AvaloniaUtils
{
    /// <summary>
    /// 用于测量控件尺寸的隐藏窗口，避免在主界面显示测量过程
    /// </summary>
    /// <remarks>
    /// 窗口设置为最小化且不可见状态，专门用于控件尺寸计算
    /// </remarks>
    internal static Window HiddenWindow = new()
    {
        Width = 10000,
        Height = 10000,
        ShowInTaskbar = false,
        WindowState = WindowState.Minimized,
        IsVisible = false
    };

    /// <summary>
    /// 测量控件在给定无限空间下的期望尺寸
    /// </summary>
    /// <param name="control">要测量的控件，不能为 null</param>
    /// <returns>控件的期望尺寸，包含宽度和高度</returns>
    /// <remarks>
    /// 通过临时将控件放入隐藏窗口进行测量，不会影响原有布局
    /// </remarks>
    /// <example>
    /// var card = new GuideCard();
    /// var size = card.MeasureDesiredSize();
    /// </example>
    public static Size MeasureDesiredSize(this Control control)
    {
        HiddenWindow.Content = control;

        control.ApplyTemplate();
        control.UpdateLayout();
        control.Measure(Size.Infinity);

        return control.DesiredSize;
    }
}