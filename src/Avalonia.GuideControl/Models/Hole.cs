using Avalonia.Media;

namespace Avalonia.GuideControl.Models;

/// <summary>
/// Hole 区域配置，用于定义遮罩层中的透明区域
/// </summary>
/// <remarks>
/// 包含 Avalonia 特定类型，序列化时会忽略不可序列化的属性如 BorderBrush
/// </remarks>
public class Hole(Rect bounds)
{
    /// <summary>
    /// Hole 区域的位置和大小
    /// </summary>
    public Rect Bounds { get; set; } = bounds;

    /// <summary>
    /// Hole 的圆角半径
    /// </summary>
    public CornerRadius CornerRadius { get; set; } = new(8);

    /// <summary>
    /// Hole 边框颜色
    /// </summary>
    public IBrush? BorderBrush { get; set; } = Brushes.Black;

    /// <summary>
    /// Hole 边框颜色的字符串表示，用于序列化
    /// </summary>
    public string? BorderBrushColor { get; set; } = "Black";

    /// <summary>
    /// Hole 边框粗细
    /// </summary>
    public double BorderThickness { get; set; } = 0.5;

    /// <summary>
    /// 是否启用 Hole 区域的事件穿透
    /// </summary>
    public bool IsHitTestVisible { get; set; } = false;
}

