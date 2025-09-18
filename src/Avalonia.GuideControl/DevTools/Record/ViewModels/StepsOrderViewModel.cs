using System.Collections.ObjectModel;
using Avalonia.GuideControl.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.GuideControl.DevTools.Record;

/// <summary>
/// 管理引导步骤顺序的视图模型，提供步骤列表的增删改查和拖拽排序功能
/// </summary>
/// <remarks>管理当前引导流程的所有步骤，支持手动排序和选择操作</remarks>
internal partial class StepsOrderViewModel(Guide guide) : ObservableObject
{
    public StepsOrderViewModel() : this(new Guide()) { }

    /// <summary>
    /// 所有可用的步骤编辑器集合，用于显示步骤列表
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<StepEditorViewModel> Steps { get; set; } =
        new(guide.StepResources?.Select(item => new StepEditorViewModel(item))?.ToArray() ?? []);

    /// <summary>
    /// 当前选中的步骤编辑器
    /// </summary>
    [ObservableProperty]
    public partial StepEditorViewModel? SelectedStep { get; set; }

    /// <summary>
    /// 添加新步骤的命令
    /// </summary>
    /// <remarks>如果有选中项则在其下方添加，否则添加到列表末尾</remarks>
    [RelayCommand]
    private void AddStep()
    {
        var newStep = new GuideStep(Guid.NewGuid());
        var newStepEditor = new StepEditorViewModel(newStep);

        if (SelectedStep != null)
        {
            var selectedIndex = Steps.IndexOf(SelectedStep);
            Steps.Insert(selectedIndex + 1, newStepEditor);
        }
        else
        {
            Steps.Add(newStepEditor);
        }

        SelectedStep = newStepEditor;
    }

    /// <summary>
    /// 删除选中步骤的命令
    /// </summary>
    /// <remarks>只有在有选中项时才能执行删除操作</remarks>
    [RelayCommand(CanExecute = nameof(CanRemoveStep))]
    private void RemoveStep()
    {
        if (SelectedStep == null) return;
        var removedIndex = Steps.IndexOf(SelectedStep);
        Steps.Remove(SelectedStep);

        // 选择下一个或上一个项目
        if (Steps.Count > 0)
        {
            if (removedIndex < Steps.Count)
                SelectedStep = Steps[removedIndex];
            else if (removedIndex > 0)
                SelectedStep = Steps[removedIndex - 1];
            else
                SelectedStep = Steps[0];
        }
        else
        {
            SelectedStep = null;
        }
    }

    /// <summary>
    /// 判断是否可以删除步骤
    /// </summary>
    private bool CanRemoveStep() => SelectedStep != null;

    /// <summary>
    /// 手动排序：将选中项向上移动
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanMoveUp))]
    private void MoveUp()
    {
        if (SelectedStep == null) return;
        var currentIndex = Steps.IndexOf(SelectedStep);
        if (currentIndex > 0)
        {
            Steps.Move(currentIndex, currentIndex - 1);
        }
    }

    /// <summary>
    /// 判断是否可以向上移动
    /// </summary>
    private bool CanMoveUp() => SelectedStep != null && Steps.IndexOf(SelectedStep) > 0;

    /// <summary>
    /// 手动排序：将选中项向下移动
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanMoveDown))]
    private void MoveDown()
    {
        if (SelectedStep == null) return;
        var currentIndex = Steps.IndexOf(SelectedStep);
        if (currentIndex < Steps.Count - 1)
        {
            Steps.Move(currentIndex, currentIndex + 1);
        }
    }

    /// <summary>
    /// 判断是否可以向下移动
    /// </summary>
    private bool CanMoveDown() => SelectedStep != null && Steps.IndexOf(SelectedStep) < Steps.Count - 1;

    /// <summary>
    /// 将当前步骤列表更新到Guide对象
    /// </summary>
    /// <param name="guide">要更新的Guide对象</param>
    public void UpdateGuide(ref Guide guide)
    {
        // hack: 这里是有线程安全问题的，暂时忽略

        guide.StepResources = [];
        guide.GuidSteps = [];

        var steps = Steps.ToArray();
        foreach (var stepEditor in steps)
        {
            var step = new GuideStep(new Guid(stepEditor.StepId));
            stepEditor.Update(ref step);
            guide.StepResources.Add(step);
            guide.GuidSteps.Add(step.Id);
        }
    }

    partial void OnSelectedStepChanged(StepEditorViewModel? value)
    {
        // 更新命令的可执行状态
        RemoveStepCommand.NotifyCanExecuteChanged();
        MoveUpCommand.NotifyCanExecuteChanged();
        MoveDownCommand.NotifyCanExecuteChanged();
    }
}