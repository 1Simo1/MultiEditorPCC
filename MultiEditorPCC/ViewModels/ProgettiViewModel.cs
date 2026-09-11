using MultiEditorPCC.Shared.DTO;
using MvvmGen;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
public partial class ProgettiViewModel : VM
{
    [Property] private string _nuovoProgetto;
    [Property] private string _cartella;
    [Property] private ObservableCollection<Progetto> _elencoProgetti;
    [Property] private Progetto _progetto;


    partial void OnInitialize()
    {
        //EventAggregator.Publish<AppMsgEvent>(new(new AppMsg(
        //    Status.Info, "Titolo Test", $"Test messaggio con variabile : {Progetto?.Cartella}")
        //    ));
    }

    [Command(CanExecuteMethod = nameof(CanConfermaNuovoProgetto))]
    public async void ConfermaNuovoProgetto()
    {

        EventAggregator.Publish(new RichiestaDialog());
        Progetto? p = null;

        try
        {
            p = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/nuovo?Nome={NuovoProgetto}&Cartella={Cartella}", "");
            p = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/caricaPercorsiProgettoAttivo", "");
            await App.Client.Risposta<String>(HttpMethod.Post, $"progetti/db/carica/info", "");
            await App.Client.Risposta<String>(HttpMethod.Post, $"progetti/db/carica/editor", "");
        }
        catch (Exception ex)
        {
            p = null;
            EventAggregator.Publish(new ChiudiDialog());
        }

        if (p != null)
        {
            Progetto = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/carica/{p.Nome}", "");
            EventAggregator.Publish(new AperturaProgetto());
            EventAggregator.Publish(new ChiudiDialog());
        }
    }

    [CommandInvalidate(nameof(NuovoProgetto))]
    [CommandInvalidate(nameof(Cartella))]
    private bool CanConfermaNuovoProgetto() => !String.IsNullOrEmpty(NuovoProgetto) && !String.IsNullOrEmpty(Cartella);


    [Command(CanExecuteMethod = nameof(CanApriProgetto))]
    public async void ApriProgetto(object ProgettoSelezionato)
    {
        var p = (Progetto)ProgettoSelezionato;
        EventAggregator.Publish(new RichiestaDialog());
        Progetto = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/carica/{p.Nome}", "");
        EventAggregator.Publish(new AperturaProgetto());
        EventAggregator.Publish(new ChiudiDialog());
    }

    [CommandInvalidate(nameof(Progetto))]
    private bool CanApriProgetto(object ProgettoSelezionato) => ((Progetto)ProgettoSelezionato)?.Id != Progetto?.Id;

}
