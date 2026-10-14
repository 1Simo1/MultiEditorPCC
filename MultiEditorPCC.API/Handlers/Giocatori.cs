using Microsoft.AspNetCore.Http.HttpResults;
using MultiEditorPCC.Shared;
using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.Handlers;

public static class Giocatori
{
    public static async Task<Ok<IEnumerable<Giocatore>>> GetElenco(Editor db)
    {
        return TypedResults.Ok(Shared.Lib.Handlers.Giocatori.GetElenco(db).AsEnumerable());
    }
}
