using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.Shared;

public class EventiMVVM
{
    public record AppMsgEvent(AppMsg Msg);

    public record RichiestaDialog(object? Dialog = null);

    public record ChiudiDialog(object? Dialog = null);

    public record RichiestaRosaSquadra(int IdSquadra, bool ElencoGiocatori = false);
    public record RosaSquadraSelezionata(System.Collections.Generic.List<Shared.DTO.Giocatore> Giocatori);

    public record AperturaProgetto();

    public record RichiestaPaesiGiocabili(VersionePCC VersionePCC = VersionePCC.NESSUNA);
    public record RispostaPaesiGiocabili(List<Paese> PaesiGiocabili);

    public record RichiestaCompetizioniPaese(VersionePCC VersionePCC, Paese Paese);
    public record RispostaCompetizioniPaese(List<String> Competizioni);
}
