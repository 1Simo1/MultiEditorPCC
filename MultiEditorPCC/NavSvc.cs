using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MultiEditorPCC.ViewModels;
using MvvmGen.Events;
using MvvmGen.ViewModels;
using System;
using System.Collections.Generic;

namespace MultiEditorPCC;

public class NavSvc
{
    private Dictionary<String, ViewModelBase?> Pagine { get; set; }

    public NavSvc()
    {
        Pagine = new();
    }

    public ViewModelBase? Nav(String Nome, bool ripetibile = false)
    {

        if (!ripetibile && Pagine.ContainsKey(Nome)) return Pagine[Nome];

        if (!Pagine.ContainsKey(Nome)) Pagine.Add(Nome, null);

        ViewModelBase v;

        switch (Nome)
        {
            case "Intro": v = new IntroViewModel(AppSvc.Services.GetRequiredService<IEventAggregator>()); break;
            case "Squadre": v = new SquadreViewModel(AppSvc.Services.GetRequiredService<IEventAggregator>()); break;
            case "Archivi": v = new ArchiviViewModel(AppSvc.Services.GetRequiredService<IEventAggregator>()); break;
            default: v = new IntroViewModel(AppSvc.Services.GetRequiredService<IEventAggregator>()); break;
        }

        Pagine[Nome] = v;
        return v;

    }


}
