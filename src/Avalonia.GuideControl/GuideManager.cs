using System.Diagnostics;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.GuideControl.Controls;
using Avalonia.GuideControl.Extensions;
using Avalonia.GuideControl.Models;
using Avalonia.Threading;

namespace Avalonia.GuideControl;

/// <summary>
/// 引导流程管理器，统一管理引导步骤的执行和状态控制
/// </summary>
/// <param name="window">要展示引导的窗口，默认为 null 时会使用主窗口的 Content</param>
/// <remarks>
/// 该管理器支持异步执行引导步骤，包括准备条件验证、界面显示、用户交互等待和完成条件验证。
/// 通过验证方法字典机制，可以灵活地定义每个步骤的执行条件。
/// </remarks>
/// <example>
/// var manager = new GuideManager(mainWindow);
/// manager.ValidMethods["user_logged_in"] = () => CurrentUser != null;
/// await manager.TestRunStep(stepId, timeout: 30);
/// </example>
public class GuideManager(Window? window= null) : IDisposable
{
    /// <summary>
    /// 所有的步骤
    /// </summary>
    public GuideStep[] StepResource { get; private set; } = [];

    /// <summary>
    /// 实际要演示的步骤
    /// </summary>
    public Guid[] Steps { get; set; } = [];

    /// <summary>
    /// 所有的验证方法
    ///
    /// 包括准备和完成的方法
    /// </summary>
    public Dictionary<string, Func<bool>> ValidMethods { get; set; } = [];

    /// <summary>
    /// 当前正在执行的步骤
    /// </summary>
    public GuideStep? CurrentStep { get; private set; }

    /// <summary>
    /// 当前步骤的取消令牌源
    /// </summary>
    private CancellationTokenSource _currentStepCancellation = new();

    /// <summary>
    /// 当前在使用的Guide
    /// </summary>
    private Guide? _currentGuide = new();

    /// <summary>
    /// 验证状态变化事件，当调用 UpdateValid 方法时触发
    /// </summary>
    /// <remarks>
    /// 事件参数为验证方法名称，用于通知其他组件某个验证条件已经满足。
    /// 如果验证方法名与当前步骤的 FinishMethod 匹配，会自动取消当前步骤的等待。
    /// </remarks>
    public event EventHandler<string>? ValidationStateChanged;

    /// <summary>
    /// 触发验证事件 - 当某一个结束属性确定为真，调用该方法手动结束任务
    /// </summary>
    /// <param name="method">验证方法名</param>
    public void UpdateValid(string method)
    {
        if (string.IsNullOrEmpty(method)) return;

        // 触发验证状态变化事件
        ValidationStateChanged?.Invoke(this, method);

        // 如果当前有正在执行的步骤，检查是否匹配其完成验证方法
        if (CurrentStep?.FinishMethod == method)
        {
            // 取消当前步骤的等待，让 RunStep 方法检查验证结果
            _currentStepCancellation?.Cancel();
        }
    }

    /// <summary>
    /// 合并 Guide 对象的资源到当前管理器
    /// </summary>
    /// <param name="guide">要合并的 Guide 对象</param>
    /// <remarks>
    /// 将 Guide 的步骤资源合并到 StepResource，将执行步骤合并到 Steps
    /// 相同 ID 的步骤会被覆盖
    /// </remarks>
    public void MergeGuide(Guide? guide)
    {
        if (guide == null) return;

        // 合并步骤资源
        if (guide.StepResources?.Any() == true)
        {
            var existingSteps = StepResource.ToList();

            foreach (var newStep in guide.StepResources)
            {
                // 移除相同ID的现有步骤
                existingSteps.RemoveAll(s => s.Id == newStep.Id);
                // 添加新步骤
                existingSteps.Add(newStep);
            }

            StepResource = existingSteps.ToArray();
        }

        // 合并执行步骤序列
        if (guide.GuidSteps?.Any() == true)
        {
            var existingSteps = Steps.ToList();

            // 将新的步骤ID添加到现有序列中，避免重复
            foreach (var stepId in guide.GuidSteps
                         .Where(stepId => !existingSteps.Contains(stepId)))
            {
                existingSteps.Add(stepId);
            }

            Steps = existingSteps.ToArray();
        }

        _currentGuide = guide;
    }

