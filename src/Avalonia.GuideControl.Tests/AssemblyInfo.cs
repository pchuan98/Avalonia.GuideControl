using Avalonia.Headless;

// This attribute makes sure the Avalonia headless environment is set up for all tests
[assembly: AvaloniaTestApplication(typeof(Avalonia.GuideControl.Tests.TestAppBuilder))]