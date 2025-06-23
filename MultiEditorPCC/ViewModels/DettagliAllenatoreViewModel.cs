using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class DettagliAllenatoreViewModel : IEventSubscriber<VisualizzaDettagliAllenatoriSquadraSelezionata>
{
    [Property] private String _nomeSquadra;
    [Property] private ObservableCollection<Allenatore> _allenatori;

    [PropertyCallMethod(nameof(AssegnaDBEAllenatoreSelezionato))]
    [Property] private Allenatore _allenatoreSelezionato;

    [Property] private bool _ignoraTesti;
    [Property] private String _nomeFileDBE;
    [Property] private ObservableCollection<String> _elencoFileDBEValidi;

    [Property] private bool _schedaAllenatore;

    partial void OnInitialize()
    {
        SchedaAllenatore = false;
        //TODO Decidere se lasciare temporaneo o se di defualt ignorare i testi da esportare ed importare su file DBE
        IgnoraTesti = true;
    }


    public void OnEvent(VisualizzaDettagliAllenatoriSquadraSelezionata eventData)
    {
        Allenatori = new(eventData.Squadra.Allenatori);
        NomeSquadra = eventData.Squadra.Nome;
        AllenatoreSelezionato = Allenatori.Last();
        SchedaAllenatore = true;
    }


    private void AssegnaDBEAllenatoreSelezionato()
    {
        if (AllenatoreSelezionato == null) return;
        var e = App.Services.GetRequiredService<EditorSvc>();
        ElencoFileDBEValidi = new(e.CercaDBEValidi(ArchivioSvc.TipoDatoDB.ALLENATORE));
        NomeFileDBE = $"AL{AllenatoreSelezionato.Id.ToString().PadLeft(5, '0')}.DBE";
    }

    [Command]

    private void ChiudiScheda()
    {
        SchedaAllenatore = false;
        EventAggregator.Publish<ChiusuraDialogDettagliAllenatoriSquadraSelezionata>(new(Allenatori.ToList()));

    }

    [Command]
    private void EsportaAllenatore()
    {
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();

        var e = AppSvc.Services.GetRequiredService<EditorSvc>();

        if (IgnoraTesti) AllenatoreSelezionato.Testi = new();

        var f = d.EsportaElementoSuFileEditor<Allenatore>(ArchivioSvc.TipoDatoDB.ALLENATORE, AllenatoreSelezionato, e.ProgettoAttivoEditor);

        //Aggiungo il file appena esportato all'elenco dei file validi per l'importazione...
        ElencoFileDBEValidi = new(e.CercaDBEValidi(ArchivioSvc.TipoDatoDB.ALLENATORE));

        //... e lo seleziono
        NomeFileDBE = $"AL{AllenatoreSelezionato.Id.ToString().PadLeft(5, '0')}.DBE";

    }

    [Command]
    private void ImportaAllenatore()
    {
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();

        var e = App.Services.GetRequiredService<EditorSvc>();

        String nomeFileEditor = $"{AppDomain.CurrentDomain.BaseDirectory}files{Path.DirectorySeparatorChar}";
        nomeFileEditor += $"{e.ProgettoAttivoEditor.Nome}{Path.DirectorySeparatorChar}{NomeFileDBE}";

        //Come idea nell'importare dati da file DBE, immagino in questa prima versione di memorizzare l'id dell'allenatore
        //che sto importando, in modo che venga sovrascritto nella squadra corretta
        int id = (int)AllenatoreSelezionato.Id;

        var temp = d.ImportaElementoDaFileEditor<Allenatore>(nomeFileEditor);
        temp.Id = (uint)id;

        AllenatoreSelezionato = temp;
    }


}
