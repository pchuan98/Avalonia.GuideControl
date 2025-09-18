using Avalonia.GuideControl.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.GuideControl.DevTools.Record;

/// <summary>
/// 管理引导流程基本信息的视图模型
/// </summary>
/// <remarks>包装了Guide模型，提供视图绑定接口和数据初始化</remarks>
/// <param name="guide">初始化数据的引导配置对象</param>
internal partial class BasicInfoViewModel(Guide guide) : ObservableObject
{
    public BasicInfoViewModel() : this(new Guide()) { }

    /// <summary>
    /// 引导流程的唯一标识符
    /// </summary>
    /// <example>Id = "guide_001";</example>
    [ObservableProperty]
    public partial string Id { get; set; } = guide.Id.ToString("N");

    /// <summary>
    /// 引导流程的友好别名，用于标识和管理
    /// </summary>
    /// <remarks>建议使用简短有意义的名称，便于识别</remarks>
    /// <example>Alias = "新用户引导";</example>
    [ObservableProperty]
    public partial string Alias { get; set; } = guide.Alias;

    /// <summary>
    /// 引导流程的详细描述信息
    /// </summary>
    /// <remarks>可以包含多行文本，描述引导流程的用途和特点</remarks>
    /// <example>Description = "帮助新用户快速了解系统功能";</example>
    [ObservableProperty]
    public partial string Description { get; set; } = guide.Description;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guide"></param>
    public void Update(ref Guide guide)
    {
        if (Guid.TryParse(Id, out var id))
            guide.Id = id;

        guide.Alias = Alias;
        guide.Description = Description;
    }
}
