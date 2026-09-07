using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MultiEditorPCC;

public class Client
{

    private HttpClient EditorClient { get; init; }

    private string BaseUrl { get; set; } = "";

    public ClientMod Mod { get; set; } = ClientMod.Editor;

    public HttpResponseMessage? ErrorMsg { get; set; }

    public Client()
    {
        EditorClient = new();
    }
    public async Task Init()
    {
        List<int> Porte = new();
        Porte.Add(2121);
        Porte.Add(21021);

        if (File.Exists("Porte.txt"))
        {
            foreach (var p in File.ReadAllLines("Porte.txt"))
            {
                int.TryParse(p, out int n);
                if (!Porte.Contains(n)) Porte.Add(n);
            }
        }

        foreach (var p in Porte)
        {

            String mediaType = "application/json";

            if (BaseUrl == "" && (await Richiesta(HttpMethod.Get, $"https://localhost:{p}/api/", "", Encoding.UTF8, mediaType)).IsSuccessStatusCode)
            {
                BaseUrl = $"https://localhost:{p}/api/";
                Mod = ClientMod.API;
            }

        }


    }

    public async Task<HttpResponseMessage>? Richiesta(HttpMethod method, string path, string stringContent, Encoding? encoding = null, string mediaType = "application/json")
    {
        if (Mod == ClientMod.Editor && (!path.EndsWith("/api") && !path.EndsWith("/api/")))
        {
            return new()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = new StringContent("Client in modalità Editor!")
            };
        }

        if (encoding == null) encoding = Encoding.UTF8;

        var trailingSlash = !String.IsNullOrEmpty(BaseUrl) &&
                            !BaseUrl.EndsWith("/") && !path.StartsWith("/") ?
                            "/" : String.Empty;

        var request = new HttpRequestMessage(method, new Uri($"{BaseUrl}{trailingSlash}{path}"));


        HttpContent content = new StringContent(stringContent);
        content.Headers.ContentType = new(mediaType);

        request.Content = content;

        HttpResponseMessage? response = null;

        try
        {
            response = await EditorClient.SendAsync(request);
        }
        catch (HttpRequestException ex)
        {
            return new()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = new StringContent("API non collegata")
            };
        }
        return response;
    }

    public async Task<T?> Risposta<T>(HttpMethod method, string path, string stringContent, Encoding? encoding = null, string mediaType = "application/json")
    {
        HttpResponseMessage response = await Richiesta(method, path, stringContent, encoding, mediaType);

        ErrorMsg = null;
        if (!response.IsSuccessStatusCode)
        {
            ErrorMsg = response;
            return default(T);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default(T);

        return response.Content.ReadFromJsonAsync<T>().Result;
    }


}

public enum ClientMod
{
    API = 1,
    Editor = 0
}