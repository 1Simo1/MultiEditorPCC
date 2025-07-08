using MultiEditorPCC.Dat.DbSet;


namespace MultiEditorPCC.Lib.Archivi;

public static partial class DBC
{
    public static int Versione { get; set; }

    public static List<Byte> dati { get; set; }

    public static String nomeSquadra { get; set; } = String.Empty;

    public static int idSquadra { get; set; } = 0;

    public static bool squadraGiocabile { get; set; } = false;

    public static Squadra LeggiSquadra(ElementoArchivio elemento)
    {
        Squadra sq = new();

        try
        {
            dati = elemento.Dat;
            sq.Id = uint.Parse(elemento.Nome.Substring(4, elemento.Nome.Length - 8));
            sq.Boh = BitConverter.ToInt16(dati.GetRange(36, 2).ToArray(), 0);
            Versione = BitConverter.ToInt16(dati.GetRange(38, 2).ToArray(), 0);

            sq.Giocabile = dati[41] == 0;

            var lnc = BitConverter.ToInt16(dati.GetRange(42, 2).ToArray(), 0);

            sq.Nome = Utils.decodificaTesto(dati.GetRange(44, lnc));

            idSquadra = (int)sq.Id;
            nomeSquadra = sq.Nome;
            squadraGiocabile = sq.Giocabile;

            sq.Stadio = LeggiStadio(elemento);

            sq.Nazione = sq.Stadio.Nazione;
            int offset = 47 + lnc + sq.Stadio.Nome.Length;
            if (Versione == 525) offset++;

            var lnl = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            sq.NomeCompleto = Utils.decodificaTesto(dati.GetRange(offset + 2, lnl));
            offset += lnl + 10;
            if (Versione == 525 || Versione == 510) offset += 4;
            sq.AnnoFondazione = (ushort)BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            offset += 2;
            if (sq.Giocabile)
            {
                offset += 2;
                sq.NumeroAbbonati = BitConverter.ToInt32(dati.GetRange(offset, 4).ToArray(), 0);
                offset += 4;
                var lnp = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                sq.NomePresidente = Utils.decodificaTesto(dati.GetRange(offset + 2, lnp));
                offset += 2 + lnp;
                sq.CassaGioco = BitConverter.ToInt32(dati.GetRange(offset, 4).ToArray(), 0);
                sq.CassaReale = BitConverter.ToInt32(dati.GetRange(offset + 4, 4).ToArray(), 0);
                offset += 8;
                var lnsp = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                sq.NomeSponsor = Utils.decodificaTesto(dati.GetRange(offset + 2, lnsp));
                offset += 2 + lnsp;
                var lnspt = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                sq.NomeSponsorTecnico = Utils.decodificaTesto(dati.GetRange(offset + 2, lnspt));
                offset += 2 + lnspt;
                sq.SquadraRiserve = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                sq.TerzaSquadra = BitConverter.ToInt16(dati.GetRange(offset + 2, 2).ToArray(), 0);
                offset += 4;
                sq.Girone2B = (Girone2B)dati[offset];
                sq.StagioniPrecedenti = new();
                for (int sp = 1; sp <= 20; sp++) sq.StagioniPrecedenti.Add(dati[offset + sp]);
                offset += 21;
                sq.StagioniA = dati[offset];
                sq.Giocate = dati[offset + 1] + dati[offset + 2] * 256;
                sq.Vinte = dati[offset + 3] + dati[offset + 4] * 256;
                sq.Pareggiate = dati[offset + 5] + dati[offset + 6] * 256;
                sq.GolSegnati = dati[offset + 7] + dati[offset + 8] * 256;
                sq.GolSubiti = dati[offset + 9] + dati[offset + 10] * 256;
                sq.PuntiTotali = dati[offset + 11] + dati[offset + 12] * 256;
                sq.Scudetti = dati[offset + 13];
                sq.SecondiPosti = dati[offset + 14];
                offset += 15;
                sq.PosizioniCampionato = new();
                for (int ps = 0; ps <= 45; ps++) sq.PosizioniCampionato.Add(dati[offset + ps]);
                offset += sq.PosizioniCampionato.Count;
                sq.LTrofeiPalmares = 0;
                if (Versione == 525)
                {
                    sq.LTrofeiPalmares = 42;
                }
                else if (Versione >= 505) sq.LTrofeiPalmares = 34;

                sq.Palmares = dati.GetRange(offset, sq.LTrofeiPalmares).ToList();
                offset += sq.LTrofeiPalmares;

            }

            sq.Tattica = new();
            sq.Tattica.TatticaCompleta = dati.GetRange(offset, 176);
            offset += 176;
            sq.Tattica.PercentualeToccoDiPrima = dati[offset];
            sq.Tattica.PercentualeContropiede = dati[offset + 1];
            sq.Tattica.TipoAttacco = (TipoAttacco)dati[offset + 2];
            sq.Tattica.TipoEntrata = (TipoEntrata)dati[offset + 3];
            sq.Tattica.TipoMarcatura = (TipoMarcatura)dati[offset + 4];
            sq.Tattica.TipoRinvii = (TipoRinvii)dati[offset + 5];
            sq.Tattica.PressingDa = (PressingDa)dati[offset + 6];
            offset += 7;
            dati.RemoveRange(0, offset);
            elemento.Dat = dati;
            int valoreControlloBloccoDati = -1;
            sq.Allenatori = new();
            sq.Giocatori = new();
            while (elemento.Dat.Count > 0)
            {
                valoreControlloBloccoDati = elemento.Dat[0];
                if (valoreControlloBloccoDati != 1 &&
                    valoreControlloBloccoDati != 2
                    )
                {
                    elemento.Dat.Clear();
                }
                else
                {

                    if (valoreControlloBloccoDati == 1)
                    {
                        Giocatore g = LeggiGiocatore(elemento);
                        int dimOffset = int.Parse(g.Testi.Last());
                        g.Testi.Remove(g.Testi.Last());
                        sq.Giocatori.Add(g);
                        elemento.Dat.RemoveRange(0, dimOffset);
                    }


                    if (valoreControlloBloccoDati == 2)
                    {
                        Allenatore a = LeggiAllenatore(elemento);
                        int dimOffset = int.Parse(a.Testi.Last());
                        a.Testi.Remove(a.Testi.Last());
                        sq.Allenatori.Add(a);
                        elemento.Dat.RemoveRange(0, dimOffset);
                    }
                }

            }


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
        if (elemento.Dat.First() != 1) return new();
        int offset = 1;
        g.Squadra = nomeSquadra;
        g.CodiceSquadra = idSquadra;
        g.AttivoInRosa = true;

        try
        {
            dati = elemento.Dat;
            g.Id = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            g.Numero = dati[offset + 2];
            offset += 3;
            var lnc = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            g.Nome = Utils.decodificaTesto(dati.GetRange(offset + 2, lnc));
            offset += 2 + lnc;
            var lnl = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            g.NomeCompleto = Utils.decodificaTesto(dati.GetRange(offset + 2, lnl));
            offset += 2 + lnl;
            g.Slot = dati[offset];
            offset++;
            g.AltriDati = dati[offset] == 1;
            offset++;
            g.Ruoli = new() {
                (Ruolo)dati[offset],
                (Ruolo)dati[offset+1],
                (Ruolo)dati[offset+2],
                (Ruolo)dati[offset+3],
                (Ruolo)dati[offset+4],
                (Ruolo)dati[offset+5],
            };

            offset += 6;
            g.Nazione = (Paese)dati[offset];
            g.codColorePelle = (ColorePelle)dati[offset + 1];
            g.codColoreCapelli = (ColoreCapelli)dati[offset + 2];
            g.Reparto = (Reparto)dati[offset + 3];
            offset += 4;
            g.GiornoNascita = dati[offset];
            g.MeseNascita = dati[offset + 1];
            g.AnnoNascita = dati[offset + 2] + dati[offset + 3] * 256;
            g.Altezza = dati[offset + 4];
            g.Peso = dati[offset + 5];
            offset += 6;
            g.Testi = new();

            if (squadraGiocabile)
            {
                g.PaeseNascita = (Paese)dati[offset];
                offset++;
                int lt = 0;
                for (int x = 1; x <= 10; x++)
                {
                    lt = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                    g.Testi.Add(Utils.decodificaTesto(dati.GetRange(offset + 2, lt)));
                    offset += 2 + lt;
                }
            }


            int m = (int)Math.Truncate((decimal)(dati[offset + 0] + dati[offset + 1] + dati[offset + 2] + dati[offset + 3]) / 4);

            g.Punteggi[0].Punteggio = m;
            for (int i = 1; i <= 10; i++) g.Punteggi[i].Punteggio = dati[offset + i - 1];

            offset += 10;

            g.Testi.Add(offset.ToString());

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
        if (elemento.Dat.First() != 2) return new();
        int offset = 1;

        try
        {
            dati = elemento.Dat;
            a.Id = (uint)BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            var lnc = BitConverter.ToInt16(dati.GetRange(offset + 2, 2).ToArray(), 0);
            a.Nome = Utils.decodificaTesto(dati.GetRange(offset + 4, lnc));
            offset += 4 + lnc;
            a.Testi = new();

            if (squadraGiocabile)
            {
                var lnla = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                a.NomeCompleto = Utils.decodificaTesto(dati.GetRange(offset + 2, lnla));
                offset += 2 + lnla;
                int lt = 0;
                for (int x = 1; x <= 6; x++)
                {
                    lt = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                    a.Testi.Add(Utils.decodificaTesto(dati.GetRange(offset + 2, lt)));
                    offset += 2 + lt;
                }
                a.exGiocatore = dati[offset] == 3;
                offset++;
                if (a.exGiocatore)
                {
                    lt = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                    a.Testi.Add(Utils.decodificaTesto(dati.GetRange(offset + 2, lt)));
                    offset += 2 + lt;
                }
                lt = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                a.Testi.Add(Utils.decodificaTesto(dati.GetRange(offset + 2, lt)));
                offset += 2 + lt;
            }

            a.Testi.Add(offset.ToString());
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
            st.Id = (uint)idSquadra;
            dati = elemento.Dat;
            int offset = 44 + nomeSquadra.Length;
            int lnst = dati[offset];
            offset += 2;
            st.Nome = Utils.decodificaTesto(dati.GetRange(offset, lnst));
            offset += lnst;
            st.Nazione = (Paese)dati[offset];
            offset++;
            if (Versione == 525) offset++;
            var lnlsq = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            offset += lnlsq + 2;
            st.Capienza = BitConverter.ToInt32(dati.GetRange(offset, 4).ToArray(), 0);
            offset += 4;
            st.PostiInPiedi = 0;
            if (Versione == 525 || Versione == 510)
            {
                st.PostiInPiedi = BitConverter.ToInt32(dati.GetRange(offset, 4).ToArray(), 0);
                offset += 4;
            }

            st.Larghezza = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
            st.Lunghezza = BitConverter.ToInt16(dati.GetRange(offset + 2, 2).ToArray(), 0);
            offset += 6;
            if (squadraGiocabile && Versione == 525)
            {
                st.AnnoCostruzione = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
                offset += 2;
            }

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
