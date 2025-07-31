using MultiEditorPCC.Dat.DbSet;
using System.Text;


namespace MultiEditorPCC.Lib.Archivi;

public static partial class FileEditor
{
    public static int Versione { get; set; }

    public static List<Byte> Dati { get; set; } = new();

    public static Squadra LeggiSquadra()
    {
        Squadra sq = new();
        int offset = 0;

        try
        {
            if (Dati == null || Dati.Count == 0 || Dati.First() != 1) { return sq; }
            sq.Id = (uint)BitConverter.ToInt16(Dati.GetRange(2, 2).ToArray(), 0);
            sq.Giocabile = Dati[4] == 1;
            int lncs = Dati[5];
            if (lncs > 0)
            {
                sq.Nome = Encoding.UTF8.GetString(Dati.GetRange(6, lncs).ToArray());
            }
            offset = 6 + lncs;
            int lnls = Dati[offset];
            if (lnls > 0)
            {
                sq.NomeCompleto = Encoding.UTF8.GetString(Dati.GetRange(offset + 1, lnls).ToArray());
            }
            offset += lnls + 1;

            sq.Nazione = (Paese)Dati[offset];
            sq.AnnoFondazione = (ushort)BitConverter.ToInt16(Dati.GetRange(offset + 1, 2).ToArray(), 0);
            sq.Boh = Dati[offset + 3];
            offset += 4;

            int lnp = Dati[offset];

            if (lnp > 0)
            {
                sq.NomePresidente = Encoding.UTF8.GetString(Dati.GetRange(offset + 1, lnp).ToArray());
            }

            offset += lnp + 1;
            sq.CassaGioco = BitConverter.ToInt32(Dati.GetRange(offset, 4).ToArray(), 0);
            sq.CassaReale = BitConverter.ToInt32(Dati.GetRange(offset + 4, 4).ToArray(), 0);
            offset += 8;

            int lnsp = Dati[offset];
            if (lnsp > 0)
            {

                sq.NomeSponsor = Encoding.UTF8.GetString(Dati.GetRange(offset + 1, lnsp).ToArray());
            }
            offset += lnsp + 1;

            int lnst = Dati[offset];
            if (lnst > 0)
            {

                sq.NomeSponsorTecnico = Encoding.UTF8.GetString(Dati.GetRange(offset + 1, lnst).ToArray());
            }
            offset += lnst + 1;

            sq.SquadraRiserve = BitConverter.ToInt16(Dati.GetRange(offset, 2).ToArray(), 0);
            sq.TerzaSquadra = BitConverter.ToInt16(Dati.GetRange(offset + 2, 2).ToArray(), 0);
            sq.Girone2B = (Girone2B)Dati[offset + 4];
            sq.Girone3 = Dati[offset + 5];

            offset += 6;

            sq.StagioniPrecedenti = new();

            for (int sp = 1; sp <= 20; sp++) sq.StagioniPrecedenti.Add(Dati[offset + sp - 1]);

            offset += 20;

            sq.StagioniA = Dati[offset];
            sq.Giocate = BitConverter.ToInt16(Dati.GetRange(offset + 1, 2).ToArray(), 0);
            sq.Vinte = BitConverter.ToInt16(Dati.GetRange(offset + 3, 2).ToArray(), 0);
            sq.Pareggiate = BitConverter.ToInt16(Dati.GetRange(offset + 5, 2).ToArray(), 0);
            sq.GolSegnati = BitConverter.ToInt16(Dati.GetRange(offset + 7, 2).ToArray(), 0);
            sq.GolSubiti = BitConverter.ToInt16(Dati.GetRange(offset + 9, 2).ToArray(), 0);
            sq.PuntiTotali = BitConverter.ToInt16(Dati.GetRange(offset + 11, 2).ToArray(), 0);
            sq.Scudetti = Dati[offset + 13];
            sq.SecondiPosti = Dati[offset + 14];
            sq.PosizioniCampionato = new();
            for (int pc = 15; pc < 61; pc++) sq.PosizioniCampionato.Add(Dati[offset + pc]);
            offset += 61;

            sq.LTrofeiPalmares = Dati[offset];
            sq.Palmares = new();
            for (int p = 1; p <= sq.LTrofeiPalmares * 3; p++) sq.Palmares.Add(Dati[offset + p]);
            offset += sq.LTrofeiPalmares * 3 + 1;

            int lt = BitConverter.ToInt16(Dati.GetRange(offset, 4).ToArray(), 0);
            offset += 4;
            sq.Tattica = new();
            sq.Tattica.TatticaCompleta = new();

            for (int t = 0; t < lt; t++) sq.Tattica.TatticaCompleta.Add(Dati[offset + t]);

            offset += lt;

            sq.Tattica.PercentualeToccoDiPrima = Dati[offset];
            sq.Tattica.PercentualeContropiede = Dati[offset + 1];
            sq.Tattica.TipoAttacco = (TipoAttacco)Dati[offset + 2];
            sq.Tattica.TipoEntrata = (TipoEntrata)Dati[offset + 3];
            sq.Tattica.TipoMarcatura = (TipoMarcatura)Dati[offset + 4];
            sq.Tattica.TipoRinvii = (TipoRinvii)Dati[offset + 5];
            sq.Tattica.PressingDa = (PressingDa)Dati[offset + 6];

            offset += 7;

            var temp = Dati;

            int lst = Dati[offset];

            Dati = temp.GetRange(offset + 1, lst);

            sq.Stadio = LeggiStadio();

            offset += lst + 1;

            Dati = temp;

            int na = Dati[offset];
            offset++;

            sq.Allenatori = new();

            for (int a = 1; a <= na; a++)
            {
                int la = BitConverter.ToInt32(Dati.GetRange(offset, 4).ToArray(), 0);
                Dati = temp.GetRange(offset + 4, la);
                sq.Allenatori.Add(LeggiAllenatore());
                offset += la + 4;
                Dati = temp;
            }

            int ng = Dati[offset];
            offset++;

            sq.Giocatori = new();

            for (int g = 1; g <= ng; g++)
            {
                int lg = BitConverter.ToInt32(Dati.GetRange(offset, 4).ToArray(), 0);
                Dati = temp.GetRange(offset + 4, lg);
                sq.Giocatori.Add(LeggiGiocatore());
                offset += lg + 4;
                Dati = temp;
            }
        }
        catch (Exception)
        {
            return sq;
        }

        return sq;
    }

