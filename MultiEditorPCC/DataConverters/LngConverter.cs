using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MultiEditorPCC.DataConverters;

public class LngConverter : IValueConverter
{
    public LngConverter()
    {

    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter == null) return String.Empty;
        if (parameter.ToString().ToUpper() == "YEAR") return DateTime.Now.Year + " ";

        //TODO Ricerca traduzione da DB da decidere
        //
        //Provvisoriamente, come prototipo senza traduzione ...
        return parameter!.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
