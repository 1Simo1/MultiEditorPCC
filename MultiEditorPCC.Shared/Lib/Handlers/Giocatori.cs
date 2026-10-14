using MultiEditorPCC.Shared.DTO;

namespace MultiEditorPCC.Shared.Lib.Handlers;

public static class Giocatori
{
    public static List<Giocatore> GetElenco(Editor db) => db.Giocatori;

}

