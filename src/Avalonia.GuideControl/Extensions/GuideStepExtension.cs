using Avalonia.Controls;
using Avalonia.GuideControl.Models;

namespace Avalonia.GuideControl.Extensions;

/// <summary>
/// 为 GuideStep 提供 Hole 区域创建和管理的扩展方法
/// </summary>
/// <remarks>
/// 用于将引导步骤转换为可视化的透明区域，支持位置偏移和边距调整
/// </remarks>
public static class GuideStepExtension
{
    /// <summary>
    /// 根据视觉树路径为指定步骤创建 Hole 区域
    /// </summary>
    /// <param name="visualtree">控件在视觉树中的路径字符串</param>
    /// <param name="step">包含偏移量和边距配置的引导步骤</param>
    /// <param name="control">搜索范围的根控件</param>
    /// <returns>创建的 Hole 对象，找不到目标控件时返回 null</returns>
    /// <remarks>
    /// 会自动应用步骤中的 HoleOffset 偏移量和 HolePadding 扩展区域
    /// </remarks>
    private static Hole? CreateHole(string visualtree, GuideStep step, Control? control)
    {
        try
        {
            var target = AvaloniaUtils.FromVisualTree(visualtree, control);
            var info = target?.Info(control);

            if (target is null || info is null) return null;

            var rect = info.Location;

            // 应用 HoleOffset 偏移量
            var offsetX = rect.X + step.HoleOffset.X;
            var offsetY = rect.Y + step.HoleOffset.Y;

            // 应用 HolePadding 扩展区域
            var x = Math.Max(0, offsetX - step.HolePadding.Left);
            var y = Math.Max(0, offsetY - step.HolePadding.Top);
            var width = rect.Width + step.HolePadding.HorizontalPadding;
            var height = rect.Height + step.HolePadding.VerticalPadding;

            return new Hole(new Rect(x, y, width, height));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return null;
        }
    }

    /// <summary>
    /// 获取引导步骤的主要 Hole 区域
    /// </summary>
    /// <param name="step">引导步骤</param>
    /// <param name="root">搜索范围的根控件</param>
    /// <returns>主要 Hole 对象，找不到目标控件时返回 null</returns>
    /// <example>
    /// var mainHole = step.MainHole(mainWindow);
    /// if (mainHole != null) mask.HoleItems = [mainHole];
    /// </example>
    public static Hole? MainHole(this GuideStep step, Control? root) => CreateHole(step.VisualTree, step, root);

    /// <summary>
    /// 获取引导步骤的所有附加 Hole 区域
    /// </summary>
    /// <param name="step">引导步骤</param>
    /// <param name="root">搜索范围的根控件</param>
    /// <returns>附加 Hole 列表，会过滤掉创建失败的项</returns>
    /// <remarks>
    /// 附加 Holes 会继承主步骤的 HoleOffset 和 HolePadding 配置
    /// </remarks>
    public static List<Hole> AddtioinalHoles(this GuideStep step, Control? root)
        => step.AdditionalHoles.Select(i => CreateHole(i, step, root))
            .Where(i => i is not null).ToList()!;

    /// <summary>
    /// 获取引导步骤的所有 Hole 区域（主要 + 附加）
    /// </summary>
    /// <param name="step">引导步骤</param>
    /// <param name="control">搜索范围的根控件</param>
    /// <returns>完整的 Hole 列表，可直接用于 Mask 控件</returns>
    /// <example>
    /// var allHoles = step.Holes(mainWindow);
    /// mask.HoleItems = allHoles;
    /// </example>
    public static List<Hole> Holes(this GuideStep step, Control? control)
    {
        var holes = new List<Hole>();

        // 添加主要 Hole
        var mainHole = step.MainHole(control);
        if (mainHole != null)
            holes.Add(mainHole);

        // 添加附加 Holes
        holes.AddRange(step.AddtioinalHoles(control));

        return holes;
    }
}