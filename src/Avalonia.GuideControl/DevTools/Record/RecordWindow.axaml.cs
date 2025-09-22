using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.GuideControl.Controls;
using Avalonia.GuideControl.DevTools.Record;
using Avalonia.GuideControl.Extensions;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace Avalonia.GuideControl;

public partial class RecordWindow : Window
{
    private bool _isDragging = false;
    private Point _lastPointerPosition;
    private bool _isTestProcess = false;
    private bool _isShowMask = false;
    private GuideManager? _manager;

    public Window? Host
    {
        get;
        set
        {
            field = value;
            _manager = new GuideManager(value);
        }
    }

    public RecordWindow()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        ContorlPanel.PointerPressed += (o, args) =>
        {
            if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

            _isDragging = true;
            _lastPointerPosition = args.GetPosition(this);
            args.Pointer.Capture(ContorlPanel);
        };
        ContorlPanel.PointerMoved += (o, args) =>
        {
            if (!_isDragging) return;

            var currentPosition = args.GetPosition(this);
            var deltaX = currentPosition.X - _lastPointerPosition.X;
            var deltaY = currentPosition.Y - _lastPointerPosition.Y;

            Position = new PixelPoint(
                Position.X + (int)deltaX,
                Position.Y + (int)deltaY);

            args.Handled = false;
        };
        ContorlPanel.PointerReleased += (o, args) =>
        {
            _isDragging = false;
            args.Pointer.Capture(null);

            args.Handled = false;
        };

        ContorlPanel.PointerWheelChanged += (o, args) =>
        {
            var delta = args.Delta.Y * 0.05;
            var newOpacity = Math.Max(0.4, Math.Min(1.0, Opacity + delta));
            Opacity = newOpacity;
        };

        DataContext ??= new RecordViewModel();

        RegisterHostKey();
    }

    private Control? selectControl = null;

    private void RegisterHostKey()
    {
        if (Host is null) return;
        if (DataContext is not RecordViewModel vm) return;

        // todo：窗口关闭的时候要有一个Unregistered
        Host.PointerMoved += (sender, args) =>
        {
            if (sender is not Window host || _isTestProcess || _isShowMask) return;
            var pos = args.GetCurrentPoint(host).Position;

            var control = host.FindControls(pos)
                .Where(c => c is not Mask)
                //.Where(c=>c.GetType().ToString().ToLower().Contains("dialog"))
                .Where(c => c is Button or MenuItem or UniformGrid)
                .Topmost(host);

            if (control is not null)
            {
                selectControl = control;

                TipsTb.Text = $"{control.GetType()} | {control.VisualTreeString(this)}";
                control.ShowControlMask();
            }
            else MaskExtensions.HiddenControlMask();
        };

        Host.KeyDown += (_, args) =>
        {
            if (args.Key == Key.F2)
            {
                if((args.KeyModifiers & KeyModifiers.Control) != 0)
                    _isShowMask = !_isShowMask;
                else vm.SelectControl = selectControl?.Info(Host);
            }
            else if (args.Key == Key.F3)
            {
                if (selectControl?.Info(Host)?.VisualTree is { } tree
                    && vm?.StepsOrder?.SelectedStep is not null)
                    vm.StepsOrder.SelectedStep.StepAdditionalHoles.Add(tree);
            }
            else { }
        };
    }

    private void OnTopmostChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox cb) return;

        this.Topmost = cb.IsChecked ?? true;
    }

    private void OnClickTestSelectedGuideStep(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not RecordViewModel vm || Host is null || _manager is null) return;

        var svm = vm.StepsOrder?.SelectedStep;
        if (svm is null) return;

        vm.RefreshGuide();

        _manager.MergeGuide(vm.Guide);

        Dispatcher.UIThread.Post(async void () =>
        {
            try
            {
                _isTestProcess = true;
                await _manager.TestRunStep(Guid.Parse(svm?.StepId ?? ""), vm.CardDisplayTimeoutput);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            finally
            {
                _isTestProcess = false;
            }
        });
    }

    #region File

    private async void OnClickOpenFile(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not RecordViewModel vm) return;

            var storageProvider = StorageProvider;

            // 定义文件过滤器
            var fileTypes = new[]
            {
                new("Guide JSON Files")
                {
                    Patterns = ["*.json"],
                    MimeTypes = ["application/json"]
                },
                FilePickerFileTypes.All
            };

            // 打开文件选择对话框
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "打开引导配置文件",
                AllowMultiple = false,
                FileTypeFilter = fileTypes
            });

            if (!(files?.Count > 0)) return;

            var file = files[0];

            await vm.OpenConfig(file.Path.LocalPath)!;
        }
        catch (Exception ex)
        {
            // TODO: 显示错误消息给用户
            System.Diagnostics.Debug.WriteLine($"打开文件失败: {ex.Message}");
        }
    }

    private async void OnClickSaveFile(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not RecordViewModel vm) return;

            var storageProvider = StorageProvider;

            // 定义文件过滤器
            var fileTypes = new FilePickerFileType[]
            {
                new("Guide JSON Files")
                {
                    Patterns = ["*.json"],
                    MimeTypes = ["application/json"]
                },
                FilePickerFileTypes.All
            };

            // 打开文件保存对话框
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "保存引导配置文件",
                FileTypeChoices = fileTypes,
                DefaultExtension = "json",
                SuggestedFileName = $"guide_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            });

            if (file == null) return;
            await vm.SaveConfig(file.Path.LocalPath);
        }
        catch (Exception ex)
        {
            // TODO: 显示错误消息给用户
            System.Diagnostics.Debug.WriteLine($"保存文件失败: {ex.Message}");
        }
    }

    #endregion
}