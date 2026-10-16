namespace MultiEditorPCC.Plugin;

public interface IEditorPlugin
{
    public String Id { get; set; }
    public String Load(String pluginCommand = "", Dictionary<String, String>? Param = null);

}
