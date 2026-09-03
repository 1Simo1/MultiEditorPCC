using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Pagine;
using MultiEditorPCC.ViewModels;
using MvvmGen.Events;

//using System.Linq;
//using System.Reflection;

namespace MultiEditorPCC;

public partial class App : Application
{

    public static ServiceProvider? Services { get; set; }

    //public static Client Client { get; set; }

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
        //svc.AddSingleton<Client>();
        svc.AddSingleton<IEventAggregator, EventAggregator>();

        svc.AddScoped<MainViewModel>();

        //var ViewModels = Assembly.GetExecutingAssembly().GetTypes()
        //                .Where(t => t.Namespace != null &&
        //                            t.Namespace.Equals("MultiEditorPCC.ViewModels"))
        //                .ToList();

        //foreach (var t in ViewModels) svc.TryAddScoped(t);



        //svc.AddSingleton<InitSvc>();


        Services = svc.BuildServiceProvider();



        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var schermataCaricamento = new SchermataCaricamento();

            desktop.MainWindow = schermataCaricamento;
            schermataCaricamento.Show();

            //await Task.Delay(2100000);

            //Client = App.Services.GetRequiredService<Client>();
            //await Client.Init();

            //await Services.GetRequiredService<InitSvc>().Load();

            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };

            desktop.MainWindow.Show();



            schermataCaricamento.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
}