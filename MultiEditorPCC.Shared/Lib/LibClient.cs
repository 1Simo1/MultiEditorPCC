using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Shared.Lib.Handlers;
using System.Reflection;
using System.Text.Json;

namespace MultiEditorPCC.Shared.Lib;

public class LibClient(String VirtualBaseUrl = "")
{
    private Delegate? Handler { get; set; }

    private Editor DB { get; set; } = App.Services.GetRequiredService<Editor>();

    public async Task<HttpResponseMessage> Risposta(HttpRequestMessage request)
    {
        var LibResponse = new HttpResponseMessage();

        Handler = GetHandler(request.Method.Method, request.RequestUri.AbsoluteUri.Substring(VirtualBaseUrl.Length));

        if (Handler == null)
        {
            LibResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            return LibResponse;
        }


        LibResponse.StatusCode = System.Net.HttpStatusCode.OK;


        List<object> Parametri = new();

        var InfoParametriRichiesti = Handler.Method.GetParameters();

        List<String> ParametriQuery = new();

        var StringaParametriQuery = request.RequestUri.Query;
        if (!String.IsNullOrEmpty(StringaParametriQuery))
        {
            StringaParametriQuery = StringaParametriQuery.Substring(1);
            ParametriQuery = StringaParametriQuery.Split('&').ToList();
        }

        foreach (ParameterInfo parametro in InfoParametriRichiesti)
        {
            if (parametro.ParameterType.Name == "Editor")
            {
                Parametri.Add(DB);
            }
            else
            {
                foreach (var pq in ParametriQuery)
                {
                    if (parametro.Name == pq.Split('=')[0])
                        Parametri.Add(System.Uri.UnescapeDataString(pq.Split('=')[1]));
                }
            }
        }

        LibResponse.Content = new StringContent(
            JsonSerializer.Serialize(Handler.Method.Invoke(Handler, Parametri.ToArray())));



        //(HttpContent);

        return LibResponse;


    }

    private Delegate? GetHandler(String method, String requestUri)
    {
        return (method, requestUri) switch
        {
            ("GET", "progetti") => Progetti.GetElenco,
            ("POST", var request) when request.StartsWith("progetti/carica")
            => Progetti.CaricaProgetto,

            (_, _) => null
        };
    }
}
