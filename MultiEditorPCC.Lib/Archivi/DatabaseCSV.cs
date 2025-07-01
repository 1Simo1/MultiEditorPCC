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

            using (var streamReader = new StreamReader(contenutoCSV))
            using (var csv = new CsvReader(streamReader, Conf))
            {
                try
                {
                    csv.Context.RegisterClassMap<SquadraMap>();
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {

                        if (csv.GetField<ArchivioSvc.TipoDatoDB>("TipoInfo") != ArchivioSvc.TipoDatoDB.SQUADRA)
                        {
                            return new();
                        }

                        var sq = new Squadra
                        {
                            Id = csv.GetField<uint>("Id"),
                            Nome = csv.GetField("Nome").Trim(),
                            NomeCompleto = csv.GetField("NomeCompleto"),
                            Giocabile = csv.GetField<bool>("Giocabile"),
                            Nazione = csv.GetField<Paese>("Nazione"),
                            AnnoFondazione = csv.GetField<ushort>("AnnoFondazione"),
                            Boh = csv.GetField<int>("Boh"),
                            NumeroAbbonati = csv.GetField<int>("NumeroAbbonati"),
                            NomePresidente = csv.GetField("NomePresidente"),
                            CassaGioco = csv.GetField<int>("CassaGioco"),
                            CassaReale = csv.GetField<int>("CassaReale"),
                            NomeSponsor = csv.GetField("NomeSponsor"),
                            NomeSponsorTecnico = csv.GetField("NomeSponsorTecnico"),
                            SquadraRiserve = csv.GetField<int>("SquadraRiserve"),
                            TerzaSquadra = csv.GetField<int>("TerzaSquadra"),
                            Girone2B = csv.GetField<Girone2B>("Girone2B"),
                            Girone3 = csv.GetField<int>("Girone3"),
                            Tattica = new()
                            {
                                PercentualeToccoDiPrima = csv.GetField<int>("PercentualeToccoDiPrima"),
                                PercentualeContropiede = csv.GetField<int>("PercentualeContropiede"),
                                TipoAttacco = csv.GetField<TipoAttacco>("TipoAttacco"),
                                TipoEntrata = csv.GetField<TipoEntrata>("TipoEntrata"),
                                TipoMarcatura = csv.GetField<TipoMarcatura>("TipoMarcatura"),
                                TipoRinvii = csv.GetField<TipoRinvii>("TipoRinvii"),
                                PressingDa = csv.GetField<PressingDa>("PressingDa"),
                                TatticaCompleta = new()
                            },
                            Allenatori = new() { new Allenatore() { Id = csv.GetField<uint>("codiceAllenatore") } }

                        };

                        if (csv.TryGetField<String>("TatticaCompleta", out _))
                        {
                            sq.Tattica.TatticaCompleta = Convert.FromBase64String(csv.GetField("TatticaCompleta")).ToList();
                        }


                        squadre.Add(sq);
                    }
                }
                catch (Exception e)
                {
                    return squadre;
                }
            }
            return squadre;
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
            using (var streamReader = new StreamReader(contenutoCSV))
            using (var csv = new CsvReader(streamReader, Conf))
            {
                try
                {
                    csv.Context.TypeConverterCache.AddConverter<List<Ruolo>>(new ElencoRuoliConverter());
                    csv.Context.TypeConverterCache.AddConverter<List<PunteggioGiocatore>>(new PunteggiConverter());
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {

                        if (csv.GetField<ArchivioSvc.TipoDatoDB>("TipoInfo") != ArchivioSvc.TipoDatoDB.GIOCATORE)
                        {
                            return new();
                        }

                        var gc = new Giocatore
                        {
                            Id = csv.GetField<int>("Id"),
                            Nome = csv.GetField("Nome").Trim(),
                            NomeCompleto = csv.GetField("NomeCompleto"),
                            Giocabile = csv.GetField<bool>("Giocabile"),
                            Nazione = csv.GetField<Paese>("Nazione"),
                            Numero = csv.GetField<int>("Numero"),
                            Slot = csv.GetField<int>("Slot"),
                            AltriDati = csv.GetField<bool>("AltriDati"),
                            AttivoInRosa = csv.GetField<bool>("AttivoInRosa"),
                            codColorePelle = csv.GetField<ColorePelle>("codColorePelle"),
                            codColoreCapelli = csv.GetField<ColoreCapelli>("codColoreCapelli"),
                            codStileCapelli = csv.GetField<StileCapelli>("codStileCapelli"),
                            codStileBarba = csv.GetField<StileBarba>("codStileBarba"),
                            Nazionalizzato = csv.GetField<bool>("Nazionalizzato"),
                            Reparto = csv.GetField<Reparto>("Reparto"),
                            GiornoNascita = csv.GetField<int>("GiornoNascita"),
                            MeseNascita = csv.GetField<int>("MeseNascita"),
                            AnnoNascita = csv.GetField<int>("AnnoNascita"),
                            Altezza = csv.GetField<int>("Altezza"),
                            Peso = csv.GetField<int>("Peso"),
                            PaeseNascita = csv.GetField<Paese>("PaeseNascita"),
                            Ruoli = csv.GetField<List<Ruolo>>("Ruoli"),
                            Punteggi = csv.GetField<List<PunteggioGiocatore>>("Punteggi"),


                            CodiceSquadra = csv.GetField<int>("CodiceSquadra"),
                            Squadra = csv.GetField("Squadra").Trim()


                        };

                        giocatori.Add(gc);
                    }


                }
                catch (Exception e)
                {
                    return giocatori;
                }

            }
        }
        catch (Exception ex)
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
            using (var streamReader = new StreamReader(contenutoCSV))
            using (var csv = new CsvReader(streamReader, Conf))
            {
                try
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {

                        if (csv.GetField<ArchivioSvc.TipoDatoDB>("TipoInfo") != ArchivioSvc.TipoDatoDB.ALLENATORE)
                        {
                            return new();
                        }

                        var al = new Allenatore
                        {
                            Id = csv.GetField<uint>("Id"),
                            Nome = csv.GetField("Nome").Trim(),
                            NomeCompleto = csv.GetField("NomeCompleto"),
                            Giocabile = csv.GetField<bool>("Giocabile"),
                            exGiocatore = csv.GetField<bool>("exGiocatore"),
                        };

                        allenatori.Add(al);
                    }
                }
                catch (Exception e)
                {

                }
            }
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
            using (var streamReader = new StreamReader(contenutoCSV))
            using (var csv = new CsvReader(streamReader, Conf))
            {
                try
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {

                        if (csv.GetField<ArchivioSvc.TipoDatoDB>("TipoInfo") != ArchivioSvc.TipoDatoDB.STADIO)
                        {
                            return new();
                        }

                        var st = new Stadio
                        {
                            Id = csv.GetField<uint>("Id"),
                            Giocabile = csv.GetField<bool>("Giocabile"),
                            Nome = csv.GetField("Nome").Trim(),
                            Capienza = csv.GetField<int>("Capienza"),
                            PostiInPiedi = csv.GetField<int>("PostiInPiedi"),
                            Larghezza = csv.GetField<short>("Larghezza"),
                            Lunghezza = csv.GetField<short>("Lunghezza"),
                            Nazione = csv.GetField<Paese>("Nazione"),
                            NumeroBoh = csv.GetField<int>("NumeroBoh"),
                            AnnoCostruzione = csv.GetField<int>("AnnoCostruzione"),

                        };

                        stadi.Add(st);
                    }
                }
                catch (Exception e)
                {

                }
            }
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
        Map(g => g.CodiceSquadra).Ignore();
        Map(g => g.Squadra).Ignore();
        Map(g => g.Id, false).Name("CodiceSquadra").TypeConverter<GiocatoriCodiceSquadraConverter>();
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

        Map(sq => sq.Giocatori).Ignore();
        Map(sq => sq.Allenatori).Name("codiceAllenatore").TypeConverter<AllenatoreAttivoSquadraConverter>();

    }
}

