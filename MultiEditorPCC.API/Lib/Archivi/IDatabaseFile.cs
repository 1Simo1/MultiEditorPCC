using MultiEditorPCC.API.DBContext;
using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.API.Lib.Archivi;

public interface IDatabaseFile
{
    public List<FileDatabaseInfo> ComponiElencoFileDatabase(Progetto progettoAttivo, String file);

    public List<Squadra> ComponiListaSquadreEditor(List<FileDatabaseInfo> DatabaseInfo);

    public List<Giocatore> ComponiListaGiocatoriEditor(List<FileDatabaseInfo> DatabaseInfo);

    // public List<Giocatore> AssegnaGiocatoriSquadre(List<Squadra> Squadre);
}
