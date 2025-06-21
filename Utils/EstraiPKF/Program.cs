using System.Text;

String path = args.Length > 1 ? args[1] : String.Empty;
if (string.IsNullOrEmpty(path))
{

    while (path.Length < 5)
    {
        Console.WriteLine("Inserisci percorso file PKF/FDI/PAK (poi premi invio) : ");
        path = Console.ReadLine();
    }

    String dir = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
    dir = dir.Substring(0, dir.Length - 4);

    Console.WriteLine($"Nome della cartella dove estrarre i file (default : {dir})");
    var temp = Console.ReadLine();
    dir = string.IsNullOrEmpty(temp?.Trim()) ? dir : temp;

    List<Byte> dati = new();
    bool fdi = false;
    bool pak = false;

    try
    {
        var f = System.IO.File.ReadAllBytes($"{path}");

        if (!BitConverter.IsLittleEndian) f.Reverse();

        dati = f.ToList();

        fdi = $"{(char)dati[4]}{(char)dati[5]}{(char)dati[6]}{(char)dati[7]}".Equals("v1.0");
        pak = $"{(char)dati[0]}{(char)dati[1]}{(char)dati[2]}".Equals("PAK");
    }
    catch (Exception e)
    {
        Console.WriteLine("Errore!");
        Console.WriteLine("Premi un tasto per terminare il programma");
        Console.Read();
    }

    List<ElementoArchivio> archivio = LeggiArchivio(dati, fdi, pak);


    EstraiArchivio(dir, archivio);

    Console.WriteLine("Fatto!");
    Console.WriteLine("Premi un tasto per terminare il programma");
    Console.ReadKey();
}

List<ElementoArchivio> LeggiArchivio(List<byte> dati, bool fdi = false, bool pak = false)
{
    List<ElementoArchivio> elementi = new();
    var header = HeaderArchivio(dati, fdi, pak);

    try
    {

        if (pak)
        {
            header.RemoveRange(0, 16);
            int delta = 0;

            while (delta < header.Count)
            {
                int offset = BitConverter.ToInt32(header.GetRange(delta + 0, 4).ToArray(), 0);
                int len = BitConverter.ToInt32(header.GetRange(delta + 4, 4).ToArray(), 0);
                bool ok = BitConverter.ToInt32(header.GetRange(delta + 8, 4).ToArray(), 0) == len;

                if (!ok) return elementi;

                delta += 12;

                bool nf = true;

                StringBuilder sb = new();

                while (nf)
                {
                    var b = header.GetRange(delta + 0, 4);
                    sb.Append($"{(char)b[0]}{(char)b[1]}{(char)b[2]}{(char)b[3]}");
                    nf = !b.Contains(0);
                    delta += 4;
                }

                String nomeFile = sb.ToString();
                nomeFile = nomeFile.Substring(0, nomeFile.IndexOf((char)0)).TrimEnd((char)0);

                elementi.Add(new()
                {
                    Codice = -1,
                    Nome = nomeFile,
                    Offset = offset,
                    Size = len,
                    Dat = dati.GetRange(offset, len)
                });

            }



            return elementi;
        }

        if (!fdi)
        {
            header.RemoveRange(0, 237);
            int n = header.Count / 38;
            for (int i = 1; i <= n; i++)
            {
                int delta = (i - 1) * 38;
                int offset = BitConverter.ToInt32(header.GetRange(delta + 26, 4).ToArray(), 0);

                if (offset != 0 && offset < dati.Count)
                {
                    String nome = DecodificaNome(header.GetRange(delta + 0, 26));
                    int len = BitConverter.ToInt32(header.GetRange(delta + 30, 4).ToArray(), 0);

                    elementi.Add(new()
                    {
                        Codice = -1,
                        Nome = nome,
                        Offset = offset,
                        Size = len,
                        Dat = dati.GetRange(offset, len)
                    });
                }
            }
        }
        else
        {
            header.RemoveRange(0, 20);
            int n = header.Count / 13;

            for (int i = 1; i <= n; i++)
            {
                int delta = (i - 1) * 13;
                int codice = BitConverter.ToInt32(header.GetRange(delta, 4).ToArray(), 0);
                int offset = BitConverter.ToInt32(header.GetRange(delta + 5, 4).ToArray(), 0);
                int len = BitConverter.ToInt32(header.GetRange(delta + 9, 4).ToArray(), 0);

                elementi.Add(new()
                {
                    Codice = codice,
                    Nome = String.Empty,
                    Offset = offset,
                    Size = len,
                    Dat = dati.GetRange(offset, len)
                });
            }


        }
    }
    catch (Exception e)
    {
        return new();
    }

    return elementi;
}

