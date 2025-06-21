using BlazorStrap;
using MultiEditorPCC.Components;
using MultiEditorPCC.Lib;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorStrap();

builder.Services.AddSingleton<Setup>();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddHttpClient();
builder.Services.AddControllers();

//builder.Services.AddSingleton<MsgService>();
//builder.Services.AddSingleton<MsgAvviso>();
var app = builder.Build();


IConfiguration configuration = app.Configuration;

var supportedCultures = app.Configuration.GetSection("Lng")
    .GetChildren().ToDictionary(k => k.Key, v => v.Value);

app.UseRequestLocalization(
    new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures.First().Key)
    .AddSupportedCultures(supportedCultures.Keys.ToArray())
    .AddSupportedUICultures(supportedCultures.Keys.ToArray())
    );


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

var conf = builder.Configuration;

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
