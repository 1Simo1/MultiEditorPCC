using MultiEditorPCC.API.DBContext;
using MultiEditorPCC.API.Lib.Archivi;
using MultiEditorPCC.Shared.DTO;
using System.Reflection;
using System.Text;

namespace MultiEditorPCC.API.Lib;

public static class Utils
{
    public static VersionePCC TestVersionePCC(String Path)
    {
        Dictionary<String, VersionePCC> FiltriRicerca = new()
        {
            { "*00036.FDI", VersionePCC.PCC2001 },
            { "*00022.FDI", VersionePCC.PCF2001 },
            { "*99036.FDI", VersionePCC.PCC7P },
            { "*99022.FDI", VersionePCC.PCF7P },
            { "EQ036036.PKF", VersionePCC.PCC6 },
            { "EQ030022.PKF", VersionePCC.PCF6_ORO },
            { "EQUIPOS.PKF", VersionePCC.PCC5 },
            { "EQ036022.PKF", VersionePCC.PCF5_ORO },
            { "EQ95*.DBC", VersionePCC.PCC4 },
            { "*.DGF", VersionePCC.PCC3 }
        };

        foreach (var Cerca in FiltriRicerca)
        {
            if (Directory.GetFiles(Path, Cerca.Key, SearchOption.AllDirectories).Any())
            {
                return Cerca.Value;
            }
        }

        return VersionePCC.NESSUNA;
    }


    /// <summary>
    /// Decodifica e traduzione testi archivi
    /// </summary>
    /// <param name="testoCodificato">Byte nel file che sono il testo codificato</param>
    /// <returns>Testo decodificato e tradotto</returns>
    public static String DecodificaTesto(List<byte> testoCodificato)
    {
        String testo = String.Empty;

        foreach (byte b in testoCodificato) testo += Convert.ToChar((byte)(b ^ 97));

        return testo;
    }

    /// <summary>
    /// Decodifica e traduzione testi archivi
    /// </summary>
    /// <param name="testo">Byte nel file che sono il testo codificato</param>
    /// <returns>Testo decodificato e tradotto</returns>
    public static List<byte> CodificaTesto(String testo)
    {
        List<byte> b = new();

        foreach (var c in testo) b.Add((byte)(BitConverter.GetBytes(c)[0] ^ 97));

        return b;
    }

    public static Progetto CaricaPercorsiArchivi(Progetto progettoAttivo)
    {

        Dictionary<VersionePCC, List<String>> FiltriArchivi = new()
        {
            { VersionePCC.PCC2001, new() { "*00036.FDI" } },
            { VersionePCC.PCF2001, new() { "*00022.FDI" } },
            { VersionePCC.PCC7P, new() { "*99036.FDI" } },
            { VersionePCC.PCF7P, new() { "*99022.FDI" } },
            { VersionePCC.PCC6, new() { "EQ036036.PKF", "*.DBC" } },
            { VersionePCC.PCF6_ORO, new() { "EQ*.PKF", "*.DBC" } },
            { VersionePCC.PCC5, new() { "EQUIPOS.PKF", "*.DBC" } },
            { VersionePCC.PCF5_ORO, new() { "EQ*.PKF", "*.DBC" } },
            { VersionePCC.PCC4, new() { "EQ95*.DBC" } },
            { VersionePCC.PCC3, new() { "*.DGF" } }
        };

        foreach (var f in FiltriArchivi[progettoAttivo.VersionePCC])
        {
            foreach (var v in Directory.GetFiles(progettoAttivo.Cartella, f, SearchOption.AllDirectories))

                progettoAttivo.DatabaseFiles.Add(v.Substring(progettoAttivo.Cartella.Length + 1));
        }

        return progettoAttivo;
    }

    public static List<FileDatabaseInfo> CaricaFileDatabaseInfo(Progetto progettoAttivo)
    {
        List<FileDatabaseInfo> DatabaseInfo = new();

        Lazy<FDI> FDI = new(() => new());
        Lazy<FDI> PKF = new(() => new());
        Lazy<FDI> DBC = new(() => new());


        foreach (var file in progettoAttivo.DatabaseFiles)
        {
            if (file.ToUpper().EndsWith(".FDI"))
            {
                DatabaseInfo.AddRange(FDI.Value.ComponiElencoFileDatabase(progettoAttivo, file));
            }

            if (file.ToUpper().EndsWith(".PKF"))
            {
                DatabaseInfo.AddRange(PKF.Value.ComponiElencoFileDatabase(progettoAttivo, file));
            }

            if (file.ToUpper().EndsWith(".DBC"))
            {
                DatabaseInfo.AddRange(DBC.Value.ComponiElencoFileDatabase(progettoAttivo, file));
            }


        }
        return DatabaseInfo;
    }

