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
