using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.GuideControl.Controls;
using Avalonia.GuideControl.Models;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Avalonia.GuideControl.Extensions;

/// <summary>
/// 引导卡片扩展方法，提供卡片位置计算和显示功能
/// </summary>
/// <remarks>
/// 用于计算引导卡片相对于目标控件的最佳显示位置
/// </remarks>
public static class CardExtension
{
    /// <summary>
    /// 根据引导步骤配置计算卡片的最佳显示位置
    /// </summary>
    /// <param name="card">引导卡片实例，用于获取卡片尺寸</param>
    /// <param name="step">引导步骤配置，包含目标控件和位置偏好</param>
    /// <param name="window">根窗口，用于获取客户区尺寸和坐标计算</param>
    /// <returns>计算出的卡片位置（绝对坐标），失败或参数无效时返回 null</returns>
    /// <remarks>
    /// 位置计算算法：
    /// 1. 获取目标 Hole 区域作为参考
    /// 2. 根据 CardPosition 枚举定义计算相对位置（九宫格模式）
    /// 3. 应用用户自定义偏移量
    /// 4. 边界检测，确保卡片不超出屏幕
    /// 5. 冲突检测，防止卡片与 Hole 区域重叠
    /// 性能注意：此方法会触发卡片的渲染测量，建议缓存结果。
    /// </remarks>
    /// <example>
    /// var position = card.CalculatePositionFromStep(step, mainWindow);
    /// if (position.HasValue) {
    ///     Canvas.SetLeft(card, position.Value.X);
    ///     Canvas.SetTop(card, position.Value.Y);
    /// }
    /// </example>
    public static Point? CalculatePositionFromStep(
        this GuideCard card,
        GuideStep step,
        Window? window)
    {
        var mainHole = step.MainHole(window);
        var position = step.Config.Position;
        var offset = step.Config.Offset;

        if (mainHole == null)
        {
            return position switch
            {
                _ => new Point(offset.X, offset.Y)
            };
        }

        if (window == null) return null;

        var clientSize = window.ClientSize;
        var cardSize = card.MeasureDesiredSize();
        var holeRect = mainHole!.Bounds;

        double x, y;

        // 处理Center位置
        if (position.HasFlag(CardPosition.Center))
        {
            x = (clientSize.Width - cardSize.Width) / 2;
            y = (clientSize.Height - cardSize.Height) / 2;
        }
        else
        {
            // 计算水平位置
            if (position.HasFlag(CardPosition.Left))
                x = holeRect.Left - cardSize.Width - 10;
            else if (position.HasFlag(CardPosition.Right))
                x = holeRect.Right + 10;
            else
                x = holeRect.Left + (holeRect.Width - cardSize.Width) / 2;

            // 计算垂直位置
            if (position.HasFlag(CardPosition.Top))
                y = holeRect.Top - cardSize.Height - 10;
            else if (position.HasFlag(CardPosition.Bottom))
                y = holeRect.Bottom + 10;
            else
                y = holeRect.Top + (holeRect.Height - cardSize.Height) / 2;
        }

        // 应用偏移
        x += offset.X;
        y += offset.Y;

        // 确保卡片在窗口客户区范围内
        x = Math.Max(10, Math.Min(x, clientSize.Width - cardSize.Width - 10));
        y = Math.Max(10, Math.Min(y, clientSize.Height - cardSize.Height - 10));

        var cardRect = new Rect(x, y, cardSize.Width, cardSize.Height);

        // 确保不重叠(仅通用，特殊情况任然无效)
        if (position.HasFlag(CardPosition.Left) && cardRect.Right > holeRect.Left)
            x = holeRect.Right + 10;
        if (position.HasFlag(CardPosition.Right) && cardRect.Left < holeRect.Right)
            x = holeRect.Left - 10 - cardRect.Width;
        if (position.HasFlag(CardPosition.Top) && cardRect.Bottom > holeRect.Top)
            y = holeRect.Bottom + 10;
        if (position.HasFlag(CardPosition.Bottom) && cardRect.Top < holeRect.Bottom)
            y = holeRect.Top - 10 - cardRect.Height;

        return new Point(x, y);
    }

    /// <summary>
    /// 创建引导卡片并应用步骤的完整配置，支持默认配置继承
    /// </summary>
    /// <param name="step">引导步骤，包含卡片的内容和样式配置</param>
    /// <param name="defaultConfig">默认配置，用于填充步骤配置中的空值，可为 null</param>
    /// <returns>完整配置的引导卡片实例，包括内容、按钮和样式</returns>
    /// <remarks>
    /// 配置优先级：步骤配置 > 默认配置 > 内置默认值。
    /// 此方法使用空合并运算符（??）实现配置继承机制，确保所有属性都有有效值。
    /// 样式类会被自动添加到卡片的 Classes 集合中。
    /// </remarks>
    /// <example>
    /// var card = CardExtension.CreateCard(step, guideConfig.DefaultCardConfig);
    /// // 卡片已经包含了所有必要的内容和按钮配置
    /// </example>
    public static GuideCard CreateCard(GuideStep step, GuideCardConfig? defaultConfig = null)
    {
        var config = step.Config;
        var card = new GuideCard
        {
            Header = config.Header,
            Content = config.Content,
            Tips = config.Tips,
            PreviousButtonText = config.PreviousButtonText ?? defaultConfig?.PreviousButtonText ?? "Previous",
            NextButtonText = config.NextButtonText ?? defaultConfig?.NextButtonText ?? "Next",
            SkipButtonText = config.SkipButtonText ?? defaultConfig?.SkipButtonText ?? "Skip",
            IsPreviousButtonVisible = config.IsPreviousButtonVisible ?? defaultConfig?.IsPreviousButtonVisible ?? false,
            IsNextButtonVisible = config.IsNextButtonVisible ?? defaultConfig?.IsNextButtonVisible ?? false,
            IsSkipButtonVisible = config.IsSkipButtonVisible ?? defaultConfig?.IsSkipButtonVisible ?? false
        };

        // 应用样式
        var styleClass = config.StyleClass ?? defaultConfig?.StyleClass;
        if (!string.IsNullOrEmpty(styleClass))
        {
            card.Classes.Add(styleClass);
        }

        return card;
    }
}