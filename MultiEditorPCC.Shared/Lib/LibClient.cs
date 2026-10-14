using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Shared.DTO;
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
                    {
                        String v = pq.Split('=')[1];
                        object p = System.Uri.UnescapeDataString(v);

                        if (parametro.ParameterType.Name != "String")
                        {

                            p = (parametro.ParameterType.Name) switch
                            {
                                "Int32" => Int32.Parse(v),
                                "VersionePCC" => (VersionePCC)int.Parse(v),
                                "Paese" => (Paese)int.Parse(v),
                                _ => new()
                            };
                        }

                        Parametri.Add(p);
                    }

                }
            }
        }

        LibResponse.Content = new StringContent(
            JsonSerializer.Serialize(Handler.Method.Invoke(Handler, Parametri.ToArray())));


        return LibResponse;


    }

    private Delegate? GetHandler(String method, String requestUri)
    {
        return (method, requestUri) switch
        {
            ("GET", "progetti") => Progetti.GetElenco,
            ("POST", var request) when request.StartsWith("progetti/nuovo") => Progetti.NuovoProgetto,
            ("POST", var request) when request.StartsWith("progetti/carica") => Progetti.CaricaProgetto,
            ("POST", "progetti/caricaPercorsiProgettoAttivo") => Progetti.CaricaDatiProgetto,
            ("POST", "progetti/db/carica/info​") => Progetti.CaricaFileDatabaseInfo,
            ("POST", "progetti/db/carica/editor") => Progetti.CaricaFileDatabaseEditor,
            ("GET", "squadre") => Squadre.GetElenco,
            ("POST", var request) when request.StartsWith("squadre/sostituisci") => Squadre.SostituisciSquadra,
            ("POST", var request) when request.StartsWith("squadre/calcolaSlot") => Squadre.CalcolaSlot,
            ("GET", "giocatori") => Giocatori.GetElenco,
            ("GET", "giocatori/svincolati") => Squadre.GiocatoriSenzaSquadra,

            (_, _) => null
        };
    }
}
