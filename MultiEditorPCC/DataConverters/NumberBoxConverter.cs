using System;
using System.Globalization;

namespace MultiEditorPCC.DataConverters;

public class NumberBoxConverter : Avalonia.Data.Converters.IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value.ToString().Equals("NaN")) return 0;
        return (int)value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value.ToString().Equals("NaN")) return 0;
        return int.Parse(value.ToString());
    }
}
