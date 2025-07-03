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


    [Property] private int _numeroFileTrovati;

    [Property] private List<Byte> _img;

    private readonly ArchivioSvc a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

    partial void OnInitialize()
    {
        ElencoCartelleArchiviPrimoLivello = new(a.CartelleDatiArchivi());
        ElencoPalette = new(a.ElencoPaletteArchivio());
        Livello = 1;


    }

    [Command]
    private void ConfermaSceltaCartella(object objLivello)
    {
        //if (FileSelezionato != null)
        //{
        //    var sk = SKImage.FromEncodedData(a.DatiProgettoAttivo.Archivi[FileSelezionato].First().Dat.ToArray());
        //    W = sk.Width;
        //    H = sk.Height;
        //    Img = new Avalonia.Media.Imaging.Bitmap(sk.Encode().AsStream());


        //}
        //

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
        //TODO
        Img = a.ElaboraCaricamentoImmagine(FileSelezionato, Palette);

    }
}
