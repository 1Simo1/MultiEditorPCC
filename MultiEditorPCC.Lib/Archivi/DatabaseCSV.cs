using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MultiEditorPCC.Dat.DbSet;
using System.Globalization;
using System.Text;


namespace MultiEditorPCC.Lib.Archivi;

/// <summary>
/// Classe per esportare ed importare i dati del database di gioco da editor a CSV e viceversa
/// I metodi di scrittura non scrivono automaticamente su disco, ma ritornano il contenuto
/// del CSV in memoria
/// </summary>
public static partial class DatabaseCSV
{
    public static int Versione { get; set; }

    public static String contenutoCSV { get; set; }

    public static CsvConfiguration Conf { get; set; } = new CsvConfiguration(CultureInfo.CurrentCulture)
    {
        HasHeaderRecord = true,
        Delimiter = ";",
        Encoding = Encoding.UTF8,
        IgnoreReferences = true,
        IgnoreBlankLines = true,
        AllowComments = true
    };

    public static bool scriviTatticaCompleta { get; set; } = false;

    public static Dictionary<int, Tuple<int, String>> dettagliSquadreGiocatore { get; set; } = new();

    public static List<Squadra> LeggiSquadre()
    {
        List<Squadra> squadre = new();

        try
        {

        }
        catch (Exception e)
        {
            return squadre;
        }

        return squadre;
    }

    public static List<Giocatore> LeggiGiocatori()
    {
        List<Giocatore> giocatori = new();

        try
        {

        }
        catch (Exception e)
        {
            return giocatori;
        }

        return giocatori;
    }

    public static List<Allenatore> LeggiAllenatori()
    {
        List<Allenatore> allenatori = new();

        try
        {

        }
        catch (Exception e)
        {
            return allenatori;
        }

        return allenatori;
    }

    public static List<Stadio> LeggiStadi()
    {
        List<Stadio> stadi = new();

        try
        {

        }
        catch (Exception e)
        {
            return stadi;
        }

        return stadi;
    }

    public static void ScriviCSVSquadre(List<Squadra> squadre)
    {
        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream))
        using (var csv = new CsvWriter(streamWriter, Conf))
        {
            try
            {
                csv.Context.RegisterClassMap<SquadraMap>();
                csv.WriteRecords(squadre);
                streamWriter.Flush();
                contenutoCSV = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception e)
            {
                return;
            }
        }
        return;
    }

    public static void ScriviCSVGiocatori(List<Giocatore> giocatori)
    {
        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream))
        using (var csv = new CsvWriter(streamWriter, Conf))
        {
            try
            {
                csv.Context.RegisterClassMap<GiocatoreMap>();
                csv.WriteRecords(giocatori);
                streamWriter.Flush();
                contenutoCSV = $"{Encoding.UTF8.GetString(memoryStream.ToArray())}";
            }
            catch (Exception e)
            {
                return;
            }
        }
        return;
    }

    public static void ScriviCSVAllenatori(List<Allenatore> allenatori)
    {
        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream))
        using (var csv = new CsvWriter(streamWriter, Conf))
        {
            try
            {
                csv.Context.RegisterClassMap<AllenatoreMap>();
                csv.WriteRecords(allenatori);
                streamWriter.Flush();
                contenutoCSV = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception e)
            {
                return;
            }
        }
        return;
    }

    public static void ScriviCSVStadi(List<Stadio> stadi)
    {
        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream))
        using (var csv = new CsvWriter(streamWriter, Conf))
        {
            try
            {
                csv.Context.RegisterClassMap<StadioMap>();
                csv.WriteRecords(stadi);
                streamWriter.Flush();
                contenutoCSV = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception e)
            {
                return;
            }
        }
        return;
    }
}


public class StadioMap : ClassMap<Stadio>
{
    public StadioMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map().Name("V").Constant(1);
        Map().Name("TipoInfo").Constant((int)ArchivioSvc.TipoDatoDB.STADIO);
    }
}


public class AllenatoreMap : ClassMap<Allenatore>
{
    public AllenatoreMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map().Name("V").Constant(1);
        Map().Name("TipoInfo").Constant((int)ArchivioSvc.TipoDatoDB.ALLENATORE);
    }
}


public class GiocatoreMap : ClassMap<Giocatore>
{
    public GiocatoreMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(g => g.Ruoli).TypeConverter<ElencoRuoliConverter>();
        Map(g => g.Punteggi).TypeConverter<PunteggiConverter>();
        Map(g => g.Id, false).Name("CodiceSquadra").TypeConverter<GiocatoriCodiceSquadraConverter>(); ;
        Map(g => g.Id, false).Name("Squadra").TypeConverter<GiocatoriNomeSquadraConverter>();
        Map().Name("V").Constant(1);
        Map().Name("TipoInfo").Constant((int)ArchivioSvc.TipoDatoDB.GIOCATORE);
    }
}


public class SquadraMap : ClassMap<Squadra>
{

