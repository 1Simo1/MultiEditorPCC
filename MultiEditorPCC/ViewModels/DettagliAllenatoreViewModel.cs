using MultiEditorPCC.Dat.DbSet;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class DettagliAllenatoreViewModel : IEventSubscriber<VisualizzaDettagliAllenatoriSquadraSelezionata>
{
    [Property] private String _nomeSquadra;
    [Property] private ObservableCollection<Allenatore> _allenatori;
    [Property] private Allenatore _allenatoreSelezionato;

    [Property] private bool _schedaAllenatore;

    partial void OnInitialize()
    {
        SchedaAllenatore = false;
    }


    public void OnEvent(VisualizzaDettagliAllenatoriSquadraSelezionata eventData)
    {
        Allenatori = new(eventData.Squadra.Allenatori);
        NomeSquadra = eventData.Squadra.Nome;
        AllenatoreSelezionato = Allenatori.Last();
        SchedaAllenatore = true;
    }

    [Command]

    private void ChiudiScheda()
    {
        SchedaAllenatore = false;
        EventAggregator.Publish<ChiusuraDialogDettagliAllenatoriSquadraSelezionata>(new(Allenatori.ToList()));

    }


}
