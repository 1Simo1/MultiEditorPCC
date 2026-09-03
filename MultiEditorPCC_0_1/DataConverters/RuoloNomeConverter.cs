using MultiEditorPCC.Dat.DbSet;
using System;
using System.Globalization;

namespace MultiEditorPCC.DataConverters;

public class RuoloNomeConverter : Avalonia.Data.Converters.IValueConverter
{

    private readonly String[] strings = new String[] {

            Locales.Resources.PlayerRole_Name_None,
            Locales.Resources.PlayerRole_Name_Goalkeeper,
            Locales.Resources.PlayerRole_Name_RightBack,
            Locales.Resources.PlayerRole_Name_LeftBack,
            Locales.Resources.PlayerRole_Name_Sweeper,
            Locales.Resources.PlayerRole_Name_InsideCentreLeft,
            Locales.Resources.PlayerRole_Name_InsideCentreRight,
            Locales.Resources.PlayerRole_Name_RightMidfielder,
            Locales.Resources.PlayerRole_Name_InsideRight,
            Locales.Resources.PlayerRole_Name_CentreForward,
            Locales.Resources.PlayerRole_Name_CentralMidfielder,
            Locales.Resources.PlayerRole_Name_LeftMidfielder,
            Locales.Resources.PlayerRole_Name_RightWinger,
            Locales.Resources.PlayerRole_Name_CentralStriker,
            Locales.Resources.PlayerRole_Name_LeftWinger,
            Locales.Resources.PlayerRole_Name_DefensiveMidfielder,
            Locales.Resources.PlayerRole_Name_RightForward,
            Locales.Resources.PlayerRole_Name_LeftForward,
            Locales.Resources.PlayerRole_Name_InsideLeft,
        };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value == null ? null : strings[(int)value].ToUpper();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (Ruolo)Array.IndexOf(strings, value.ToString());
    }
}
