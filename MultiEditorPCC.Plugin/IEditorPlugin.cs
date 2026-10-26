namespace MultiEditorPCC.Plugin;

public interface IEditorPlugin
{
    public String Id { get; set; }

    public String Autore { get; set; }

    public String Descrizione { get; set; }
    public String Load(String pluginCommand = "", Dictionary<String, String>? Param = null);

    public void Show();

}
