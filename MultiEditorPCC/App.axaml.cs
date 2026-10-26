using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiEditorPCC.Pagine;
using MultiEditorPCC.Shared;
using MultiEditorPCC.ViewModels;
using MvvmGen.Events;
using SukiUI.Dialogs;
using System.Linq;
using System.Reflection;

//using System.Linq;
//using System.Reflection;

namespace MultiEditorPCC;

public partial class App : Application
{

    public static ServiceProvider? Services { get; set; }

    public static Client Client { get; set; }

    //public static AppSettings Config { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }


    public async override void OnFrameworkInitializationCompleted()
    {
        var svc = new ServiceCollection();

        IConfigurationBuilder config = new ConfigurationBuilder();

        //svc.AddSingleton<AppSettings>();
        svc.AddSingleton<Client>();
        svc.AddSingleton<IEventAggregator, EventAggregator>();
        svc.AddSingleton<DialogService>();
        svc.AddTransient<ISukiDialogManager, SukiUI.Dialogs.SukiDialogManager>();

        svc.AddScoped<MainViewModel>();

        var ViewModels = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(t => t.Namespace != null &&
                                    t.Namespace.Equals("MultiEditorPCC.ViewModels"))
                        .ToList();

        foreach (var t in ViewModels) svc.TryAddScoped(t);


        svc.AddSingleton<Editor>();
        svc.AddSingleton<InitSvc>();

        svc.AddSingleton<PluginSvc>();

        //var pluginSvc = new PluginSvc();

        //foreach (var plugin in pluginSvc.LoadPlugins())
        //    svc.AddSingleton<IEditorPlugin>(plugin);


        svc.AddSingleton<MainWindow>();
        Shared.App.Services = svc.BuildServiceProvider();

        Services = Shared.App.Services;



        var listaPlugin = Services.GetRequiredService<PluginSvc>().LoadPlugins();



        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {

            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };

            //Services.GetRequiredService<DialogService>().MainWindow = desktop.MainWindow;

            desktop.MainWindow.Show();

            var sc = new SchermataCaricamento();
            Shared.App.Services.GetRequiredService<DialogService>().ApriDialog(sc);

            //Services.GetRequiredService<MainViewModel>().OnEvent(new RichiestaDialog(new SchermataCaricamento()));

            Client = App.Services.GetRequiredService<Client>();
            await Client.Init();

            await Services.GetRequiredService<InitSvc>().Load();

            Services.GetRequiredService<DialogService>().ChiudiDialog(sc);

        }

        base.OnFrameworkInitializationCompleted();
    }
}