namespace MultiEditorPCC.Lib;


public static class Utils
{
    /// <summary>
    /// Decodifica e traduzione testi archivi
    /// </summary>
    /// <param name="testoCodificato">Byte nel file che sono il testo codificato</param>
    /// <returns>Testo decodificato e tradotto</returns>
    public static String decodificaTesto(List<byte> testoCodificato)
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
    public static List<byte> codificaTesto(String testo)
    {
        List<byte> b = new();

        foreach (var c in testo) b.Add((byte)(BitConverter.GetBytes(c)[0] ^ 97));

        return b;
    }
}


