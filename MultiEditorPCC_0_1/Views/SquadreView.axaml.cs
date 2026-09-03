using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MultiEditorPCC.ViewModels;

namespace MultiEditorPCC.Views;

public partial class SquadreView : UserControl
{
    public SquadreView()
    {
        InitializeComponent();
        var dataContext = AppSvc.Services.GetService<SquadreViewModel>();
        this.DataContext = dataContext;
        if (Design.IsDesignMode)
        {
            dataContext.SchedaVisualizzata = false;
            Design.SetDataContext(this, dataContext);
        }
    }
}