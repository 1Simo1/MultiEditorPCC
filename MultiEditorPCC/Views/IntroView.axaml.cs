using System;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MultiEditorPCC.ViewModels;

namespace MultiEditorPCC.Views;

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

            ((IntroViewModel)this.DataContext!).Cartella = dir;

            ((IntroViewModel)this.DataContext!).VersionePCC =
                App.Services.GetRequiredService<EditorSvc>().CercaVersioneGioco(dir);
        }

    }





}