using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using MultiEditorPCC.ViewModels;
using System;


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


        //TEST OK
        //for (int i = 0; i < 1000; i++)
        //{
        //    await Task.Delay(1);
        //}

        s.Close();
        s.Dispose();

    }
}