using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib;
using MultiEditorPCC.Views;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
public partial class IntroViewModel : IEventSubscriber<ConfermatoNuovoProgettoAttivo>
{
    [Property] private bool _nuovoProgetto;

    [Property]
    private String _nome;


    [Property]
    private String _cartella;

    [Property] private String? _versionePCC;

    [Property] private ObservableCollection<ProgettoEditorPCC> _progettiEditor;

    public EditorSvc EditorSvc { get; set; }
    public ArchivioSvc ArchivioSvc { get; set; }

    [Property] private bool _estraiFileNuovoProgetto;
    [Property] private bool _scriviDBNuovoProgetto;

    partial void OnInitialize()
    {

        EditorSvc = App.Services.GetRequiredService<EditorSvc>();
        NuovoProgetto = EditorSvc.ProgettoAttivoEditor == null;
        ProgettiEditor = EditorSvc.ProgettiEditor == null ? new() : new(EditorSvc.ProgettiEditor);
        ArchivioSvc = App.Services.GetRequiredService<ArchivioSvc>();
        if (EditorSvc.ProgettoAttivoEditor != null)
        {
            ArchivioSvc.CaricaDatiDBGioco(EditorSvc.ProgettoAttivoEditor);
            App.Services.GetRequiredService<IEventAggregator>()
               .Publish(new ConfermatoNuovoProgettoAttivo(EditorSvc.ProgettoAttivoEditor));
            return;
        }
        Cartella = String.Empty;
        VersionePCC = String.Empty;

        EstraiFileNuovoProgetto = false;
        ScriviDBNuovoProgetto = false;
    }



    [Command]
    private void ConfiguraNuovoProgetto()
    {
        //NuovoProgetto = true;
    }




    [Command(CanExecuteMethod = nameof(CanExecuteConfermaNuovoProgetto))]
    private void ConfermaNuovoProgetto()
    {

        ProgettoEditorPCC p = new()
        {
            id = new(),
            Cartella = Cartella,
            Nome = Nome,
            VersionePCC = VersionePCC,
            DataRegistrazione = DateTime.Now,
            Modifica = DateTime.Now,
            VersioneProgetto = "1"
        };

        var ok = EditorSvc.NuovoProgetto(p);


        if (ok)
        {
            var schermataCaricamento = new SchermataCaricamento();
            schermataCaricamento.Show();
            ArchivioSvc.CaricaDatiDBGioco(EditorSvc.ProgettoAttivoEditor, true);
            schermataCaricamento.Hide();
            ProgettiEditor.Add(p);

            App.Services.GetRequiredService<IEventAggregator>()
                .Publish(new ConfermatoNuovoProgettoAttivo(EditorSvc.ProgettoAttivoEditor));


            if (EstraiFileNuovoProgetto) ArchivioSvc.SetupFileEditor(EditorSvc.ProgettoAttivoEditor);

            if (ScriviDBNuovoProgetto) ArchivioSvc.SetupDatabase(EditorSvc.ProgettoAttivoEditor);

        }

    }

    [CommandInvalidate(nameof(Nome))]
    private bool CanExecuteConfermaNuovoProgetto()
    {

        return !String.IsNullOrEmpty(Cartella) &&
               !String.IsNullOrEmpty(VersionePCC) &&
               !String.IsNullOrEmpty(Nome?.Trim());
    }

    [Command]

    private void ApriProgetto(object id)
    {
        EditorSvc.ProgettoAttivoEditor = ProgettiEditor.Where(p => p.id == (Guid)id).First();

        var p = EditorSvc.db.Progetti.Find(EditorSvc.ProgettoAttivoEditor.id);
        p.Modifica = DateTime.Now;

        EditorSvc.db.Update(p);

        EditorSvc.db.SaveChanges();

            var schermataCaricamento = new SchermataCaricamento();
            schermataCaricamento.Show();
        ArchivioSvc.CaricaDatiDBGioco(EditorSvc.ProgettoAttivoEditor);
        schermataCaricamento.Hide();
        
        App.Services.GetRequiredService<IEventAggregator>()
              .Publish(new ConfermatoNuovoProgettoAttivo(EditorSvc.ProgettoAttivoEditor));


        if (EstraiFileNuovoProgetto) ArchivioSvc.SetupFileEditor(EditorSvc.ProgettoAttivoEditor);

        if (ScriviDBNuovoProgetto) ArchivioSvc.SetupDatabase(EditorSvc.ProgettoAttivoEditor);

    }

    [Command]
    private void NuovoProgettoModAvanzata()
    {
        VersionePCC = "*";
        ConfermaNuovoProgetto();
    }


    public void OnEvent(ConfermatoNuovoProgettoAttivo e)
    {
        if (ArchivioSvc.DatiProgettoAttivo == null) return;
    }
}
