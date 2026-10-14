using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MultiEditorPCC.Shared;
using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.Handlers;

public static class Progetti
{
    public static async Task<Ok<IEnumerable<Progetto>>> GetElenco(Editor db)
    {
        var progetti = Shared.Lib.Handlers.Progetti.GetElenco(db);

        return TypedResults.Ok(progetti.AsEnumerable());
    }

    public static async Task<Results<Ok<Progetto>, NotFound>> InfoProgetto(Editor db, String Nome)
    {

        Progetto? progetto = Shared.Lib.Handlers.Progetti.InfoProgetto(db, Nome);

        if (progetto == null) return TypedResults.NotFound();

        return TypedResults.Ok(progetto);

    }

    public static async Task<Results<Ok<Progetto>, BadRequest<ProblemDetails>>> NuovoProgetto(Editor db, String Nome, String Cartella)
    {

        Progetto Progetto = Shared.Lib.Handlers.Progetti.NuovoProgetto(db, Nome, Cartella);

        if (Progetto == null) return TypedResults.BadRequest<ProblemDetails>(new() { Status = 400, Detail = "Errore nell'aggiunta di un nuovo progetto" });

        return TypedResults.Ok(Progetto);

    }

    public static async Task<Results<Ok<Progetto>, BadRequest<ProblemDetails>>> CaricaProgetto(Editor db, String Nome)
    {
        Progetto? progetto = Shared.Lib.Handlers.Progetti.CaricaProgetto(db, Nome);

        if (progetto == null)
        {
            return TypedResults.BadRequest<ProblemDetails>(new() { Status = 400, Detail = "Errore: progetto non trovato" });
        }

        return TypedResults.Ok(progetto);
    }

    public static async Task<Results<Ok<Progetto>, BadRequest<ProblemDetails>>> CaricaDatiProgetto(Editor db)
    {
        Progetto? progetto = Shared.Lib.Handlers.Progetti.CaricaDatiProgetto(db);

        if (db.ProgettoAttivo == null)
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Nessun progetto attivo nell'editor" }
                );

        return TypedResults.Ok(db.ProgettoAttivo);
    }

    public static async Task<Results<Ok<String>, BadRequest<ProblemDetails>>> CaricaFileDatabaseInfo(Editor db)
    {
        int n = Shared.Lib.Handlers.Progetti.CaricaFileDatabaseInfo(db);
        if (n == -3)
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Nessun progetto attivo nell'editor" }
                );
        if (n == -1)
            return TypedResults.BadRequest<ProblemDetails>(
               new() { Status = 400, Detail = "Errore nel caricamento delle informazioni" }
               );

        return TypedResults.Ok(n.ToString());
    }

    public static async Task<Results<Ok<String>, BadRequest<ProblemDetails>>> CaricaFileDatabaseEditor(Editor db)
    {
        int n = Shared.Lib.Handlers.Progetti.CaricaFileDatabaseEditor(db);
        if (n == -3)
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Nessun progetto attivo nell'editor" }
                );

        if (n == -7)
            return TypedResults.BadRequest<ProblemDetails>(
                new() { Status = 400, Detail = "Errore nella scrittura dei file CSV" }
                );

        return TypedResults.Ok($"{n}");
    }
}
