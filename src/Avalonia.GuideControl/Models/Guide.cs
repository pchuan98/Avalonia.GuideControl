using System.Text.Json.Serialization;

namespace Avalonia.GuideControl.Models;

/// <summary>
/// 表示一个完整的引导流程，包含多个引导步骤和默认配置
/// </summary>
/// <remarks>
/// Guide 是引导系统的核心容器，管理整个引导流程的步骤序列和默认设置
/// </remarks>
/// <example>
/// var guide = new Guide
/// {
///     Alias = "user-onboarding",
///     Description = "新用户引导流程",
///     GuidSteps = [step1.Id, step2.Id, step3.Id]
/// };
/// </example>
public class Guide
{

    /// <summary>
    /// 引导流程的唯一标识符，用于区分不同的引导流程
    /// </summary>
    /// <remarks>创建时自动生成，确保每个引导流程都有唯一标识</remarks>
    public Guid Id { get; internal set; } = Guid.NewGuid();

    /// <summary>
    /// 引导流程的别名，用于在代码中方便地引用特定的引导流程
    /// </summary>
    /// <remarks>建议使用有意义的名称，如 "user-onboarding"、"feature-introduction" 等</remarks>
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// 引导流程的描述信息，说明这个引导流程的用途和内容
    /// </summary>
    /// <remarks>用于文档化和管理，帮助开发者理解引导流程的目的</remarks>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 默认的引导卡片配置，当具体步骤的配置值为空时会继承此配置
    /// </summary>
    /// <remarks>
    /// 提供统一的默认样式和行为，减少重复配置。步骤级配置会覆盖默认配置
    /// </remarks>
    public GuideCardConfig DefaultCardConfig { get; set; } = GuideCardConfig.Default;

    /// <summary>
    /// 引导步骤资源集合，包含所有可用的引导步骤定义
    /// </summary>
    /// <remarks>
    /// 所有步骤资源会被合并到一个资源池中，通过 GuidSteps 中的 ID 来引用具体步骤
    /// </remarks>
    public List<GuideStep>? StepResources { get; set; }

    /// <summary>
    /// 当前引导流程的实际执行步骤序列，按顺序执行
    /// </summary>
    /// <remarks>
    /// 包含要执行的步骤 ID 列表，按数组顺序依次执行。ID 必须在 StepResources 中存在
    /// </remarks>
    /// <example>GuidSteps = [stepId1, stepId2, stepId3]</example>
    public List<Guid>? GuidSteps { get; set; }

    /// <summary>
    /// 将当前 Guide 对象序列化为 JSON 字符串
    /// </summary>
    /// <returns>格式化的 JSON 字符串</returns>
    /// <remarks>使用 AOT 友好的源生成器进行序列化，支持中文字符显示</remarks>
    /// <example>
    /// var guide = new Guide { Alias = "test" };
    /// string json = guide.ToJsonString();
    /// </example>
    public override string ToString()
    {
        try
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return System.Text.Json.JsonSerializer.Serialize(this, options);
        }
        catch
        {
            return "{}";
        }
    }
}

/// <summary>
/// 引导控件模型的 JSON 序列化上下文，支持 AOT 编译
/// </summary>
/// <remarks>
/// 为引导系统的所有模型类提供 AOT 友好的 JSON 序列化支持
/// </remarks>
[JsonSerializable(typeof(Guide))]
[JsonSerializable(typeof(GuideStep))]
[JsonSerializable(typeof(GuideCardConfig))]
[JsonSerializable(typeof(ControlInfo))]
[JsonSerializable(typeof(Hole))]
[JsonSerializable(typeof(CardPosition))]
[JsonSerializable(typeof(Offset))]
[JsonSerializable(typeof(HolePadding))]
[JsonSerializable(typeof(Guide[]))]
[JsonSerializable(typeof(GuideStep[]))]
[JsonSerializable(typeof(List<Guide>))]
[JsonSerializable(typeof(List<GuideStep>))]
[JsonSerializable(typeof(List<Guid>))]
[JsonSerializable(typeof(List<string>))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class GuideJsonContext : JsonSerializerContext;