using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Lib;
using MultiEditorPCC.ViewModels;
using System;

namespace MultiEditorPCC.Views;

public partial class DettagliGiocatore : UserControl
{
    public DettagliGiocatore()
    {

        InitializeComponent();
        var dataContext = AppSvc.Services.GetService<DettagliGiocatoreViewModel>();
        this.DataContext = dataContext;

        if (Design.IsDesignMode)
        {
            dataContext.SchedaGiocatore = true;
            Design.SetDataContext(this, dataContext);
            //this.Height = 3021;
            SchedaGiocatoreScrollViewer.Height = 3621;
            InfoDettagliGiocatore.MinHeight = SchedaGiocatoreScrollViewer.Height;
        }
    }

    private void NumberBox_ValueChanged(FluentAvalonia.UI.Controls.NumberBox sender, FluentAvalonia.UI.Controls.NumberBoxValueChangedEventArgs args)
    {
        var a = v1.Value.Equals("NaN") ? 0 : v1.Value;
        var b = v2.Value.Equals("NaN") ? 0 : v2.Value;
        var c = v3.Value.Equals("NaN") ? 0 : v3.Value;
        var d = v4.Value.Equals("NaN") ? 0 : v4.Value;

        int m = (int)Math.Truncate(a + b + c + d) / 4;
        v0.Text = $"{m}";
    }

    private void Grid_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SchedaGiocatoreScrollViewer.MaxHeight = 621;
        SchedaGiocatoreScrollViewer.Height = this.Bounds.Height - 21;
        ((Control)sender).Height = this.Bounds.Height + 21;
    }

    private void BirthDate_ValueChanged(FluentAvalonia.UI.Controls.NumberBox sender, FluentAvalonia.UI.Controls.NumberBoxValueChangedEventArgs args)
    {
        var anno = aaaa.Value.Equals("NaN") ? 2021 : (int)aaaa.Value;
        var mese = mm.Value.Equals("NaN") ? 1 : (int)mm.Value;
        var giorno = gg.Value.Equals("NaN") ? 1 : (int)gg.Value;

        try
        {
            var d = new DateOnly(anno, mese, giorno);
        }
        catch (Exception ex)
        {
            var g = DateTime.DaysInMonth(anno, mese);
            gg.Value = g;
        }


    }
}