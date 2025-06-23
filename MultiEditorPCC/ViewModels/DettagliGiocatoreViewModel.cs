using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class DettagliGiocatoreViewModel : IEventSubscriber<VisualizzaDettagliGiocatoreSelezionato>
{
    [Property] private bool _schedaGiocatore;

    [PropertyPublishEvent(typeof(VisualizzaDettagliGiocatoreSelezionato),
        EventConstructorArgs = "value", PublishCondition = "value is not null && value.Id!=0")]
    [Property]
    private Giocatore _giocatoreSelezionato;

    [Property] private List<PiedePreferito> _selezionePiedePreferito;
    [Property] private List<Reparto> _selezioneRepartoGiocatore;
    [Property] private List<Ruolo> _selezioneRuoloGiocatore;
    [Property] private List<Paese> _selezionePaese;
    [Property] private List<ColorePelle> _selezioneColorePelle;
    [Property] private List<ColoreCapelli> _selezioneColoreCapelli;
    [Property] private List<StileCapelli> _selezioneStileCapelli;
    [Property] private List<StileBarba> _selezioneStileBarba;
    [Property] private bool _ignoraTesti;
    [Property] private String _nomeFileDBE;
    [Property] private ObservableCollection<String> _elencoFileDBEValidi;

    public void OnEvent(VisualizzaDettagliGiocatoreSelezionato eventData)
    {
        GiocatoreSelezionato = eventData.Giocatore;
        SchedaGiocatore = GiocatoreSelezionato.Id != 0;
        var e = App.Services.GetRequiredService<EditorSvc>();
        ElencoFileDBEValidi = new(e.CercaDBEValidi(ArchivioSvc.TipoDatoDB.GIOCATORE));
        NomeFileDBE = $"GT{GiocatoreSelezionato.Id.ToString().PadLeft(5, '0')}.DBE";
    }

    partial void OnInitialize()
    {
        GiocatoreSelezionato = new();
        SchedaGiocatore = false;
        SelezionePiedePreferito = Enum.GetValues<PiedePreferito>().ToList();
        SelezioneRepartoGiocatore = Enum.GetValues<Reparto>().ToList();
        SelezioneRuoloGiocatore = Enum.GetValues<Ruolo>().ToList();
        SelezionePaese = Enum.GetValues<Paese>().ToList();
        SelezioneColorePelle = Enum.GetValues<ColorePelle>().ToList();
        SelezioneColoreCapelli = Enum.GetValues<ColoreCapelli>().ToList();
        SelezioneStileCapelli = Enum.GetValues<StileCapelli>().ToList();
        SelezioneStileBarba = Enum.GetValues<StileBarba>().ToList();
        IgnoraTesti = true;
    }

    [Command]
    private void ChiudiScheda()
    {
        SchedaGiocatore = false;
        EventAggregator.Publish<ChiusuraDialogDettagliGiocatoreSelezionato>(new(GiocatoreSelezionato));
    }


    [Command]
    private void EsportaGiocatore()
    {
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();

        var e = AppSvc.Services.GetRequiredService<EditorSvc>();

        Giocatore giocatore = GiocatoreSelezionato;

        if (IgnoraTesti) giocatore.Testi = new();

        var f = d.EsportaElementoSuFileEditor<Giocatore>(ArchivioSvc.TipoDatoDB.GIOCATORE, giocatore, e.ProgettoAttivoEditor);

        //Aggiungo il file appena esportato all'elenco dei file validi per l'importazione...
        ElencoFileDBEValidi = new(e.CercaDBEValidi(ArchivioSvc.TipoDatoDB.GIOCATORE));

        //... e lo seleziono
        NomeFileDBE = $"GT{GiocatoreSelezionato.Id.ToString().PadLeft(5, '0')}.DBE";
    }

    [Command]
    private void ImportaGiocatore()
    {
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();

        var e = App.Services.GetRequiredService<EditorSvc>();

        String nomeFileEditor = $"{AppDomain.CurrentDomain.BaseDirectory}files{Path.DirectorySeparatorChar}";
        nomeFileEditor += $"{e.ProgettoAttivoEditor.Nome}{Path.DirectorySeparatorChar}{NomeFileDBE}";

        //Come idea nell'importare dati da file DBE, immagino in questa prima versione di memorizzare l'id del giocatore
        //che sto importando, in modo che venga sovrascritto nella squadra corretta
        int id = GiocatoreSelezionato.Id;

        var temp = d.ImportaElementoDaFileEditor<Giocatore>(nomeFileEditor);
        temp.Id = id;

        GiocatoreSelezionato = temp;



    }

}
