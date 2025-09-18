namespace Avalonia.GuideControl.Models;

/// <summary>
/// 表示引导流程中的单个步骤，包含步骤配置和验证信息
/// </summary>
/// <remarks>
/// 每个步骤定义了引导卡片的显示位置、内容配置以及完成条件
/// </remarks>
/// <example>
/// var step = new GuideStep 
/// {
///     Id = Guid.NewGuid(),
///     Alias = "welcome-step",
///     VisualTree = "MainWindow.WelcomeButton",
///     Config = new GuidCardConfig { ... }
/// };
/// </example>
public class GuideStep(Guid id)
{
    public GuideStep() : this(Guid.Empty) { }

    /// <summary>
    /// 步骤的唯一标识符，用于区分和管理不同的引导步骤
    /// </summary>
    /// <remarks>建议在创建时生成新的 Guid，避免重复</remarks>
    public Guid Id { get; set; } = id;

    /// <summary>
    /// 步骤的别名，用于在代码中方便地引用特定步骤
    /// </summary>
    /// <remarks>建议使用有意义的名称，如 "welcome-step"、"create-project-step" 等</remarks>
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// 目标控件在可视化树中的路径，用于定位引导卡片应该指向的控件
    /// </summary>
    /// <remarks>
    /// 格式如 "MainWindow.LeftPanel.CreateButton"，用于在控件树中查找目标元素
    /// </remarks>
    /// <example>VisualTree = "MainWindow.MenuBar.FileMenu.NewButton"</example>
    public string VisualTree { get; set; } = string.Empty;

    /// <summary>
    /// 额外的VisualTree，这个将会额外显示 Hole 区域
    /// 这里面的hole将会继承从 VisualTree 处得到所有 Hole 配置信息
    /// </summary>
    public List<string> AdditionalHoles { get; set; } = [];

    /// <summary>
    /// Hole 区域相对于目标控件的内边距扩展
    /// </summary>
    /// <remarks>用于扩展 Hole 显示区域，使其超出控件边界</remarks>
    /// <example>HolePadding = new HolePadding { Left = 5, Top = 5, Right = 5, Bottom = 5 }</example>
    public HolePadding HolePadding { get; set; } = new(8, 4, 8, 4);

    /// <summary>
    /// Hole 区域相对于目标控件的位置偏移量
    /// </summary>
    /// <remarks>用于微调 Hole 位置，X为水平偏移，Y为垂直偏移</remarks>
    /// <example>HoleOffset = new Offset { X = 0, Y = -2 }</example>
    public Offset HoleOffset { get; set; } = new();

    /// <summary>
    /// 判断当前的准备工作有没有完成
    /// </summary>
    public string? PreparedMethod { get; set; }

    /// <summary>
    /// 判断当前步骤是否完成的验证方法表示，后期通过验证方法字典完成验证
    /// </summary>
    /// <remarks>
    /// 可以为空，表示步骤需要手动确认完成。如果指定，系统会根据此方法判断步骤完成状态
    /// </remarks>
    public string? FinishMethod { get; set; }

    /// <summary>
    /// 引导卡片的配置信息，包含显示内容、样式和行为设置
    /// </summary>
    /// <remarks>不能为 null，必须提供有效的配置对象</remarks>
    public GuideCardConfig Config { get; set; } = new();
}