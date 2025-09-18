using Avalonia.Controls;

namespace Avalonia.GuideControl.Models;

/// <summary>
/// 存储控件的引导信息，包括控件类型、名称、位置和树结构信息
/// </summary>
/// <remarks>
/// 包含运行时控件引用，序列化时会忽略不可序列化的属性
/// </remarks>
public class ControlInfo
{
    /// <summary>
    /// 控件的类型名称，用于标识控件的种类
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 控件的元素名称，可为空
    /// </summary>
    public string? ElementName { get; set; }

    /// <summary>
    /// 控件的作用域范围，用于限定引导的上下文
    /// </summary>
    /// <remarks>运行时控件引用，不参与 JSON 序列化</remarks>
    public Control? Scope { get; set; }

    /// <summary>
    /// 控件的视觉树结构信息，以字符串形式表示
    /// </summary>
    public string? VisualTree { get; set; }

    /// <summary>
    /// 控件的逻辑树结构信息，以字符串形式表示
    /// </summary>
    public string? LogicTree { get; set; }

    /// <summary>
    /// 控件在屏幕上的位置和尺寸信息
    /// </summary>
    public Rect Location { get; set; }

    public Hole AsHole() => new(Location);

    public override string ToString()
    {
        var result = $"类型: {TypeName}\n";
        result += $"名称: {ElementName ?? "未命名"}\n";
        result += $"位置: {Location.X:F0}, {Location.Y:F0}\n";
        result += $"尺寸: {Location.Width:F0} x {Location.Height:F0}\n";
        
        if (!string.IsNullOrEmpty(VisualTree))
            result += $"视觉树: {VisualTree}\n";
            
        if (!string.IsNullOrEmpty(LogicTree))
            result += $"逻辑树: {LogicTree}\n";
            
        return result.TrimEnd('\n');
    }
}