List<Byte> HeaderArchivio(List<Byte> dati, bool fdi = false, bool pak = false)
{
    List<Byte> header = new List<Byte>();

    try
    {
        if (pak)
        {

            int headerLen = BitConverter.ToInt32(dati.GetRange(16, 4).ToArray(), 0);

            return dati.GetRange(0, headerLen);
        }


        if (!fdi)
        {

            header = dati.GetRange(0, 237);

            var b = 237;

            int offset = b;

            while (offset != 0)
            {
                var pkf_header_buffer = dati.GetRange(b, 1216);

                offset = BitConverter.ToInt32(pkf_header_buffer.GetRange(1204, 4).ToArray(), 0) +
                         BitConverter.ToInt32(pkf_header_buffer.GetRange(1208, 4).ToArray(), 0);

                header.AddRange(pkf_header_buffer);

                b = offset;

                if (offset >= dati.Count) offset = 0;

            }


        }
        else
        {
            int n = BitConverter.ToInt32(dati.GetRange(16, 4).ToArray(), 0);

            header = dati.GetRange(0, 20);
            header.AddRange(dati.GetRange(20, 13 * n));
        }

    }
    catch (Exception e)
    {
        return new();
    }

    return header;
}

void EstraiArchivio(String dir, List<ElementoArchivio> elementiArchivio)
{
    try
    {
        Directory.CreateDirectory(dir);

        foreach (var f in elementiArchivio)
        {
            String nf = !String.IsNullOrEmpty(f.Nome) ? f.Nome : $"{f.Codice}.DBC";
            var percorsoCompleto = nf.Split(Path.DirectorySeparatorChar);
            if (percorsoCompleto.Length > 1)
            {
                StringBuilder sb = new();
                sb.Append(dir + Path.DirectorySeparatorChar);
                for (int x = 0; x < percorsoCompleto.Length - 1; x++)
                    sb.Append(percorsoCompleto[x] + Path.DirectorySeparatorChar);

                Directory.CreateDirectory(sb.ToString());
            }

            File.WriteAllBytes($"{dir}/{nf}", f.Dat.ToArray());
        }

    }
    catch (Exception e)
    {
        return;
    }

}

string DecodificaNome(List<byte> b)
{
    List<int> k = new() { 256,
                          223, 192, 163, 136, 111,
                           88, 67, 48, 31, 16,
                            3, 248, 239, 232, 227,
                            224, 223, 224, 227, 232,
                            239, 248, 3, 16, 29 };

    StringBuilder sb = new();
    var n = 0;
    foreach (byte v in b)
    {
        if (n != 0 && (b[n] ^ k[n]) != 0 && b[n] != 0) { sb.Append((char)(b[n] ^ k[n])); }
        if (n != 0 && (b[n] ^ k[n]) == 0)
        {
            return sb.ToString();
        }
        n++;
    }
    return sb.ToString();
}

public class ElementoArchivio
{
    public int Codice { get; set; } = -1;
    public String Nome { get; set; } = String.Empty;

    public int Offset { get; set; }

    public int Size { get; set; }

    public List<Byte> Dat { get; set; } = new();

}