using Microsoft.Extensions.DependencyInjection;
using MvvmGen.Events;
using static MultiEditorPCC.Shared.EventiMVVM;

namespace MultiEditorPCC.Shared;

public class DialogService
{
    public bool ApriDialog(object Dialog)
    {
        App.Services.GetRequiredService<IEventAggregator>()
            .Publish<RichiestaDialog>(new(Dialog));

        return true;
    }

    public bool ChiudiDialog(object? Dialog = null)
    {
        App.Services.GetRequiredService<IEventAggregator>()
            .Publish<ChiudiDialog>(new(Dialog));
        return true;
    }

}
