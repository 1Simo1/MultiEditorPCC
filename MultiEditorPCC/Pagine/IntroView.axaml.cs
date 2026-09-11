using Avalonia.Controls;
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


}