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
public partial class GiocatoriViewModel : ViewModelBase, IEventSubscriber<RichiestaRosaSquadra>
{
    [Property] private ObservableCollection<Giocatore> _elencoGiocatori;

    [Property] private Giocatore _giocatore;

    public void OnEvent(RichiestaRosaSquadra eventData)
    {
        EventAggregator.Publish<RosaSquadraSelezionata>((new(ElencoGiocatori.Where(g => g.CodiceSquadra == eventData.IdSquadra).ToList())));
    }
}