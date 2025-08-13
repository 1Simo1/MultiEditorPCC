using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class MainViewModel : IEventSubscriber<ConfermatoNuovoProgettoAttivo>
{
    public String nomePagina { get; set; }

    [Property] private ViewModelBase _pag;

    [Property] private String _nome;

    [Property] private String _cartella;

    [Property] private String _versionePCC;

    [Property] private String _testoCP;

    [Property] private bool _scriviTatticaCompletaSquadraGiocatoreCSV;


    [Property] private bool _cSVop;


    [Property] private ObservableCollection<String> _elencoFileCSVSquadreValidi;
    [Property] private ObservableCollection<String> _elencoFileCSVGiocatoriValidi;
    [Property] private ObservableCollection<String> _elencoFileCSVAllenatoriValidi;
    [Property] private ObservableCollection<String> _elencoFileCSVStadiValidi;

    [Property] private String? _nomeFileSquadreCSV;
    [Property] private String? _nomeFileGiocatoriCSV;
    [Property] private String? _nomeFileAllenatoriCSV;
    [Property] private String? _nomeFileStadiCSV;


    private readonly String a = $@"MjAyNSBTaW1vbmUgcGV
                                   yIFBDQ2FsY2lvNEV2ZXIgaHR0
                                   cHM6Ly9wY2NhbGNpbzRldmVyLmZ
                                   vcnVtY29tbXVuaXR5Lm5ldC8gUHJvZ2V0dG8g
                                   T3BlbiBTb3VyY2UgOiBodHRwczovL2dpdGh1Yi5
                                   jb20vMVNpbW8xL011bHRpRWRpdG9yUENDVmVyc2lvbm
                                   UgMC4xIGFscGhhIChpbiBjb3N0cnV6aW9uZSk =";

    partial void OnInitialize()
    {
        TestoCP = Encoding.ASCII.GetString(Convert.FromBase64String(a)).Replace("PCCV", "PCC V");
        ScriviTatticaCompletaSquadraGiocatoreCSV = false;
        CSVop = false;

        if (Pag == null)
        {
            nomePagina = "Intro";
            Pag = App.Services.GetRequiredService<NavSvc>().Nav(nomePagina);
        }
    }


    [Command]
    private void Nav(object Nome)
    {
        if (Nome.ToString() == nomePagina) return;

        Pag = App.Services.GetRequiredService<NavSvc>().Nav(Nome.ToString());

        if (Pag == null) return; //nomePagina = String.Empty;

        nomePagina = Nome.ToString();

    }

    public void OnEvent(ConfermatoNuovoProgettoAttivo e)
    {

        if (e.Progetto == null) return; //In caso di tentativo di caricamento di progetto non valido

        Nome = e.Progetto.Nome;
        Cartella = e.Progetto.Cartella;
        VersionePCC = e.Progetto.VersionePCC;
        CSVop = false;
    }



    [Command]
    private async void EsportaDBSuCSV()
    {
        CSVop = true;

        var p = App.Services.GetRequiredService<EditorSvc>().ProgettoAttivoEditor;

        if (p == null) return;

        var x = await Task.Run(() => App.Services.GetRequiredService<IDatSvc>().EsportaDBEditorSuFileCSV(p, ScriviTatticaCompletaSquadraGiocatoreCSV));

        CSVop = false;

        //EventAggregator.Publish<RichiestaCSV>(new(true, true));
    }

    [Command]
    private void ImportaSquadreCSV()
    {


        //TODO Comando Importa squadre da CSV, dati i file scelti


    }

    [Command]

    private void AggiornaElenchiCSV()
    {
        var e = App.Services.GetRequiredService<EditorSvc>();

        ElencoFileCSVSquadreValidi = new(e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.SQUADRA, 0));

        if (!ElencoFileCSVSquadreValidi.Any()) NomeFileSquadreCSV = null;
        if (ElencoFileCSVSquadreValidi.Count == 1) NomeFileSquadreCSV = ElencoFileCSVSquadreValidi[0];

        ElencoFileCSVGiocatoriValidi = new(e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.GIOCATORE, 0));

        if (!ElencoFileCSVGiocatoriValidi.Any()) NomeFileGiocatoriCSV = null;
        if (ElencoFileCSVGiocatoriValidi.Count == 1) NomeFileGiocatoriCSV = ElencoFileCSVGiocatoriValidi[0];

        ElencoFileCSVAllenatoriValidi = new(e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.ALLENATORE, 0));

        if (!ElencoFileCSVAllenatoriValidi.Any()) NomeFileAllenatoriCSV = null;
        if (ElencoFileCSVAllenatoriValidi.Count == 1) NomeFileAllenatoriCSV = ElencoFileCSVAllenatoriValidi[0];

        ElencoFileCSVStadiValidi = new(e.CercaFileCSVDatiValidi(ArchivioSvc.TipoDatoDB.STADIO, 0));


        if (!ElencoFileCSVStadiValidi.Any()) NomeFileStadiCSV = null;
        if (ElencoFileCSVStadiValidi.Count == 1) NomeFileStadiCSV = ElencoFileCSVStadiValidi[0];
    }



}
