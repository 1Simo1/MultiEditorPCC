using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MultiEditorPCC.ViewModels;
using MultiEditorPCC.Views;
using MvvmGen.Events;
using System.Threading.Tasks;

namespace MultiEditorPCC;

public partial class App : Application
{

    public static ServiceProvider Services { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        Locales.Resources.Culture = System.Globalization.CultureInfo.CurrentCulture;

        BindingPlugins.DataValidators.RemoveAt(0);

        var collection = new ServiceCollection();
        collection.AddCommonServices();

        Services = collection.BuildServiceProvider();

        AppSvc.Services = Services;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var schermataCaricamento = new SchermataCaricamento();

            desktop.MainWindow = schermataCaricamento;
            schermataCaricamento.Show();

            desktop.MainWindow = new MainWindow
            {
                DataContext = await Task.Run(() => Services.GetRequiredService<MainViewModel>())
            };

            desktop.MainWindow.Show();



            schermataCaricamento.Close();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
            base.OnFrameworkInitializationCompleted();
        }


    }
}

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IEventAggregator, EventAggregator>();
        collection.AddSingleton<ArchivioSvc>();
        collection.AddSingleton<EditorSvc>();
        collection.AddSingleton<IDatSvc, DatSvc>();
        collection.AddSingleton<NavSvc>();
        collection.AddTransient<SquadreViewModel>();
        collection.AddTransient<DettagliGiocatoreViewModel>();
        collection.AddTransient<DettagliAllenatoreViewModel>();
        collection.AddTransient<ArchiviViewModel>();
        collection.AddTransient<MainViewModel>();
    }
}