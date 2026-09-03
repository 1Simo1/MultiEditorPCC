using MultiEditorPCC.API.DBContext;
using MultiEditorPCC.Shared.DTO;


namespace MultiEditorPCC.API.Lib.Archivi;

public class FDI : IDatabaseFile
{
    public List<FileDatabaseInfo> ComponiElencoFileDatabase(Progetto progettoAttivo, String file)
    {
        List<FileDatabaseInfo> elenco = new();

        String path =
            (progettoAttivo.Cartella.EndsWith(Path.DirectorySeparatorChar) || file.StartsWith(Path.DirectorySeparatorChar)) ?
            $"{progettoAttivo.Cartella}{file}" : $"{progettoAttivo.Cartella}{Path.DirectorySeparatorChar}{file}";

        List<Byte> DatFDI = File.ReadAllBytes(path).ToList();

        var numeroElementi = BitConverter.ToInt32(DatFDI.GetRange(16, 4).ToArray(), 0);

        List<Byte> Header = DatFDI.GetRange(20, numeroElementi * 13);

        for (int i = 1; i <= numeroElementi; i++)
        {
            FileDatabaseInfo info = new();

            info.TipoFileDatabase = TipoFileDatabase.FDI;

            if (file.ToUpper().Contains("EQ")) info.TipoDatoDB = TipoDatoDB.SQUADRA;
            if (file.ToUpper().Contains("ENT")) info.TipoDatoDB = TipoDatoDB.ALLENATORE;
            if (file.ToUpper().Contains("EST")) info.TipoDatoDB = TipoDatoDB.STADIO;
            if (file.ToUpper().Contains("JUG")) info.TipoDatoDB = TipoDatoDB.GIOCATORE;

            info.VersionePCC = progettoAttivo.VersionePCC;


            info.VersioneElemento = 800;

            if (info.VersionePCC != VersionePCC.PCC2001 && info.VersionePCC != VersionePCC.PCF2001)
            {
                int delta = info.TipoDatoDB == TipoDatoDB.SQUADRA ? 38 : 2;
                info.VersioneElemento = BitConverter.ToInt16(DatFDI.GetRange(delta + 20 + 13 * numeroElementi, 2).ToArray(), 0);
            }




            int d = (i - 1) * 13;
            info.Codice = BitConverter.ToInt32(Header.GetRange(d, 4).ToArray(), 0);
            int offset = BitConverter.ToInt32(Header.GetRange(d + 5, 4).ToArray(), 0);
            int len = BitConverter.ToInt32(Header.GetRange(d + 9, 4).ToArray(), 0);
            info.Dat = DatFDI.GetRange(offset, len);
            elenco.Add(info);
        }


        return elenco;
    }