    public static Giocatore LeggiGiocatore()
    {
        Giocatore g = new();
        int offset = 0;

        try
        {
            if (Dati == null || Dati.Count == 0 || Dati.First() != 2) { return g; }
            g.Id = (int)BitConverter.ToInt16(Dati.GetRange(2, 2).ToArray(), 0);
            int lng = Dati[4];
            if (lng > 0)
            {
                g.Nome = Encoding.UTF8.GetString(Dati.GetRange(5, lng).ToArray());
            }
            offset = 5 + lng;
            int lncg = Dati[offset];
            if (lncg > 0)
            {
                g.NomeCompleto = Encoding.UTF8.GetString(Dati.GetRange(offset + 1, lncg).ToArray());
            }
            offset += 1 + lncg;

            g.Giocabile = Dati[offset] == 1;
            g.Numero = Dati[offset + 1];
            g.Slot = Dati[offset + 2];
            g.AltriDati = Dati[offset + 3] == 1;
            g.AttivoInRosa = Dati[offset + 4] == 1;
            offset += 5;

            g.Ruoli = new();
            g.Ruoli.Add((Ruolo)Dati[offset]);
            g.Ruoli.Add((Ruolo)Dati[offset + 1]);
            g.Ruoli.Add((Ruolo)Dati[offset + 2]);
            g.Ruoli.Add((Ruolo)Dati[offset + 3]);
            g.Ruoli.Add((Ruolo)Dati[offset + 4]);
            g.Ruoli.Add((Ruolo)Dati[offset + 5]);

            g.Nazione = (Paese)Dati[offset + 6];

            offset += 7;

            //In questa prima versione, tengo come 
            //"segnaposto" per eventuale futuro 
            //utilizzo se ci fossero diverse tabelle dei Paesi
            //nelle prime versioni del gioco
            //(credo che prima di PCC5 i codici Paese fossero diversi)
            //In questa versione 1, lo ignoro in lettura
            //offset++;

            //g.PaeseNascita = (Paese)Dati[offset];
            g.codColorePelle = (ColorePelle)Dati[offset + 1];
            g.codColoreCapelli = (ColoreCapelli)Dati[offset + 2];
            g.codStileCapelli = (StileCapelli)Dati[offset + 3];
            g.codStileBarba = (StileBarba)Dati[offset + 4];
            g.Nazionalizzato = Dati[offset + 5] == 1;
            g.Reparto = (Reparto)Dati[offset + 6];
            g.GiornoNascita = Dati[offset + 7];
            g.MeseNascita = Dati[offset + 8];
            g.AnnoNascita = Dati[offset + 9];
            g.AnnoNascita += Dati[offset + 10] * 256;
            g.Altezza = Dati[offset + 11];
            g.Peso = Dati[offset + 12];
            g.PaeseNascita = (Paese)Dati[offset + 13];

            offset += 14;

            g.Testi = new();

            if (Dati[offset] == 0)
            {
                offset++;
            }
            else
            {
                int numeroTesti = Dati[offset];

                for (int t = 1; t <= numeroTesti; t++)
                {
                    int ltesto = BitConverter.ToInt32(Dati.GetRange(Dati[offset + 1], 4).ToArray());

                    g.Testi.Add(Encoding.UTF8.GetString(Dati.GetRange(offset + 5, ltesto).ToArray()));

                    offset += 4 + ltesto;

                }
            }

            g.Punteggi[1].Punteggio = Dati[Dati.Count - 16];
            g.Punteggi[2].Punteggio = Dati[Dati.Count - 15];
            g.Punteggi[3].Punteggio = Dati[Dati.Count - 14];
            g.Punteggi[4].Punteggio = Dati[Dati.Count - 13];

            int m = (int)Math.Truncate((decimal)(g.Punteggi[1].Punteggio + g.Punteggi[2].Punteggio + g.Punteggi[3].Punteggio + g.Punteggi[4].Punteggio) / 4);

            g.Punteggi[0].Punteggio = m;


            g.Punteggi[5].Punteggio = Dati[Dati.Count - 12];
            g.Punteggi[6].Punteggio = Dati[Dati.Count - 11];
            g.Punteggi[7].Punteggio = Dati[Dati.Count - 10];
            g.Punteggi[8].Punteggio = Dati[Dati.Count - 9];
            g.Punteggi[9].Punteggio = Dati[Dati.Count - 8];
            g.Punteggi[10].Punteggio = Dati[Dati.Count - 7];
            g.Punteggi[11].Punteggio = Dati[Dati.Count - 6];
            g.Punteggi[12].Punteggio = Dati[Dati.Count - 5];
            g.Punteggi[13].Punteggio = Dati[Dati.Count - 4];
            g.Punteggi[14].Punteggio = Dati[Dati.Count - 3];
            g.Punteggi[15].Punteggio = Dati[Dati.Count - 2];
            g.Punteggi[16].Punteggio = Dati[Dati.Count - 1];

        }

        catch (Exception)
        {
            return g;
        }

        return g;
    }