    public static List<Squadra> CaricaSquadre(Progetto progettoAttivo, List<FileDatabaseInfo> DatabaseFiles)
    {
        List<Squadra> Squadre = new();

        //Versioni con un solo tipo di file database possibile (FDI)
        VersionePCC[] v = { VersionePCC.PCC2001, VersionePCC.PCF2001, VersionePCC.PCC7P, VersionePCC.PCF7P };

        Lazy<FDI> FDI = new(() => new());
        Lazy<FDI> PKF = new(() => new());
        Lazy<FDI> DBC = new(() => new());


        if (v.Contains(progettoAttivo.VersionePCC))
        {
            return FDI.Value.ComponiListaSquadreEditor(DatabaseFiles);
        }
        else
        {
            Squadre = PKF.Value.ComponiListaSquadreEditor(DatabaseFiles);
            foreach (var Squadra in DBC.Value.ComponiListaSquadreEditor(DatabaseFiles))
            {
                if (Squadre.Where(sq => sq.Id == Squadra.Id).Any())
                {
                    Squadre.Remove(Squadre.Where(sq => sq.Id == Squadra.Id).First());
                }

                Squadre.Add(Squadra);
            }
        }

        return Squadre;
    }

    public static List<Giocatore> CaricaGiocatori(Progetto progettoAttivo, List<FileDatabaseInfo> DatabaseFiles)
    {
        List<Giocatore> Giocatori = new();

        //Versioni con un solo tipo di file database possibile (FDI)
        VersionePCC[] v = { VersionePCC.PCC2001, VersionePCC.PCF2001, VersionePCC.PCC7P, VersionePCC.PCF7P };

        Lazy<FDI> FDI = new(() => new());
        Lazy<FDI> PKF = new(() => new());
        Lazy<FDI> DBC = new(() => new());


        if (v.Contains(progettoAttivo.VersionePCC))
        {
            return FDI.Value.ComponiListaGiocatoriEditor(DatabaseFiles);
        }
        else
        {
            Giocatori = PKF.Value.ComponiListaGiocatoriEditor(DatabaseFiles);
            foreach (var Giocatore in DBC.Value.ComponiListaGiocatoriEditor(DatabaseFiles))
            {
                if (Giocatori.Where(sq => sq.Id == Giocatore.Id).Any())
                {
                    Giocatori.Remove(Giocatori.Where(sq => sq.Id == Giocatore.Id).First());
                }

                Giocatori.Add(Giocatore);
            }
        }

        return Giocatori;
    }

    public static List<Giocatore> AssegnaGiocatoriSquadre(List<Squadra> Squadre, List<Giocatore> Giocatori)
    {
        foreach (var Squadra in Squadre)
        {
            if (Squadra.Note.Contains("#"))
            {
                foreach (var c in Squadra.Note.Split("#"))
                {
                    Giocatore? g = Giocatori.Where(g => g.Id == int.Parse(c.Split("|")[1])).FirstOrDefault();

                    if (g != null)
                    {
                        g.CodiceSquadra = Squadra.Id;
                        g.Squadra = Squadra.Nome;
                        g.AttivoInRosa = c.Split("|")[0] == true.ToString();
                    }
                }
            }
        }
        return Giocatori;
    }

    public static bool ScriviCSV(Progetto ProgettoAttivo, List<Squadra> Squadre, List<Giocatore> Giocatori)
    {
        try
        {
            Directory.CreateDirectory($"Progetti/{ProgettoAttivo.Nome}/CSV");

            StringBuilder sb = new();

            String HeaderCSV = Squadre.First().ToString().Split(Environment.NewLine)[0];

            sb.Append(HeaderCSV);

            foreach (var Squadra in Squadre)
            {
                sb.AppendLine();
                sb.Append(Squadra.ToString().Split(Environment.NewLine)[1]);
            }

            File.WriteAllText($"Progetti/{ProgettoAttivo.Nome}/CSV/Squadre_{ProgettoAttivo.VersionePCC}.csv", sb.ToString());
            sb = new();

            HeaderCSV = Giocatori.First().ToString().Split(Environment.NewLine)[0];

            sb.Append(HeaderCSV);

            foreach (var Giocatore in Giocatori)
            {
                sb.AppendLine();
                sb.Append(Giocatore.ToString().Split(Environment.NewLine)[1]);
            }

            File.WriteAllText($"Progetti/{ProgettoAttivo.Nome}/CSV/Giocatori_{ProgettoAttivo.VersionePCC}.csv", sb.ToString());
        }
        catch (Exception)
        {

            return false;
        }



        return true;
    }

