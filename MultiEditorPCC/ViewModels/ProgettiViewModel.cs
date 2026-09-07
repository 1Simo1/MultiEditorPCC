using MultiEditorPCC.Shared.DTO;
using MvvmGen;
using System;
using System.Collections.ObjectModel;

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


    }

    [CommandInvalidate(nameof(NuovoProgetto))]
    [CommandInvalidate(nameof(Cartella))]
    private bool CanConfermaNuovoProgetto() => !String.IsNullOrEmpty(NuovoProgetto) && !String.IsNullOrEmpty(Cartella);


    [Command(CanExecuteMethod = nameof(CanApriProgetto))]
    public void ApriProgetto(object ProgettoSelezionato)
    {

    }

    [CommandInvalidate(nameof(Progetto))]
    private bool CanApriProgetto(object ProgettoSelezionato) => ((Progetto)ProgettoSelezionato).Id != Progetto.Id;

}
