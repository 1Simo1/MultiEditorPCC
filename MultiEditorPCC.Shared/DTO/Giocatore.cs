namespace MultiEditorPCC.Shared.DTO;

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

    public Paese Nazione { get; set; }

    public ColorePelle CodColorePelle { get; set; } = ColorePelle.Nessuno;

    public ColoreCapelli CodColoreCapelli { get; set; } = ColoreCapelli.Nessuno;

    public StileCapelli CodStileCapelli { get; set; } = StileCapelli.Calvo;

    public StileBarba CodStileBarba { get; set; } = StileBarba.No;

    public bool Nazionalizzato { get; set; } = false;

    public Reparto Reparto { get; set; }

    public int GiornoNascita { get; set; } = -1;

    public int MeseNascita { get; set; } = -1;

    public int AnnoNascita { get; set; } = 2021;

    public int Altezza { get; set; } = -1;

    public int Peso { get; set; } = -1;

    public Paese PaeseNascita { get; set; } = Paese.ITALIA;

    public Ruolo Ruolo { get; set; }

    public List<Ruolo> AltriRuoli { get; set; } = new();

    public UInt16 VE { get; set; } = 0;
    public UInt16 RE { get; set; } = 0;
    public UInt16 AG { get; set; } = 0;
    public UInt16 QU { get; set; } = 0;

    public UInt16 RI { get; set; } = 0;
    public UInt16 DR { get; set; } = 0;
    public UInt16 PA { get; set; } = 0;
    public UInt16 TI { get; set; } = 0;
    public UInt16 EN { get; set; } = 0;
    public UInt16 GM { get; set; } = 0;

    public PiedePreferito PPR { get; set; } = PiedePreferito.Ambidestro;

    public UInt16 RIG { get; set; } = 0;
    public UInt16 CSX { get; set; } = 0;
    public UInt16 CDX { get; set; } = 0;
    public UInt16 FSX { get; set; } = 0;
    public UInt16 FDX { get; set; } = 0;

    public uint CodiceSquadra { get; set; } = 0;
    public String Squadra { get; set; } = String.Empty;

    public String Note { get; set; } = String.Empty;

    public int TipoInfo { get; set; } = 3;
    public int V { get; set; } = 3;

    public string DataString()
    {
        string header = string.Empty;
        string st = string.Empty;

        var fl = typeof(Giocatore).GetProperties();

        foreach (var f in fl)
        {
            header += $"{f.Name};";
            if (f.Name != "AltriRuoli") st += $"{f.GetValue(this)};";
            if (f.Name == "AltriRuoli")
            {
                foreach (var r in f.GetValue(this) as List<Ruolo>)
                {
                    st += $"{r}|";
                }

                st = st.TrimEnd('|') + ";";
            }
        }

        return $"{header.TrimEnd(';')}{Environment.NewLine}{st.TrimEnd(';')}";
    }

    public override string ToString() => Nome;

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
