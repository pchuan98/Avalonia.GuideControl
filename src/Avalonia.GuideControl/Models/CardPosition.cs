using System.Text.Json.Serialization;

namespace Avalonia.GuideControl.Models;

/// <summary>
/// 引导卡片相对于目标控件的位置枚举
/// </summary>
/// <remarks>
/// 支持位操作符组合，除了Center以外的其他值都可以组合使用
/// Center表示屏幕中央，不与其他位置组合
/// </remarks>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<CardPosition>))]
public enum CardPosition
{
    /// <summary>
    /// 无位置（默认）
    /// </summary>
    None = 0,

    /// <summary>
    /// 左侧位置
    /// </summary>
    Left = 1 << 0,

    /// <summary>
    /// 顶部位置
    /// </summary>
    Top = 1 << 1,

    /// <summary>
    /// 右侧位置
    /// </summary>
    Right = 1 << 2,

    /// <summary>
    /// 底部位置
    /// </summary>
    Bottom = 1 << 3,

    /// <summary>
    /// 屏幕中央位置（不与其他位置组合）
    /// </summary>
    Center = 1 << 4,

    // 常用组合位置
    /// <summary>
    /// 左上角
    /// </summary>
    TopLeft = Top | Left,

    /// <summary>
    /// 右上角
    /// </summary>
    TopRight = Top | Right,

    /// <summary>
    /// 左下角
    /// </summary>
    BottomLeft = Bottom | Left,

    /// <summary>
    /// 右下角
    /// </summary>
    BottomRight = Bottom | Right
}