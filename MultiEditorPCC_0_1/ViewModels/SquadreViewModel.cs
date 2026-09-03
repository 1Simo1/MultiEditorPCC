using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class SquadreViewModel : IEventSubscriber<SquadraSelezionataElenco,
                                                         ChiusuraDialogDettagliGiocatoreSelezionato,
                                                         ChiusuraDialogDettagliAllenatoriSquadraSelezionata>
{
    [Property] private List<Paese> _elencoPaesi;
    [Property] private Paese _paese;
    [Property] private String _nomeSquadra;
    [Property] private int _numeroSquadreTrovate;
    [Property] private ObservableCollection<Squadra> _squadreTrovate;
    [Property] private ObservableCollection<Giocatore> _giocatoriSquadraSelezionata;

    [PropertyPublishEvent(typeof(SquadraSelezionataElenco), EventConstructorArgs = "value")]
    [Property]
    private Squadra _squadraSelezionata;

    /* Flag per memorizzare se una qualsiasi scheda (giocatore, allenatore, stadio)
     * è stata selezionata per visualizzare i dettagli: in questo caso, la parte della
     * squadra non sarà visibile, ma solo la scheda dettaglio del tipo scelto
     * (SchedaGiocatore, SchedaAllenatore, SchedaStadio) */
    [Property] private bool _schedaVisualizzata;

    [Property] private bool _schedaGiocatore;
    [Property] private bool _schedaAllenatore;
    [Property] private bool _schedaStadio;
    [Property] private bool _schedaInformazioniSquadra;

    [Property] private String _nomeFileSquadraDBE;
    [Property] private ObservableCollection<String> _elencoFileDBESquadraValidi;

    [Property] private ObservableCollection<String> _elencoFileCSVSquadraValidi;
    [Property] private ObservableCollection<String> _elencoFileCSVGiocatoriValidi;
    [Property] private ObservableCollection<String> _elencoFileCSVAllenatoreValidi;
    [Property] private ObservableCollection<String> _elencoFileCSVStadioValidi;

    [Property] private String? _nomeFileSquadraCSV;
    [Property] private String? _nomeFileGiocatoriCSV;
    [Property] private String? _nomeFileAllenatoreCSV;
    [Property] private String? _nomeFileStadioCSV;

    [Property] private Giocatore _giocatoreSelezionato;

    partial void OnInitialize()
    {
        ElencoPaesi = Enum.GetValues<Paese>().ToList();
        NumeroSquadreTrovate = 0;

        SchedaGiocatore = false;
        SchedaAllenatore = false;
        SchedaStadio = false;
        SchedaInformazioniSquadra = false;

        SchedaVisualizzata = SchedaGiocatore || SchedaAllenatore || SchedaStadio || SchedaInformazioniSquadra;

    }


    [Command]
    private void ConfermaSceltaPaese(object PaeseSelezionato)
    {
        if (PaeseSelezionato != null && PaeseSelezionato.ToString().Equals("*"))
        {
            Paese = 0;
        }
    }

    [Command]
    private void ConfermaSquadrePaese()
    {
        NomeSquadra = String.Empty;
    }


    [Command]
    private void Cerca()
    {

        SquadraSelezionata = null;
        NomeFileSquadraDBE = null;

        var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

        if (String.IsNullOrEmpty(NomeSquadra?.Trim()) && Paese == 0)
        {//Tutte le squadre 
            SquadreTrovate = new(a.DatiProgettoAttivo.Squadre);

        }

        if (String.IsNullOrEmpty(NomeSquadra?.Trim()) && Paese != 0)
        {//Squadre del Paese specificato
            SquadreTrovate = new(a.DatiProgettoAttivo.Squadre.Where(sq => sq.Nazione == Paese));

        }

        if (!String.IsNullOrEmpty(NomeSquadra?.Trim()) && Paese == 0)
        {//Squadre trovate per nome
            SquadreTrovate = new(a.DatiProgettoAttivo.Squadre.Where(sq => sq.Nome.Contains(NomeSquadra)));
        }

        if (!String.IsNullOrEmpty(NomeSquadra?.Trim()) && Paese != 0)
        {//Ricerca esatta, all'interno del Paese scelto, delle squadre con nome simile a quello specificato
            SquadreTrovate = new(a.DatiProgettoAttivo.Squadre.Where(sq => sq.Nome.Contains(NomeSquadra) && sq.Nazione == Paese));

        }

        NumeroSquadreTrovate = SquadreTrovate.Count;
    }

    public void OnEvent(SquadraSelezionataElenco eventData)
    {
        GiocatoriSquadraSelezionata = new();

        if (SquadraSelezionata == null || !SquadraSelezionata.Giocatori.Any()) return;

        var e = App.Services.GetRequiredService<EditorSvc>();

        ElencoFileDBESquadraValidi = new(e.CercaDBEValidi(ArchivioSvc.TipoDatoDB.SQUADRA));
        NomeFileSquadraDBE = $"SQ{SquadraSelezionata.Id.ToString().PadLeft(5, '0')}.DBE";


        if (SquadraSelezionata.Giocatori.First().Numero == -1)
        {
            var d = AppSvc.Services.GetRequiredService<IDatSvc>();
            SquadraSelezionata = d.ComponiInformazioniCompleteSquadra(SquadraSelezionata);
        }

        GiocatoriSquadraSelezionata = new(SquadraSelezionata.Giocatori.OrderBy(g => g.Slot));

        AggiornaElenchiCSVSquadraValidi();
    }

    [Command]

    private void AggiornaElenchiCSVSquadraValidi()
    {
        var e = App.Services.GetRequiredService<EditorSvc>();

        ElencoFileCSVSquadraValidi = new(
          e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.SQUADRA,
                                   (int)SquadraSelezionata.Id)
                                    .Select(x => x.Percorso)
          );

        if (!ElencoFileCSVSquadraValidi.Any()) NomeFileSquadraCSV = null;
        if (ElencoFileCSVSquadraValidi.Count == 1) NomeFileSquadraCSV = ElencoFileCSVSquadraValidi[0];

        ElencoFileCSVGiocatoriValidi = new(
         e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.GIOCATORE,
                                  (int)SquadraSelezionata.Id)
                                    .Select(x => x.Percorso)
        );

        if (!ElencoFileCSVGiocatoriValidi.Any()) NomeFileGiocatoriCSV = null;
        if (ElencoFileCSVGiocatoriValidi.Count == 1) NomeFileGiocatoriCSV = ElencoFileCSVGiocatoriValidi[0];

        ElencoFileCSVAllenatoreValidi = new(
         e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.ALLENATORE,
                                  (int)SquadraSelezionata.Allenatori.Last().Id)
         .Select(x => x.Percorso)
        );

        if (!ElencoFileCSVAllenatoreValidi.Any()) NomeFileAllenatoreCSV = null;
        if (ElencoFileCSVAllenatoreValidi.Count == 1) NomeFileAllenatoreCSV = ElencoFileCSVAllenatoreValidi[0];

        ElencoFileCSVStadioValidi = new(
         e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.STADIO,
                                  (int)SquadraSelezionata.Id)
         .Select(x => x.Percorso)
        );

        if (!ElencoFileCSVStadioValidi.Any()) NomeFileStadioCSV = null;
        if (ElencoFileCSVStadioValidi.Count == 1) NomeFileStadioCSV = ElencoFileCSVStadioValidi[0];
    }

    [Command]
    private void DettagliGiocatore(object giocatore)
    {
        if (giocatore != null)
        {
            SchedaGiocatore = true;
            SchedaVisualizzata = true;
        }

        EventAggregator.Publish<VisualizzaDettagliGiocatoreSelezionato>(new((Giocatore)giocatore));

    }


    [Command]
    private void VisualizzaInformazioniSquadra()
    {
        if (SquadraSelezionata == null) return;
        SchedaInformazioniSquadra = true;
        SchedaVisualizzata = true;
    }

    [Command]
    private void VisualizzaDettagliStadio()
    {
        if (SquadraSelezionata == null) return;
        SchedaStadio = true;
        SchedaVisualizzata = true;
    }


    [Command]
    private void VisualizzaDettagliAllenatori()
    {
        if (SquadraSelezionata == null) return;
        EventAggregator.Publish<VisualizzaDettagliAllenatoriSquadraSelezionata>(new(SquadraSelezionata));
        SchedaAllenatore = true;
        SchedaVisualizzata = true;
    }

    [Command]
    private void EsportaSquadra()
    {
        if (SquadraSelezionata == null) return;
        //TODO EsportaSquadra
    }

    [Command]
    private void ImportaSquadra()
    {
        if (NomeFileSquadraDBE == null) return;
        //TODO ImportaSquadra
    }


    [Command]
    private void EsportaSquadraCSV()
    {
        if (SquadraSelezionata == null) return;
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();
        SquadraSelezionata = d.ComponiInformazioniCompleteSquadra(SquadraSelezionata);
        var p = AppSvc.Services.GetRequiredService<EditorSvc>().ProgettoAttivoEditor;
        var sq = new List<Squadra>() { SquadraSelezionata };
        var al = new List<Allenatore>() { SquadraSelezionata.Allenatori.Last() };
        var st = new List<Stadio>() { SquadraSelezionata.Stadio };
        d.EsportaDBEditorSuFileCSV(p, true, sq, SquadraSelezionata.Giocatori, al, st);
    }

    [Command]
    private void ImportaSquadraCSV()
    {
        if (SquadraSelezionata == null) return;

        //TODO Comando Importa squadra da CSV, dati i file scelti


    }

    public void OnEvent(ChiusuraDialogDettagliGiocatoreSelezionato eventData)
    {
        var a = AppSvc.Services.GetRequiredService<ArchivioSvc>().DatiProgettoAttivo.Giocatori;

        int m = (int)Math.Truncate((decimal)
                 (eventData.Giocatore.Punteggi[1].Punteggio +
                  eventData.Giocatore.Punteggi[2].Punteggio +
                  eventData.Giocatore.Punteggi[3].Punteggio +
                  eventData.Giocatore.Punteggi[4].Punteggio) / 4);

        eventData.Giocatore.Punteggi[0].Punteggio = m;


        if (a.Any())
        {
            a[a.IndexOf(a.Find(g => g.Id == eventData.Giocatore.Id))] = eventData.Giocatore;
            var d = AppSvc.Services.GetRequiredService<IDatSvc>();
            SquadraSelezionata = d.ComponiInformazioniCompleteSquadra(SquadraSelezionata);
        }
        SchedaGiocatore = false;
        SchedaVisualizzata = false;
        GiocatoriSquadraSelezionata = new(SquadraSelezionata.Giocatori.OrderBy(g => g.Slot));
    }

    public void OnEvent(ChiusuraDialogDettagliAllenatoriSquadraSelezionata eventData)
    {
        SquadraSelezionata.Allenatori = eventData.Allenatori;
        var d = AppSvc.Services.GetRequiredService<IDatSvc>();
        SquadraSelezionata = d.ComponiInformazioniCompleteSquadra(SquadraSelezionata);
        SchedaAllenatore = false;
        SchedaVisualizzata = false;
    }

}
