using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.Globalization;
using System.IO;

namespace MultiEditorPCC.DataConverters;

public class PunteggioImgConverter : Avalonia.Data.Converters.IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        int v = value != null ? int.Parse(value.ToString()) : -1;

        if (v == -1 || v > 99) return null;

        SKBitmap img = new(100, 10);


        for (int x = 0; x < v; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                img.SetPixel(x, y, new(0, 255, 0));
            }
        }

        for (int x = v; x < 100; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                img.SetPixel(x, y, new(255, 0, 0));
            }
        }

        return new Bitmap(new MemoryStream(SKImage.FromBitmap(img).Encode().ToArray()));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {


        //SKImage img = (SKImage)value;

        //SKBitmap bmp = SKBitmap.FromImage(img);

        //for (int x = 1; x < 100; x++) if (bmp.GetPixel(x, 0).Equals(new SKColor(255, 0, 0))) return x;

        return null;
    }
}
