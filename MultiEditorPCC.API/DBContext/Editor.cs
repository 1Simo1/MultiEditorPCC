using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.DBContext;

public class Editor()
{
    public List<Progetto> Progetti { get; set; } = new();

    public Progetto? ProgettoAttivo { get; set; }

    public List<Squadra> Squadre { get; set; } = new();

    public List<Giocatore> Giocatori { get; set; } = new();

    public List<FileDatabaseInfo> DatabaseFiles { get; set; } = new();

}

public class FileDatabaseInfo
{
    public int Codice { get; set; } = -1;
    public TipoFileDatabase TipoFileDatabase { get; set; } = TipoFileDatabase.NESSUNO;

    public TipoDatoDB TipoDatoDB { get; set; } = TipoDatoDB.NESSUNO;

    public VersionePCC VersionePCC { get; set; } = VersionePCC.NESSUNA;

    public int VersioneElemento { get; set; } = -1;

    public List<Byte> Dat { get; set; } = new();


}

public enum TipoFileDatabase
{
    NESSUNO,
    FDI,
    PKF,
    DBC
}

public enum TipoDatoDB
{
    NESSUNO = 0,
    SQUADRA = 1,
    GIOCATORE = 3,
    ALLENATORE,
    STADIO
}