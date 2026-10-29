namespace MultiEditorPCC.Shared.Plugins;

public interface IEditorPlugin
{
    public String Id { get; set; }

    public String Autore { get; set; }

    public String Descrizione { get; set; }

    public bool IsDialog { get; set; }
    public String Load(String pluginCommand = "", Dictionary<String, String>? Param = null);

    public object LoadGUI();

    public void Show();

}