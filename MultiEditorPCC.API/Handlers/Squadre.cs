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

}
