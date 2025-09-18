using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Avalonia.GuideControl;

public static partial class AvaloniaUtils
{
    /// <summary>
    /// 生成控件的视觉树路径字符串，从指定作用域开始
    /// </summary>
    /// <param name="control">目标控件，不能为空</param>
    /// <param name="scope">作用域控件，为空时使用根控件</param>
    /// <returns>视觉树路径字符串，格式如："Grid[0]/StackPanel[1]/Button[0]"</returns>
    /// <example>
    /// var path = myButton.VisualTreeString(mainWindow);
    /// </example>
    public static string VisualTreeString(this Control? control, Control? scope)
    {
        var path = new List<string>();
        var current = control;

        while (current != null && current != scope)
        {
            var parent = current.GetVisualParent<Control>();
            if (parent == null) break;

            // 获取当前控件在父容器中的索引（在相同类型的控件中）
            var child = current;
            var index = -1;
            var visualChildren = parent.GetVisualChildren().OfType<Control>()
                .Where(c => c.GetType().Name == current.GetType().Name)
                .ToList();
            for (var i = 0; i < visualChildren.Count; i++)
            {
                if (visualChildren[i] == child)
                    index = i;
            }
            var typeName = current.GetType().Name;
            path.Insert(0, $"{typeName}[{index}]");

            current = parent;
        }

        return string.Join("/", path);
    }

    /// <summary>
    /// 根据视觉树路径字符串查找控件
    /// </summary>
    /// <param name="path">视觉树路径，格式如："Grid[0]/StackPanel[1]/Button[0]"</param>
    /// <param name="scope">搜索的作用域，为空时从根开始搜索</param>
    /// <returns>找到的控件，未找到时返回null</returns>
    /// <example>
    /// var button = AvaloniaUtils.FromVisualTree("Grid[0]/StackPanel[1]/Button[0]", mainWindow);
    /// </example>
    public static Control? FromVisualTree(string path, Control? scope)
    {
        if (string.IsNullOrEmpty(path) || scope == null)
            return null;

        var segments = path.Split('/');
        var current = scope;

        foreach (var segment in segments)
        {
            if (string.IsNullOrEmpty(segment))
                continue;

            // 解析格式：TypeName[index]
            var bracketIndex = segment.LastIndexOf('[');
            if (bracketIndex == -1 || !segment.EndsWith("]"))
                return null;

            var typeName = segment[..bracketIndex];
            var indexStr = segment.Substring(bracketIndex + 1, segment.Length - bracketIndex - 2);

            if (!int.TryParse(indexStr, out var index))
                return null;

            // 在当前节点的视觉子节点中查找
            var visualChildren = current.GetVisualChildren().OfType<Control>()
                .Where(c => c.GetType().Name == typeName)
                .ToList();

            if (index < 0 || index >= visualChildren.Count)
                return null;

            current = visualChildren[index];
        }

        return current;
    }
}