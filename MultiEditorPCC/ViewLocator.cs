using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MvvmGen.ViewModels;
using System;

namespace MultiEditorPCC
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? viewModel)
        {
            if (viewModel is null) return null;

            var nomePagina = viewModel.GetType().FullName!.Replace("ViewModel", "View", StringComparison.InvariantCulture);

            var pag = Type.GetType(nomePagina);

            if (pag == null) return null;

            Control ctrl = (Control)Activator.CreateInstance(pag)!;
            ctrl.DataContext = viewModel;
            return ctrl;

        }

        public bool Match(object? data) => data is ViewModelBase;

    }
}
