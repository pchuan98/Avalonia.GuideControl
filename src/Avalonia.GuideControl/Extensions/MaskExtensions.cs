using Avalonia.Controls;
using Avalonia.GuideControl.Controls;
using Avalonia.GuideControl.Models;
using Avalonia.Media;

namespace Avalonia.GuideControl.Extensions;

/// <summary>
/// 遮罩扩展方法，提供简化的遮罩创建和管理功能
/// </summary>
/// <remarks>
/// 包含单控件遮罩、多控件遮罩和自定义样式遮罩的扩展方法
/// </remarks>
public static class MaskExtensions
{
    /// <summary>
    /// 预配置的红色遮罩实例，用于控件调试和演示
    /// </summary>
    /// <remarks>
    /// 默认配置：红色背景，50%透明度，不响应鼠标事件
    /// </remarks>
    public static Mask ControlMask { get; } = new()
    {
        Background = Brushes.Red,
        Opacity = 0.2,
        IsHitTestVisible = false
    };

    /// <summary>
    /// 为指定控件显示预配置的调试遮罩
    /// </summary>
    /// <param name="control">目标控件，不能为空</param>
    /// <example>
    /// button.ShowControlMask();
    /// </example>
    public static void ShowControlMask(this Control control) => ControlMask.Show(control);

    public static void HiddenControlMask() => ControlMask.Hidden();
}