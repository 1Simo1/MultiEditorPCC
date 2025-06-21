namespace MultiEditorPCC.Components.Pages;

public partial class Home
{

    private List<String> cartelleValide { get; set; }

    private String versioneSelezionata { get; set; }

    protected override Task OnInitializedAsync()
    {
        cartelleValide = new List<String>();
        var progetto = Setup.ProgettoAttivoEditor;
        return base.OnInitializedAsync();
    }

    private void ImpostaVersione()
    {
        //cartelleValide = Setup.CercaCartelleValide(versioneSelezionata).Result;
    }
}
