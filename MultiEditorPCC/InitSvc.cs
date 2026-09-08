using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Shared.DTO;
using MultiEditorPCC.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiEditorPCC;

public class InitSvc
{
    public async Task Load()
    {
        var m = App.Services.GetRequiredService<MainViewModel>();
        //TODO Aggiungere gradualmente i vari viewmodel per le singole pagine
        //m.TestViewModel = App.Services.GetRequiredService<TestViewModel>();
        m.ProgettiViewModel = App.Services.GetRequiredService<ProgettiViewModel>();
        m.SquadreViewModel = App.Services.GetRequiredService<SquadreViewModel>();
        m.GiocatoriViewModel = App.Services.GetRequiredService<GiocatoriViewModel>();

        /* TODO Init varie operazioni, una volta definito il collegamento con API, 
         * anche i casi di modalità Editor senza API (ClientMod.Editor) 
         */

        if (App.Client.Mod == ClientMod.Editor)
        {

        }

        if (App.Client.Mod == ClientMod.API)
        {
            m.ProgettiViewModel.ElencoProgetti = await App.Client.Risposta<ObservableCollection<Progetto>>(HttpMethod.Get, "progetti", "");
            if (m.ProgettiViewModel.ElencoProgetti.Any())
            {
                m.ProgettiViewModel.Progetto = m.ProgettiViewModel.ElencoProgetti.OrderByDescending(p => p.Modifica.Ticks).First();
                m.ProgettiViewModel.Progetto = await App.Client.Risposta<Progetto>(HttpMethod.Post, $"progetti/carica/{m.ProgettiViewModel.Progetto.Nome}", "");
            }


            m.SquadreViewModel.ElencoSquadre = await App.Client.Risposta<ObservableCollection<Squadra>>(HttpMethod.Get, "squadre", "");

            m.GiocatoriViewModel.ElencoGiocatori = await App.Client.Risposta<ObservableCollection<Giocatore>>(HttpMethod.Get, "giocatori", "");

        }
    }


}
