namespace MultiEditorPCC.Lib;

public class VersionePCCSupportataEditor
{
    public String Id { get; set; }
    public String NomeGioco { get; set; }
    public List<String> CercaFilePattern { get; set; }
    public List<String> CartelleSuperiori { get; set; }

    public int VersioneArchiviDB { get; set; } = -1;
}
