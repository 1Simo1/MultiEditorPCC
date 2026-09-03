using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MvvmGen;
using MvvmGen.Events;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class SalvaDatiViewModel
{

    public EditorSvc EditorSvc { get; set; }
    public ArchivioSvc ArchivioSvc { get; set; }


    partial void OnInitialize()
    {
        EditorSvc = AppSvc.Services.GetRequiredService<EditorSvc>();
        ArchivioSvc = AppSvc.Services.GetRequiredService<ArchivioSvc>();
        RicaricaDatabaseDaCSV(); //TODO Temp, rimuovere appena verificato il comando RicaricaDatabaseDaCSV()
    }

    [Command]
    public void RicaricaDatabaseDaCSV()
    {
        var progettoAttivo = EditorSvc.ProgettoAttivoEditor;
        if (progettoAttivo == null) return;
        ArchivioSvc.CalcolaTabellaDBFileCSV(EditorSvc.CercaFileCSVDatiValidi(richiediPercorsoCompleto: true));
        ArchivioSvc.ElaboraCSV(true); //TODO Verificare se far specificare true/false da GUI
    }
}
