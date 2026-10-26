using System.Reflection;
using System.Text.Json;

namespace MultiEditorPCC.Plugin;

public class EditorPlugin : IEditorPlugin
{
    public virtual String Id { get; set; } = "EditorPlugin";

    public virtual String Autore { get; set; } = "";

    public virtual String Descrizione { get; set; } = "";

    public override String ToString() => Id;

    public String Load(String pluginCommand = "", Dictionary<string, string>? Param = null)
    {
        MethodInfo? method = this.GetType().GetMethod(pluginCommand);
        if (method == null)
        {
            method = this.GetType().GetMethod("Run");
            if (method == null) return JsonSerializer.Serialize((false, String.Empty));
        }

        object result = String.Empty;

        try
        {

            result = method.Invoke(this, Param?.Values.ToArray());

        }
        catch (Exception e)
        {

            return JsonSerializer.Serialize(new Output(false, String.Empty));
        }
        finally
        {

        }
        return JsonSerializer.Serialize(new Output(true, result));
    }

    public virtual void Show() { }
}

public record Output(bool OK, object Result);