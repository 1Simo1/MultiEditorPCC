using MultiEditorPCC.Dat.DbSet;
using System.Collections.Generic;

namespace MultiEditorPCC;

public class EventiMVVM
{
    public record ConfermatoNuovoProgettoAttivo(ProgettoEditorPCC? Progetto);
    public record SquadraSelezionataElenco(Squadra Squadra);

    public record VisualizzaDettagliGiocatoreSelezionato(Giocatore Giocatore);

    public record VisualizzaDettagliAllenatoriSquadraSelezionata(Squadra Squadra);

    public record ChiusuraDialogDettagliGiocatoreSelezionato(Giocatore Giocatore);
    public record ChiusuraDialogDettagliAllenatoriSquadraSelezionata(List<Allenatore> Allenatori);

    public record ElaboraCaricamentoImmagine();

    //public record RichiestaCSV(bool CSV_op_attiva, bool mod_exp);


}
