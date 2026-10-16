using MultiEditorPCC.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace MultiEditorPCC;

public class PluginSvc : AssemblyLoadContext
{

    private readonly AssemblyDependencyResolver _resolver;

    private IEnumerable<IEditorPlugin> Plugins { get; set; }

    public PluginSvc() : base(isCollectible: true)
    {

    }

    public IEnumerable<IEditorPlugin> LoadPlugins()
    {
        if (!Directory.Exists("Plugins"))
        {
            Directory.CreateDirectory("Plugins");
            return Enumerable.Empty<IEditorPlugin>();
        }

        List<IEditorPlugin> plugins = new List<IEditorPlugin>();

        foreach (var path in Directory.GetFiles($"{AppContext.BaseDirectory}/Plugins", "*.dll"))
        {
            var assembly = this.LoadFromAssemblyPath(path);

            plugins.AddRange(assembly.GetTypes()
                .Where(t => typeof(IEditorPlugin).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => (IEditorPlugin)Activator.CreateInstance(t)!));
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
        var plugin = Plugins.FirstOrDefault(p => p.Id == pluginId)
            ?? throw new InvalidOperationException($"Plugin {pluginId} non trovato.");

        return plugin.Load(pluginCommand, Param);
    }

}
