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
}
