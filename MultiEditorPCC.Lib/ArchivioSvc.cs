using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using SkiaSharp;
using System.Text;


namespace MultiEditorPCC.Lib;

public class ArchivioSvc
{
    public Dictionary<String, List<ElementoArchivio>> ArchiviProgetto { get; set; }

    private List<String> FileArchiviDBGioco { get; set; }

    public DatiProgettoAttivo DatiProgettoAttivo { get; set; }

    /// <summary>
    /// Legge i dati da file di gioco e li memorizza in ArchiviProgetto
    /// </summary>
    /// <param name="progettoEditor">Progetto attivo</param>
    public void CaricaDatiDBGioco(Dat.DbSet.ProgettoEditorPCC? progettoEditor, bool nuovoProgetto = false)
    {
        if (progettoEditor == null) return;

        String path = progettoEditor.Cartella + Path.DirectorySeparatorChar;



        if (progettoEditor.VersionePCC.Equals("*"))
        /* Progetto libero, costruisco i dati da file, non uso una versione
         * fissa del gioco. Utile per costruire un database in comune 
         * a più progetti e a più versioni del gioco */
        {
            FileArchiviDBGioco = new();

        }
        else ElencoArchiviDefault(progettoEditor.VersionePCC);

        try
        {
            var f = Directory.GetFiles(path, "*.PAK", SearchOption.AllDirectories);
            foreach (var nf in f) if (!FileArchiviDBGioco.Contains(nf.Substring(path.Length))) FileArchiviDBGioco.Add(nf.Substring(path.Length));
            if (!FileArchiviDBGioco.Where(f => f.EndsWith(".FDI")).Any())
            {
                f = Directory.GetFiles(path, "*.FDI", SearchOption.AllDirectories);
                foreach (var nf in f) if (!FileArchiviDBGioco.Contains(nf.Substring(path.Length))) FileArchiviDBGioco.Add(nf.Substring(path.Length));
            }
            f = Directory.GetFiles(path, "*.PKF", SearchOption.AllDirectories);
            foreach (var nf in f) if (!FileArchiviDBGioco.Contains(nf.Substring(path.Length))) FileArchiviDBGioco.Add(nf.Substring(path.Length));
            f = Directory.GetFiles(path, "*.DBC", SearchOption.AllDirectories);
            foreach (var nf in f) if (!FileArchiviDBGioco.Contains(nf.Substring(path.Length))) FileArchiviDBGioco.Add(nf.Substring(path.Length));

        }
        catch (Exception ex)
        {

            return;
        }


        ArchiviProgetto = new();






        foreach (var file in FileArchiviDBGioco)
        {

            try
            {
                int lp = path.Length;
                if (progettoEditor.VersionePCC.Equals("*"))
                {
                    //path = String.Empty;
                    lp = 0;
                }
                String percorsoFile = $"{path}{file}";

                var bf = System.IO.File.ReadAllBytes($"{percorsoFile}");

                if (!BitConverter.IsLittleEndian) bf.Reverse();

                List<byte> dati = new(bf.ToList());

                bool fdi = false;
                bool pak = false;

                var a = LeggiTipoArchivio(dati.GetRange(0, 8));

                if (a == TipoArchivio.FDI) fdi = true;
                if (a == TipoArchivio.PAK) pak = true;

                ArchiviProgetto.Add(file, LeggiArchivio(dati, fdi, pak));



            }
            catch (Exception e)
            {
                continue;
            }


        }
        try
        {
            SetupDatiProgettoAttivo(progettoEditor, ArchiviProgetto);
        }
        catch (Exception e)
        {

        }

    }