public class AllenatoreAttivoSquadraConverter : DefaultTypeConverter
{
    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return ((List<Allenatore>)value).Last().Id.ToString();
    }

}

public class TatticaCompletaConverter : DefaultTypeConverter
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

public class StadioConverter : DefaultTypeConverter
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


public class GiocatoriCodiceSquadraConverter : DefaultTypeConverter
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

public class GiocatoriNomeSquadraConverter : DefaultTypeConverter
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

public class ElencoRuoliConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        List<Ruolo> ruoli = new();
        foreach (var codRuolo in text.Split("|"))
        {
            try
            {
                ruoli.Add(Enum.Parse<Ruolo>(codRuolo));
            }
            catch (Exception _) { }
        }
        return ruoli;
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

public class PunteggiConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {

        List<PunteggioGiocatore> punteggi = new();
        int n = 0;
        var v = text.Split("|");
        foreach (var punteggio in v)
        {
            try
            {
                if (n == 0)
                {
                    punteggi.Add(new PunteggioGiocatore() { Tipo = TipoPunteggioGiocatore.Media, Punteggio = int.Parse(punteggio) });
                }
                if (n == 11)
                {
                    switch (int.Parse(punteggio))
                    {
                        case 0:
                            punteggi.Add(new PunteggioGiocatore()
                            {
                                Tipo = TipoPunteggioGiocatore.PiedePreferito,
                                Punteggio = (int)PiedePreferito.Destro
                            });
                            break;
                        case 1:
                            punteggi.Add(new PunteggioGiocatore()
                            {
                                Tipo = TipoPunteggioGiocatore.PiedePreferito,
                                Punteggio = (int)PiedePreferito.Sinistro
                            });
                            break;

                        default:
                            punteggi.Add(new PunteggioGiocatore()
                            {
                                Tipo = TipoPunteggioGiocatore.PiedePreferito,
                                Punteggio = (int)PiedePreferito.Ambidestro
                            });
                            break;
                    }

                }
                if (n != 0 && n != 11)
                {
                    punteggi.Add(new PunteggioGiocatore() { Tipo = (TipoPunteggioGiocatore)n, Punteggio = int.Parse(punteggio) });
                }

            }
            catch (Exception _)
            {

            }
            finally
            {
                n++;
            }
        }
        return punteggi;
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