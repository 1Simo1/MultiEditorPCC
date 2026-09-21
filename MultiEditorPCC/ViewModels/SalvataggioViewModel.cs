using MultiEditorPCC.Shared.DTO;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static MultiEditorPCC.EventiMVVM;


namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class SalvataggioViewModel : ViewModelBase, IEventSubscriber<RispostaPaesiGiocabili>
{
    [Property] private ObservableCollection<VersionePCC> _versioni;

    [Property]
    [PropertyCallMethod(nameof(NuovaVersioneSelezionata))]
    private VersionePCC? _versioneSelezionata;



    [Property] private ObservableCollection<Paese> _paesiGiocabili;
    [Property] private Paese _paeseSelezionato;
    [Property] private String _torneoSelezionato;

    partial void OnInitialize()
    {


        TorneoSelezionato = String.Empty;
    }

    private void NuovaVersioneSelezionata()
    {
        if (VersioneSelezionata == null) return;

        EventAggregator.Publish<RichiestaPaesiGiocabili>(new((VersionePCC)VersioneSelezionata));
    }

    public void OnEvent(RispostaPaesiGiocabili eventData)
    {
        PaesiGiocabili = new(eventData.PaesiGiocabili);

        PaeseSelezionato = PaesiGiocabili.First();
    }
}
