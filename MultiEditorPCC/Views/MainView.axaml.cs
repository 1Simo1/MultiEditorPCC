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
            SideMenu.SetValue(DockPanel.DockProperty, Dock.Top);
            MenuOptions.Orientation = Avalonia.Layout.Orientation.Horizontal;
            return;
        }

        SideMenu.SetValue(DockPanel.DockProperty, Dock.Left);
        MenuOptions.Orientation = Avalonia.Layout.Orientation.Vertical;

    }
}
