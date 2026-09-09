using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Linq;

namespace MultiEditorPCC.DataConverters;

public class EnumDataConverter : IValueConverter
{
    public EnumDataConverter()
    {

    }
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum) return null;

        if (parameter != null && parameter.Equals("List")) return Enum.GetNames(value.GetType());

        return value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        if (!targetType.IsEnum) return null;

        return Enum.GetNames(targetType).Where(v => v == value).FirstOrDefault();


    }
}
