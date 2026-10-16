namespace MultiEditorPCC.Plugin.TestPlugin;

public class PluginTest : EditorPlugin
{
    public override string Id { get; set; } = "TestPlugin";
    public int Test()
    {
        File.WriteAllText("Plugins/TestPlugin.txt", $"Questo file è stato scritto caricando il plugin : {Id}");
        return 21;
    }
}