    public void SetupDatiProgettoAttivo(ProgettoEditorPCC progettoEditor, Dictionary<string, List<ElementoArchivio>> archiviProgetto)
    {
        DatiProgettoAttivo = new();

        var d = AppSvc.Services.GetRequiredService<IDatSvc>();

        d.ElaboraFileEditorPersonalizzati(progettoEditor.Nome);

        foreach (var a in ArchiviProgetto)
        {
            foreach (var e in a.Value)
            {

                List<TipoDatoDB> t = CalcolaTipoDatoDB(progettoEditor.VersionePCC, a.Key);

                String? archivio = (t.Count() == 1 && t.First() == TipoDatoDB.ARCHIVIO) ? a.Key : null;


                d.ElaboraInfoElementoDB(progettoEditor.VersionePCC, t, e, archivio);

            }
        }

        return;
    }

    private List<TipoDatoDB> CalcolaTipoDatoDB(String versionePCC, String nomeFile)
    {
        if (nomeFile.EndsWith("FDI"))
        {
            //2001 E 7+
            if (nomeFile.Contains("JUG")) return new() { TipoDatoDB.GIOCATORE };
            if (nomeFile.Contains("ENT")) return new() { TipoDatoDB.ALLENATORE };


            if (!nomeFile.Contains("990")) //2001
            {
                if (nomeFile.Contains("EQ")) return new() { TipoDatoDB.SQUADRA };
                if (nomeFile.Contains("EST")) return new() { TipoDatoDB.STADIO };
            }
            else //7+
            {
                if (nomeFile.Contains("EQ")) return new() { TipoDatoDB.STADIO, TipoDatoDB.SQUADRA };
            }
        }

        if (nomeFile.StartsWith("EQ") && nomeFile.EndsWith("PKF"))
            return new() { TipoDatoDB.STADIO, TipoDatoDB.ALLENATORE, TipoDatoDB.GIOCATORE, TipoDatoDB.SQUADRA };

        //Cerco se il file è un archivio (PKF non di squadra, PAK)
        //PKF di squadra escluso qui (se lo è il metodo è tornato al passaggio precedente)
        if (nomeFile.Contains("PAK") || nomeFile.Contains("PKF")) return new() { TipoDatoDB.ARCHIVIO };

        return new() { TipoDatoDB.NESSUNO };
    }

    //private TipoDatoDB TipoDatoDB()

    public enum TipoDatoDB
    {
        NESSUNO = 0,
        SQUADRA,
        GIOCATORE,
        ALLENATORE,
        STADIO,
        ARCHIVIO
    }


    /// <summary>
    /// Legge i dati in ArchiviProgetto e sovrascrive i file modificati nella cartella del gioco
    /// </summary>
    public void SalvaDatiDBGioco(Dat.DbSet.ProgettoEditorPCC? progettoEditor, bool dbc = false)
    {
        //TODO
        throw new NotImplementedException();
    }

    /// <summary>
    /// Legge i dati di ArchiviProgetto e li scrive sul database del singolo progetto
    /// </summary>

    public async Task SetupDatabase(ProgettoEditorPCC progettoEditor)
    {
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();

        foreach (var a in ArchiviProgetto)
        {
            foreach (var e in a.Value)
            {
                await Task.Run(() => d.ScriviDB(progettoEditor, a.Key, e, true));
            }
        }
    }

    /// <summary>
    /// Legge i dati di ArchiviProgetto e li scrive nella cartella files del singolo progetto
    /// </summary>

    public async Task SetupFileEditor(ProgettoEditorPCC progettoEditor)
    {
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();

        foreach (var a in ArchiviProgetto)
        {
            foreach (var e in a.Value)
            {
                await Task.Run(() => Task.Run(() => d.EstraiFileEditor(progettoEditor, a.Key, e)));
            }
        }
    }

