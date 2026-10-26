using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Shared;

namespace MultiEditorPCC.Plugin.TestPlugin
{
    public partial class WindowTestPlugin : UserControl
    {
        public WindowTestPlugin()
        {
            this.DataContext = new PluginTestViewModel();
            InitializeComponent();
        }

        private void Test_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if ((this.DataContext as PluginTestViewModel).TestInput == 21)
                {
                    App.Services.GetRequiredService<DialogService>().ChiudiDialog(this);
                }
            }
            catch (Exception exc)
            {
                return;
            }

        }
    }
}