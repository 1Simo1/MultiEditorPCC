namespace MultiEditorPCC.Dat.DbSet;
public class ProgettoEditorPCC
{
    public Guid id { get; set; }

    public String Nome { get; set; }

    public String Cartella { get; set; }

    public String VersionePCC { get; set; }

    public DateTime DataRegistrazione { get; set; }

    public DateTime Modifica { get; set; }

    public String VersioneProgetto { get; set; } = String.Empty;
}
