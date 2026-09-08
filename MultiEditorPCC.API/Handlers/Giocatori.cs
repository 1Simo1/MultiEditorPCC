using Microsoft.AspNetCore.Http.HttpResults;
using MultiEditorPCC.API.DBContext;
using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.Handlers;

public static class Giocatori
{
    public static async Task<Ok<IEnumerable<Giocatore>>> GetElenco(Editor db)
    {
        return TypedResults.Ok(db.Giocatori.AsEnumerable());
    }
}
