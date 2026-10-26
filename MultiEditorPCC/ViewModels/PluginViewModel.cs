using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Shared.Plugins;
using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class PluginViewModel : ViewModelBase
{

    [Property] private ObservableCollection<EditorPlugin> _elencoPlugins;

    partial void OnInitialize()
    {
        ElencoPlugins = new(App.Services.GetRequiredService<PluginSvc>().InfoPlugins());
    }

    [Command]
    private void CaricaPlugin(object Plugin)
    {
        if (Plugin == null) return;
        try
        {
            Shared.App.Services.GetRequiredService<PluginSvc>().Run(Plugin.ToString());

        }
        catch (Exception ecx)
        {
            return;
        }

    }


}
