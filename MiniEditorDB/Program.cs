using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib;
using MultiEditorPCC.Lib.Archivi;

Config();

var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

//LeggiFilesGioco => a.DatiProgettoAttivo => ScriviCSV
//Modifico esternamente CSV
//LeggiCSV => a.DatiProgettoAttivo => ScriviFiles


/*LeggiFilesGioco();
ScriviCSV();*/

LeggiCSV();
ScriviFiles();

void Config()
{
    var collection = new ServiceCollection();
    collection.AddSingleton<ArchivioSvc>();
    collection.AddSingleton<EditorSvc>(e => ActivatorUtilities.CreateInstance<EditorSvc>(e, true));
    collection.AddSingleton<IDatSvc, DatSvc>();
    AppSvc.Services = collection.BuildServiceProvider();
    DatabaseCSV.Versione = 1;

    Directory.CreateDirectory($"{AppContext.BaseDirectory}/Files");
    Directory.CreateDirectory($"{AppContext.BaseDirectory}/CSV");
    Directory.CreateDirectory($"{AppContext.BaseDirectory}/DBDAT");

    var e = AppSvc.Services.GetRequiredService<EditorSvc>();
    List<String> v = e.versioniPCC_Editor.Select(v => v.Id).ToList();

    foreach (var c in v) Directory.CreateDirectory($"{AppContext.BaseDirectory}/DBDAT/{c}");


}

int testFiles()
{
    List<String> f = new();
    f.AddRange(Directory.GetFiles($"{AppContext.BaseDirectory}/Files", "*.FDI"));
    f.AddRange(Directory.GetFiles($"{AppContext.BaseDirectory}/Files", "*.DBC"));
    return f.Count;
}

List<InfoFileDatiCSV> elencoFileValidiCSV()
{
    var e = AppSvc.Services.GetRequiredService<EditorSvc>();
    List<InfoFileDatiCSV> v = new();
    foreach (var c in Directory.GetFiles($"{AppContext.BaseDirectory}/CSV", "*.CSV"))
    {
        InfoFileDatiCSV? info = e.TestFileCorrettoCSV(ArchivioSvc.TipoDatoDB.NESSUNO, c);
        if (info != null) v.Add(info);
    }

    if (!v.Any()) return v;

    return v.OrderBy(x => x.TipoDatoDB).ThenByDescending(x => x.NumeroElementiFile).ToList();
}


void LeggiFilesGioco()
{
    if (testFiles() > 0)
    {
        var p = new ProgettoEditorPCC()
        {
            Cartella = AppContext.BaseDirectory,
            VersionePCC = "*"

        };

        a.FileArchiviDBGioco = new();

        a.FileArchiviDBGioco.AddRange(
        Directory.GetFiles($"{AppContext.BaseDirectory}/Files", "*.FDI")
        );

        a.FileArchiviDBGioco.AddRange(
        Directory.GetFiles($"{AppContext.BaseDirectory}/Files", "*.DBC")
        );

        a.ArchiviProgettoDaFileArchiviDBGioco(p);
        a.SetupDatiProgettoAttivo(p, a.ArchiviProgetto);
    }
}


