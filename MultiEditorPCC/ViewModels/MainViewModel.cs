using System;
using Microsoft.Extensions.DependencyInjection;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
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

    partial void OnInitialize()
    {
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
