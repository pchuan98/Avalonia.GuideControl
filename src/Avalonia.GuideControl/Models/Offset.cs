using System.Text.Json.Serialization;

namespace Avalonia.GuideControl.Models;

/// <summary>
/// 位置偏移量配置
/// </summary>
/// <remarks>
/// 用于微调元素相对于计算位置的偏移，支持负值，可用于卡片 Hole 等元素
/// </remarks>
public struct Offset
{
    /// <summary>
    /// 水平偏移量（像素）
    /// </summary>
    /// <remarks>正值向右偏移，负值向左偏移</remarks>
    /// <example>X = 10 // 向右偏移10像素</example>
    public double X { get; set; }

    /// <summary>
    /// 垂直偏移量（像素）
    /// </summary>
    /// <remarks>正值向下偏移，负值向上偏移</remarks>
    /// <example>Y = -5 // 向上偏移5像素</example>
    public double Y { get; set; }

    /// <summary>
    /// 初始化新的偏移量实例
    /// </summary>
    /// <param name="x">水平偏移量</param>
    /// <param name="y">垂直偏移量</param>
    public Offset(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// 零偏移量
    /// </summary>
    public static Offset Zero => new(0, 0);

    /// <summary>
    /// 检查是否为零偏移量
    /// </summary>
    [JsonIgnore]
    public readonly bool IsZero => X == 0 && Y == 0;

    /// <summary>
    /// 转换为字符串表示
    /// </summary>
    /// <returns>格式为 "X: {X}, Y: {Y}" 的字符串</returns>
    public readonly override string ToString() => $"X: {X}, Y: {Y}";

    /// <summary>
    /// 偏移量相加
    /// </summary>
    public static Offset operator +(Offset left, Offset right) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// 偏移量相减
    /// </summary>
    public static Offset operator -(Offset left, Offset right) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// 偏移量取反
    /// </summary>
    public static Offset operator -(Offset offset) =>
        new(-offset.X, -offset.Y);

    /// <summary>
    /// 偏移量缩放
    /// </summary>
    public static Offset operator *(Offset offset, double scale) =>
        new(offset.X * scale, offset.Y * scale);

    /// <summary>
    /// 偏移量缩放
    /// </summary>
    public static Offset operator *(double scale, Offset offset) =>
        new(offset.X * scale, offset.Y * scale);
}