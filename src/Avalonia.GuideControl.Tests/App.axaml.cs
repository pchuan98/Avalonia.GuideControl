using Avalonia.Markup.Xaml;

namespace Avalonia.GuideControl.Tests;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}