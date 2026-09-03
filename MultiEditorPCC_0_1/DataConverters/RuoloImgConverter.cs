using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.Globalization;
using System.IO;

namespace MultiEditorPCC.DataConverters;

public class RuoloImgConverter : Avalonia.Data.Converters.IValueConverter
{
    /**
     * Colori di base immagine campo
     * Bordo esterno : #000000
     * Campo : #8CAA1E
     * Linee campo : #B4D232
     * "Cerchio" esterno giocatore : #000000
     * Colore indicatore portiere : #FFDF00
     * Colore indicatore giocatore di movimento : #FFFFFF
     * 
     * Pixel del "cerchio" indicatore, possono essere visti come un cerchio inscritto in 
     * un quadrato di 4 per 4 pixel, con gli angoli che mantegono il colore su cui sono,
     * quindi
     * - E E -
     * E I I E
     * E I I E
     * - E E -
     * 
     * dove con E indico il bordo esterno, sempre a #000000, con I l'indicatore
     * modifico quindi solo 12 pixel rispetto all'immagine base per indicare la
     * posizione in campo
     * 
    **/
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return null;

        var img = SKImage.FromEncodedData((byte[])Resources.Files.ResourceManager.GetObject("CAMPO"));

        var b = SKBitmap.FromImage(img);

        int x = 0, y = 0;

        int v = (int)value;

        switch (v)
        {
            case 1: x = 0; y = 5; break;
            case 2: x = 4; y = 9; break;
            case 3: x = 4; y = 1; break;
            case 4: x = 2; y = 5; break;
            case 5: x = 4; y = 4; break;
            case 6: x = 4; y = 6; break;
            case 7: x = 11; y = 9; break;
            case 8: x = 12; y = 7; break;
            case 9: x = 19; y = 5; break;
            case 10: x = 11; y = 4; break;
            case 11: x = 11; y = 1; break;
            case 12: x = 17; y = 8; break;
            case 13: x = 14; y = 5; break;
            case 14: x = 17; y = 2; break;
            case 15: x = 7; y = 5; break;
            case 16: x = 14; y = 7; break;
            case 17: x = 14; y = 3; break;
            case 18: x = 12; y = 3; break;

            default:
                return new Bitmap(new MemoryStream(SKImage.FromBitmap(b).Encode().ToArray()));
        }

        b = CostruisciImmagine(x, y, b, v);

        return new Bitmap(new MemoryStream(SKImage.FromBitmap(b).Encode().ToArray()));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }

    private SKBitmap CostruisciImmagine(int x, int y, SKBitmap b, int value)
    {
        b.SetPixel(x + 1, y, new(0, 0, 0));
        b.SetPixel(x + 2, y, new(0, 0, 0));
        b.SetPixel(x, y + 1, new(0, 0, 0));
        b.SetPixel(x, y + 2, new(0, 0, 0));
        b.SetPixel(x + 3, y + 1, new(0, 0, 0));
        b.SetPixel(x + 3, y + 2, new(0, 0, 0));
        b.SetPixel(x + 1, y + 3, new(0, 0, 0));
        b.SetPixel(x + 2, y + 3, new(0, 0, 0));

        if (value == 1)
        {
            b.SetPixel(x + 1, y + 1, new(255, 223, 0));
            b.SetPixel(x + 1, y + 2, new(255, 223, 0));
            b.SetPixel(x + 2, y + 1, new(255, 223, 0));
            b.SetPixel(x + 2, y + 2, new(255, 223, 0));
        }
        else
        {
            b.SetPixel(x + 1, y + 1, new(255, 255, 255));
            b.SetPixel(x + 1, y + 2, new(255, 255, 255));
            b.SetPixel(x + 2, y + 1, new(255, 255, 255));
            b.SetPixel(x + 2, y + 2, new(255, 255, 255));
        }

        return b;
    }
}
