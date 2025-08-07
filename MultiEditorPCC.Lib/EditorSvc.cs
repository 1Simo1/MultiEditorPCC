using Microsoft.EntityFrameworkCore;
using MultiEditorPCC.Dat.DbContext;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib.Archivi;
using static MultiEditorPCC.Lib.ArchivioSvc;

namespace MultiEditorPCC.Lib;

public class EditorSvc
{

    public DbContextOptionsBuilder<MultiEditorPCCDbContext> optionsBuilder { get; set; } = new();
    public MultiEditorPCCDbContext db { get; set; }
    public MultiEditorPCCDbContext? dbProgetto { get; set; }
    public ProgettoEditorPCC? ProgettoAttivoEditor { get; set; }

    public List<ProgettoEditorPCC> ProgettiEditor { get; set; }
    public List<VersionePCCSupportataEditor> versioniPCC_Editor { get; set; } = new();

    public EditorSvc()
    {
        if (versioniPCC_Editor == null || !versioniPCC_Editor.Any()) SetupVersioniPCCDisponibiliEditor();
        if (ProgettoAttivoEditor == null) CaricaProgetto();
        ProgettiEditor = !db!.Progetti.Any() ? new() : db.Progetti.ToList();

    }

    private void CaricaProgetto()
    {
        if (db == null)
        {
            String dir = AppDomain.CurrentDomain.BaseDirectory;
            String nome_def_db = "EditorPCC";

            NuovaDirectory(AppDomain.CurrentDomain.BaseDirectory + "files");
            NuovaDirectory(AppDomain.CurrentDomain.BaseDirectory + "db");


            optionsBuilder = new();
            optionsBuilder.UseSqlite($"Data Source={dir}/db/{nome_def_db}.db")

#if DEBUG
            .EnableSensitiveDataLogging()
#endif
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);


            db = new(optionsBuilder.Options);

            db.Database.EnsureCreated();

            ProgettoAttivoEditor = db.Progetti.Any() ? db.Progetti.OrderByDescending(p => p.Modifica).First() : null;

            if (ProgettoAttivoEditor != null)
            {

                NuovaDirectory(AppDomain.CurrentDomain.BaseDirectory + $"db/{ProgettoAttivoEditor.Nome.Replace(" ", "_")}");
                NuovaDirectory(AppDomain.CurrentDomain.BaseDirectory + $"files/{ProgettoAttivoEditor.Nome.Replace(" ", "_")}");

                /* Se presente, carico l'ultimo progetto modificato */
                optionsBuilder = new();
                optionsBuilder.UseSqlite($"Data Source={dir}/db/{ProgettoAttivoEditor.Nome}/Progetto_{ProgettoAttivoEditor.Nome.Replace(" ", "_")}.db")
#if DEBUG
            .EnableSensitiveDataLogging()
#endif
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                dbProgetto = new(optionsBuilder.Options, true);
                dbProgetto.Database.EnsureCreated();
            }

        }
    }

    private void NuovaDirectory(String dir)
    {
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    public bool NuovoProgetto(ProgettoEditorPCC progetto)
    {
        try
        {
            db.Progetti.Add(progetto);
            ProgettoAttivoEditor = progetto;
            db.SaveChanges();
        }
        catch (Exception)
        {

            return false;
        }

        return true;
    }

    private void SetupVersioniPCCDisponibiliEditor()
    {
        versioniPCC_Editor = new();

        versioniPCC_Editor.Add(new()
        {
            Id = "2001",
            NomeGioco = "PC Calcio 2001",
            CercaFilePattern = new() { "*00036.FDI" },
            CartelleSuperiori = new() { "DBDAT", "Dbdat" }
        });

        versioniPCC_Editor.Add(new()
        {
            Id = "2001F",
            NomeGioco = "PC Futbol 2001",
            CercaFilePattern = new() { "*00022.FDI" },
            CartelleSuperiori = new() { "DBDAT", "Dbdat" }
        });

        versioniPCC_Editor.Add(new()
        {
            Id = "7+",
            NomeGioco = "PC Calcio 7+",
            CercaFilePattern = new() { "*99036.FDI" },
            CartelleSuperiori = new() { "DBDAT" }
        });

        versioniPCC_Editor.Add(new()
        {
            Id = "7+F",
            NomeGioco = "PC Futbol 7+",
            CercaFilePattern = new() { "*99022.FDI" },
            CartelleSuperiori = new() { "DBDAT" }
        });

        versioniPCC_Editor.Add(new()
        {
            Id = "6",
            NomeGioco = "PC Calcio 6",
            CercaFilePattern = new() { "EQ036036.PKF" },
            CartelleSuperiori = new() { "Dbdat", "EQ036036" }
        });

        versioniPCC_Editor.Add(new()
        {
            Id = "5",
            NomeGioco = "PC Calcio 5",
            CercaFilePattern = new() { "EQUIPOS.PKF" },
            CartelleSuperiori = new() { "DBDAT", "EQUIPOS" }
        });

        versioniPCC_Editor.Add(new()
        {
            Id = "4",
            NomeGioco = "PC Calcio 4",
            CercaFilePattern = new() { "EQ95*.DBC" },
            CartelleSuperiori = new() { "DBDAT" }
        });

        versioniPCC_Editor.Add(new()
        {
            Id = "3",
            NomeGioco = "PC Calcio 3",
            CercaFilePattern = new() { "*.DGF" },
            CartelleSuperiori = new() { "EQUIP" }
        });

    }


    public String? CercaVersioneGioco(String dir)
    {
        try
        {
            foreach (var v in versioniPCC_Editor)
            {
                foreach (var p in v.CercaFilePattern)
                {
                    var f = Directory.GetFiles(dir, p, SearchOption.AllDirectories);

                    if (f.Length != 0)
                    {
                        foreach (var pf in f)
                        {

                            var temp = pf.Substring(0, pf.LastIndexOf(Path.DirectorySeparatorChar));
                            foreach (var d in v.CartelleSuperiori)
                                temp = temp.Replace(Path.DirectorySeparatorChar + d, String.Empty);

                            if (Directory.GetFiles(temp, "*.EXE").Length > 1 &&
                                temp.Equals(dir)) return v.NomeGioco;
                        }
                    }
                }
            }


        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }

    //public async Task<List<CartellaGioco>> CercaCartelleValide()
    public List<CartellaGioco> CercaCartelleValide()
    {
        foreach (DriveInfo path in DriveInfo.GetDrives().Reverse().ToList())
        {
            foreach (var dir in Directory.GetDirectories(path.RootDirectory.FullName))
            {
                try
                {
                    foreach (var v in versioniPCC_Editor)
                    {

                        foreach (var p in v.CercaFilePattern)
                        {
                            var f = Directory.GetFiles(dir, p, SearchOption.AllDirectories);

                            if (f.Length != 0)
                            {
                                foreach (var pf in f)
                                {

                                    var temp = pf.Substring(0, pf.LastIndexOf(Path.DirectorySeparatorChar));
                                    foreach (var d in v.CartelleSuperiori)
                                        temp = temp.Replace(Path.DirectorySeparatorChar + d, String.Empty);

                                    if (Directory.GetFiles(temp, "*.EXE").Length > 1 &&
                                        db.CartelleGioco.Find(temp) == null)
                                    {
                                        db.CartelleGioco.Add(new()
                                        {
                                            Path = temp,
                                            VersionePCC = v.Id
                                        });

                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }


                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        return db.CartelleGioco.ToList();
    }

    public List<String> CercaDBEValidi(TipoDatoDB tipoDato = TipoDatoDB.NESSUNO)
    {
        List<String> elencoDBE = new();

        try
        {
            var f = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}files{Path.DirectorySeparatorChar}{ProgettoAttivoEditor!.Nome}{Path.DirectorySeparatorChar}", "*.DBE", SearchOption.AllDirectories);

            if (tipoDato == TipoDatoDB.NESSUNO) return f.ToList();

            foreach (var pf in f)
            {
                var dat = File.ReadAllBytes(pf);
                if (dat.Length > 0 && dat[0] == (byte)tipoDato)
                {
                    var temp = pf.Substring(0, pf.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    elencoDBE.Add(pf.Replace(temp, String.Empty));
                }
            }
        }
        catch (Exception)
        {
            return elencoDBE;
        }
        return elencoDBE;
    }


    public List<String> CercaFileCSVDatiValidi(TipoDatoDB tipoDato = TipoDatoDB.NESSUNO, int codiceElemento = 0)
    {
        List<String> elencoCSV = new();

        try
        {
            var f = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}files{Path.DirectorySeparatorChar}{ProgettoAttivoEditor!.Nome}{Path.DirectorySeparatorChar}", "*.CSV", SearchOption.AllDirectories);

            if (tipoDato == TipoDatoDB.NESSUNO) return f.ToList();

            foreach (var pf in f)
            {
                DatabaseCSV.Versione = 1;
                DatabaseCSV.contenutoCSV = pf; //Passo il percorso del file in lettura, non il contenuto

                if (tipoDato == TipoDatoDB.SQUADRA)
                {

                    var squadre = DatabaseCSV.LeggiSquadre();

                    if (squadre.Any() && (codiceElemento == 0 || (squadre.Count == 1 && squadre.First().Id == codiceElemento)))
                    {
                        var temp = pf.Substring(0, pf.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                        elencoCSV.Add(pf.Replace(temp, String.Empty));
                    }

                }

                //TODO Ricontrollare cosiderazioni giocatore
                if (tipoDato == TipoDatoDB.GIOCATORE)
                {
                    var giocatori = DatabaseCSV.LeggiGiocatori();

                    if (giocatori.Any() && (codiceElemento == 0 || (giocatori.First().CodiceSquadra == codiceElemento)))
                    {

                        var temp = pf.Substring(0, pf.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                        elencoCSV.Add(pf.Replace(temp, String.Empty));
                    }

                }

                if (tipoDato == TipoDatoDB.ALLENATORE)
                {

                    var allenatori = DatabaseCSV.LeggiAllenatori();

                    if (allenatori.Any() && (codiceElemento == 0 || (allenatori.Count == 1 && allenatori.First().Id == codiceElemento)))
                    {

                        var temp = pf.Substring(0, pf.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                        elencoCSV.Add(pf.Replace(temp, String.Empty));
                    }

                }

                if (tipoDato == TipoDatoDB.STADIO)
                {

                    var stadi = DatabaseCSV.LeggiStadi();

                    if (stadi.Any() && (codiceElemento == 0 || (stadi.Count == 1 && stadi.First().Id == codiceElemento)))
                    {

                        var temp = pf.Substring(0, pf.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                        elencoCSV.Add(pf.Replace(temp, String.Empty));
                    }

                }



            }
        }
        catch (Exception)
        {
            return elencoCSV;
        }
        return elencoCSV;
    }

}
