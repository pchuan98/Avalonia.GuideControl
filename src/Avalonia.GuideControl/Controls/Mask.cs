using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.GuideControl.Models;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using DependencyPropertyGenerator;

namespace Avalonia.GuideControl.Controls;

/// <summary>
/// 创建带透明 Hole 的遮罩层，常用于用户引导和高亮显示特定控件
/// </summary>
/// <remarks>
/// 遮罩会覆盖整个窗口并自动置于最顶层，通过设置 HoleItems 在指定区域挖出透明 Hole 
/// </remarks>
/// <example>
/// var mask = new Mask();
/// mask.HoleItems = [new Hole(new Rect(100, 100, 200, 150))];
/// </example>
[DependencyProperty<IEnumerable<Hole>>("HoleItems")]
[DependencyProperty<IBrush>("MaskColor")]
[DependencyProperty<bool>("IsShow", IsReadOnly = true)]
public partial class Mask : Canvas, IDisposable
{
    /// <summary>
    /// 创建遮罩实例并可选择性地显示在指定控件上
    /// </summary>
    /// <param name="control">目标控件，如果为 null 则覆盖整个主窗口</param>
    /// <remarks>构造时会自动设置半透明黑色背景</remarks>
    public Mask(Control? control = null)
    {
        MaskColor = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };
    }

    /// <summary>
    /// 显示遮罩并将其添加到窗口的最顶层
    /// </summary>
    /// <param name="control">目标控件，如果为 null 则自动使用主窗口内容</param>
    /// <remarks>
    /// 遮罩会自动计算位置和大小以覆盖指定控件，并添加到窗口顶层确保始终可见
    /// </remarks>
    /// <example>
    /// mask.Show(myButton); // 覆盖特定按钮
    /// mask.Show(); // 覆盖整个窗口
    /// </example>
    public void Show(Control? control = null)
    {
        var window = control.FindLogicalAncestorOfType<Window>()
                     ?? ((IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime)?.MainWindow;

        if (window is null) return;
        if (control is null && window.Content is Control windowContent)
            control = windowContent;

        if (control is null) return;

        var bounds = control.Bounds;
        var transform = control.TransformToVisual(window);
        if (transform == null) return;

        var position = new Point(0, 0).Transform(transform.Value);

        // 设置遮罩位置和大小匹配控件
        Margin = new Thickness(position.X, position.Y, 0, 0);
        Width = bounds.Width;
        Height = bounds.Height;
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Top;

        if (Parent is Popup { Parent: Panel root })
            root.ClearTopmost();

        AvaloniaUtils.TopmostAppend(window, this);
        IsShow = true;
    }

    /// <summary>
    /// 隐藏遮罩并从父容器中移除
    /// </summary>
    /// <remarks>
    /// 从窗口中移除遮罩但不清理资源，可通过 Show 方法重新显示
    /// </remarks>
    /// <example>
    /// mask.Hidden(); // 隐藏遮罩
    /// mask.Show(); // 重新显示
    /// </example>
    public void Hidden(Control? control = null)
    {
        var window = control.FindLogicalAncestorOfType<Window>()
                     ?? ((IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime)?.MainWindow;

        if (window is null) return;
        if (control is null && window.Content is Control windowContent)
            control = windowContent;

        if (control is null) return;
        AvaloniaUtils.TopmostRemove(window, this);
        IsShow = false;
    }

    /// <summary>
    /// 当 Hole 列表发生变化时自动重新绘制遮罩和 Hole 
    /// </summary>
    /// <remarks>
    /// 使用几何布尔运算从全屏遮罩中减去所有 Hole 区域，每个 Hole 可单独设置边框样式
    /// 保留非Path类型的子元素（如GuideCard）
    /// </remarks>
    partial void OnHoleItemsChanged()
    {
        // 保存非Path类型的子元素（如GuideCard）
        var nonPathChildren = Children
            .Where(child => child is not Avalonia.Controls.Shapes.Path)
            .ToList();

        // 清除所有子元素
        Children.Clear();

        if (HoleItems == null || !HoleItems.Any())
        {
            // 重新添加保存的非Path元素
            foreach (var child in nonPathChildren)
            {
                Children.Add(child);
            }
            return;
        }

        if (!IsShow) Show();

        // 获取当前Canvas的宽度和高度
        var width = Width;
        var height = Height;

        // 创建全尺寸矩形
        var fullRect = new RectangleGeometry(new Rect(0, 0, width, height));

        // 从全尺寸矩形中排除所有 Hole 
        var currentGeometry = (
            from hole in HoleItems
            let cornerRadius = hole.CornerRadius.TopLeft
            select new RectangleGeometry(hole.Bounds, cornerRadius, cornerRadius))
            .Aggregate<RectangleGeometry, Geometry>(fullRect,
                (current, holeRect) => new CombinedGeometry(GeometryCombineMode.Exclude, current, holeRect));

        // 创建遮罩路径
        var maskPath = new Avalonia.Controls.Shapes.Path
        {
            Data = currentGeometry,
            Fill = MaskColor,
            IsHitTestVisible = true
        };
        Children.Add(maskPath);

        // 为每个 Hole 添加边框
        foreach (var hole in HoleItems)
        {
            if (hole.BorderBrush == null) continue;

            var cornerRadius = hole.CornerRadius.TopLeft;
            var borderGeometry = new RectangleGeometry(hole.Bounds, cornerRadius, cornerRadius);
            var highlightBorder = new Avalonia.Controls.Shapes.Path
            {
                Data = borderGeometry,
                Fill = Brushes.Transparent,
                Stroke = hole.BorderBrush,
                StrokeThickness = hole.BorderThickness,
                IsHitTestVisible = hole.IsHitTestVisible
            };
            Children.Add(highlightBorder);
        }

        // 重新添加保存的非Path元素（确保它们在最上层）
        foreach (var child in nonPathChildren)
        {
            Children.Add(child);
        }
    }

    /// <summary>
    /// 清理遮罩资源并从父容器中移除
    /// </summary>
    /// <remarks>
    /// 清除所有绘制的路径，从窗口中移除遮罩，并重置所有属性以避免内存泄漏
    /// </remarks>
    /// <example>
    /// using var mask = new Mask();
    /// // 或手动调用 mask.Dispose();
    /// </example>
    public void Dispose()
    {
        // 清除所有子元素
        Children.Clear();

        // 从父容器中移除自己
        if (Parent is Panel parentPanel)
        {
            parentPanel.Children.Remove(this);
        }

        // 清理依赖属性
        HoleItems = null;
        MaskColor = null;

        // 重置布局属性
        Width = double.NaN;
        Height = double.NaN;
        Margin = new Thickness(0);

        GC.SuppressFinalize(this);
    }
}