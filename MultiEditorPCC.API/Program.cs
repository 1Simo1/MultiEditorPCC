using Microsoft.OpenApi;
using MultiEditorPCC.API;
using MultiEditorPCC.API.DBContext;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Editor>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "MultiEditorPCC.API",
            Version = "v1",
            Description = "MultiEditorPCC API"
        };
        document.Components ??= new OpenApiComponents();
        return Task.CompletedTask;
    });

});



var app = builder.Build();

app.Urls.Add("https://localhost:21021");
app.Urls.Add("http://localhost:2121");

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.BluePlanet)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    }
    );
}

app.RegisterEndpoints();

app.Run();

