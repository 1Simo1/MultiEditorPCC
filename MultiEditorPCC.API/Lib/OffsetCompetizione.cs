using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.Lib;

public class OffsetCompetizione
{
    public VersionePCC Id { get; set; } = VersionePCC.NESSUNA;
    public int Dimensione { get; set; } = 0;
    public Paese Paese { get; set; } = (Paese)0;
    public String Competizione { get; set; } = String.Empty;

    public bool Coppa { get; set; } = false;

    public int Offset { get; set; } = 0;
    public int B { get; set; } = 0;

    public int NumeroSquadre { get; set; } = 0;

    public String Note { get; set; } = String.Empty;

}