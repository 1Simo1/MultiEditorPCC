namespace MultiEditorPCC.Dat.DbSet;

public class Allenatore
{
    public uint Id { get; set; } = 0;
    public bool Giocabile { get; set; }
    public String Nome { get; set; } = String.Empty;
    public String NomeCompleto { get; set; } = String.Empty;
    public List<String> Testi { get; set; } = new();
    public bool exGiocatore { get; set; } = false;
}
