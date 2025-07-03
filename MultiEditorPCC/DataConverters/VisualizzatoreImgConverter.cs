using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace MultiEditorPCC.DataConverters;

public class VisualizzatoreImgConverter : Avalonia.Data.Converters.IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return null;
        try
        {
            var v = (List<Byte>)value;
            var b = new Bitmap(new MemoryStream(v.ToArray()));

            if (parameter.ToString().Equals("*")) return b;
            if (parameter.ToString().Equals("W")) return b.Size.Width;
            if (parameter.ToString().Equals("H")) return b.Size.Height;

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
