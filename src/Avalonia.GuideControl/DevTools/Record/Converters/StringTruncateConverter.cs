using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia.GuideControl.DevTools.Record;

/// <summary>
/// 字符串截断转换器，用于截取字符串的前N位字符
/// </summary>
internal class StringTruncateConverter : IValueConverter
{
    public static readonly StringTruncateConverter Instance = new();

    public const int Length = 8;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str || string.IsNullOrEmpty(str))
            return value;

        return str.Length < Length ? value : str[..Length];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        BindingOperations.DoNothing;
}