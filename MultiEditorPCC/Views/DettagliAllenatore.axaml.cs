using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MultiEditorPCC.ViewModels;

namespace MultiEditorPCC.Views;

public partial class DettagliAllenatore : UserControl
{
    public DettagliAllenatore()
    {

        InitializeComponent();
        this.DataContext = AppSvc.Services.GetService<DettagliAllenatoreViewModel>();

    }




}