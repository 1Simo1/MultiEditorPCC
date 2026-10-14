using MultiEditorPCC.Shared.DTO;
using System.Text.Json;

namespace MultiEditorPCC.Shared.Lib.Handlers;

public static class Progetti
{
    public static List<Progetto> GetElenco(Editor db)
    {
        List<Progetto> progetti = new();

        if (!Directory.Exists("Progetti"))
        {
            Directory.CreateDirectory("Progetti");
            return new();
        }

        var files = Directory.GetFiles("Progetti", "Pro.json", SearchOption.AllDirectories);

        foreach (var file in files)
        {

            try
            {
                progetti.Add(JsonSerializer.Deserialize<Progetto>(File.ReadAllText(file)));
            }
            catch (Exception ex)
            {

                continue;
            }


        }

        if (!db.Progetti.Any()) db.Progetti.AddRange(progetti);

        if (db.Progetti.Any()) db.ProgettoAttivo = db.Progetti.Where(p => p.Modifica.Ticks == db.Progetti.Max(p => p.Modifica.Ticks)).First();

        return progetti;
    }

    public static Progetto? InfoProgetto(Editor db, String Nome)
    {
        Progetto? progetto = db.Progetti.Where(p => p.Nome == Nome).FirstOrDefault();

        if (progetto == null) return null;

        return progetto;
    }

    public static Progetto? NuovoProgetto(Editor db, String Nome, String Cartella)
    {
        try
        {
            Directory.CreateDirectory($"Progetti/{Nome}");

            if (File.Exists($"Progetti/{Nome}/Pro.json")) return null;


            Progetto progetto = new()
            {
                Nome = Nome,
                Cartella = Cartella
            };

            progetto.VersionePCC = Utils.TestVersionePCC(Cartella);

            File.WriteAllText($"Progetti/{Nome}/Pro.json", JsonSerializer.Serialize(progetto));

            db.Progetti.Add(progetto);

            db.ProgettoAttivo = progetto;

            return progetto;

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static Progetto? CaricaProgetto(Editor db, String Nome)
    {

        Progetto? progetto = db.Progetti.Where(p => p.Nome == Nome).FirstOrDefault();
        if (progetto == null) return null;


        db.ProgettoAttivo = progetto;


        db.Squadre = Utils.CaricaSquadreCSV(db.ProgettoAttivo);
        db.Giocatori = Utils.CaricaGiocatoriCSV(db.ProgettoAttivo, db.Squadre);

        foreach (var sq in db.Squadre)
        {
            try
            {
                var GiocatoriSquadra = db.Giocatori.Where(g => g.CodiceSquadra == sq.Id);
                if (GiocatoriSquadra.Any())
                {
                    var anno = GiocatoriSquadra.Max(g => g.AnnoNascita);
                    sq.SquadraOriginale = GiocatoriSquadra.Where(g => g.AnnoNascita == anno).First().AnnoNascita < 2000;
                }
            }
            catch (Exception ex)
            {
            }
        }

        progetto.Modifica = DateTime.Now;
        db.ProgettoAttivo.Modifica = DateTime.Now;

        File.WriteAllText($"Progetti/{db.ProgettoAttivo.Nome}/Pro.json", JsonSerializer.Serialize(db.ProgettoAttivo));

        return progetto;
    }

    public static Progetto? CaricaDatiProgetto(Editor db)
    {
        if (db.ProgettoAttivo == null) return null;


        db.ProgettoAttivo = Utils.CaricaPercorsiArchivi(db.ProgettoAttivo);
        db.ProgettoAttivo.Modifica = DateTime.Now;

        File.WriteAllText($"Progetti/{db.ProgettoAttivo.Nome}/Pro.json", JsonSerializer.Serialize(db.ProgettoAttivo));


        //db.Squadre = Utils.CaricaSquadreCSV(db.ProgettoAttivo);
        //db.Giocatori = Utils.CaricaGiocatoriCSV(db.ProgettoAttivo);

        return db.ProgettoAttivo;
    }

    public static int CaricaFileDatabaseInfo(Editor db)
    {
        if (db.ProgettoAttivo == null) return -3;

        try
        {
            db.DatabaseFiles = !db.DatabaseFiles.Any() ?
                Utils.CaricaFileDatabaseInfo(db.ProgettoAttivo) :
                db.DatabaseFiles;
        }
        catch (Exception)
        {
            return -1;
        }

        db.ProgettoAttivo.Modifica = DateTime.Now;


        File.WriteAllText($"Progetti/{db.ProgettoAttivo.Nome}/Pro.json", JsonSerializer.Serialize(db.ProgettoAttivo));
        //db.Squadre = Utils.CaricaSquadreCSV(db.ProgettoAttivo);
        //db.Giocatori = Utils.CaricaGiocatoriCSV(db.ProgettoAttivo);

        return db.DatabaseFiles.Count;
    }

    public static int CaricaFileDatabaseEditor(Editor db)
    {
        if (db.ProgettoAttivo == null) return -3;

        db.Squadre = Utils.CaricaSquadre(db.ProgettoAttivo, db.DatabaseFiles);
        db.Giocatori = Utils.CaricaGiocatori(db.ProgettoAttivo, db.DatabaseFiles);

        if (db.DatabaseFiles.Where(e => e.TipoFileDatabase == TipoFileDatabase.FDI).Any())
        {
            db.Giocatori = Utils.AssegnaGiocatoriSquadre(db.Squadre, db.Giocatori);
            db.Squadre.ToList().ForEach(u => u.Note = String.Empty);
        }

        if (!Utils.ScriviCSV(db.ProgettoAttivo, db.Squadre, db.Giocatori)) return -7;

        db.ProgettoAttivo.Modifica = DateTime.Now;

        File.WriteAllText($"Progetti/{db.ProgettoAttivo.Nome}/Pro.json", JsonSerializer.Serialize(db.ProgettoAttivo));

        return db.Squadre.Count;
    }
}
