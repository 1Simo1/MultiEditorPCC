using Microsoft.AspNetCore.Http.HttpResults;

namespace MultiEditorPCC.API.Handlers;

public static class Init
{
    public static async Task<Ok<String>> GetInit()
    {

        return TypedResults.Ok("21");
    }

    public static async Task<Ok<String>> PostInit(String Msg)
    {
        return TypedResults.Ok(Msg);
    }

}
