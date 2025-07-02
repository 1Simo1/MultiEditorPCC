using MultiEditorPCC.Dat.DbSet;


namespace MultiEditorPCC.Lib.Archivi;

public static partial class DBC
{
    public static int Versione { get; set; }

    public static List<Byte> dati { get; set; }

    public static String nomeSquadra { get; set; } = String.Empty;

    public static int idSquadra { get; set; } = 0;

    public static Squadra LeggiSquadra(ElementoArchivio elemento)
    {
        Squadra sq = new();

        try
        {

        }
        catch (Exception e)
        {
            return sq;
        }

        return sq;
    }



    public static Giocatore LeggiGiocatore(ElementoArchivio elemento)
    {
        Giocatore g = new();

        try
        {

        }
        catch (Exception e)
        {
            return g;
        }

        return g;
    }

    public static Allenatore LeggiAllenatore(ElementoArchivio elemento)
    {

        Allenatore a = new();

        try
        {

        }
        catch (Exception e)
        {
            return a;
        }

        return a;
    }

    public static Stadio LeggiStadio(ElementoArchivio elemento)
    {

        Stadio st = new();

        try
        {

        }
        catch (Exception e)
        {
            return st;
        }

        return st;
    }

    public static List<Byte> ScriviSquadra(Squadra squadra)
    {
        throw new NotImplementedException();
    }

    public static List<Byte> ScriviGiocatore(Giocatore giocatore)
    {
        throw new NotImplementedException();
    }


    public static List<Byte> ScriviAllenatore(Allenatore allenatore)
    {
        throw new NotImplementedException();
    }

    public static List<Byte> ScriviStadio(Stadio stadio)
    {
        throw new NotImplementedException();
    }

}
