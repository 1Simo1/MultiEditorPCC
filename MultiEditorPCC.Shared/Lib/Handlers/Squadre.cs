using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.Shared.Lib.Handlers;

public static class Squadre
{
    public static List<Squadra> GetElenco(Editor db) => db.Squadre;

    public static List<Giocatore> GetGiocatoriSquadra(Editor db, int id)
    {
        return db.Giocatori.Where(g => g.CodiceSquadra == id).ToList();
    }

    public static List<Giocatore> GiocatoriSenzaSquadra(Editor db)
    {
        return db.Giocatori.Where(g => g.CodiceSquadra == 0).ToList();
    }

    public static bool SostituisciSquadra(Editor db, int id, int nuovoId)
    {
        if (id == nuovoId) return true;

        try
        {
            Squadra? Precedente = db.Squadre.Where(sq => sq.Id == id).FirstOrDefault();
            Squadra? Nuova = db.Squadre.Where(sq => sq.Id == nuovoId).FirstOrDefault();

            if (Precedente == null || Nuova == null) return false;

            var idSquadra = Precedente.Id;

            foreach (Giocatore g in db.Giocatori.Where(g => g.CodiceSquadra == nuovoId)) g.CodiceSquadra = idSquadra;

            //Precedente = Nuova;
            //Precedente.Id = idSquadra;

            db.Squadre.Remove(Nuova);
        }
        catch (Exception ex)
        {

            return false;
        }
        Utils.ScriviCSV(db.ProgettoAttivo, db.Squadre, db.Giocatori, false, true);
        return true;

    }

    public static (List<Squadra>, List<SlotCompetizioneSquadra>) CalcolaSlot(Editor db, VersionePCC VersionePCC, Paese Paese, String Competizione)
    {
        List<Squadra> Squadre = db.Squadre;
        List<SlotCompetizioneSquadra> Slot = db.ProgettoAttivo.SlotCompetizioniSquadre;

        bool Coppa = !Slot.First().Campionato;

        if (Coppa)
        {
            //TODO
            return (Squadre, Slot);
        }

        if (Paese > Paese.NESSUNO)
        {
            Squadre = Squadre.Where(sq => sq.Nazione == Paese).ToList();
            Slot = Slot.Where(slot => slot.Paese == Paese &&
                                      slot.VersionePCC == VersionePCC &&
                                      slot.Campionato
            ).ToList();
        }

        foreach (var CodiceSquadra in Slot.Select(slot => slot.CodiceSquadraAssegnata))
        {
            Squadre.Remove(Squadre.Where(sq => sq.Id == CodiceSquadra).FirstOrDefault());
        }

        Slot = Slot.Where(slot => slot.Competizione == Competizione).ToList();

        return (Squadre, Slot);
    }
}
