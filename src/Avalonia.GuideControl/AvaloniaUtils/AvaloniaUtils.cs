using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace Avalonia.GuideControl;

public static partial class AvaloniaUtils
{
    /// <summary>
    /// 清除面板中所有由 TopmostAppend 创建的遮罩弹出层
    ///
    /// HACK: 由于这种实现的方式，调用 TopmostAppend 的方式的回收方式会导致每次开始都会清除所有旧的popup
    /// </summary>
    /// <param name="panel">要清理的面板容器</param>
    /// <remarks>
    /// 只清除名称包含 "GuideTopmost" 的 Popup 控件，避免误删其他弹出层
    /// </remarks>
    /// <example>
    /// mainPanel.ClearTopmost();
    /// </example>
    public static void ClearTopmost(this Panel panel)
    {
        var children = panel.Children.ToList();
        foreach (var child in children)
        {
            if (child is not Popup { Name: not null } popup
                || !popup.Name.Contains("GuideTopmost")) continue;

            popup.IsOpen = false;
            popup.Child = null;

            panel.Children.Remove(popup);
        }
    }

    /// <summary>
    /// 将控件添加到窗口内容的最顶层，适配各种布局容器
    /// </summary>
    /// <param name="window">目标窗口，不能为空</param>
    /// <param name="control">要添加的控件，不能为空</param>
    /// <remarks>
    /// 使用 Popup 包装控件以确保在所有层级之上显示，自动适配窗口大小变化
    /// </remarks>
    /// <exception cref="NotSupportedException">当窗口内容不是 Grid 或 Panel 类型时抛出</exception>
    /// <example>
    /// AvaloniaUtils.TopmostAppend(mainWindow, overlayControl);
    /// </example>
    public static void TopmostAppend(Window window, Control control)
    {
        // must new one
        var popup = new Popup
        {
            Name = $"GuideTopmost_{DateTime.Now.Ticks}",
            IsOpen = true,
            PlacementTarget = window,
            Placement = PlacementMode.Pointer,
            Width = window.Width,
            Height = window.Height,
            Child = control
        };

        window.SizeChanged += (_, e) =>
        {
            popup.Width = e.NewSize.Width;
            popup.Height = e.NewSize.Height;
        };

        var content = window.Content;
        if (content is null) return;

        if (content.GetType() == typeof(Grid))
        {
            var grid = (Grid)content;
            Grid.SetColumnSpan(popup, Math.Max(grid.ColumnDefinitions.Count, 1));
            Grid.SetRowSpan(popup, Math.Max(grid.RowDefinitions.Count, 1));
            grid.Children.Add(popup);
        }
        else if (content.GetType() == typeof(Panel))
            ((Panel)content).Children.Add(popup);
        else throw new NotSupportedException();
    }

    public static void TopmostRemove(Window window, Control control)
    {
        if (window.Content is not Panel panel) return;

        var children = panel.Children.ToList();
        foreach (var child in children)
        {
            if (child is not Popup popup || popup.Child != control) continue;

            popup.IsOpen = false;
            popup.Child = null;

            panel.Children.Remove(popup);
        }
    }

    /// <summary>
    /// 根据坐标位置查找指定控件范围内的所有匹配控件
    /// </summary>
    /// <param name="control">搜索的根控件范围</param>
    /// <param name="point">相对于根控件的坐标位置</param>
    /// <returns>包含该坐标点的所有控件集合，按层级从父到子排序</returns>
    /// <remarks>
    /// 递归搜索可见控件，自动处理坐标系转换。注意：Popup 控件需要设置 OverlayPopups = true 才能被找到
    /// </remarks>
    /// <example>
    /// var foundControls = mainPanel.FindControls(new Point(100, 200));
    /// var topControl = foundControls.LastOrDefault();
    /// </example>
    public static IEnumerable<Control> FindControls(this Control control, Point point)
    {
        // hack: 这里有一个问题，就是关于popup的控件，找不到，目前是在窗口的options配置OverlayPopups = true解决

        var result = new List<Control>();

        // 检查控件是否可见
        if (!control.IsVisible)
            return result;

        // 创建控件的渲染边界矩形（相对于自身坐标系）
        var renderBounds = new Rect(0, 0, control.Bounds.Width, control.Bounds.Height);

        // 检查点是否在当前控件的渲染边界内
        if (!renderBounds.Contains(point))
            return result;

        // 添加当前控件到结果
        result.Add(control);

        // 递归检查所有可见子控件
        foreach (var child in control.GetVisualChildren().OfType<Control>())
        {
            if (!child.IsVisible)
                continue;

            // 将点从当前控件坐标系转换为子控件坐标系
            var childPoint = new Point(point.X - child.Bounds.X, point.Y - child.Bounds.Y);

            // 递归查找子控件
            var childResults = child.FindControls(childPoint);
            result.AddRange(childResults);
        }

        return result;
    }

    /// <summary>
    /// 从控件集合中获取最上层的可见控件
    /// </summary>
    /// <param name="controls">待筛选的控件集合，可以为null</param>
    /// <param name="root">作用域根控件，为null时所有控件都有效</param>
    /// <returns>最上层的控件，无有效控件时返回null</returns>
    /// <remarks>
    /// 排序规则：视觉树深度 > ZIndex > 集合中的位置顺序，确保最前端的控件被选中
    /// </remarks>
    /// <example>
    /// var topControl = foundControls.Topmost(mainWindow);
    /// if (topControl != null) { /* 处理最上层控件 */ }
    /// </example>
    public static Control? Topmost(this IEnumerable<Control>? controls, Control? root = null)
    {
        if (controls == null) return null;

        var validControls = new List<Control>();

        // 筛选有效控件
        foreach (var control in controls)
        {
            if (root == null)
            {
                // 无根控件限制，所有控件都有效
                validControls.Add(control);
            }
            else
            {
                // 检查控件是否为根控件的子控件
                var current = control;
                var isChildOfRoot = false;

                while (current != null)
                {
                    if (current == root)
                    {
                        isChildOfRoot = true;
                        break;
                    }
                    current = current.GetVisualParent() as Control;
                }

                if (isChildOfRoot)
                {
                    validControls.Add(control);
                }
            }
        }

        return validControls.Count switch
        {
            0 => null,
            1 => validControls[0],
            _ => validControls
                .Select((control, index) => new
                {
                    Control = control,
                    Depth = GetControlDepth(control),
                    ZIndex = control.ZIndex,
                    OriginalIndex = index
                })
                .OrderByDescending(x => x.Depth)
                .ThenByDescending(x => x.ZIndex)
                .ThenByDescending(x => x.OriginalIndex)
                .First()
                .Control
        };
    }

    /// <summary>
    /// 计算控件在视觉树中的深度层级
    /// </summary>
    /// <param name="control">要计算深度的控件</param>
    /// <returns>控件深度，根控件为0</returns>
    private static int GetControlDepth(Control control)
    {
        var depth = 0;
        var current = control;
        while (current.GetVisualParent() != null)
        {
            depth++;
            current = current.GetVisualParent() as Control;
            if (current == null) break;
        }
        return depth;
    }
}