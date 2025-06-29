namespace MultiEditorPCC.Dat.DbSet;

public class Giocatore
{
    public int Id { get; set; } = 0;
    public bool Giocabile { get; set; }

    public int Numero { get; set; } = -1;

    public String Nome { get; set; } = String.Empty;

    public String NomeCompleto { get; set; } = String.Empty;

    public int Slot { get; set; } = -1;

    public bool AltriDati { get; set; }

    public bool AttivoInRosa { get; set; } = true;
    public List<Ruolo> Ruoli { get; set; }

    public Paese Nazione { get; set; }

    public ColorePelle codColorePelle { get; set; } = ColorePelle.Nessuno;

    public ColoreCapelli codColoreCapelli { get; set; } = ColoreCapelli.Nessuno;

    public StileCapelli codStileCapelli { get; set; } = StileCapelli.Calvo;

    public StileBarba codStileBarba { get; set; } = StileBarba.No;

    public bool Nazionalizzato { get; set; } = false;

    public Reparto Reparto { get; set; }

    public int GiornoNascita { get; set; } = -1;

    public int MeseNascita { get; set; } = -1;

    public int AnnoNascita { get; set; } = 2021;

    public int Altezza { get; set; } = -1;

    public int Peso { get; set; } = -1;

    public Paese PaeseNascita { get; set; }

    public List<String> Testi { get; set; } = new();

    public List<PunteggioGiocatore> Punteggi { get; set; } = PunteggiDefault();

    private static List<PunteggioGiocatore> PunteggiDefault()
    {

        List<PunteggioGiocatore> p = new()
        {
            {new() { Tipo = TipoPunteggioGiocatore.Media, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Velocità, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Resistenza, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Aggressività, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Qualità, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Rifinitura, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Dribbling, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Passaggio, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Tiro, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.Entrate, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.GiocoMani, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.PiedePreferito, Punteggio = (int) PiedePreferito.Ambidestro} },
            {new() { Tipo = TipoPunteggioGiocatore.Rigori, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.CornerSX, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.CornerDX, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.FalloSX, Punteggio = -1} },
            {new() { Tipo = TipoPunteggioGiocatore.FalloDX, Punteggio = -1} },
        };

        return p;
    }

    public int CodiceSquadra { get; set; } = 0;
    public String Squadra { get; set; } = String.Empty;

}

public enum ColorePelle
{
    Bianco = 1,
    Nero,
    Mulatto,
    Altro, //Valore byte = 4, ma presente solo in PCC2001, forse PCC2000 e non so se anche 7+ => colore associato? 
    Nessuno = 0
}

public enum ColoreCapelli
{
    Biondo = 1,
    Nessuno,
    Bruno,
    Bianco,
    Fulvo,
    Castano
}

public enum StileCapelli
{
    Calvo = 1,
    Corto,
    Normale,
    Lungo,
    Medio,
    Codino,
    Riga
}

public enum StileBarba
{
    No = 1,
    Baffi,
    Pizzetto,
    Barba
}



public enum Reparto
{
    PORTIERE,
    DIFENSORE,
    CENTROCAMPISTA,
    ATTACCANTE
}

public enum Ruolo
{
    NESSUNO = 0,
    PORTIERE = 1,
    TERZINO_DX,
    TERZINO_SX,
    LIBERO,
    CENTRALE_SX,
    CENTRALE_DX,
    CENTROCAMPISTA_DX,
    INTERNO_DX,
    CENTRAVANTI,
    REGISTA,
    CENTROCAMPISTA_SX,
    ESTERNO_DX,
    MEZZAPUNTA_CENTRALE,
    ESTERNO_SX,
    CENTROCAMPISTA_DIFENSIVO,
    MEZZAPUNTA_DX,
    MEZZAPUNTA_SX,
    INTERNO_SX
}

public enum PiedePreferito
{
    Destro,
    Sinistro,
    Ambidestro
}

public enum TipoPunteggioGiocatore
{
    Media = -1,
    Velocità = 1,
    Resistenza,
    Aggressività,
    Qualità,
    Rifinitura,
    Dribbling,
    Passaggio,
    Tiro,
    Entrate,
    GiocoMani,
    PiedePreferito,
    Rigori,
    CornerSX,
    CornerDX,
    FalloSX,
    FalloDX
}

public class PunteggioGiocatore
{
    public TipoPunteggioGiocatore Tipo { get; set; }
    public int Punteggio { get; set; }
}