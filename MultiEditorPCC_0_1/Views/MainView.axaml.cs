using Avalonia.Controls;

namespace MultiEditorPCC.Views;

public partial class MainView : UserControl
{
    public MainView()
    {

        InitializeComponent();
    }

    private void Toggle_Theme(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        App.Current.RequestedThemeVariant = App.Current.ActualThemeVariant.Key.Equals("Light") ?
                                            Avalonia.Styling.ThemeVariant.Dark :
                                            Avalonia.Styling.ThemeVariant.Light;
    }

    private void Switch_Menu_Orientation(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (MenuOptions.Orientation == Avalonia.Layout.Orientation.Vertical)
        {
            SideMenu.SetValue(Grid.RowProperty, 1);
            SideMenu.SetValue(Grid.ColumnSpanProperty, 2);
            Contenuto.SetValue(Grid.ColumnProperty, 0);
            Contenuto.SetValue(Grid.ColumnSpanProperty, 2);
            MenuOptions.Orientation = Avalonia.Layout.Orientation.Horizontal;
            return;
        }

        SideMenu.SetValue(Grid.RowProperty, 2);
        SideMenu.SetValue(Grid.ColumnSpanProperty, 1);
        Contenuto.SetValue(Grid.ColumnProperty, 1);
        Contenuto.SetValue(Grid.ColumnSpanProperty, 1);
        MenuOptions.Orientation = Avalonia.Layout.Orientation.Vertical;

    }
}
