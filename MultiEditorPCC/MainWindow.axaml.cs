using SukiUI;
using SukiUI.Controls;
using SukiUI.Enums;

namespace MultiEditorPCC
{
    public partial class MainWindow : SukiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void LightDark(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SukiTheme.GetInstance().SwitchBaseTheme();
        }

        private void OrangeTheme(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SukiTheme.GetInstance().ChangeColorTheme(SukiColor.Orange);
        }
        private void RedTheme(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SukiTheme.GetInstance().ChangeColorTheme(SukiColor.Red);
        }

        private void GreenTheme(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SukiTheme.GetInstance().ChangeColorTheme(SukiColor.Green);
        }

        private void BlueTheme(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SukiTheme.GetInstance().ChangeColorTheme(SukiColor.Blue);
        }


    }
}