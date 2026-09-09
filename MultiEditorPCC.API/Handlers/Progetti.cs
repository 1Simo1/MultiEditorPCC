using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MultiEditorPCC.API.DBContext;
using MultiEditorPCC.API.Lib;
using MultiEditorPCC.Shared.DTO;
using System.Text.Json;

namespace MultiEditorPCC.API.Handlers;

public static class Progetti
{
    public static async Task<Ok<IEnumerable<Progetto>>> GetElenco(Editor db)
    {
        List<Progetto> progetti = new();

        if (!Directory.Exists("Progetti"))
        {
            Directory.CreateDirectory("Progetti");
            return TypedResults.Ok(progetti.AsEnumerable());
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

        return TypedResults.Ok(progetti.AsEnumerable());

    }

    public static async Task<Results<Ok<Progetto>, NotFound>> InfoProgetto(Editor db, String Nome)
    {

        Progetto? progetto = db.Progetti.Where(p => p.Nome == Nome).FirstOrDefault();

        if (progetto == null) return TypedResults.NotFound();

        return TypedResults.Ok(progetto);

    }

    public static async Task<Results<Ok<Progetto>, BadRequest<ProblemDetails>>> NuovoProgetto(Editor db, String Nome, String Cartella)
    {
        try
        {
            Directory.CreateDirectory($"Progetti/{Nome}");

            if (File.Exists($"Progetti/{Nome}/Pro.json"))
            {
                return TypedResults.BadRequest<ProblemDetails>(new() { Status = 400, Detail = "Errore: progetto già esistente" });
            }

            Progetto progetto = new()
            {
                Nome = Nome,
                Cartella = Cartella
            };

            progetto.VersionePCC = Utils.TestVersionePCC(Cartella);

            File.WriteAllText($"Progetti/{Nome}/Pro.json", JsonSerializer.Serialize(progetto));

            db.Progetti.Add(progetto);

            db.ProgettoAttivo = progetto;

            //await CaricaDatiProgetto(db);

            return TypedResults.Ok(progetto);


        }
        catch (Exception ex)
        {

            return TypedResults.BadRequest<ProblemDetails>(new() { Status = 400, Detail = "Errore nell'aggiunta di un nuovo progetto" });
        }
    }

    public static async Task<Results<Ok<Progetto>, BadRequest<ProblemDetails>>> CaricaProgetto(Editor db, String Nome)
    {
        Progetto? progetto = db.Progetti.Where(p => p.Nome == Nome).FirstOrDefault();

        if (progetto == null)
        {
            return TypedResults.BadRequest<ProblemDetails>(new() { Status = 400, Detail = "Errore: progetto non trovato" });
        }

        db.ProgettoAttivo = progetto;


        db.Squadre = Utils.CaricaSquadreCSV(db.ProgettoAttivo);
        db.Giocatori = Utils.CaricaGiocatoriCSV(db.ProgettoAttivo, db.Squadre);



        return TypedResults.Ok(progetto);
    }

    public static async Task<Results<Ok<Progetto>, BadRequest<ProblemDetails>>> CaricaDatiProgetto(Editor db)
    {
        if (db.ProgettoAttivo == null)
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Nessun progetto attivo nell'editor" }
                );

        db.ProgettoAttivo = Utils.CaricaPercorsiArchivi(db.ProgettoAttivo);
        db.ProgettoAttivo.Modifica = DateTime.Now;

        File.WriteAllText($"Progetti/{db.ProgettoAttivo.Nome}/Pro.json", JsonSerializer.Serialize(db.ProgettoAttivo));


        db.Squadre = Utils.CaricaSquadreCSV(db.ProgettoAttivo);
        db.Giocatori = Utils.CaricaGiocatoriCSV(db.ProgettoAttivo);

        return TypedResults.Ok(db.ProgettoAttivo);
    }

    public static async Task<Results<Ok<String>, BadRequest<ProblemDetails>>> CaricaFileDatabaseInfo(Editor db)
    {
        if (db.ProgettoAttivo == null)
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Nessun progetto attivo nell'editor" }
                );
        try
        {
            db.DatabaseFiles = !db.DatabaseFiles.Any() ?
                Utils.CaricaFileDatabaseInfo(db.ProgettoAttivo) :
                db.DatabaseFiles;
        }
        catch (Exception)
        {

            return TypedResults.BadRequest<ProblemDetails>(
               new() { Status = 400, Detail = "Errore nel caricamento delle informazioni" }
               );
        }

        db.ProgettoAttivo.Modifica = DateTime.Now;

        File.WriteAllText($"Progetti/{db.ProgettoAttivo.Nome}/Pro.json", JsonSerializer.Serialize(db.ProgettoAttivo));
        db.Squadre = Utils.CaricaSquadreCSV(db.ProgettoAttivo);
        db.Giocatori = Utils.CaricaGiocatoriCSV(db.ProgettoAttivo);

        return TypedResults.Ok(db.DatabaseFiles.Count.ToString());
    }

    public static async Task<Results<Ok<String>, BadRequest<ProblemDetails>>> CaricaFileDatabaseEditor(Editor db)
    {
        if (db.ProgettoAttivo == null)
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Nessun progetto attivo nell'editor" }
                );

        db.Squadre = Utils.CaricaSquadre(db.ProgettoAttivo, db.DatabaseFiles);
        db.Giocatori = Utils.CaricaGiocatori(db.ProgettoAttivo, db.DatabaseFiles);

        if (db.DatabaseFiles.Where(e => e.TipoFileDatabase == TipoFileDatabase.FDI).Any())
        {
            db.Giocatori = Utils.AssegnaGiocatoriSquadre(db.Squadre, db.Giocatori);
            db.Squadre.ToList().ForEach(u => u.Note = String.Empty);
        }




        if (!Utils.ScriviCSV(db.ProgettoAttivo, db.Squadre, db.Giocatori))
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Errore nella scrittura dei file CSV" }
                );

        db.ProgettoAttivo.Modifica = DateTime.Now;

        File.WriteAllText($"Progetti/{db.ProgettoAttivo.Nome}/Pro.json", JsonSerializer.Serialize(db.ProgettoAttivo));

        return TypedResults.Ok($"{db.Squadre.Count} {db.Giocatori.Count}");
    }
}
