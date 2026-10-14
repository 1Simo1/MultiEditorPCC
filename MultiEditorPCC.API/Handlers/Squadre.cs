using Microsoft.AspNetCore.Http.HttpResults;
using MultiEditorPCC.Shared;
using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.Handlers;

public static class Squadre
{
    public static async Task<Ok<IEnumerable<Squadra>>> GetElenco(Editor db)
    {
        return TypedResults.Ok(Shared.Lib.Handlers.Squadre.GetElenco(db).AsEnumerable());
    }

    public static async Task<Ok<IEnumerable<Giocatore>>> GetGiocatoriSquadra(Editor db, int id)
    {
        return TypedResults.Ok(Shared.Lib.Handlers.Squadre.GetGiocatoriSquadra(db, id).AsEnumerable());
    }

    public static async Task<Ok<IEnumerable<Giocatore>>> GiocatoriSenzaSquadra(Editor db)
    {
        return TypedResults.Ok(Shared.Lib.Handlers.Squadre.GiocatoriSenzaSquadra(db).AsEnumerable());
    }

    public static async Task<Results<Ok<String>, InternalServerError<String>>> SostituisciSquadra(Editor db, int id, int nuovoId)
    {
        var sostituisciSquadra = Shared.Lib.Handlers.Squadre.SostituisciSquadra(db, id, nuovoId);
        if (!sostituisciSquadra) return TypedResults.InternalServerError($"Errore");
        return TypedResults.Ok("");
    }

    public static async Task<Ok<(IEnumerable<Squadra>, IEnumerable<SlotCompetizioneSquadra>)>> CalcolaSlot(Editor db, VersionePCC VersionePCC, Paese Paese, String Competizione)
    {
        var calcolaSlot = Shared.Lib.Handlers.Squadre.CalcolaSlot(db, VersionePCC, Paese, Competizione);
        return TypedResults.Ok((calcolaSlot.Item1.AsEnumerable(), calcolaSlot.Item2.AsEnumerable()));
    }
}
