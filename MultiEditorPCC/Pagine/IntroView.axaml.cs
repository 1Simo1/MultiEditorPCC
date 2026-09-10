using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using MultiEditorPCC.Shared.DTO;
using MultiEditorPCC.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;


namespace MultiEditorPCC.Pagine;

public partial class IntroView : UserControl
{
    public IntroView()
    {
        InitializeComponent();
    }

    private async void Folder_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var storageProvider = topLevel.StorageProvider;
        var startFolder = await storageProvider.TryGetFolderFromPathAsync(AppDomain.CurrentDomain.BaseDirectory);

        var chosenDir = await storageProvider.OpenFolderPickerAsync(new());

        if (chosenDir != null && chosenDir.Count != 0)
        {
            var dir = chosenDir[0].TryGetLocalPath();

            ((MainViewModel)this.DataContext!).ProgettiViewModel.Cartella = dir;
        }

    }

    private async void Nuovo_Progetto(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {


        var s = new SchermataCaricamento();
        s.ShowDialog<bool?>((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);

        //TODO Caricamento Nuovo progetto

        var Nome = ((MainViewModel)this.DataContext!).ProgettiViewModel.NuovoProgetto;
        var Cartella = ((MainViewModel)this.DataContext!).ProgettiViewModel.Cartella;
        Progetto? NuovoProgetto = null;
        try
        {
            NuovoProgetto = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/nuovo?Nome={Nome}&Cartella={Cartella}", "");
            //NuovoProgetto = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/carica/{NuovoProgetto.Nome}", "");
            NuovoProgetto = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/caricaPercorsiProgettoAttivo", "");
            await App.Client.Risposta<String>(HttpMethod.Post, $"progetti/db/carica/info", "");
            await App.Client.Risposta<String>(HttpMethod.Post, $"progetti/db/carica/editor", "");
        }
        catch (Exception ex)
        {



        }

        if (NuovoProgetto != null)
        {
            ((MainViewModel)this.DataContext!).ProgettiViewModel.Progetto =
            await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/carica/{NuovoProgetto.Nome}", "");

            ((MainViewModel)this.DataContext!).SquadreViewModel.ElencoSquadre =
                await App.Client.Risposta<ObservableCollection<Squadra>>(HttpMethod.Get, "squadre", "");

            ((MainViewModel)this.DataContext!).GiocatoriViewModel.ElencoGiocatori =
                await App.Client.Risposta<ObservableCollection<Giocatore>>(HttpMethod.Get, "giocatori", "");
        }
        s.Close();
        s.Dispose();

    }
}