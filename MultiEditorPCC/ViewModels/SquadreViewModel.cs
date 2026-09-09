using MultiEditorPCC.Shared.DTO;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class SquadreViewModel : ViewModelBase, IEventSubscriber<RosaSquadraSelezionata>
{
    [Property] private ObservableCollection<Squadra> _elencoSquadre;

    [Property]
    [PropertyCallMethod(nameof(SquadraSelezionata), MethodArgs = "value?.Id")]
    private Squadra _squadra;

    [Property] private ObservableCollection<Giocatore> _elencoGiocatoriSquadra;

    //[Property] private Giocatore _giocatore;


    private void SquadraSelezionata(uint? IdSquadra)
    {
        if (IdSquadra == null) return;
        EventAggregator.Publish<RichiestaRosaSquadra>(new((int)IdSquadra));
    }

    public void OnEvent(RosaSquadraSelezionata eventData)
    {
        ElencoGiocatoriSquadra = new(eventData.Giocatori.OrderBy(g => g.Slot));
    }
}
