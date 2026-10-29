using MultiEditorPCC.Shared.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace MultiEditorPCC;

public class PluginSvc : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    private IEnumerable<object> Plugins { get; set; }



    public PluginSvc() : base(isCollectible: true)
    {

    }

    public IEnumerable<object> LoadPlugins()
    {
        if (!Directory.Exists("Plugins"))
        {
            Directory.CreateDirectory("Plugins");
            return Enumerable.Empty<IEditorPlugin>();
        }

        List<object> plugins = new List<object>();

        foreach (var path in Directory.GetFiles($"{AppContext.BaseDirectory}/Plugins", "*.dll", SearchOption.AllDirectories))
        {
            var assembly = this.LoadFromAssemblyPath(path);

            plugins.AddRange(assembly.GetTypes()
                .Where(t => t.BaseType != null && t.BaseType.FullName == typeof(EditorPlugin).FullName)
                .Select(t => Activator.CreateInstance(t)!));
        }

        this.Plugins = plugins;
        return plugins;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        String path = String.Empty;
        if (assemblyName.Name == "MultiEditorPCC.Plugin") return null;

        try
        {
            if (_resolver == null) return null;
            path = _resolver.ResolveAssemblyToPath(assemblyName);
        }
        catch (Exception ex)
        {

            return null;
        }


        if (path != null) return LoadFromAssemblyPath(path);
        return null;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        return base.LoadUnmanagedDll(unmanagedDllName);
    }

    public String Run(String pluginId, String pluginCommand = "", Dictionary<String, String>? Param = null)
    {
        foreach (var cercaPlugin in this.Plugins.ToList())
        {
            if (cercaPlugin.GetType().GetProperty("Id").GetValue(cercaPlugin) == pluginId)
            {
                MethodInfo? method = null;

                method = cercaPlugin.GetType().GetMethod("Show");

                bool IsDialog = (bool)cercaPlugin.GetType().GetProperty("IsDialog").GetValue(cercaPlugin);

                if (method != null && IsDialog) method.Invoke(cercaPlugin, null);

                if (pluginCommand == "" && IsDialog) return JsonSerializer.Serialize(new Output(true, null));

                if (pluginCommand == "" && !IsDialog)
                {
                    method = cercaPlugin.GetType().GetMethod("LoadGUI");

                    return JsonSerializer.Serialize(new Output(true, method == null ? null : method.Invoke(cercaPlugin, null)));
                }


                method = cercaPlugin.GetType().GetMethod(pluginCommand);
                if (method != null)
                {
                    object result = String.Empty;

                    try
                    {

                        result = method.Invoke(cercaPlugin, Param?.Values.ToArray());

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

            }

        }

        throw new InvalidOperationException($"Plugin {pluginId} non trovato.");
    }

    public List<EditorPlugin> InfoPlugins()
    {
        if (!Plugins.Any()) return new();

        List<EditorPlugin> info = new List<EditorPlugin>();

        foreach (var plugin in Plugins) { info.Add((EditorPlugin)plugin); }

        return info;
    }

}
