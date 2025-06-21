using Microsoft.EntityFrameworkCore;
using MultiEditorPCC.Dat.DbContext;
using MultiEditorPCC.Setup;


List<VersionePCCSupportataEditor> versioniPCC_Editor = SetupVersioniPCCDisponibiliEditor();

String dir = AppDomain.CurrentDomain.BaseDirectory;
String nome_def_db = "EditorPCC";

if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "files")) Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "files");
if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "db")) Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "db");


DbContextOptionsBuilder<MultiEditorPCCDbContext> optionsBuilder = new();
optionsBuilder.UseSqlite($"Data Source={dir}/db/{nome_def_db}.db")

#if DEBUG
.EnableSensitiveDataLogging()
#endif
.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

MultiEditorPCCDbContext db = new(optionsBuilder.Options);

db.Database.EnsureCreated();

CercaCartelleValide();

/// <summary>
/// Imposta le versioni del gioco disponibili nell'editor
/// </summary>
List<VersionePCCSupportataEditor> SetupVersioniPCCDisponibiliEditor()
{
    List<VersionePCCSupportataEditor> versioniPCC_Editor = new();

    versioniPCC_Editor.Add(new()
    {
        Id = "2001",
        NomeGioco = "PC Calcio 2001",
        CercaFilePattern = new() { "*00036.FDI" },
        CartelleSuperiori = new() { "DBDAT" }
    });

    versioniPCC_Editor.Add(new()
    {
        Id = "2001F",
        NomeGioco = "PC Futbol 2001",
        CercaFilePattern = new() { "*00022.FDI" },
        CartelleSuperiori = new() { "DBDAT" }
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
        Id = "6",
        NomeGioco = "PC Calcio 6",
        CercaFilePattern = new() { "EQ036036.PKF", "EQ97*.DBC" },
        CartelleSuperiori = new() { "DBDAT", "EQ036036" }
    });

    versioniPCC_Editor.Add(new()
    {
        Id = "5",
        NomeGioco = "PC Calcio 5",
        CercaFilePattern = new() { "EQUIPOS.PKF", "EQ96*.DBC" },
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
        CercaFilePattern = new() { "CAL1EQ*.DGF" },
        CartelleSuperiori = new() { "EQUIP" }
    });

    return versioniPCC_Editor;

}

void CercaCartelleValide()
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

                                    Console.WriteLine($"{temp} ({v.Id})");

                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                continue;
            }
        }
    }


    return;
}