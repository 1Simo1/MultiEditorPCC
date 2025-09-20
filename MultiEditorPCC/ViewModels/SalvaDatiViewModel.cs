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

        RicaricaDatabaseDaCSV();
    }

    [Command]
    public void RicaricaDatabaseDaCSV()
    {
        var progettoAttivo = EditorSvc.ProgettoAttivoEditor;
        if (progettoAttivo == null) return;
        var test = EditorSvc.CercaFileCSVDatiValidi();
    }
}
