namespace MultiEditorPCC.Shared.DTO;

public class Progetto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required String Nome { get; set; } = String.Empty;

    public required String Cartella { get; set; } = String.Empty;

    public VersionePCC VersionePCC { get; set; } = VersionePCC.NESSUNA;

    public DateTime DataRegistrazione { get; set; } = DateTime.Now;

    public DateTime Modifica { get; set; } = DateTime.Now;

    public String VersioneProgetto { get; set; } = "1";

    public List<String> DatabaseFiles { get; set; } = new();

    public List<String> Archivi { get; set; } = new();

    public List<SlotCompetizioneSquadra> SlotCompetizioniSquadre { get; set; } = new();

    public String Note { get; set; } = String.Empty;

}

public enum VersionePCC
{
    NESSUNA = 0,
    PCC2001,
    PCF2001,
    PCC7P,
    PCF7P,
    PCC6,
    PCF6_ORO,
    PCC5,
    PCF5_ORO,
    PCC4,
    PCC3
}

public class SlotCompetizioneSquadra
{
    public VersionePCC VersionePCC { get; set; } = VersionePCC.NESSUNA;

    public String Competizione { get; set; } = String.Empty;

    public Paese Paese { get; set; } = Paese.ITALIA;

    public int Slot { get; set; } = -1;

    public uint CodiceSquadra { get; set; } = 0;

    public int TotaleSquadre { get; set; } = -1;

    public uint CodiceSquadraAssegnata { get; set; } = 0;

    public bool Campionato { get; set; } = true;

}
