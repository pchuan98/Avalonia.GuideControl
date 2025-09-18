using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.GuideControl.Controls;
using Avalonia.GuideControl.Extensions;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;

namespace Avalonia.GuideControl.Tests;

public class MaskPositionTests
{
    public MaskPositionTests()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
    }

    [AvaloniaFact]
    public void TestMaskPositionAndSize()
    {
        // 创建与Demo相同的窗口结构
        var window = new Window
        {
            Width = 400,
            Height = 300
        };

        var mainGrid = new Grid();
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

        // 创建顶部按钮区域（与Demo相同）
        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(10)
        };
        Grid.SetRow(buttonPanel, 0);

        var testButton = new Button { Name = "TestButton", Content = "测试按钮", Margin = new Thickness(5) };
        var maskButton = new Button { Name = "MaskButton", Content = "添加遮罩", Margin = new Thickness(5) };
        buttonPanel.Children.Add(testButton);
        buttonPanel.Children.Add(maskButton);

        // 创建测试控件区域（与Demo相同）
        var testGrid = new Grid { Margin = new Thickness(10) };
        testGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        testGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        testGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        testGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        Grid.SetRow(testGrid, 1);

        // 创建4个测试控件（与Demo相同）
        var testControl1 = new Border
        {
            Name = "TestControl1",
            Background = Avalonia.Media.Brushes.LightBlue,
            Margin = new Thickness(5),
            CornerRadius = new CornerRadius(5),
            Child = new TextBlock { Text = "测试控件1", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };
        Grid.SetRow(testControl1, 0);
        Grid.SetColumn(testControl1, 0);

        var testControl2 = new Border
        {
            Name = "TestControl2",
            Background = Avalonia.Media.Brushes.LightGreen,
            Margin = new Thickness(5),
            CornerRadius = new CornerRadius(5),
            Child = new TextBlock { Text = "测试控件2", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };
        Grid.SetRow(testControl2, 0);
        Grid.SetColumn(testControl2, 1);

        var testControl3 = new Border
        {
            Name = "TestControl3",
            Background = Avalonia.Media.Brushes.LightCoral,
            Margin = new Thickness(5),
            CornerRadius = new CornerRadius(5),
            Child = new TextBlock { Text = "测试控件3", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };
        Grid.SetRow(testControl3, 1);
        Grid.SetColumn(testControl3, 0);

        var testControl4 = new Border
        {
            Name = "TestControl4",
            Background = Avalonia.Media.Brushes.LightYellow,
            Margin = new Thickness(5),
            CornerRadius = new CornerRadius(5),
            Child = new TextBlock { Text = "测试控件4", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };
        Grid.SetRow(testControl4, 1);
        Grid.SetColumn(testControl4, 1);

        testGrid.Children.Add(testControl1);
        testGrid.Children.Add(testControl2);
        testGrid.Children.Add(testControl3);
        testGrid.Children.Add(testControl4);

        mainGrid.Children.Add(buttonPanel);
        mainGrid.Children.Add(testGrid);
        window.Content = mainGrid;
        window.Show();

        // 测试每个控件的遮罩位置和尺寸
        var testControls = new Control[] { testControl1, testControl2, testControl3, testControl4 };
        
        foreach (var control in testControls)
        {
            TestControlMaskPositionAndSize(control, window);
        }
    }

    private void TestControlMaskPositionAndSize(Control control, Window window)
    {
        // Arrange
        var mask = new Mask();
        
        // 获取控件在窗口中的预期位置
        var transform = control.TransformToVisual(window);
        Assert.NotNull(transform);
        
        var expectedPosition = new Point(0, 0).Transform(transform.Value);
        var expectedSize = control.Bounds;

        // Act
        //var result = control.MaskAdd(mask);

        
    }
}