    public static List<Squadra> CaricaSquadreCSV(Progetto ProgettoAttivo)
    {
        List<Squadra> Squadre = new();

        var csv = File.ReadAllLines(
            $"Progetti/{ProgettoAttivo.Nome}/CSV/Squadre_{ProgettoAttivo.VersionePCC}.csv");

        var Header = csv[0];
        foreach (var row in csv)
        {
            if (row != Header)
            {
                Squadra Squadra = CaricaElementoCSV<Squadra>(Header, row);

                Squadre.Add(Squadra);
            }
        }

        var ElencoCSV = Directory.GetFiles($"Progetti/{ProgettoAttivo.Nome}/CSV", "Squadre*.csv", SearchOption.TopDirectoryOnly);

        //In caso di CSV aggiornati, sostituiscono i dati originali
        //e i CSV originali servono per tenere conto dei valori incogniti
        //di ogni squadra e dei codici originali per poi distribuire 
        //le squadre nei vari gruppi di competizioni nel gioco
        //e nei vari Paesi
        //if (ElencoCSV.Length > 1) Squadre = new();

        foreach (var fileCSV in ElencoCSV)
        {
            if (!fileCSV.EndsWith($"Squadre_{ProgettoAttivo.VersionePCC}.csv"))
            {
                csv = File.ReadAllLines(fileCSV);
                Header = csv[0];
                foreach (var row in csv)
                {
                    if (row != Header)
                    {
                        Squadra Squadra = CaricaElementoCSV<Squadra>(Header, row);

                        Squadra? CercaSquadra = Squadre.Where
                            (sq => (sq.Nome == Squadra.Nome && sq.Nazione == Squadra.Nazione) ||
                                   (sq.NomeStadio == Squadra.NomeStadio || Squadra.NomeStadio.Contains(sq.NomeStadio) || sq.NomeStadio.Contains(Squadra.NomeStadio)) ||
                                   (sq.Nome.Contains(Squadra.Nome) || Squadra.Nome.Contains(sq.Nome))

                            ).FirstOrDefault();

                        if (CercaSquadra != null)
                        {
                            try
                            {
                                Squadre.Remove(CercaSquadra);
                            }
                            catch (Exception ex)
                            {

                            }
                            Squadra.SquadraOriginale = false;
                            Squadra.Note = Squadra.Id.ToString();
                            Squadra.Id = CercaSquadra.Id;
                            Squadra.Nome = CercaSquadra.Nome;
                            Squadra.AnnoFondazione = CercaSquadra.AnnoFondazione;
                            Squadra.Boh = CercaSquadra.Boh;
                            Squadra.NumeroAbbonati = CercaSquadra.NumeroAbbonati;
                            Squadra.CassaGioco = CercaSquadra.CassaGioco;
                            Squadra.CassaReale = CercaSquadra.CassaReale;
                            Squadra.SquadraRiserve = CercaSquadra.SquadraRiserve;
                            Squadra.TerzaSquadra = CercaSquadra.TerzaSquadra;
                            Squadra.Girone2B = CercaSquadra.Girone2B;
                            Squadra.Girone3 = CercaSquadra.Girone3;
                            Squadra.PercentualeToccoDiPrima = CercaSquadra.PercentualeToccoDiPrima;
                            Squadra.PercentualeContropiede = CercaSquadra.PercentualeContropiede;
                            Squadra.TipoAttacco = CercaSquadra.TipoAttacco;
                            Squadra.TipoEntrata = CercaSquadra.TipoEntrata;
                            Squadra.TipoMarcatura = CercaSquadra.TipoMarcatura;
                            Squadra.TipoRinvii = CercaSquadra.TipoRinvii;
                            Squadra.PressingDa = CercaSquadra.PressingDa;
                            Squadra.TatticaCompleta = CercaSquadra.TatticaCompleta;
                            Squadra.NumeroBoh = CercaSquadra.NumeroBoh;

                        }




                        Squadre.Add(Squadra);
                    }
                }
            }

        }

        return Squadre;
    }