    public static Allenatore LeggiAllenatore()
    {
        Allenatore a = new();

        int offset = 0;

        try
        {
            if (Dati == null || Dati.Count == 0 || Dati.First() != 3) { return a; }
            a.Id = (uint)BitConverter.ToInt16(Dati.GetRange(2, 2).ToArray(), 0);
            int lna = Dati[4];
            if (lna > 0)
            {
                a.Nome = Encoding.UTF8.GetString(Dati.GetRange(5, lna).ToArray());
            }
            offset = 5 + lna;
            a.Giocabile = Dati[offset] == 1;
            int lnca = Dati[offset + 1];
            offset += 2;
            if (lnca > 0)
            {
                a.NomeCompleto = Encoding.UTF8.GetString(Dati.GetRange(offset, lnca).ToArray());
            }
            offset += lnca;
            a.exGiocatore = Dati[offset] == 1;
            int nt = Dati[offset + 1];
            offset += 2;

            a.Testi = new();

            for (int i = 1; i <= nt; i++)
            {
                int lt = BitConverter.ToInt16(Dati.GetRange(offset, 4).ToArray(), 0);
                a.Testi.Add(Encoding.UTF8.GetString(Dati.GetRange(4, lt).ToArray()));
                offset += lt + 4;
            }

        }
        catch (Exception)
        {
            return a;
        }

        return a;
    }