void ScriviCSV()
{
    if (testFiles() == 0) return;

    DatabaseCSV.scriviTatticaCompleta = true;
    DatabaseCSV.ScriviCSVSquadre(a.DatiProgettoAttivo.Squadre);
    File.WriteAllText($"{AppContext.BaseDirectory}/CSV/Squadre.CSV", DatabaseCSV.contenutoCSV);
    DatabaseCSV.ScriviCSVAllenatori(a.DatiProgettoAttivo.Allenatori);
    File.WriteAllText($"{AppContext.BaseDirectory}/CSV/Allenatori.CSV", DatabaseCSV.contenutoCSV);
    DatabaseCSV.ScriviCSVStadi(a.DatiProgettoAttivo.Stadi);
    File.WriteAllText($"{AppContext.BaseDirectory}/CSV/Stadi.CSV", DatabaseCSV.contenutoCSV);

    foreach (var gc in a.DatiProgettoAttivo.Giocatori)
    {
        int id = gc.Id;

        var sq = a.DatiProgettoAttivo.Squadre.Where(sq => sq.Giocatori.Find(gc => gc.Id == id) != null).FirstOrDefault();
        String Squadra = String.Empty;
        if (sq != null) Squadra = sq.Nome;

        var nSquadra = sq != null ? sq.Id : 0;

        DatabaseCSV.dettagliSquadreGiocatore.Add(id, new((int)nSquadra, Squadra));
    }


    DatabaseCSV.ScriviCSVGiocatori(a.DatiProgettoAttivo.Giocatori);
    File.WriteAllText($"{AppContext.BaseDirectory}/CSV/Giocatori.CSV", DatabaseCSV.contenutoCSV);
}

void LeggiCSV()
{
    List<InfoFileDatiCSV> elencoInfoFileCSV = elencoFileValidiCSV();
    if (!elencoInfoFileCSV.Any()) return;

    a.DatiProgettoAttivo = new();
    foreach (InfoFileDatiCSV info in elencoInfoFileCSV)
    {
        DatabaseCSV.contenutoCSV = info.Percorso;

        if (info.TipoDatoDB == ArchivioSvc.TipoDatoDB.SQUADRA)
        {
            List<uint> id = a.DatiProgettoAttivo.Squadre.Select(s => s.Id).ToList();
            List<Squadra> e = DatabaseCSV.LeggiSquadre();

            if (!a.DatiProgettoAttivo.Squadre.Any())
            {
                a.DatiProgettoAttivo.Squadre.AddRange(e);
            }
            else
            {
                foreach (var x in e)
                {
                    if (id.Contains(x.Id))
                    {
                        a.DatiProgettoAttivo.Squadre.RemoveAt(id.IndexOf(x.Id));
                    }

                    a.DatiProgettoAttivo.Squadre.Add(x);
                }
            }
        }
        if (info.TipoDatoDB == ArchivioSvc.TipoDatoDB.STADIO)
        {
            List<uint> id = a.DatiProgettoAttivo.Stadi.Select(s => s.Id).ToList();
            List<Stadio> e = DatabaseCSV.LeggiStadi();

            if (!a.DatiProgettoAttivo.Stadi.Any())
            {
                a.DatiProgettoAttivo.Stadi.AddRange(e);
            }
            else
            {
                foreach (var x in e)
                {
                    if (id.Contains(x.Id))
                    {
                        a.DatiProgettoAttivo.Stadi.RemoveAt(id.IndexOf(x.Id));
                    }

                    a.DatiProgettoAttivo.Stadi.Add(x);
                }
            }
        }
        if (info.TipoDatoDB == ArchivioSvc.TipoDatoDB.ALLENATORE)
        {
            List<uint> id = a.DatiProgettoAttivo.Allenatori.Select(s => s.Id).ToList();
            List<Allenatore> e = DatabaseCSV.LeggiAllenatori();

            if (!a.DatiProgettoAttivo.Allenatori.Any())
            {
                a.DatiProgettoAttivo.Allenatori.AddRange(e);
            }
            else
            {
                foreach (var x in e)
                {
                    if (id.Contains(x.Id))
                    {
                        a.DatiProgettoAttivo.Allenatori.RemoveAt(id.IndexOf(x.Id));
                    }

                    a.DatiProgettoAttivo.Allenatori.Add(x);
                }
            }
        }
        if (info.TipoDatoDB == ArchivioSvc.TipoDatoDB.GIOCATORE)
        {
            List<int> id = a.DatiProgettoAttivo.Giocatori.Select(s => s.Id).ToList();
            List<Giocatore> e = DatabaseCSV.LeggiGiocatori();

            if (!a.DatiProgettoAttivo.Giocatori.Any())
            {
                a.DatiProgettoAttivo.Giocatori.AddRange(e);
            }
            else
            {
                foreach (var x in e)
                {
                    if (id.Contains(x.Id))
                    {
                        a.DatiProgettoAttivo.Giocatori.RemoveAt(id.IndexOf(x.Id));
                    }

                    a.DatiProgettoAttivo.Giocatori.Add(x);
                }
            }
        }
    }
}

