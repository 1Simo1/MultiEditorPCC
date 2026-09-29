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
public partial class SalvataggioViewModel : ViewModelBase, IEventSubscriber<RispostaPaesiGiocabili, RispostaCompetizioniPaese>
{
    [Property] private ObservableCollection<VersionePCC> _versioni;

    [Property]
    [PropertyCallMethod(nameof(NuovaVersioneSelezionata))]
    private VersionePCC? _versioneSelezionata;



    [Property] private ObservableCollection<Paese> _paesiGiocabili;


    [Property]
    [PropertyCallMethod(nameof(CercaCompetizioniPaeseSelezionato))]
    private Paese _paeseSelezionato;

    [Property] private ObservableCollection<String> _tornei;

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

    private void CercaCompetizioniPaeseSelezionato()
    {
        if (PaeseSelezionato == null || VersioneSelezionata == null) return;

        EventAggregator.Publish<RichiestaCompetizioniPaese>(new((VersionePCC)VersioneSelezionata, PaeseSelezionato));
    }


    public void OnEvent(RispostaPaesiGiocabili eventData)
    {
        PaesiGiocabili = new(eventData.PaesiGiocabili);

        PaeseSelezionato = PaesiGiocabili.First();
    }

    public void OnEvent(RispostaCompetizioniPaese eventData)
    {
        Tornei = new(eventData.Competizioni);
        TorneoSelezionato = Tornei.First();
    }
}
