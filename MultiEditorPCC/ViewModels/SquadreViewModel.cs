using MultiEditorPCC.Shared.DTO;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class SquadreViewModel : ViewModelBase, IEventSubscriber<AperturaProgetto, RosaSquadraSelezionata>
{

    [Property]
    [PropertyCallMethod(nameof(ElencoModificato))]
    private ObservableCollection<Squadra> _elencoSquadre;

    [Property]
    [PropertyCallMethod(nameof(SquadraSelezionata), MethodArgs = "value?.Id")]
    private Squadra _squadra;



    [Property] private ObservableCollection<Giocatore> _elencoGiocatoriSquadra;



    [Property] private Squadra _squadraAggiornata;

    [Property] private int _totaleSquadreAggiornateValide;


    private void SquadraSelezionata(uint? IdSquadra)
    {
        if (IdSquadra == null) return;
        EventAggregator.Publish<RichiestaRosaSquadra>(new((int)IdSquadra));
    }

    public void OnEvent(RosaSquadraSelezionata eventData)
    {
        ElencoGiocatoriSquadra = new(eventData.Giocatori.OrderBy(g => g.Slot));
        if (Squadra.SquadraOriginale && ElencoGiocatoriSquadra.Any()) Squadra.SquadraOriginale = false;
    }

    private void ElencoModificato()
    {
        var n = ElencoSquadre.Where(sq => !sq.SquadraOriginale).Count();
        TotaleSquadreAggiornateValide = n == 0 ? ElencoSquadre.Count : n;
    }

    public async void OnEvent(AperturaProgetto eventData)
    {
        ElencoSquadre = await App.Client.Risposta<ObservableCollection<Squadra>>(HttpMethod.Get, "squadre", "");
    }




    [Command(CanExecuteMethod = nameof(CanAggiornaSostituisciSquadra))]
    private async void AggiornaSostituisciSquadra()
    {

        int codiceSquadra = (int)Squadra.Id;
        await App.Client.Risposta<String>(HttpMethod.Post, $"squadre/{Squadra.Id}/sostituisciCon/{SquadraAggiornata.Id}", "");

        ElencoSquadre = new(await App.Client.Risposta<ObservableCollection<Squadra>>(HttpMethod.Get, "squadre", ""));
        Squadra = ElencoSquadre.Where(sq => sq.Id == codiceSquadra).FirstOrDefault();
        if (Squadra == null) return;

        EventAggregator.Publish<RichiestaRosaSquadra>(new(codiceSquadra, true));


        if (Squadra.SquadraOriginale && ElencoGiocatoriSquadra.Any()) Squadra.SquadraOriginale = false;
    }

    [CommandInvalidate(nameof(Squadra))]
    [CommandInvalidate(nameof(SquadraAggiornata))]
    private bool CanAggiornaSostituisciSquadra()
    {
        return Squadra == null || SquadraAggiornata == null || Squadra.Id == SquadraAggiornata.Id ? false : true;
    }
}