    public List<Squadra> ComponiListaSquadreEditor(List<FileDatabaseInfo> DatabaseInfo)
    {
        List<Squadra> Squadre = new();

        var infoSquadre = DatabaseInfo.Where(d => d.TipoDatoDB == TipoDatoDB.SQUADRA).ToList();
        var al = DatabaseInfo.Where(d => d.TipoDatoDB == TipoDatoDB.ALLENATORE).ToList();
        var st = DatabaseInfo.Where(d => d.TipoDatoDB == TipoDatoDB.STADIO).ToList();



        foreach (var e in infoSquadre)
        {
            Squadra sq = new();

            sq.Id = (uint)e.Codice;
            sq.Giocabile = e.Dat[41] == 0;
            var lnc = BitConverter.ToInt16(e.Dat.GetRange(42, 2).ToArray(), 0);
            sq.Nome = Utils.DecodificaTesto(e.Dat.GetRange(44, lnc));

            int offset = 0;


            if (e.VersioneElemento == 800)
            {
                sq.Nazione = (Paese)e.Dat[44 + lnc + 2];
                var lnl = BitConverter.ToInt16(e.Dat.GetRange(47 + lnc, 2).ToArray(), 0);
                sq.NomeCompleto = Utils.DecodificaTesto(e.Dat.GetRange(49 + lnc, lnl));
                sq.AnnoFondazione = (ushort)BitConverter.ToInt16(e.Dat.GetRange(49 + lnc + lnl, 2).ToArray(), 0);
                sq.Boh = e.Dat[51 + lnc + lnl];
                offset = 52 + lnc + lnl;
                if (sq.Giocabile)
                {
                    sq.NumeroAbbonati = BitConverter.ToInt32(e.Dat.GetRange(52 + lnc + lnl, 4).ToArray(), 0);

                    var lnp = BitConverter.ToInt16(e.Dat.GetRange(56 + lnc + lnl, 2).ToArray(), 0);
                    sq.NomePresidente = Utils.DecodificaTesto(e.Dat.GetRange(58 + lnc + lnl, lnp));
                    sq.CassaGioco = BitConverter.ToInt32(e.Dat.GetRange(58 + lnc + lnl + lnp, 4).ToArray(), 0);
                    sq.CassaReale = BitConverter.ToInt32(e.Dat.GetRange(62 + lnc + lnl + lnp, 4).ToArray(), 0);

                    var lnsp = BitConverter.ToInt16(e.Dat.GetRange(66 + lnc + lnl + lnp, 2).ToArray(), 0);
                    sq.NomeSponsor = Utils.DecodificaTesto(e.Dat.GetRange(68 + lnc + lnl + lnp, lnsp));
                    var lnst = BitConverter.ToInt16(e.Dat.GetRange(68 + lnc + lnl + lnp + lnsp, 2).ToArray(), 0);
                    sq.NomeSponsorTecnico = Utils.DecodificaTesto(e.Dat.GetRange(70 + lnc + lnl + lnp + lnsp, lnst));
                    sq.SquadraRiserve = BitConverter.ToInt16(e.Dat.GetRange(70 + lnc + lnl + lnp + lnsp + lnst, 2).ToArray(), 0);
                    sq.Girone2B = (Girone2B)e.Dat[72 + lnc + lnl + lnp + lnsp + lnst];
                    sq.Girone3 = e.Dat[73 + lnc + lnl + lnp + lnsp + lnst];
                    offset = 74 + lnc + lnl + lnp + lnsp + lnst;
                    offset = offset + 82 + (e.Dat[offset + 81] * 3);
                }


                var codiceStadio = (uint)BitConverter.ToInt16(e.Dat.GetRange(44 + lnc, 2).ToArray(), 0);
                var dati = st.Where(e => e.Codice == codiceStadio).First().Dat;
                var lns = BitConverter.ToInt16(dati.GetRange(0, 2).ToArray(), 0);
                sq.NomeStadio = Utils.DecodificaTesto(dati.GetRange(2, lns));
                sq.Larghezza = dati[lns + 2];
                sq.Lunghezza = dati[lns + 3];
                sq.NumeroBoh = dati[lns + 4];
                sq.Nazione = (Paese)dati[lns + 5];
                sq.AnnoCostruzione = (ushort)BitConverter.ToInt16(dati.GetRange(lns + 6, 2).ToArray(), 0);
                sq.Capienza = BitConverter.ToInt32(dati.GetRange(lns + 8, 4).ToArray(), 0);
                sq.PostiInPiedi = BitConverter.ToInt32(dati.GetRange(lns + 12, 4).ToArray(), 0);



            }
            else
            {
                var lns = BitConverter.ToInt16(e.Dat.GetRange(44 + lnc, 2).ToArray(), 0);
                sq.Nazione = (Paese)e.Dat[44 + lnc + lns + 2];


                sq.NomeStadio = Utils.DecodificaTesto(e.Dat.GetRange(44 + lnc + 2, lns));

                sq.Boh = e.Dat[47 + lnc + lns]; // byte dati[47 + lnc + lns] significato ignoto
                sq.NumeroBoh = sq.Boh; //Nella versione FDI 700, c'è un solo byte incognito, perchè non c'è un file distinto per gli stadi
                                       // var lnsq = BitConverter.ToInt16(e.Dat.GetRange(48 + lnc + lns, 2).ToArray(), 0);

                var lnl = BitConverter.ToInt16(e.Dat.GetRange(48 + lnc + lns, 2).ToArray(), 0);
                sq.NomeCompleto = Utils.DecodificaTesto(e.Dat.GetRange(50 + lnc + lns, lnl));

                offset = 50 + lnc + lns + lnl;

                sq.Capienza = BitConverter.ToInt32(e.Dat.GetRange(offset, 4).ToArray(), 0);

                sq.PostiInPiedi = BitConverter.ToInt32(e.Dat.GetRange(offset + 4, 4).ToArray(), 0);

                sq.Larghezza = BitConverter.ToInt16(e.Dat.GetRange(offset + 8, 2).ToArray(), 0);
                sq.Lunghezza = BitConverter.ToInt16(e.Dat.GetRange(offset + 10, 2).ToArray(), 0);

                if (sq.Giocabile) sq.AnnoCostruzione = (ushort)BitConverter.ToInt16(e.Dat.GetRange(offset + 14, 2).ToArray(), 0);


                offset = 62 + lnc + lns + lnl;

                sq.AnnoFondazione = (ushort)BitConverter.ToInt16(e.Dat.GetRange(offset, 2).ToArray(), 0);

                offset = 64 + lnc + lns + lnl;

                if (sq.Giocabile)
                {
                    sq.NumeroAbbonati = BitConverter.ToInt32(e.Dat.GetRange(offset + 2, 4).ToArray(), 0);

                    var lnp = BitConverter.ToInt16(e.Dat.GetRange(offset + 6, 2).ToArray(), 0);
                    sq.NomePresidente = Utils.DecodificaTesto(e.Dat.GetRange(offset + 8, lnp));
                    sq.CassaGioco = BitConverter.ToInt32(e.Dat.GetRange(offset + 8 + lnp, 4).ToArray(), 0);
                    sq.CassaReale = BitConverter.ToInt32(e.Dat.GetRange(offset + 12 + lnp, 4).ToArray(), 0);

                    var lnsp = BitConverter.ToInt16(e.Dat.GetRange(offset + 16 + lnp, 2).ToArray(), 0);
                    sq.NomeSponsor = Utils.DecodificaTesto(e.Dat.GetRange(offset + 18 + lnp, lnsp));
                    var lnst = BitConverter.ToInt16(e.Dat.GetRange(offset + 18 + lnp + lnsp, 2).ToArray(), 0);
                    sq.NomeSponsorTecnico = Utils.DecodificaTesto(e.Dat.GetRange(offset + 20 + lnp + lnsp, lnst));
                    sq.SquadraRiserve = BitConverter.ToInt16(e.Dat.GetRange(offset + 20 + lnp + lnsp + lnst, 2).ToArray(), 0);
                    sq.Girone2B = (Girone2B)e.Dat[offset + 22 + lnp + lnsp + lnst];
                    offset = offset + 23 + lnp + lnsp + lnst;
                    offset = offset + 82 + (e.Dat[offset + 81] * 3);
                }




            }

            sq.TatticaCompleta = e.Dat.GetRange(offset, 1760);

            offset += 1760;

            sq.PercentualeToccoDiPrima = e.Dat[offset];
            sq.PercentualeContropiede = e.Dat[offset + 1];
            sq.TipoAttacco = (TipoAttacco)e.Dat[offset + 2];
            sq.TipoEntrata = (TipoEntrata)e.Dat[offset + 3];
            sq.TipoMarcatura = (TipoMarcatura)e.Dat[offset + 4];
            sq.TipoRinvii = (TipoRinvii)e.Dat[offset + 5];
            sq.PressingDa = (PressingDa)e.Dat[offset + 6];

            offset += 7;

            int na = e.Dat[offset];

            offset++;

            for (int a = 1; a <= na; a++)
            {
                sq.CodiceAllenatore = (uint)BitConverter.ToInt32(e.Dat.GetRange(offset, 4).ToArray(), 0);

                offset += 4;
            }



            if (al.Where(a => a.Codice == sq.CodiceAllenatore).FirstOrDefault() != null)
            {
                var allenatore = al.Find(a => a.Codice == sq.CodiceAllenatore)!.Dat;

                var lna = BitConverter.ToInt16(allenatore.GetRange(7, 2).ToArray(), 0);

                sq.NomeAllenatore = Utils.DecodificaTesto(allenatore.GetRange(9, lna));

                if (sq.Giocabile)
                {
                    var lnl = 0;
                    if (11 + lna <= allenatore.Count)
                        lnl = BitConverter.ToInt16(allenatore.GetRange(9 + lna, 2).ToArray(), 0);

                    if (11 + lna + lnl <= allenatore.Count)
                        sq.NomeCompletoAllenatore = Utils.DecodificaTesto(allenatore.GetRange(11 + lna, lnl));
                }

                //sq.ExGiocatore = false;

            }

            int ng = e.Dat[offset];

            offset++;

            if (e.VersioneElemento == 800)
            {
                ng += 256 * e.Dat[offset + 1];
                offset++;
            }

            for (int g = 1; g <= ng; g++)
            {
                String v = g == 1 ? String.Empty : "#";
                sq.Note = $"{sq.Note}{v}{e.Dat[offset] == 0}|{BitConverter.ToInt32(e.Dat.GetRange(offset + 1, 4).ToArray(), 0)}";
                offset += 5;
            }


            Squadre.Add(sq);
        }



        return Squadre;
    }

