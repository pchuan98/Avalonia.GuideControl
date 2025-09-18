using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.GuideControl.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.GuideControl.DevTools.Record;

/// <summary>
/// 录制窗口的主视图模型，统一管理所有子视图模型
/// </summary>
/// <remarks>协调各个组件之间的数据交互和业务逻辑</remarks>
internal partial class RecordViewModel : ObservableObject
{
    private Guide? _currentGuide;

    public Guide? Guide => _currentGuide;

    public void RefreshGuide()
    {
        if (_currentGuide is null) return;

        BasicInfo?.Update(ref _currentGuide);
        DefaultCard?.Update(ref _currentGuide);
        StepsOrder?.UpdateGuide(ref _currentGuide);

        JsonString = _currentGuide.ToString();
    }

    private void UpdateJson(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(JsonString)) return;

        RefreshGuide();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        UpdateJson(this, e);
    }

    [ObservableProperty]
    public partial ControlInfo? SelectControl { get; set; }

    partial void OnSelectControlChanged(ControlInfo? value)
    {
        StepsOrder?.SelectedStep?.StepVisualTree = value?.VisualTree ?? "";
    }

    #region external vm

    /// <summary>
    /// 基本信息视图模型
    /// </summary>
    [ObservableProperty]
    public partial BasicInfoViewModel? BasicInfo { get; set; }

    /// <summary>
    /// 默认卡片配置视图模型
    /// </summary>
    [ObservableProperty]
    public partial DefaultCardViewModel? DefaultCard { get; set; }

    /// <summary>
    /// 步骤编辑器视图模型
    /// </summary>
    [ObservableProperty]
    public partial StepEditorViewModel? StepEditor { get; set; }

    /// <summary>
    /// 步骤顺序管理视图模型
    /// </summary>
    [ObservableProperty]
    public partial StepsOrderViewModel? StepsOrder { get; set; }

    partial void OnBasicInfoChanged(BasicInfoViewModel? value) => value?.PropertyChanged += UpdateJson;
    partial void OnDefaultCardChanged(DefaultCardViewModel? value) => value?.PropertyChanged += UpdateJson;
    partial void OnStepEditorChanged(StepEditorViewModel? value) => value?.PropertyChanged += UpdateJson;

    partial void OnStepsOrderChanged(StepsOrderViewModel? value)
    {
        value?.PropertyChanged += UpdateJson;
        value?.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != nameof(StepsOrderViewModel.SelectedStep)) return;
            if (sender is not StepsOrderViewModel order) return;
            StepEditor = order.SelectedStep;
        };
    }

    #endregion

    #region MenuItem Commands

    /// <summary>
    /// 创建新的配置，重置所有数据到初始状态
    /// </summary>
    /// <remarks>清空基本信息、默认卡片配置、步骤编辑器和步骤列表</remarks>
    [RelayCommand]
    private void NewConfig() => UpdateGuide(new Guide());

    public void UpdateGuide(Guide guide)
    {
        _currentGuide = null;   // fix: because of RefreshGuide, if not set null, all guide value will be set default value.

        BasicInfo = new BasicInfoViewModel(guide);
        DefaultCard = new DefaultCardViewModel(guide);
        StepsOrder = new StepsOrderViewModel(guide);

        _currentGuide = guide;
        RefreshGuide();
    }

    /// <summary>
    /// 从指定路径打开配置文件
    /// </summary>
    /// <param name="path">配置文件路径，必须是有效的JSON文件</param>
    /// <remarks>加载JSON文件中的引导配置数据到各个视图模型</remarks>
    public async Task OpenConfig(string path)
    {
        var json = await File.ReadAllTextAsync(path, Encoding.UTF8);

        var guide = JsonSerializer.Deserialize(json, GuideJsonContext.Default.Guide);

        if (guide is not null)
            UpdateGuide(guide);
    }

    /// <summary>
    /// 将当前配置保存到指定路径
    /// </summary>
    /// <param name="path">保存路径，文件扩展名建议为.json</param>
    /// <remarks>将所有视图模型数据序列化为JSON格式保存</remarks>
    public async Task SaveConfig(string path)
    {
        // 确保数据是最新的
        RefreshGuide();

        await File.WriteAllTextAsync(path, JsonString, Encoding.UTF8);
    }

    #endregion

    #region Preview

    [ObservableProperty]
    public partial int CardDisplayTimeoutput { get; set; } = 1;

    #endregion

    [ObservableProperty]
    public partial string JsonString { get; set; } = string.Empty;
}