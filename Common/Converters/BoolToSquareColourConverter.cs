using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace L_0_Chess_Engine.Converters;

public class BoolToSquareColourConverter : IValueConverter
{
    private static readonly SolidColorBrush LightSquareBrush = new (Color.Parse("#F0D9B5"));
    private static readonly SolidColorBrush DarkSquareBrush = new (Color.Parse("#B58863"));
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLight)
        {
            return isLight ? LightSquareBrush : DarkSquareBrush;
        }

        return AvaloniaProperty.UnsetValue;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException("BoolToSquareColorConverter only supports one-way conversion.");
}