    public List<Giocatore> ComponiListaGiocatoriEditor(List<FileDatabaseInfo> DatabaseInfo)
    {
        List<Giocatore> Giocatori = new();
        var infoGiocatori = DatabaseInfo.Where(d => d.TipoDatoDB == TipoDatoDB.GIOCATORE).ToList();

        foreach (var e in infoGiocatori)
        {
            Giocatore g = new();

            g.Id = e.Codice;
            g.Giocabile = e.Dat[4] == 0;
            g.Numero = e.Dat[7];

            var lnc = BitConverter.ToInt16(e.Dat.GetRange(8, 2).ToArray(), 0);

            g.Nome = Utils.DecodificaTesto(e.Dat.GetRange(10, lnc));

            var lnl = BitConverter.ToInt16(e.Dat.GetRange(10 + lnc, 2).ToArray(), 0);
            g.NomeCompleto = Utils.DecodificaTesto(e.Dat.GetRange(12 + lnc, lnl));
            g.Slot = e.Dat[12 + lnc + lnl];
            g.AltriDati = e.Dat[13 + lnc + lnl] == 0;

            g.Ruolo = ((Ruolo)e.Dat[14 + lnc + lnl]);
            var testAltriRuoli = ((Ruolo)e.Dat[15 + lnc + lnl]) == Ruolo.NESSUNO;

            if (testAltriRuoli)
            {
                g.AltriRuoli = new() { Ruolo.NESSUNO };
            }
            else
            {
                for (int r = 2; r <= 6; r++)
                {
                    Ruolo ruolo = ((Ruolo)e.Dat[13 + lnc + lnl + r]);
                    if (ruolo != Ruolo.NESSUNO || !g.AltriRuoli.Contains(Ruolo.NESSUNO)) g.AltriRuoli.Add(ruolo);
                }
            }

            g.Nazione = (Paese)e.Dat[20 + lnc + lnl];

            g.CodColorePelle = (ColorePelle)e.Dat[21 + lnc + lnl];
            g.CodColoreCapelli = (ColoreCapelli)e.Dat[22 + lnc + lnl];
            g.Reparto = (Reparto)e.Dat[23 + lnc + lnl];

            int delta = e.VersioneElemento == 700 ? 0 : 3;

            if (e.VersioneElemento == 800)
            {
                g.CodStileCapelli = (StileCapelli)e.Dat[24 + lnc + lnl];
                g.CodStileBarba = (StileBarba)e.Dat[25 + lnc + lnl];
                g.Nazionalizzato = e.Dat[26 + lnc + lnl] == 1;
            }

            g.GiornoNascita = e.Dat[24 + delta + lnc + lnl];
            g.MeseNascita = e.Dat[25 + delta + lnc + lnl];
            g.AnnoNascita = e.Dat[26 + delta + lnc + lnl] + 256 * e.Dat[27 + delta + lnc + lnl];
            g.Altezza = e.Dat[28 + delta + lnc + lnl];
            g.Peso = e.Dat[29 + delta + lnc + lnl];

            if (g.Giocabile)
            {
                g.PaeseNascita = (Paese)e.Dat[30 + delta + lnc + lnl];
            }

            int offset = e.Dat.Count - 16;
            g.VE = e.Dat[offset];
            g.RE = e.Dat[offset + 1];
            g.AG = e.Dat[offset + 2];
            g.QU = e.Dat[offset + 3];
            g.RI = e.Dat[offset + 4];
            g.DR = e.Dat[offset + 5];
            g.PA = e.Dat[offset + 6];
            g.TI = e.Dat[offset + 7];
            g.EN = e.Dat[offset + 8];
            g.GM = e.Dat[offset + 9];
            g.PPR = (PiedePreferito)e.Dat[offset + 10];
            g.RIG = e.Dat[offset + 11];
            g.CSX = e.Dat[offset + 12];
            g.CDX = e.Dat[offset + 13];
            g.FSX = e.Dat[offset + 14];
            g.FDX = e.Dat[offset + 15];

            Giocatori.Add(g);
        }


        return Giocatori;
    }


}
