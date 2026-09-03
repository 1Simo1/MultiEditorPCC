using MultiEditorPCC.Dat.DbSet;
using System;
using System.Globalization;

namespace MultiEditorPCC.DataConverters;

public class RepartoSiglaConverter : Avalonia.Data.Converters.IValueConverter
{
    private readonly String[] strings = new String[] {

        Locales.Resources.PlayerFieldPositionGK,
        Locales.Resources.PlayerFieldPositionDEF,
        Locales.Resources.PlayerFieldPositionMID,
        Locales.Resources.PlayerFieldPositionFOR

    };
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return strings[(int)value];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (Reparto)Array.IndexOf(strings, value.ToString());
    }
}