    public static List<Giocatore> CaricaGiocatoriCSV(Progetto ProgettoAttivo, List<Squadra>? Squadre = null)
    {
        List<Giocatore> Giocatori = new();

        var csv = File.ReadAllLines(
            $"Progetti/{ProgettoAttivo.Nome}/CSV/Giocatori_{ProgettoAttivo.VersionePCC}.csv");

        var Header = csv[0];
        foreach (var row in csv)
        {
            if (row != Header)
            {
                Giocatore Giocatore = CaricaElementoCSV<Giocatore>(Header, row);

                Giocatori.Add(Giocatore);
            }
        }

        var ElencoCSV = Directory.GetFiles($"Progetti/{ProgettoAttivo.Nome}/CSV", "Giocatori*.csv", SearchOption.TopDirectoryOnly);

        //In caso di CSV aggiornati, sostituiscono i dati originali
        //e i CSV originali servono per tenere conto dei valori incogniti
        //di ogni squadra e dei codici originali per poi distribuire 
        //le squadre nei vari gruppi di competizioni nel gioco
        //e nei vari Paesi
        if (ElencoCSV.Length > 1)
        {
            int Limite_Index_Squadra = 65621;

            if (ProgettoAttivo != null)
            {

                switch (ProgettoAttivo.VersionePCC)
                {
                    case VersionePCC.PCC2001:
                    case VersionePCC.PCF2001:
                    case VersionePCC.PCC7P:
                    case VersionePCC.PCF7P: Limite_Index_Squadra = 9900; break;
                }

                Giocatori = new(Giocatori.Where(g => g.CodiceSquadra >= Limite_Index_Squadra));
            }
        }

        foreach (var fileCSV in ElencoCSV)
        {
            if (!fileCSV.EndsWith($"Giocatori_{ProgettoAttivo.VersionePCC}.csv"))
            {

                if (Squadre != null)
                {
                    var test = Squadre.Where(sq => sq.Note != "").ToList();
                    foreach (var sq in test)
                    {
                        Giocatori.RemoveAll(g => g.CodiceSquadra == sq.Id);
                    }
                }


                csv = File.ReadAllLines(fileCSV);
                Header = csv[0];
                foreach (var row in csv)
                {
                    if (row != Header)
                    {
                        Giocatore Giocatore = CaricaElementoCSV<Giocatore>(Header, row);

                        if (Squadre != null)
                        {

                            var test = Squadre.Where(sq => sq.Note != "").Select(sq => sq.Note);
                            if (test.Contains(Giocatore.CodiceSquadra.ToString()))
                            {
                                Giocatore.CodiceSquadra = Squadre.Where(sq => sq.Note == Giocatore.CodiceSquadra.ToString()).First().Id;
                            }
                        }


                        Giocatori.Add(Giocatore);
                    }
                }
            }

        }

        return Giocatori;
    }


    public static T? CaricaElementoCSV<T>(String Header, String Row, String Sep = ";") where T : class, new()
    {
        T Elemento = new();

        try
        {

            string[] Col = Header.Split(Sep);
            string[] Val = Row.Split(Sep);



            for (int n = 0; n < Col.Length; n++)
            {
                PropertyInfo? prop = Elemento.GetType().GetProperty(Col[n]);
                if (prop != null)
                {
                    object v = Val[n];
                    if (prop.PropertyType != typeof(string))
                    {

                        var nome = prop.PropertyType.Name;

                        if (nome.StartsWith("Nullable") || nome.StartsWith("List"))
                        {
                            nome = prop.PropertyType.GenericTypeArguments[0].Name;

                            if (prop.PropertyType.GenericTypeArguments[0].IsEnum)
                            {
                                nome = "List<Enum>";
                            }
                        }

                        switch (nome)
                        {
                            case "UInt16": v = UInt16.Parse(Val[n]); break;
                            case "UInt32": v = uint.Parse(Val[n]); break;
                            case "Int16": v = Int16.Parse(Val[n]); break;
                            case "Int32": v = int.Parse(Val[n]); break;
                            case "Boolean": v = Val[n] == true.ToString(); break;
                            case "Byte": v = Convert.FromBase64String(Val[n]).ToList(); break;

                        }

                        if (prop.PropertyType.IsEnum)
                        {
                            try
                            {
                                v = Enum.Parse(prop.PropertyType, Val[n]);
                            }
                            catch (Exception)
                            {


                            }
                        }

                        if (nome == "List<Enum>")
                        {
                            var e = Val[n].Split("|");

                            var ruoli = new List<Ruolo>();

                            foreach (var en in e)
                            {
                                ruoli.Add((Ruolo)(Enum.Parse(prop.PropertyType.GenericTypeArguments[0], en)));
                            }

                            prop.SetValue(Elemento, ruoli, null);
                            continue;
                        }

                    }

                    prop.SetValue(Elemento, v, null);
                }
            }


        }
        catch (Exception ex)
        {

            return Elemento;
        }



        return Elemento;
    }

}