    private void ElencoArchiviDefault(String VersionePCC)
    {
        switch (VersionePCC)
        {
            case "PC Calcio 2001":
                FileArchiviDBGioco = new()
                { $"Dbdat{Path.DirectorySeparatorChar}EST00036.FDI",
                  $"Dbdat{Path.DirectorySeparatorChar}ENT00036.FDI",
                  $"Dbdat{Path.DirectorySeparatorChar}JUG00036.FDI",
                  $"Dbdat{Path.DirectorySeparatorChar}EQ00036.FDI" };
                break;
            case "PC Futbol 2001":
                FileArchiviDBGioco = new()
                { $"DBDAT{Path.DirectorySeparatorChar}EST00022.FDI",
                  $"DBDAT{Path.DirectorySeparatorChar}ENT00022.FDI",
                  $"DBDAT{Path.DirectorySeparatorChar}JUG00022.FDI",
                  $"DBDAT{Path.DirectorySeparatorChar}EQ00022.FDI" };
                break;
            case "PC Futbol 7+":
                FileArchiviDBGioco = new()
                { $"DBDAT{Path.DirectorySeparatorChar}JUG99022.FDI",
                  $"DBDAT{Path.DirectorySeparatorChar}ENT99022.FDI",
                  $"DBDAT{Path.DirectorySeparatorChar}EQ99022.FDI"};
                break;
            case "PC Calcio 7+":
                FileArchiviDBGioco = new()
                { $"DBDAT{Path.DirectorySeparatorChar}JUG99036.FDI",
                  $"DBDAT{Path.DirectorySeparatorChar}ENT99036.FDI",
                  $"DBDAT{Path.DirectorySeparatorChar}EQ99036.FDI"};
                break;
            case "PC Calcio 6":
                FileArchiviDBGioco = new()
                { $"Dbdat{Path.DirectorySeparatorChar}EQ036036.PKF"};
                break;
            case "PC Calcio 5":
                FileArchiviDBGioco = new()
                { $"DBDAT{Path.DirectorySeparatorChar}EQUIPOS.PKF"};
                break;
            case "PC Calcio 4":
                FileArchiviDBGioco = new();
                //{ $"DBDAT{Path.DirectorySeparatorChar}*.DBC"};
                break;
            case "PC Calcio 3":
                FileArchiviDBGioco = new();
                //{ $"CAL94951.DBC"};
                break;

            default: break;
        }
    }


    public List<ElementoArchivio> LeggiArchivio(List<byte> dati, bool fdi = false, bool pak = false)
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

    private List<Byte> HeaderArchivio(List<Byte> dati, bool fdi = false, bool pak = false)
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

    private String DecodificaNome(List<byte> b)
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




    public TipoArchivio LeggiTipoArchivio(List<byte> dati)
    {
        try
        {
            if ($"{(char)dati[4]}{(char)dati[5]}{(char)dati[6]}{(char)dati[7]}".Equals("v1.0")) return TipoArchivio.FDI;
            if ($"{(char)dati[0]}{(char)dati[1]}{(char)dati[2]}".Equals("PAK")) return TipoArchivio.PAK;

            return TipoArchivio.PKF;
        }
        catch (Exception)
        {
            return TipoArchivio.NESSUNO;
        }
    }

    public List<String> ElencoPaletteArchivio() => DatiProgettoAttivo.Archivi.Keys
                                                   .Where(k => k.ToUpper().EndsWith(".PAL")).ToList();


    public List<String> CartelleDatiArchivi(int Livello = 1, Dictionary<string, List<ElementoArchivio>>? archivi = null, String cartella = "")
    {
        if (archivi == null) archivi = DatiProgettoAttivo.Archivi;

        if (Livello <= 0) return new();

        if (!String.IsNullOrEmpty(cartella))

            return CartelleDatiArchivi(Livello + 1, archivi.Where(d => d.Key.StartsWith(cartella)).ToDictionary());

        if (Livello == 1) return archivi.Where(d => d.Key.Split(Path.DirectorySeparatorChar).Length >= Livello + 1)
                                        .Select(d => $"{d.Key.Split(Path.DirectorySeparatorChar)[0]}")
                                        .Distinct()
                                        .ToList();




        var e = archivi.Where(d => d.Key.Split(Path.DirectorySeparatorChar).Length >= Livello + 1)
               .Select(d =>
               {
                   StringBuilder sb = new();
                   sb.Append($"{d.Key.Split(Path.DirectorySeparatorChar)[0]}");
                   for (int i = 1; i < Livello; i++)
                   {
                       sb.Append(Path.DirectorySeparatorChar);
                       sb.Append($"{d.Key.Split(Path.DirectorySeparatorChar)[i]}");
                   }

                   return sb.ToString();

               })
               .Distinct()
               .ToList();



        return e;

    }

