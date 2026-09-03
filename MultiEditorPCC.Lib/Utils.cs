using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.Lib;


public static class Utils
{
    public static VersionePCC TestVersionePCC(String Path)
    {
        Dictionary<String, VersionePCC> FiltriRicerca = new()
        {
            { "*00036.FDI", VersionePCC.PCC2001 },
            { "*00022.FDI", VersionePCC.PCF2001 },
            { "*99036.FDI", VersionePCC.PCC7P },
            { "*99022.FDI", VersionePCC.PCF7P },
            { "EQ036036.PKF", VersionePCC.PCC6 },
            { "EQ030022.PKF", VersionePCC.PCF6_ORO },
            { "EQUIPOS.PKF", VersionePCC.PCC5 },
            { "EQ036022.PKF", VersionePCC.PCC5_ORO },
            { "EQ95*.DBC", VersionePCC.PCC4 },
            { "*.DGF", VersionePCC.PCC3 }
        };

        foreach (var Cerca in FiltriRicerca)
        {
            if (Directory.GetFiles(Path, Cerca.Key, SearchOption.AllDirectories).Any())
            {
                return Cerca.Value;
            }
        }

        return VersionePCC.NESSUNA;
    }


    /// <summary>
    /// Decodifica e traduzione testi archivi
    /// </summary>
    /// <param name="testoCodificato">Byte nel file che sono il testo codificato</param>
    /// <returns>Testo decodificato e tradotto</returns>
    public static String DecodificaTesto(List<byte> testoCodificato)
    {
        String testo = String.Empty;

        foreach (byte b in testoCodificato) testo += Convert.ToChar((byte)(b ^ 97));

        return testo;
    }

    /// <summary>
    /// Decodifica e traduzione testi archivi
    /// </summary>
    /// <param name="testo">Byte nel file che sono il testo codificato</param>
    /// <returns>Testo decodificato e tradotto</returns>
    public static List<byte> CodificaTesto(String testo)
    {
        List<byte> b = new();

        foreach (var c in testo) b.Add((byte)(BitConverter.GetBytes(c)[0] ^ 97));

        return b;
    }
}



