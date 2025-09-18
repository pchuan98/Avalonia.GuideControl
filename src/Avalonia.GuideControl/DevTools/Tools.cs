using Avalonia.Controls;
using Avalonia.GuideControl.DevTools.Record;
using Avalonia.Input;

namespace Avalonia.GuideControl.DevTools;

public class RecordConfigOption
{
    public Dictionary<string, Func<bool>> ValidMethods { get; set; } = [];

    public IEnumerable<Type>? AllowControlTypes { get; set; } = [
        typeof(UserControl),
        typeof(Button),

    ];
}

public static class Tools
{
    private static RecordWindow? Record;

    private static void Show(Window host)
    {
        try
        {
            if (Record is null
                || Record.Bounds is { Height: 0, Width: 0 })
            {
                Record = new RecordWindow()
                {
                    Host = host,
                    Opacity = 1,
                    CanResize = false,
                    SystemDecorations = SystemDecorations.BorderOnly,
                    DataContext = new RecordViewModel()
                };
                Record.Closed += (sender, args) => { Record = null; };
            }

            Record.Show();

            host.Closing += (sender, args) =>
            {
                Record?.Close();
            };
        }
        catch (Exception)
        {
            Record = null;
        }
    }

    private static void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not Window window) return;
        if (e.Key == Key.F1) Show(window);
    }

    public static void AttachGuideDevTools(this Window window)
    {
        window.KeyDown += OnWindowKeyDown;
    }
}