    public static Stadio LeggiStadio()
    {
        Stadio st = new();
        int offset = 0;

        try
        {
            if (Dati == null || Dati.Count == 0 || Dati.First() != 4) { return st; }
            st.Id = (uint)BitConverter.ToInt16(Dati.GetRange(2, 2).ToArray(), 0);
            int lns = Dati[4];
            if (lns > 0)
            {
                st.Nome = Encoding.UTF8.GetString(Dati.GetRange(5, lns).ToArray());
            }
            offset = 5 + lns;
            st.Giocabile = Dati[offset] == 1;

            st.Capienza = BitConverter.ToInt32(Dati.GetRange(offset + 1, 4).ToArray(), 0);
            st.PostiInPiedi = BitConverter.ToInt32(Dati.GetRange(offset + 5, 4).ToArray(), 0);
            offset += 10;
            st.Larghezza = Dati[offset];
            st.Lunghezza = Dati[offset + 1];
            st.Nazione = (Paese)Dati[offset + 2];
            st.NumeroBoh = Dati[offset + 3];
            st.AnnoCostruzione = BitConverter.ToInt16(Dati.GetRange(offset + 4, 2).ToArray(), 0);
        }
        catch (Exception)
        {
            return st;
        }

        return st;
    }

    public static List<Byte> ScriviSquadra(Squadra squadra)
    {
        Dati.Add(1);
        Dati.Add((byte)Versione);

        Dati.Add((byte)(squadra.Id % 256));
        Dati.Add((byte)((squadra.Id - (squadra.Id % 256)) / 256));

        int giocabile = squadra.Giocabile ? 1 : 0;
        Dati.Add((byte)giocabile);

        Dati.Add((byte)squadra.Nome.Length);
        if (squadra.Nome.Length > 0)
        {
            Dati.AddRange(Encoding.UTF8.GetBytes(squadra.Nome));
        }
        Dati.Add((byte)squadra.NomeCompleto.Length);
        if (squadra.NomeCompleto.Length > 0)
        {
            Dati.AddRange(Encoding.UTF8.GetBytes(squadra.NomeCompleto));
        }

        Dati.Add((byte)squadra.Nazione);

        Dati.Add((byte)(squadra.AnnoFondazione % 256));
        if (squadra.AnnoFondazione < 2048)
        {
            Dati.Add(7);
        }
        else
        {
            Dati.Add(8);
        }

        Dati.Add((byte)squadra.Boh);

        int n = (int)squadra.NumeroAbbonati!;

        Dati.AddRange(BitConverter.GetBytes(n));

        Dati.Add((byte)squadra.NomePresidente!.Length);
        if (squadra.NomePresidente.Length > 0)
        {
            Dati.AddRange(Encoding.UTF8.GetBytes(squadra.NomePresidente));
        }

        Dati.AddRange(BitConverter.GetBytes((int)squadra.CassaGioco!));

        Dati.AddRange(BitConverter.GetBytes((int)squadra.CassaReale!));

        Dati.Add((byte)squadra.NomeSponsor!.Length);
        if (squadra.NomeSponsor.Length > 0)
        {
            Dati.AddRange(Encoding.UTF8.GetBytes(squadra.NomeSponsor));
        }

        Dati.Add((byte)squadra.NomeSponsorTecnico!.Length);
        if (squadra.NomeSponsorTecnico.Length > 0)
        {
            Dati.AddRange(Encoding.UTF8.GetBytes(squadra.NomeSponsorTecnico));
        }


        Dati.Add((byte)((int)squadra.SquadraRiserve! % 256));
        Dati.Add((byte)((squadra.SquadraRiserve - (squadra.SquadraRiserve % 256)) / 256));
        Dati.Add((byte)((int)squadra.TerzaSquadra! % 256));
        Dati.Add((byte)((squadra.TerzaSquadra - (squadra.TerzaSquadra % 256)) / 256));
        Dati.Add((byte)squadra.Girone2B);
        Dati.Add((byte)squadra.Girone3);
        foreach (var sp in squadra.StagioniPrecedenti) Dati.Add((byte)sp);
        Dati.Add((byte)squadra.StagioniA);
        Dati.Add((byte)(squadra.Giocate % 256));
        Dati.Add((byte)((squadra.Giocate - (squadra.Giocate % 256)) / 256));
        Dati.Add((byte)(squadra.Vinte % 256));
        Dati.Add((byte)((squadra.Vinte - (squadra.Vinte % 256)) / 256));
        Dati.Add((byte)(squadra.Pareggiate % 256));
        Dati.Add((byte)((squadra.Pareggiate - (squadra.Pareggiate % 256)) / 256));
        Dati.Add((byte)(squadra.GolSegnati % 256));
        Dati.Add((byte)((squadra.GolSegnati - (squadra.GolSegnati % 256)) / 256));
        Dati.Add((byte)(squadra.GolSubiti % 256));
        Dati.Add((byte)((squadra.GolSubiti - (squadra.GolSubiti % 256)) / 256));
        Dati.Add((byte)(squadra.PuntiTotali % 256));
        Dati.Add((byte)((squadra.PuntiTotali - (squadra.PuntiTotali % 256)) / 256));
        Dati.Add((byte)squadra.Scudetti);
        Dati.Add((byte)squadra.SecondiPosti);
        foreach (var pc in squadra.PosizioniCampionato) Dati.Add((byte)pc);
        Dati.Add((byte)squadra.LTrofeiPalmares);
        Dati.AddRange(squadra.Palmares);
        Dati.AddRange(BitConverter.GetBytes(squadra.Tattica.TatticaCompleta.Count));
        Dati.AddRange(squadra.Tattica.TatticaCompleta);
        Dati.Add((byte)squadra.Tattica.PercentualeToccoDiPrima);
        Dati.Add((byte)squadra.Tattica.PercentualeContropiede);
        Dati.Add((byte)squadra.Tattica.TipoAttacco);
        Dati.Add((byte)squadra.Tattica.TipoEntrata);
        Dati.Add((byte)squadra.Tattica.TipoMarcatura);
        Dati.Add((byte)squadra.Tattica.TipoRinvii);
        Dati.Add((byte)squadra.Tattica.PressingDa);
        var infoStadio = ScriviStadio(squadra.Stadio);
        Dati.Add((byte)infoStadio.Count);
        Dati.AddRange(infoStadio);
        Dati.Add((byte)squadra.Allenatori.Count);
        foreach (var allenatore in squadra.Allenatori)
        {
            var infoAllenatore = ScriviAllenatore(allenatore);
            Dati.AddRange(BitConverter.GetBytes(infoAllenatore.Count));
            Dati.AddRange(infoAllenatore);
        }

        Dati.Add((byte)squadra.Giocatori.Count);
        foreach (var giocatore in squadra.Giocatori)
        {
            var infoGiocatore = ScriviGiocatore(giocatore);
            Dati.AddRange(BitConverter.GetBytes(infoGiocatore.Count));
            Dati.AddRange(infoGiocatore);
        }
        return Dati;
    }

