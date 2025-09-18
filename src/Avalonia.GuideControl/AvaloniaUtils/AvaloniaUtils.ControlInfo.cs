// ReSharper disable InvertIf

using Avalonia.Controls;
using Avalonia.GuideControl.Models;
using Avalonia.LogicalTree;

namespace Avalonia.GuideControl;

/// <summary>
/// 控件信息相关的工具方法
/// </summary>
/// <remarks>
/// 提供控件信息提取和匹配功能，支持多种匹配方式
/// </remarks>
public static partial class AvaloniaUtils
{
    /// <summary>
    /// 获取控件的详细信息
    /// </summary>
    /// <param name="control">目标控件，不能为null</param>
    /// <param name="scope">作用域控件，用于限定搜索范围</param>
    /// <returns>包含控件信息的ControlInfo对象</returns>
    /// <remarks>
    /// 此方法当前未实现，将在后续版本中提供
    /// </remarks>
    /// <example>
    /// var info = myButton.Info(mainWindow);
    /// </example>
    public static ControlInfo Info(this Control control, Control? scope)
    {
        var window = scope?.FindLogicalAncestorOfType<Window>() 
                     ?? control.FindLogicalAncestorOfType<Window>();
        var location = control.Bounds;
        
        // 计算控件相对于窗口的位置
       
        if (window != null)
        {
            var transform = control.TransformToVisual(window);
            if (transform != null)
            {
                var position = new Point(0, 0).Transform(transform.Value);
                location = new Rect(position.X, position.Y, control.Bounds.Width, control.Bounds.Height);
            }
        }

        return new ControlInfo
        {
            TypeName = control.GetType().Name,
            ElementName = control.Name,
            Scope = scope,
            VisualTree = control.VisualTreeString(scope),
            LogicTree = control.LogicTreeString(scope),
            Location = location
        };
    }

    /// <summary>
    /// 检查控件名称是否与指定信息匹配
    /// </summary>
    /// <param name="control">要检查的控件</param>
    /// <param name="info">包含匹配条件的控件信息</param>
    /// <returns>是否匹配成功</returns>
    /// <remarks>
    /// 此方法当前未实现，将在后续版本中提供
    /// </remarks>
    public static bool CheckInfoName(this Control control, ControlInfo info)
    {
        // 检查类型名称
        if (control.GetType().Name != info.TypeName)
            return false;

        // 检查元素名称
        return string.IsNullOrEmpty(info.ElementName)
               || string.Equals(control.Name, info.ElementName, StringComparison.Ordinal);
    }

    /// <summary>
    /// 检查控件的视觉树路径是否与指定信息匹配
    /// </summary>
    /// <param name="control">要检查的控件</param>
    /// <param name="info">包含匹配条件的控件信息</param>
    /// <returns>是否匹配成功</returns>
    /// <remarks>
    /// 此方法当前未实现，将在后续版本中提供
    /// </remarks>
    public static bool CheckInfoVisualPath(this Control control, ControlInfo info)
    {
        if (string.IsNullOrEmpty(info.VisualTree))
            return true;

        var currentVisualPath = control.VisualTreeString(info.Scope);
        var visualControl = FromVisualTree(info.VisualTree, info.Scope);

        return string.Equals(currentVisualPath, info.VisualTree, StringComparison.Ordinal)
            && control == visualControl;
    }

    /// <summary>
    /// 检查控件的逻辑树路径是否与指定信息匹配
    /// </summary>
    /// <param name="control">要检查的控件</param>
    /// <param name="info">包含匹配条件的控件信息</param>
    /// <returns>是否匹配成功</returns>
    /// <remarks>
    /// 此方法当前未实现，将在后续版本中提供
    /// </remarks>
    public static bool CheckInfoLogicPath(this Control control, ControlInfo info)
    {
        if (string.IsNullOrEmpty(info.LogicTree))
            return true;

        var currentLogicPath = control.LogicTreeString(info.Scope);
        var logicControl = FromLogicTree(info.LogicTree, info.Scope);
        return string.Equals(currentLogicPath, info.LogicTree, StringComparison.Ordinal)
               && control == logicControl;
    }

    /// <summary>
    /// 检查控件的位置和大小是否与指定信息匹配
    /// </summary>
    /// <param name="control">要检查的控件</param>
    /// <param name="info">包含匹配条件的控件信息</param>
    /// <returns>是否匹配成功</returns>
    /// <remarks>
    /// 此方法当前未实现，将在后续版本中提供
    /// </remarks>
    public static bool CheckInfoRectPath(this Control control, ControlInfo info)
    {
        const double tolerance = 1.0;

        return Math.Abs(control.Bounds.X - info.Location.X) <= tolerance &&
               Math.Abs(control.Bounds.Y - info.Location.Y) <= tolerance &&
               Math.Abs(control.Bounds.Width - info.Location.Width) <= tolerance &&
               Math.Abs(control.Bounds.Height - info.Location.Height) <= tolerance;
    }
}