    public static bool scriviTatticaCompleta { get; set; } = false;
    public SquadraMap()
    {
        scriviTatticaCompleta = DatabaseCSV.scriviTatticaCompleta;
        AutoMap(CultureInfo.InvariantCulture);
        Map(sq => sq.Stadio).Ignore();
        Map(sq => sq.Stadio.Id).Name("V").Constant(1);
        Map(sq => sq.Stadio.Nome).Ignore();
        Map(sq => sq.Stadio.Nazione).Ignore();
        Map(sq => sq.Stadio.Larghezza).Ignore();
        Map(sq => sq.Stadio.Lunghezza).Ignore();
        Map(sq => sq.Stadio.NumeroBoh).Ignore();
        Map(sq => sq.Stadio.AnnoCostruzione).Ignore();
        Map(sq => sq.Stadio.Capienza).Ignore();
        Map(sq => sq.Stadio.PostiInPiedi).Ignore();
        Map(sq => sq.Stadio.Giocabile).Ignore();
        Map(sq => sq.StagioniA).Ignore();
        Map(sq => sq.Giocate).Ignore();
        Map(sq => sq.Vinte).Ignore();
        Map(sq => sq.Pareggiate).Ignore();
        Map(sq => sq.GolSegnati).Ignore();
        Map(sq => sq.GolSubiti).Ignore();
        Map(sq => sq.PuntiTotali).Ignore();
        Map(sq => sq.Scudetti).Ignore();
        Map(sq => sq.SecondiPosti).Ignore();
        Map(sq => sq.LTrofeiPalmares).Ignore();
        Map(sq => sq.Tattica.Id).Name("TipoInfo").Constant((int)ArchivioSvc.TipoDatoDB.SQUADRA);
        if (scriviTatticaCompleta) Map(sq => sq.Tattica.TatticaCompleta).TypeConverter<TatticaCompletaConverter>();
        //Map(sq => sq.Giocatori).TypeConverter<ElencoGiocatoriSquadraConverter>();
        Map(sq => sq.Giocatori).Ignore();
        Map(sq => sq.Allenatori).Name("codiceAllenatore").TypeConverter<AllenatoreAttivoSquadraConverter>();

    }
}

internal class AllenatoreAttivoSquadraConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        //TODO da CSV ad Allenatore attivo inserito nella Squadra
        throw new NotImplementedException();
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return ((List<Allenatore>)value).Last().Id.ToString();
    }

}

internal class TatticaCompletaConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return Convert.FromBase64String(text).ToList();
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value == null) return String.Empty;
        return Convert.ToBase64String(((List<Byte>)value).ToArray());
    }
}

internal class StadioConverter : DefaultTypeConverter
{

    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return base.ConvertFromString(text, row, memberMapData);
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return value.ToString();
    }

}


internal class GiocatoriCodiceSquadraConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return base.ConvertFromString(text, row, memberMapData);
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value == null) return "0";
        return DatabaseCSV.dettagliSquadreGiocatore[int.Parse(value.ToString())].Item1.ToString();
    }

}

internal class GiocatoriNomeSquadraConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return base.ConvertFromString(text, row, memberMapData);
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value == null) return String.Empty;
        return DatabaseCSV.dettagliSquadreGiocatore[int.Parse(value.ToString())].Item2;
    }

}

// TODO Decommentare se lascio i codici giocatori anche nel csv squadre
//internal class ElencoGiocatoriSquadraConverter : DefaultTypeConverter
//{
//    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
//    {
//        return base.ConvertFromString(text, row, memberMapData);
//    }

//    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
//    {
//        String elencoCodiciGiocatori = String.Empty;

//        List<Giocatore> GiocatoriSquadra = (List<Giocatore>)value;

//        bool primo = true;

//        foreach (var g in GiocatoriSquadra)
//        {
//            if (!primo) elencoCodiciGiocatori += "|";
//            elencoCodiciGiocatori += g.Id.ToString();
//            if (primo) primo = false;
//        }

//        return elencoCodiciGiocatori;
//    }


//}

internal class ElencoRuoliConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return base.ConvertFromString(text, row, memberMapData);
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        String elencoRuoliGiocatore = String.Empty;

        List<Ruolo> RuoliGiocatore = (List<Ruolo>)value;

        bool primo = true;

        foreach (var g in RuoliGiocatore)
        {
            if (!primo) elencoRuoliGiocatore += "|";
            elencoRuoliGiocatore += g.ToString();
            if (primo) primo = false;
        }

        return elencoRuoliGiocatore;
    }
}

internal class PunteggiConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return base.ConvertFromString(text, row, memberMapData);
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        String elencoPunteggiGiocatore = String.Empty;

        List<PunteggioGiocatore> PunteggiGiocatore = (List<PunteggioGiocatore>)value;

        bool primo = true;

        foreach (var g in PunteggiGiocatore)
        {
            if (!primo) elencoPunteggiGiocatore += "|";
            elencoPunteggiGiocatore += g.Punteggio.ToString();
            if (primo) primo = false;
        }

        return elencoPunteggiGiocatore;
    }
}