    public static List<Byte> ScriviGiocatore(Giocatore giocatore)
    {
        Dati.Add(2);
        Dati.Add((byte)Versione);
        Dati.Add((byte)(giocatore.Id % 256));
        Dati.Add((byte)((giocatore.Id - (giocatore.Id % 256)) / 256));

        var sb = Encoding.UTF8.GetBytes(giocatore.Nome);
        Dati.Add((byte)sb.Length);
        if (sb.Length > 0)
        {
            Dati.AddRange(sb);
        }
        sb = Encoding.UTF8.GetBytes(giocatore.NomeCompleto);
        Dati.Add((byte)sb.Length);
        if (sb.Length > 0)
        {
            Dati.AddRange(sb);
        }

        int giocabile = giocatore.Giocabile ? 1 : 0;
        Dati.Add((byte)giocabile);
        Dati.Add((byte)giocatore.Numero);
        Dati.Add((byte)giocatore.Slot);
        int ad = giocatore.AltriDati ? 1 : 0;
        Dati.Add((byte)ad);
        int attivo = giocatore.AttivoInRosa ? 1 : 0;
        Dati.Add((byte)attivo);
        Dati.Add((byte)giocatore.Ruoli[0]);
        Dati.Add((byte)giocatore.Ruoli[1]);
        Dati.Add((byte)giocatore.Ruoli[2]);
        Dati.Add((byte)giocatore.Ruoli[3]);
        Dati.Add((byte)giocatore.Ruoli[4]);
        Dati.Add((byte)giocatore.Ruoli[5]);
        Dati.Add((byte)giocatore.Nazione);
        //In questa prima versione, tengo come 
        //"segnaposto" per eventuale futuro 
        //utilizzo se ci fossero diverse tabelle dei Paesi
        //nelle prime versioni del gioco
        //(credo che prima di PCC5 i codici Paese fossero diversi)
        //In questa versione 1, lo ignoro in lettura
        Dati.Add((byte)giocatore.PaeseNascita);
        Dati.Add((byte)giocatore.codColorePelle);
        Dati.Add((byte)giocatore.codColoreCapelli);
        Dati.Add((byte)giocatore.codStileCapelli);
        Dati.Add((byte)giocatore.codStileBarba);
        int nazionalizzato = giocatore.Nazionalizzato ? 1 : 0;
        Dati.Add((byte)nazionalizzato);
        Dati.Add((byte)giocatore.Reparto);
        Dati.Add((byte)giocatore.GiornoNascita);
        Dati.Add((byte)giocatore.MeseNascita);
        Dati.Add((byte)(giocatore.AnnoNascita % 256));
        if (giocatore.AnnoNascita < 2048)
        {
            Dati.Add(7);
        }
        else
        {
            Dati.Add(8);
        }

        Dati.Add((byte)giocatore.Altezza);
        Dati.Add((byte)giocatore.Peso);
        Dati.Add((byte)giocatore.PaeseNascita);

        if (!giocatore.Testi.Any())
        {
            Dati.Add(0);
        }
        else
        {
            Dati.Add((byte)giocatore.Testi.Count);

            foreach (var testo in giocatore.Testi)
            {

                Dati.AddRange(BitConverter.GetBytes(testo.Length));
                Dati.AddRange(Encoding.UTF8.GetBytes(testo));
            }

        }

        Dati.Add((byte)giocatore.Punteggi[1].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[2].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[3].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[4].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[5].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[6].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[7].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[8].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[9].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[10].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[11].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[12].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[13].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[14].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[15].Punteggio);
        Dati.Add((byte)giocatore.Punteggi[16].Punteggio);


        return Dati;
    }


