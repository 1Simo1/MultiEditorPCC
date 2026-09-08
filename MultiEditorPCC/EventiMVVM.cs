namespace MultiEditorPCC;

public class EventiMVVM
{
    public record AppMsgEvent(AppMsg Msg);

    public record RichiestaRosaSquadra(int IdSquadra);
    public record RosaSquadraSelezionata(System.Collections.Generic.List<Shared.DTO.Giocatore> Giocatori);
}
