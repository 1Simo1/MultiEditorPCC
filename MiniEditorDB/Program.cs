using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib;
using MultiEditorPCC.Lib.Archivi;
using System.IO;

Directory.CreateDirectory($"{AppContext.BaseDirectory}/Files");
Directory.CreateDirectory($"{AppContext.BaseDirectory}/CSV");
Directory.CreateDirectory($"{AppContext.BaseDirectory}/DBDAT");

var e = new EditorSvc(true);
List<String> v = e.versioniPCC_Editor.Select(v => v.Id).ToList();

foreach (var c in v) Directory.CreateDirectory($"{AppContext.BaseDirectory}/DBDAT/{c}");

var collection = new ServiceCollection();
collection.AddSingleton<ArchivioSvc>();
//collection.AddSingleton<EditorSvc>();
collection.AddSingleton<IDatSvc, DatSvc>();
AppSvc.Services = collection.BuildServiceProvider();

var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

int testFiles()
{
    List<String> f = new();
    f.AddRange(Directory.GetFiles($"{AppContext.BaseDirectory}/Files", "*.FDI"));
    f.AddRange(Directory.GetFiles($"{AppContext.BaseDirectory}/Files", "*.DBC"));
    return f.Count;
}

int testCSV()
{
    List<String> f = new();
    f.AddRange(Directory.GetFiles($"{AppContext.BaseDirectory}/CSV", "*.CSV"));
    int n = f.Count;

    foreach (var c in f)
    {
        //TODO Controllo validità file CSV
        if (e.TestFileCorrettoCSV(ArchivioSvc.TipoDatoDB.NESSUNO, c) == null) n--;
    }


    return n;
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

    DatabaseCSV.Versione = 1;
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



void ScriviFiles()
{
    var d = AppSvc.Services.GetRequiredService<IDatSvc>();
    List<Squadra> e = d.ImportaDBEditorDaFileCSV<Squadra>
        ($"{AppContext.BaseDirectory}/CSV/Squadre.CSV");
    a.DatiProgettoAttivo.Squadre = e;
    
    
    //TODO
}