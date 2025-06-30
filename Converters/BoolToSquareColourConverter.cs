using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace L_0_Chess_Engine.Converters;

public class BoolToSquareColourConverter : IValueConverter
{
    private static readonly IBrush LightSquareBrush = Brushes.Beige;
    private static readonly IBrush DarkSquareBrush = Brushes.SaddleBrown;
    
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