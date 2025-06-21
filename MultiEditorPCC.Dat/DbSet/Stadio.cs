namespace MultiEditorPCC.Dat.DbSet;
public class Stadio
{
    public uint Id { get; set; } = 0;
    public bool Giocabile { get; set; }
    public String Nome { get; set; } = String.Empty;
    public int Capienza { get; set; } = -1;
    public int PostiInPiedi { get; set; } = -1;
    public Int16 Larghezza { get; set; } = -1;
    public Int16 Lunghezza { get; set; } = -1;
    public Paese Nazione { get; set; } = Paese.ITALIA;
    public int NumeroBoh { get; set; } = 0;
    public int AnnoCostruzione { get; set; } = 2021;


}
