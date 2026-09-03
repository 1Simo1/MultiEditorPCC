using MvvmGen;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
public partial class MainViewModel : VM
{
    [Property] private string _testo;
    [Property] private string _footer;

    partial void OnInitialize()
    {
        Testo = string.Empty;

        Footer = TestScritturaFooter();
    }

    private string TestScritturaFooter()
    {
        return """
            Simone per PCCalcio4Ever https://pccalcio4ever.forumcommunity.net/ Progetto Open Source : https://github.com/1Simo1/MultiEditorPCC Versione 0.1 alpha (in costruzione)
            """;
    }
}
