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
        var dataContext = AppSvc.Services.GetService<DettagliAllenatoreViewModel>();
        this.DataContext = dataContext;

        if (Design.IsDesignMode)
        {
            dataContext.SchedaAllenatore = true;
            Design.SetDataContext(this, dataContext);
        }
    }




}