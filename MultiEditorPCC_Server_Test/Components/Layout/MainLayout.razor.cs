using BlazorStrap.V5;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;

namespace MultiEditorPCC.Components.Layout;

public partial class MainLayout
{
    private string? Selected { get; set; }
    private List<string> _themes = new List<string>();
    private const String VERSION = "5.1.3";
    private const String baseThemeLink =
        $"https://cdn.jsdelivr.net/npm/bootswatch@{VERSION}/dist/___/bootstrap.min.css";
    private String? _themeLink = null;

    private String? NavClass = "null";

    private const String Titolo = "Editor PCC";

    private List<NavMenuLink> Nav = new();

    private CultureInfo? selectedLng;

    private void ToggleNav(MouseEventArgs e)
    {
        if (NavClass == null) return;
        NavClass = NavClass.Contains("collapse") ? NavClass.Replace(" collapse", String.Empty) : $"{NavClass} collapse";

    }


    protected override async void OnInitialized()
    {
        _themes = Enum.GetNames(typeof(Theme)).ToList();
        Selected = "cerulean";
        NavClass = "my-5";
        Nav = NavLinkInit();
        selectedLng = CultureInfo.CurrentCulture;
    }

    protected override async Task OnAfterRenderAsync(bool firstrun)
    {
        if (firstrun)
        {
            Selected = "cerulean";
            await _blazorStrap.SetBootstrapCss(baseThemeLink.Replace("___", Selected));
        }
    }

    private async Task NuovoTema(EventArgs e)
    {
        Selected = ((ChangeEventArgs)e).Value.ToString();
        if (Selected!.ToLower().Equals("bootstrap"))
        {
            _themeLink = baseThemeLink.Replace("___", "css");
            _themeLink = _themeLink.Replace("bootswatch", "bootstrap");
        }
        else _themeLink = baseThemeLink.Replace("___", Selected.ToLower());

        await _blazorStrap.SetBootstrapCss(_themeLink);

    }
    private List<NavMenuLink> NavLinkInit()
    {
        Nav = new();

        Nav.Add(new()
        {
            Id = Nav.Count + 1,
            Link = "/",
            IconString = "fa-solid fa-home-user",
            Title = "Home"
        });

        return Nav;
    }




    private async Task ChangeLanguage()
    {
        if (CultureInfo.CurrentCulture != selectedLng)
        {
            var fullRedirectUri = $"Lng/Set?lng={Uri.EscapeDataString(selectedLng.Name)}";
            fullRedirectUri += $"&localRedirectUri={Uri.EscapeDataString(
                                                    new Uri(Navigation.Uri)
                                                    .GetComponents(UriComponents.PathAndQuery,
                                                                   UriFormat.Unescaped))}";

            Navigation.NavigateTo($"{fullRedirectUri}", forceLoad: true);
        }

    }
}