void ScriviFiles()
{

    var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();
    var e = AppSvc.Services.GetRequiredService<EditorSvc>();

    int[] lv = new int[2] { 800, 700 };
    foreach (var v in lv)
    {
        FDI.Versione = v;

        foreach (var sq in a.DatiProgettoAttivo.Squadre)
        {
            if (!sq.Giocatori.Any()) sq.Giocatori = a.DatiProgettoAttivo.Giocatori.Where(g => g.CodiceSquadra == sq.Id).ToList();
            if (sq.Stadio.Id == 0) sq.Stadio = a.DatiProgettoAttivo.Stadi.SingleOrDefault(s => s.Id == sq.Id);
            sq.Allenatori = new() { a.DatiProgettoAttivo.Allenatori.FirstOrDefault(a => a.Id == sq.Allenatori.Last().Id) };


            FDI.ScriviSquadra(sq);
        }

        foreach (var vp in e.versioniPCC_Editor.Where(a => a.VersioneArchiviDB == v))
        {
            SalvaFile("EQ", vp, "FDI", FDI.Scrivi());
        }

        foreach (var al in a.DatiProgettoAttivo.Allenatori)
        {
            FDI.ScriviAllenatore(al);
        }

        foreach (var vp in e.versioniPCC_Editor.Where(a => a.VersioneArchiviDB == v))
        {
            SalvaFile("ENT", vp, "FDI", FDI.Scrivi());
        }

        foreach (var gc in a.DatiProgettoAttivo.Giocatori)
        {
            FDI.ScriviGiocatore(gc);
        }

        foreach (var vp in e.versioniPCC_Editor.Where(a => a.VersioneArchiviDB == v))
        {
            SalvaFile("JUG", vp, "FDI", FDI.Scrivi());
        }

        if (v == 800)
        {
            foreach (var st in a.DatiProgettoAttivo.Stadi)
            {
                FDI.ScriviStadio(st);
            }

            foreach (var vp in e.versioniPCC_Editor.Where(a => a.VersioneArchiviDB == 800))
            {
                SalvaFile("EST", vp, "FDI", FDI.Scrivi());
            }

        }



    }

    foreach (var sq in a.DatiProgettoAttivo.Squadre)
    {
        if (!sq.Giocatori.Any()) sq.Giocatori = a.DatiProgettoAttivo.Giocatori.Where(g => g.CodiceSquadra == sq.Id).ToList();
        if (sq.Stadio.Id == 0) sq.Stadio = a.DatiProgettoAttivo.Stadi.SingleOrDefault(s => s.Id == sq.Id);
        sq.Allenatori = new() { a.DatiProgettoAttivo.Allenatori.FirstOrDefault(a => a.Id == sq.Allenatori.Last().Id) };

        foreach (var vp in e.versioniPCC_Editor.Where(a => a.VersioneArchiviDB < 600))
        {
            SalvaFile($"EQ{91 + int.Parse(vp.Id)}{(sq.Id % 10000).ToString().PadLeft(4, '0')}",
                        vp, "DBC", DBC.ScriviSquadra(sq));
        }



    }


}

void SalvaFile(String pref, VersionePCCSupportataEditor v, String tipoArchivio, List<byte> dat)
{
    var nomeFile = tipoArchivio.Equals("DBC") ?
        $"{AppContext.BaseDirectory}/DBDAT/{v.Id}/{pref}.DBC" :
        $"{AppContext.BaseDirectory}/DBDAT/{v.Id}/{v.CercaFilePattern.First().Replace("*", pref)}";
#if !DEBUG
    File.WriteAllBytes(nomeFile, dat.ToArray());
#endif
}