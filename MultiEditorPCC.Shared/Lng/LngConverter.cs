using Avalonia.Data.Converters;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace MultiEditorPCC.Shared.Lng;

public class LngConverter : IValueConverter
{
    public LngConverter()
    {

    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter == null) return String.Empty;
        if (parameter.ToString().ToUpper() == "YEAR") return DateTime.Now.Year + " ";


        return App.Services.GetRequiredService<LngService>().Traduci(parameter.ToString());
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

