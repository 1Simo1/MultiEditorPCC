using MvvmGen;
using MvvmGen.Events;
using MvvmGen.ViewModels;

namespace MultiEditorPCC.ViewModels;

[ViewModel]
[Inject(typeof(IEventAggregator))]
public partial class VM : ViewModelBase
{
}
