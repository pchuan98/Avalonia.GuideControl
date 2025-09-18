namespace Avalonia.GuideControl.Models;

/// <summary>
/// 引导操作枚举，定义引导流程中可能的操作类型
/// </summary>
/// <remarks>
/// 用于标识用户在引导过程中执行的操作或系统状态
/// </remarks>
public enum GuideOperation
{
    /// <summary>
    /// 无操作，默认状态
    ///
    /// 错误也用这个
    /// </summary>
    None,
    
    /// <summary>
    /// 上一步操作，返回到前一个引导步骤
    /// </summary>
    Previous,
    
    /// <summary>
    /// 跳过操作，跳过当前或整个引导流程
    /// </summary>
    Skip,
    
    /// <summary>
    /// 下一步操作，进入下一个引导步骤
    /// </summary>
    Next,
    
    /// <summary>
    /// 准备未完成状态，当前步骤的准备条件不满足
    /// </summary>
    NoPrepare,
    
    /// <summary>
    /// 完成状态，引导流程已完成
    /// </summary>
    Finished,
}