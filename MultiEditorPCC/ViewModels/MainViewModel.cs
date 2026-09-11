using MultiEditorPCC.Pagine;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using SukiUI.Dialogs;
using static MultiEditorPCC.EventiMVVM;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
public partial class MainViewModel : ViewModelBase, IEventSubscriber<AppMsgEvent, RichiestaDialog, ChiudiDialog>
{

    [Property] private AppMsg? _msg;

    [Property] private string _testo;
    [Property] private string _footer;

    public ISukiDialogManager DialogManager { get; } = new SukiDialogManager();

    //TODO Aggiungere gradualmente i vari viewmodel per le singole pagine
    //[Property] private TestViewModel _testViewModel;
    [Property] private ProgettiViewModel _progettiViewModel;
    [Property] private SquadreViewModel _squadreViewModel;
    [Property] private GiocatoriViewModel _giocatoriViewModel;

    partial void OnInitialize()
    {
        Testo = string.Empty;

        Footer = ScriviFooter();
    }

    private string ScriviFooter()
    {
        return """
            Simone per PCCalcio4Ever https://pccalcio4ever.forumcommunity.net/ Progetto Open Source : https://github.com/1Simo1/MultiEditorPCC Versione 0.1 alpha (in costruzione)
            """;
    }


    public void OnEvent(AppMsgEvent eventData)
    {
        Msg = new(eventData.Msg.Status, eventData.Msg.Title, eventData.Msg.Content);
    }

    public void OnEvent(RichiestaDialog eventData)
    {
        var d = DialogManager.CreateDialog()
            .WithContent(new SchermataCaricamento())
            .Dialog;

        DialogManager.TryShowDialog(d);


    }

    public void OnEvent(ChiudiDialog eventData)
    {
        if (eventData.Dialog == null)
        {
            DialogManager.DismissDialog();
            return;
        }

        DialogManager.TryDismissDialog(eventData.Dialog);
    }
}
