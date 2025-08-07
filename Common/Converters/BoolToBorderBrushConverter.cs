using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace L_0_Chess_Engine.Converters;

public class BoolToBorderBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is true) ? Brushes.Yellow : Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}