    public List<String> FileDatiArchivi(int Livello = 1, Dictionary<string, List<ElementoArchivio>>? archivi = null, String cartella = "")
    {
        if (archivi == null) archivi = DatiProgettoAttivo.Archivi;

        List<String> files = new List<String>();

        if (String.IsNullOrEmpty(cartella))
        {
            /* Non si dovrebbe utilizzare il metodo senza specificare la cartella
             * tuttavia in questo caso torno tutti i file di un dato Livello */
            foreach (var c in CartelleDatiArchivi(Livello, archivi))
            {
                files.AddRange(FileDatiArchivi(Livello, archivi, c));
            }

            return files;
        }

        //return CartelleDatiArchivi(Livello + 1, archivi.Where(d => d.Key.StartsWith(cartella)).ToDictionary());

        return archivi.Where(a => a.Key.StartsWith(cartella) && a.Key.Split(Path.DirectorySeparatorChar).Length == Livello + 1).Select(a => a.Key).ToList();

    }

    public int NumeroElementi(String fileSelezionato)
    {
        if (!DatiProgettoAttivo.Archivi.ContainsKey(fileSelezionato)) return 0;

        return DatiProgettoAttivo.Archivi[fileSelezionato].Count;
    }

    public List<Byte> ElaboraCaricamentoImmagine(String fileSelezionato, String palette, int n = 1)
    {
        List<Byte> img = new();

        try
        {
            var testNome = fileSelezionato.ToUpper();

            if (!testNome.EndsWith(".BMP") &&
                !testNome.EndsWith(".GIF") &&
                !testNome.EndsWith(".PAL")
                )
            {

                if (DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat[0] != 66 ||
                    DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat[1] != 77)
                    return img;
            }

            if (testNome.EndsWith(".GIF"))
            {
                if (!DatiProgettoAttivo.Archivi.ContainsKey($"Palette_{testNome}.PAL"))
                {
                    DatiProgettoAttivo.Archivi.Add($"Palette_{testNome}.PAL", new List<ElementoArchivio>()
                {
                    new() {
                        Dat = DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat.GetRange(13,768)
                    }
                });
                }

                return DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat;
            }


            //TODO Elaborazione caricamento immagine BMP/PAL

            var bm = !(DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat[0] != 66 ||
                    DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat[1] != 77);

            if (testNome.EndsWith(".BMP") || bm)
            {

                var fb = DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat;

                int w = 0;
                int h = 0;

                int bc = 24;


                if (fb[10] == 26)
                {
                    w = fb[18] + 256 * fb[19];
                    h = fb[20] + 256 * fb[21];
                    bc = fb[24];
                }

                if (fb[10] == 54)
                {
                    w = fb[18] + 256 * fb[19];
                    h = fb[22] + 256 * fb[23];


                    if ((DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat.Count) - (w * h + 54) >= 1024)
                    {
                        //Palette inclusa nel BMP
                        if (!DatiProgettoAttivo.Archivi.ContainsKey($"Palette_{testNome}.PAL"))
                        {
                            DatiProgettoAttivo.Archivi.Add($"Palette_{testNome}.PAL", new List<ElementoArchivio>()
                {
                    new() {
                        Dat = DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat.GetRange(54,1024)
                    }
                });

                        }

                        return DatiProgettoAttivo.Archivi[fileSelezionato][n - 1].Dat;
                    }

                    bc = fb[28];
                }

                if (fb[10] != 26 && fb[10] != 54)
                {

                    return fb;
                }

                List<Byte> Byte_Immagine = fb.GetRange(fb[10], w * h * bc / 8);

                var fp = DatiProgettoAttivo.Archivi[palette].First().Dat;

                var pal = fp;

                if (fp.Count > 1024)
                {
                    pal = fp.GetRange(fp.Count - 1024, 1024);
                }

                if (fp.Count == 768)
                {
                    pal = new();

                    for (int i = 0; i < 256; i++)
                    {
                        pal.Add(fp[i * 3]);
                        pal.Add(fp[i * 3 + 1]);
                        pal.Add(fp[i * 3 + 2]);
                        pal.Add(0);
                    }
                }

                List<Byte> bmp = new();

                bmp.Add(66);
                bmp.Add(77);

                int dim = 54 + pal.Count + (w * h);

                bmp.AddRange(BitConverter.GetBytes(dim));
                bmp.AddRange(BitConverter.GetBytes(0));
                bmp.AddRange(BitConverter.GetBytes(54 + pal.Count));
                bmp.AddRange(BitConverter.GetBytes(40));
                bmp.AddRange(BitConverter.GetBytes(w));
                bmp.AddRange(BitConverter.GetBytes(h));
                bmp.AddRange(BitConverter.GetBytes(524289));
                bmp.AddRange(BitConverter.GetBytes(0));
                bmp.AddRange(BitConverter.GetBytes(3072)); //Funzionante anche con 0 ?
                bmp.AddRange(BitConverter.GetBytes(2834));
                bmp.AddRange(BitConverter.GetBytes(2834));
                bmp.AddRange(BitConverter.GetBytes(0));
                bmp.AddRange(BitConverter.GetBytes(0));

                bmp.AddRange(pal);
                bmp.AddRange(Byte_Immagine);

                return bmp;
            }

            if (testNome.EndsWith(".PAL"))
            {
                var pal = DatiProgettoAttivo.Archivi[fileSelezionato].First().Dat;

                var info = new SKImageInfo(800, 592);
                using var surface = SKSurface.Create(info);
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                for (int i = 0; i < 256; i++)
                {
                    int r = (int)Math.Truncate((decimal)(i / 16));
                    int c = i % 16;
                    int x = (50 * c);
                    int y = (37 * r);

                    using var paint = new SKPaint
                    {
                        Color = new SKColor(pal[i * 4], pal[(i * 4) + 1], pal[(i * 4) + 2]),
                        Style = SKPaintStyle.Fill
                    };

                    canvas.DrawRect(x, y, 50, 37, paint);

                    using var t = new SKPaint
                    {
                        Color = new SKColor((byte)(255 - (int)pal[(i * 4) + 0]), (byte)(255 - (int)pal[(i * 4) + 1]), (byte)(255 - (int)pal[(i * 4) + 2])),
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill
                    };
                    using var font = new SKFont
                    {
                        Size = 21
                    };


                    canvas.DrawText(i.ToString("X").PadLeft(2, '0'), new() { X = x + 25, Y = y + 18 }, SKTextAlign.Center, font, t);

                }


                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var output = File.OpenWrite($"temp.png");
                data.SaveTo(output);
                output.Close();

                var f = File.ReadAllBytes($"temp.png").ToList();

                File.Delete($"temp.png");

                return f;


            }


        }
        catch (Exception ex)
        {

            return img;

        }

        return img;
    }
}

public enum TipoArchivio
{
    NESSUNO = 0,
    FDI,
    PAK,
    PKF
}

public class ElementoArchivio
{
    public int Codice { get; set; } = -1;
    public String Nome { get; set; } = String.Empty;

    public int Offset { get; set; }

    public int Size { get; set; }

    public List<Byte> Dat { get; set; } = new();

    public static implicit operator ElementoArchivio(List<byte> v)
    {
        throw new NotImplementedException();
    }
}

public class DatiProgettoAttivo
{
    public List<Squadra> Squadre { get; set; } = new();

    public List<Giocatore> Giocatori { get; set; } = new();

    public List<Allenatore> Allenatori { get; set; } = new();

    public List<Stadio> Stadi { get; set; } = new();

    public Dictionary<String, List<ElementoArchivio>> Archivi { get; set; } = new();

}