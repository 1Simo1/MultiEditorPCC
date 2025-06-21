using Microsoft.Extensions.DependencyInjection;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using System;
using System.Text;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
public partial class MainViewModel : IEventSubscriber<ConfermatoNuovoProgettoAttivo>
{


    public String nomePagina { get; set; }

    [Property] private ViewModelBase _pag;

    [Property] private String _nome;

    [Property] private String _cartella;

    [Property] private String _versionePCC;

    [Property] private String _testoCP;

    private readonly String a = $@"MjAyNSBTaW1vbmUgcGV
                                   yIFBDQ2FsY2lvNEV2ZXIgaHR0
                                   cHM6Ly9wY2NhbGNpbzRldmVyLmZ
                                   vcnVtY29tbXVuaXR5Lm5ldC8gUHJvZ2V0dG8g
                                   T3BlbiBTb3VyY2UgOiBodHRwczovL2dpdGh1Yi5
                                   jb20vMVNpbW8xL011bHRpRWRpdG9yUENDVmVyc2lvbm
                                   UgMC4xIGFscGhhIChpbiBjb3N0cnV6aW9uZSk =";

    partial void OnInitialize()
    {
        TestoCP = Encoding.ASCII.GetString(Convert.FromBase64String(a)).Replace("PCCV", "PCC V");


        if (Pag == null)
        {


            nomePagina = "Intro";
            Pag = App.Services.GetRequiredService<NavSvc>().Nav(nomePagina);
        }
    }


    [Command]
    private void Nav(object Nome)
    {
        if (Nome.ToString() == nomePagina) return;

        Pag = App.Services.GetRequiredService<NavSvc>().Nav(Nome.ToString());

        if (Pag == null) nomePagina = String.Empty;

    }

    public void OnEvent(ConfermatoNuovoProgettoAttivo e)
    {
        Nome = e.Progetto.Nome;
        Cartella = e.Progetto.Cartella;
        VersionePCC = e.Progetto.VersionePCC;
    }
}