    public static List<Byte> ScriviAllenatore(Allenatore allenatore)
    {
        Dati.Add(3);
        Dati.Add((byte)Versione);
        Dati.Add((byte)(allenatore.Id % 256));
        Dati.Add((byte)((allenatore.Id - (allenatore.Id % 256)) / 256));

        var sb = Encoding.UTF8.GetBytes(allenatore.Nome);
        Dati.Add((byte)sb.Length);
        if (sb.Length > 0)
        {
            Dati.AddRange(sb);
        }

        int giocabile = allenatore.Giocabile ? 1 : 0;
        Dati.Add((byte)giocabile);

        sb = Encoding.UTF8.GetBytes(allenatore.NomeCompleto);
        Dati.Add((byte)sb.Length);
        if (sb.Length > 0)
        {
            Dati.AddRange(sb);
        }


        int exGiocatore = allenatore.exGiocatore ? 1 : 0;
        Dati.Add((byte)exGiocatore);

        if (!allenatore.Testi.Any())
        {
            Dati.Add(0);
        }
        else
        {
            Dati.Add((byte)allenatore.Testi.Count);
            List<int> lunghezzeTesti = new();
            foreach (var testo in allenatore.Testi)
            {

                Dati.AddRange(BitConverter.GetBytes(testo.Length));
                Dati.AddRange(Encoding.UTF8.GetBytes(testo));
            }

        }

        return Dati;
    }

    public static List<Byte> ScriviStadio(Stadio stadio)
    {
        Dati.Add(4);
        Dati.Add((byte)Versione);
        Dati.Add((byte)(stadio.Id % 256));
        Dati.Add((byte)((stadio.Id - (stadio.Id % 256)) / 256));
        Dati.Add((byte)stadio.Nome.Length);
        if (stadio.Nome.Length > 0)
        {
            Dati.AddRange(Encoding.UTF8.GetBytes(stadio.Nome));
        }
        int giocabile = stadio.Giocabile ? 1 : 0;
        Dati.Add((byte)giocabile);

        Dati.AddRange(BitConverter.GetBytes(stadio.Capienza));
        Dati.AddRange(BitConverter.GetBytes(stadio.PostiInPiedi));
        Dati.Add((byte)(stadio.Larghezza % 256));
        Dati.Add((byte)(stadio.Lunghezza % 256));
        Dati.Add((byte)stadio.Nazione);
        Dati.Add((byte)stadio.NumeroBoh);
        Dati.Add((byte)(stadio.AnnoCostruzione % 256));
        if (stadio.AnnoCostruzione < 2048)
        {
            Dati.Add(7);
        }
        else
        {
            Dati.Add(8);
        }
        return Dati;
    }

}
