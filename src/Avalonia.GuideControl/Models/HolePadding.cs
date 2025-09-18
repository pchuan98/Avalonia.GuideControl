using System.Text.Json.Serialization;

namespace Avalonia.GuideControl.Models;

/// <summary>
/// Hole 区域内边距扩展配置
/// </summary>
/// <remarks>
/// 用于扩展 Hole 显示区域，使其超出目标控件的边界
/// </remarks>
public struct HolePadding
{
    /// <summary>
    /// 左侧内边距（像素）
    /// </summary>
    /// <remarks>正值向左扩展 Hole 区域</remarks>
    /// <example>Left = 5 // 向左扩展5像素</example>
    public double Left { get; set; }

    /// <summary>
    /// 顶部内边距（像素）
    /// </summary>
    /// <remarks>正值向上扩展 Hole 区域</remarks>
    /// <example>Top = 5 // 向上扩展5像素</example>
    public double Top { get; set; }

    /// <summary>
    /// 右侧内边距（像素）
    /// </summary>
    /// <remarks>正值向右扩展 Hole 区域</remarks>
    /// <example>Right = 5 // 向右扩展5像素</example>
    public double Right { get; set; }

    /// <summary>
    /// 底部内边距（像素）
    /// </summary>
    /// <remarks>正值向下扩展 Hole 区域</remarks>
    /// <example>Bottom = 5 // 向下扩展5像素</example>
    public double Bottom { get; set; }

    /// <summary>
    /// 初始化新的 Hole 内边距实例
    /// </summary>
    /// <param name="left">左侧内边距</param>
    /// <param name="top">顶部内边距</param>
    /// <param name="right">右侧内边距</param>
    /// <param name="bottom">底部内边距</param>
    public HolePadding(double left, double top, double right, double bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// 初始化统一内边距实例
    /// </summary>
    /// <param name="uniformPadding">统一的内边距值</param>
    public HolePadding(double uniformPadding)
    {
        Left = Top = Right = Bottom = uniformPadding;
    }

    /// <summary>
    /// 零内边距
    /// </summary>
    public static HolePadding Zero => new(0, 0, 0, 0);

    /// <summary>
    /// 统一内边距
    /// </summary>
    /// <param name="padding">统一的内边距值</param>
    /// <returns>统一内边距实例</returns>
    public static HolePadding Uniform(double padding) => new(padding);

    /// <summary>
    /// 检查是否为零内边距
    /// </summary>
    [JsonIgnore]
    public readonly bool IsZero => Left == 0 && Top == 0 && Right == 0 && Bottom == 0;

    /// <summary>
    /// 获取水平内边距总和（左+右）
    /// </summary>
    [JsonIgnore]
    public readonly double HorizontalPadding => Left + Right;

    /// <summary>
    /// 获取垂直内边距总和（上+下）
    /// </summary>
    [JsonIgnore]
    public readonly double VerticalPadding => Top + Bottom;

    /// <summary>
    /// 转换为字符串表示
    /// </summary>
    /// <returns>格式为 "Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}" 的字符串</returns>
    public readonly override string ToString() => $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";

    /// <summary>
    /// 内边距相加
    /// </summary>
    public static HolePadding operator +(HolePadding left, HolePadding right) =>
        new(left.Left + right.Left, left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom);

    /// <summary>
    /// 内边距相减
    /// </summary>
    public static HolePadding operator -(HolePadding left, HolePadding right) =>
        new(left.Left - right.Left, left.Top - right.Top, left.Right - right.Right, left.Bottom - right.Bottom);

    /// <summary>
    /// 内边距缩放
    /// </summary>
    public static HolePadding operator *(HolePadding padding, double scale) =>
        new(padding.Left * scale, padding.Top * scale, padding.Right * scale, padding.Bottom * scale);

    /// <summary>
    /// 内边距缩放
    /// </summary>
    public static HolePadding operator *(double scale, HolePadding padding) =>
        new(padding.Left * scale, padding.Top * scale, padding.Right * scale, padding.Bottom * scale);
}