using MultiEditorPCC.API.Handlers;

namespace MultiEditorPCC.API;

public static class EndpointExtensions
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder e)
    {
        RouteGroupBuilder api = e.MapGroup("/api");
        api.MapGet("", Init.GetInit);
        api.MapPost("{Msg?}", Init.PostInit);

        var progetti = api.MapGroup("/progetti");

        progetti.MapGet("", Progetti.GetElenco).WithDescription("Elenco progetti Editor");
        progetti.MapGet("{Nome}", Progetti.InfoProgetto).WithDescription("Dettagli progetto");
        progetti.MapPost("nuovo", Progetti.NuovoProgetto).WithDescription("Nuovo progetto");
        progetti.MapPost("carica/{Nome}", Progetti.CaricaProgetto).WithDescription("Carica progetto");
        progetti.MapPost("caricaPercorsiProgettoAttivo", Progetti.CaricaDatiProgetto).WithDescription("Carica i percorsi da esaminare per il progetto attivo");
        progetti.MapPost("db/carica/info", Progetti.CaricaFileDatabaseInfo).WithDescription("Carica i dati dei file di database di gioco per il progetto attivo nell'editor");
        progetti.MapPost("db/carica/editor", Progetti.CaricaFileDatabaseEditor).WithDescription("Carica i dati di squadre e giocatori nel database dell'editor");


        var squadre = api.MapGroup("/squadre");

        squadre.MapGet("", Squadre.GetElenco).WithDescription("Elenco squadre caricate nel progetto Editor");
        squadre.MapGet("{id:int}", Squadre.GetGiocatoriSquadra).WithDescription("Giocatori della squadra selezionata");
        squadre.MapPost("{id:int}/sostituisciCon/{nuovoId:int}", Squadre.SostituisciSquadra).WithDescription("Sostiutisce ed aggiorna una squadra con un'altra in elenco");


        var giocatori = api.MapGroup("/giocatori");
        giocatori.MapGet("", Giocatori.GetElenco).WithDescription("Elenco giocatori caricati nel progetto Editor");
        giocatori.MapGet("svincolati", Squadre.GiocatoriSenzaSquadra).WithDescription("Elenco giocatori senza squadra");

    }
}