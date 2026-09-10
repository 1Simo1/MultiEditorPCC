using Microsoft.AspNetCore.Http.HttpResults;
using MultiEditorPCC.API.DBContext;
using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.Handlers;

public static class Squadre
{
    public static async Task<Ok<IEnumerable<Squadra>>> GetElenco(Editor db)
    {
        return TypedResults.Ok(db.Squadre.AsEnumerable());
    }

    public static async Task<Ok<IEnumerable<Giocatore>>> GetGiocatoriSquadra(Editor db, int id)
    {
        return TypedResults.Ok(db.Giocatori.Where(g => g.CodiceSquadra == id));
    }

    public static async Task<Ok<IEnumerable<Giocatore>>> GiocatoriSenzaSquadra(Editor db)
    {
        return TypedResults.Ok(db.Giocatori.Where(g => g.CodiceSquadra == 0));
    }

    public static async Task<Results<Ok, NotFound, InternalServerError<String>>> SostituisciSquadra(Editor db, int id, int nuovoId)
    {
        if (id == nuovoId) return TypedResults.Ok();

        try
        {
            Squadra? Precedente = db.Squadre.Where(sq => sq.Id == id).FirstOrDefault();
            Squadra? Nuova = db.Squadre.Where(sq => sq.Id == id).FirstOrDefault();

            if (Precedente == null || Nuova == null) return TypedResults.NotFound();

            Precedente = Nuova;

            db.Squadre.Remove(Nuova);
        }
        catch (Exception ex)
        {

            return TypedResults.InternalServerError($"Errore : {ex.Message}");
        }

        return TypedResults.Ok();

    }
}
