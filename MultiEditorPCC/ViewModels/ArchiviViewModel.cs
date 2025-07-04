using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class ArchiviViewModel : IEventSubscriber<ElaboraCaricamentoImmagine>
{

    [Property] private ObservableCollection<String> _elencoCartelleArchiviPrimoLivello;
    [Property] private String _cartellaArchivioPrimoLivello;
    [Property] private ObservableCollection<String> _elencoCartelleArchivi;
    [Property] private String _cartellaArchivio;

    [Property] private int _livello;

    [Property] private ObservableCollection<String> _elencoPalette;


    [PropertyPublishEvent(typeof(ElaboraCaricamentoImmagine))]
    [Property]
    private String? _palette;

    [Property] private ObservableCollection<String> _elencoFilesCartellaArchivio;

    [PropertyPublishEvent(typeof(ElaboraCaricamentoImmagine))]
    [Property]
    private String? _fileSelezionato;

    //Numero di elementi compresi nell'elemento dell'archivio
    [Property] private int _t;

    [Property] private int _n;

    [Property] private String? _indicatore;



    [Property] private int _numeroFileTrovati;

    [Property] private List<Byte> _img;

    //Fattore di ingrandimento immagine visualizzata
    //(in questa pagina imnosto un comando che mantiene le proporzioni)
    //(la parte che elabora le immagini, però, supporta sia il fattore
    //di ingrandimento in larghezza, sia quello in altezza)
    [Property] private int _f;

    private readonly ArchivioSvc a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

    partial void OnInitialize()
    {
        ElencoCartelleArchiviPrimoLivello = new(a.CartelleDatiArchivi());
        ElencoPalette = new(a.ElencoPaletteArchivio());
        Livello = 1;
        N = 1;
        F = 1;
    }

    [Command]
    private void ConfermaSceltaCartella(object objLivello)
    {
        if (objLivello == null || !int.TryParse(objLivello.ToString(), out _) || int.Parse(objLivello!.ToString()) < 1) return;

        Livello = int.Parse(objLivello!.ToString());

        if (Livello == 1)
        {
            ElencoCartelleArchivi = new(a.CartelleDatiArchivi(Livello, cartella: CartellaArchivioPrimoLivello));
            ElencoFilesCartellaArchivio = new(a.FileDatiArchivi(Livello, cartella: CartellaArchivioPrimoLivello));

        }
        else
        { //Scelta una sottocartella non di primo livello
            if (CartellaArchivio == null) return;
            var c = CartellaArchivio;
            ElencoCartelleArchivi = new(a.CartelleDatiArchivi(Livello, cartella: CartellaArchivio));

            if (!ElencoCartelleArchivi.Any()) ElencoCartelleArchivi = new() { CartellaArchivio };

            ElencoFilesCartellaArchivio = new(a.FileDatiArchivi(Livello, cartella: c));
        }


        if (ElencoCartelleArchivi.Any() && Livello > 1)
        {
            CartellaArchivio = ElencoCartelleArchivi.First();
        }


        NumeroFileTrovati = ElencoFilesCartellaArchivio.Count;
        Livello++;

    }

    public void OnEvent(ElaboraCaricamentoImmagine _)
    {

        if (Palette == null || FileSelezionato == null) return;

        var temp = Palette;

        T = a.NumeroElementi(FileSelezionato);

        N = Math.Max(N, 1);
        N = Math.Min(N, T);

        Indicatore = $"{N} / {T}";

        Img = a.ElaboraCaricamentoImmagine(FileSelezionato, Palette, N);

        ElencoPalette = new(a.ElencoPaletteArchivio());
        Palette = temp;
    }

    /// <summary>
    /// Ci sono elementi degli archivi che si ripetono con lo stesso nome
    /// </summary>
    /// <param name="d">Direzione: incremento o decremento di N</param>
    [Command]
    private void SfogliaFileElemento(object d)
    {
        if (d == null) return;

        if (d.ToString().Equals("+") || d.ToString().ToUpper().Equals("INC"))
        {
            N++;

        }
        else N--;

        EventAggregator.Publish<ElaboraCaricamentoImmagine>(new());



    }

    [Command]
    private void VisualizzaPalette()
    {
        if (Palette == null) return;
        Img = a.ElaboraCaricamentoImmagine(Palette, Palette, 1);
    }

    [Command]
    private void IngrandisciImmagine(object op)
    {
        if (Palette == null || FileSelezionato == null || op == null) return;

        int m = op.ToString().Equals("+") ? F + 1 : Math.Max(F - 1, 1);

        F = m;

        Img = a.IngrandisciImmagine(null, FileSelezionato, Palette, N, F);
    }

}
