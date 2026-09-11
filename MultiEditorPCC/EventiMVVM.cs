using SukiUI.Dialogs;

namespace MultiEditorPCC;

public class EventiMVVM
{
    public record AppMsgEvent(AppMsg Msg);

    public record RichiestaDialog(ISukiDialog? Dialog = null);

    public record ChiudiDialog(ISukiDialog? Dialog = null);

    public record RichiestaRosaSquadra(int IdSquadra, bool ElencoGiocatori = false);
    public record RosaSquadraSelezionata(System.Collections.Generic.List<Shared.DTO.Giocatore> Giocatori);

    public record AperturaProgetto();
}