    /// <summary>
    /// 异步执行指定的引导步骤，包括准备验证、界面显示和完成验证
    /// </summary>
    /// <param name="id">要执行的步骤 ID，必须在 StepResource 中存在</param>
    /// <param name="timeout">超时时间（秒），为 null 时不设置超时，主要用于测试</param>
    /// <returns>
    /// 返回 GuideOperation 枪举值：
    /// - None: 执行失败或超时
    /// - NoPrepare: 准备条件不满足
    /// - Previous: 用户点击上一步
    /// - Next: 用户点击下一步
    /// - Skip: 用户点击跳过
    /// - Finished: 验证条件满足，步骤完成
    /// </returns>
    /// <remarks>
    /// 执行流程：
    /// 1. 检查是否有其他步骤正在执行
    /// 2. 验证准备条件（PreparedMethod）
    /// 3. 显示遵罩层和引导卡片
    /// 4. 等待用户操作或验证条件满足
    /// 5. 清理资源并返回结果
    /// 此方法线程安全，支持取消操作。
    /// </remarks>
    /// <example>
    /// var result = await manager.TestRunStep(stepId, timeout: 30);
    /// if (result == GuideOperation.Finished) {
    ///     // 步骤成功完成
    /// }
    /// </example>
    public async Task<GuideOperation> TestRunStep(Guid id, int? timeout = null)
    {
        if (CurrentStep is not null) return GuideOperation.None;

        // 查找目标步骤
        var step = StepResource.FirstOrDefault(s => s.Id == id);
        if (step == null) return GuideOperation.None;
        CurrentStep = step;

        // 1. 判断 - 准备是否完成
        if (!string.IsNullOrEmpty(step.PreparedMethod))
        {
            if (ValidMethods.TryGetValue(step.PreparedMethod, out var preparedMethod) &&
                !preparedMethod())
                return GuideOperation.NoPrepare;
        }

        _currentStepCancellation = new CancellationTokenSource();

        var tcs = new TaskCompletionSource<GuideOperation>();

        var mask = new Mask();

        try
        {
            // 3. 显示引导步骤
            var holes = step.Holes(window);

            var card = CardExtension.CreateCard(step, _currentGuide?.DefaultCardConfig);
            var cardPosition = card.CalculatePositionFromStep(step, window);
            if (cardPosition == null)
            {
                mask.Dispose();
                return GuideOperation.None;
            }

            card.PreviousClick += (sender, args) =>
            {
                Dispatcher.UIThread.Post(() => tcs.SetResult(GuideOperation.Previous));
            };
            card.SkipClick += (sender, args) =>
            {
                try
                {
                    tcs.SetResult(GuideOperation.Skip);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"ERROR1 {e.Message}");
                }
            };
            card.NextClick += (sender, args) =>
            {
                tcs.SetResult(GuideOperation.Next);
            };

            Canvas.SetLeft(card, cardPosition.Value.X);
            Canvas.SetTop(card, cardPosition.Value.Y);

            mask.Show();
            mask.Children.Add(card);
            mask.HoleItems = string.IsNullOrEmpty(step.VisualTree) && !step.AdditionalHoles.Any()
                ? [new Hole(new Rect(0, 0, 0, 0))]
                : holes;

            if (ValidMethods.TryGetValue(step.FinishMethod ?? "", out var validationMethod))
            {
                _ = Task.Run(async () =>
                {
                    // 定期检查验证方法（每500毫秒检查一次）
                    while (!_currentStepCancellation.Token.IsCancellationRequested)
                    {
                        if (validationMethod()) tcs.TrySetResult(GuideOperation.Finished);

                        await Task.Delay(500);
                    }
                });
            }

            // timeout for test
            if (timeout is not null)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay((int)timeout * 1000);

                    tcs.TrySetResult(GuideOperation.None);
                });
            }

            return await tcs.Task;
        }
        catch (OperationCanceledException)
        {
            return GuideOperation.Finished;
        }
        catch (Exception)
        {
            return GuideOperation.None;
        }
        finally
        {
            await _currentStepCancellation.CancelAsync();

            CurrentStep = null;
            mask.Dispose();
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public void Dispose()
    {
        CurrentStep = null;
        _currentStepCancellation.Cancel();

        ValidMethods.Clear();
        ValidationStateChanged = null;
    }
}