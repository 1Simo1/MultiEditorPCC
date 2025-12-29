using MultiEditorPCC.Dat.DbSet;


namespace MultiEditorPCC.Lib.Archivi;

public static partial class FDI
{
    public static int Versione { get; set; }

    public static List<Byte> dati { get; set; } = new();

    public static List<ElementoArchivio> elementi { get; set; } = new();

    public static Squadra LeggiSquadra(ElementoArchivio elemento)
    {
        Squadra sq = new();

        try
        {
            //var lt = BitConverter.ToInt16(dati.GetRange(36, 2).ToArray(), 0);

            var vfdi = BitConverter.ToInt16(dati.GetRange(38, 2).ToArray(), 0);

            //TODO Commento if temporaneo per test 7+ ITA/SPA, decommentare successivamente
            //if (vfdi != Versione) return sq;

            sq.Id = (uint)elemento.Codice;
            sq.Giocabile = dati[41] == 0;

            var lnc = BitConverter.ToInt16(dati.GetRange(42, 2).ToArray(), 0);

            sq.Nome = Utils.decodificaTesto(dati.GetRange(44, lnc));

            int offset = 0;

            if (Versione == 800)
            {
                sq.Stadio = new()
                {
                    Id = (uint)BitConverter.ToInt16(dati.GetRange(44 + lnc, 2).ToArray(), 0)
                };

                sq.Nazione = (Paese)dati[44 + lnc + 2];
                var lnl = BitConverter.ToInt16(dati.GetRange(47 + lnc, 2).ToArray(), 0);
                sq.NomeCompleto = Utils.decodificaTesto(dati.GetRange(49 + lnc, lnl));
                sq.AnnoFondazione = (ushort)BitConverter.ToInt16(dati.GetRange(49 + lnc + lnl, 2).ToArray(), 0);
                sq.Boh = dati[51 + lnc + lnl];
                offset = 52 + lnc + lnl;
                if (sq.Giocabile)
                {
                    sq.NumeroAbbonati = BitConverter.ToInt32(dati.GetRange(52 + lnc + lnl, 4).ToArray(), 0);

                    var lnp = BitConverter.ToInt16(dati.GetRange(56 + lnc + lnl, 2).ToArray(), 0);
                    sq.NomePresidente = Utils.decodificaTesto(dati.GetRange(58 + lnc + lnl, lnp));
                    sq.CassaGioco = BitConverter.ToInt32(dati.GetRange(58 + lnc + lnl + lnp, 4).ToArray(), 0);
                    sq.CassaReale = BitConverter.ToInt32(dati.GetRange(62 + lnc + lnl + lnp, 4).ToArray(), 0);

                    var lnsp = BitConverter.ToInt16(dati.GetRange(66 + lnc + lnl + lnp, 2).ToArray(), 0);
                    sq.NomeSponsor = Utils.decodificaTesto(dati.GetRange(68 + lnc + lnl + lnp, lnsp));
                    var lnst = BitConverter.ToInt16(dati.GetRange(68 + lnc + lnl + lnp + lnsp, 2).ToArray(), 0);
                    sq.NomeSponsorTecnico = Utils.decodificaTesto(dati.GetRange(70 + lnc + lnl + lnp + lnsp, lnst));
                    sq.SquadraRiserve = BitConverter.ToInt16(dati.GetRange(70 + lnc + lnl + lnp + lnsp + lnst, 2).ToArray(), 0);
                    sq.Girone2B = (Girone2B)dati[72 + lnc + lnl + lnp + lnsp + lnst];
                    sq.Girone3 = dati[73 + lnc + lnl + lnp + lnsp + lnst];

                    offset = 74 + lnc + lnl + lnp + lnsp + lnst;

                    sq.StagioniPrecedenti = new();

                    for (int sp = offset; sp < offset + 20; sp++) sq.StagioniPrecedenti.Add(dati[sp]);

                    sq.StagioniA = dati[offset + 20];
                    sq.Giocate = BitConverter.ToInt16(dati.GetRange(offset + 21, 2).ToArray(), 0);
                    sq.Vinte = BitConverter.ToInt16(dati.GetRange(offset + 23, 2).ToArray(), 0);
                    sq.Pareggiate = BitConverter.ToInt16(dati.GetRange(offset + 25, 2).ToArray(), 0);
                    sq.GolSegnati = BitConverter.ToInt16(dati.GetRange(offset + 27, 2).ToArray(), 0);
                    sq.GolSubiti = BitConverter.ToInt16(dati.GetRange(offset + 29, 2).ToArray(), 0);
                    sq.PuntiTotali = BitConverter.ToInt16(dati.GetRange(offset + 31, 2).ToArray(), 0);
                    sq.Scudetti = dati[offset + 33];
                    sq.SecondiPosti = dati[offset + 34];
                    sq.PosizioniCampionato.Clear();
                    for (int sp = offset + 35; sp < offset + 81; sp++) sq.PosizioniCampionato.Add(dati[sp]);
                    var ilp = dati[offset + 81];
                    sq.LTrofeiPalmares = ilp;
                    for (int sp = offset + 82; sp < offset + 82 + (ilp * 3); sp++) sq.Palmares.Add(dati[sp]);
                    offset = offset + 82 + (ilp * 3);
                }
            }
            else
            {
                sq.Stadio = new()
                {
                    Id = (uint)elemento.Codice
                };

                var lns = BitConverter.ToInt16(dati.GetRange(44 + lnc, 2).ToArray(), 0);

                sq.Nazione = (Paese)dati[44 + lnc + lns + 2];
                sq.Boh = dati[47 + lnc + lns];
                var lnl = BitConverter.ToInt16(dati.GetRange(48 + lnc + lns, 2).ToArray(), 0);
                sq.NomeCompleto = Utils.decodificaTesto(dati.GetRange(50 + lnc + lns, lnl));

                offset = 62 + lnc + lns + lnl;

                sq.AnnoFondazione = (ushort)BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);

                offset = 64 + lnc + lns + lnl;

                if (sq.Giocabile)
                {
                    sq.NumeroAbbonati = BitConverter.ToInt32(dati.GetRange(offset + 2, 4).ToArray(), 0);

                    var lnp = BitConverter.ToInt16(dati.GetRange(offset + 6, 2).ToArray(), 0);
                    sq.NomePresidente = Utils.decodificaTesto(dati.GetRange(offset + 8, lnp));
                    sq.CassaGioco = BitConverter.ToInt32(dati.GetRange(offset + 8 + lnp, 4).ToArray(), 0);
                    sq.CassaReale = BitConverter.ToInt32(dati.GetRange(offset + 12 + lnp, 4).ToArray(), 0);

                    var lnsp = BitConverter.ToInt16(dati.GetRange(offset + 16 + lnp, 2).ToArray(), 0);
                    sq.NomeSponsor = Utils.decodificaTesto(dati.GetRange(offset + 18 + lnp, lnsp));
                    var lnst = BitConverter.ToInt16(dati.GetRange(offset + 18 + lnp + lnsp, 2).ToArray(), 0);
                    sq.NomeSponsorTecnico = Utils.decodificaTesto(dati.GetRange(offset + 20 + lnp + lnsp, lnst));
                    sq.SquadraRiserve = BitConverter.ToInt16(dati.GetRange(offset + 20 + lnp + lnsp + lnst, 2).ToArray(), 0);
                    sq.Girone2B = (Girone2B)dati[offset + 22 + lnp + lnsp + lnst];


                    offset = offset + 23 + lnp + lnsp + lnst;

                    sq.StagioniPrecedenti = new();

                    for (int sp = offset; sp < offset + 20; sp++) sq.StagioniPrecedenti.Add(dati[sp]);

                    sq.StagioniA = dati[offset + 20];
                    sq.Giocate = BitConverter.ToInt16(dati.GetRange(offset + 21, 2).ToArray(), 0);
                    sq.Vinte = BitConverter.ToInt16(dati.GetRange(offset + 23, 2).ToArray(), 0);
                    sq.Pareggiate = BitConverter.ToInt16(dati.GetRange(offset + 25, 2).ToArray(), 0);
                    sq.GolSegnati = BitConverter.ToInt16(dati.GetRange(offset + 27, 2).ToArray(), 0);
                    sq.GolSubiti = BitConverter.ToInt16(dati.GetRange(offset + 29, 2).ToArray(), 0);
                    sq.PuntiTotali = BitConverter.ToInt16(dati.GetRange(offset + 31, 2).ToArray(), 0);
                    sq.Scudetti = dati[offset + 33];
                    sq.SecondiPosti = dati[offset + 34];
                    sq.PosizioniCampionato.Clear();
                    for (int sp = offset + 35; sp < offset + 81; sp++) sq.PosizioniCampionato.Add(dati[sp]);
                    var ilp = dati[offset + 81];
                    sq.LTrofeiPalmares = ilp;
                    for (int sp = offset + 82; sp < offset + 82 + (ilp * 3); sp++) sq.Palmares.Add(dati[sp]);
                    offset = offset + 82 + (ilp * 3);
                }
            }

            sq.Tattica = new();

            sq.Tattica.TatticaCompleta = dati.GetRange(offset, 1760);

            offset += 1760;

            sq.Tattica.PercentualeToccoDiPrima = dati[offset];
            sq.Tattica.PercentualeContropiede = dati[offset + 1];
            sq.Tattica.TipoAttacco = (TipoAttacco)dati[offset + 2];
            sq.Tattica.TipoEntrata = (TipoEntrata)dati[offset + 3];
            sq.Tattica.TipoMarcatura = (TipoMarcatura)dati[offset + 4];
            sq.Tattica.TipoRinvii = (TipoRinvii)dati[offset + 5];
            sq.Tattica.PressingDa = (PressingDa)dati[offset + 6];

            offset += 7;

            int na = dati[offset];

            offset++;

            sq.Allenatori = new();

            for (int a = 1; a <= na; a++)
            {
                sq.Allenatori.Add(new()
                {
                    Id = (uint)BitConverter.ToInt32(dati.GetRange(offset, 4).ToArray(), 0)
                });

                offset += 4;
            }

            sq.Giocatori = new();

            int ng = dati[offset];
            if (ng == 0) return sq;
            offset++;

            if (Versione == 800)
            {
                ng += 256 * dati[offset + 1];
                offset++;
            }

            if ((dati.Count - offset) / 5 != ng) return sq;

            for (int g = 1; g <= ng; g++)
            {
                sq.Giocatori.Add(new()
                {
                    AttivoInRosa = dati[offset] == 0,
                    Id = BitConverter.ToInt32(dati.GetRange(offset + 1, 4).ToArray(), 0)
                });

                offset += 5;
            }

        }
        catch (Exception)
        {
            return sq;
        }

        return sq;
    }

    private static int CalcoloManualeNumeroGiocatori()
    {
        int delta = Versione == 700 ? 1 : 2;

        for (int n = 1; n <= ((dati.Count - 1771) / 5); n++)
            if (dati[dati.Count - ((n * 5) + delta)] == n) return n;

        return 0;
    }

    public static Giocatore LeggiGiocatore(ElementoArchivio elemento)
    {
        Giocatore g = new();



        //var lt = BitConverter.ToInt16(dati.GetRange(0, 2).ToArray(), 0);

        var vfdi = BitConverter.ToInt16(dati.GetRange(2, 2).ToArray(), 0);

        //TODO Commento if temporaneo per test 7+ ITA/SPA, decommentare successivamente
        //if (vfdi != Versione) return g;


        g.Giocabile = dati[4] == 0;

        g.Id = BitConverter.ToInt16(dati.GetRange(5, 2).ToArray(), 0);
        if (g.Id < 0) g.Id += 65536;
        g.Numero = dati[7];

        var lnc = BitConverter.ToInt16(dati.GetRange(8, 2).ToArray(), 0);

        g.Nome = Utils.decodificaTesto(dati.GetRange(10, lnc));

        var lnl = BitConverter.ToInt16(dati.GetRange(10 + lnc, 2).ToArray(), 0);
        g.NomeCompleto = Utils.decodificaTesto(dati.GetRange(12 + lnc, lnl));
        g.Slot = dati[12 + lnc + lnl];
        g.AltriDati = dati[13 + lnc + lnl] == 0;

        g.Ruoli = new();

        for (int r = 1; r <= 6; r++) g.Ruoli.Add((Ruolo)dati[13 + lnc + lnl + r]);

        g.Nazione = (Paese)dati[20 + lnc + lnl];

        g.codColorePelle = (ColorePelle)dati[21 + lnc + lnl];
        g.codColoreCapelli = (ColoreCapelli)dati[22 + lnc + lnl];
        g.Reparto = (Reparto)dati[23 + lnc + lnl];

        int delta = Versione == 700 ? 0 : 3;

        if (Versione == 800)
        {
            g.codStileCapelli = (StileCapelli)dati[24 + lnc + lnl];
            g.codStileBarba = (StileBarba)dati[25 + lnc + lnl];
            g.Nazionalizzato = dati[26 + lnc + lnl] == 1;
        }

        g.GiornoNascita = dati[24 + delta + lnc + lnl];
        g.MeseNascita = dati[25 + delta + lnc + lnl];
        g.AnnoNascita = dati[26 + delta + lnc + lnl] + 256 * dati[27 + delta + lnc + lnl];
        g.Altezza = dati[28 + delta + lnc + lnl];
        g.Peso = dati[29 + delta + lnc + lnl];

        int lunghezzaTesti = 0;

        if (g.Giocabile)
        {
            g.PaeseNascita = (Paese)dati[30 + delta + lnc + lnl];
            for (int txt = 1; txt <= 10; txt++)
            {
                var lt = BitConverter.ToInt16(dati.GetRange(31 + delta + lnc + lnl + lunghezzaTesti, 2).ToArray(), 0);
                g.Testi.Add(Utils.decodificaTesto(dati.GetRange(31 + delta + lnc + lnl + lunghezzaTesti, lt)));
                lunghezzaTesti += 2 + lt;
            }

        }

        if (31 + delta + lnc + lnl + lunghezzaTesti + 16 != dati.Count && g.Giocabile) return g;

        int offset = dati.Count - 16;


        int m = (int)Math.Truncate((decimal)(dati[offset + 0] + dati[offset + 1] + dati[offset + 2] + dati[offset + 3]) / 4);

        g.Punteggi = new()
        {
            new() { Tipo = TipoPunteggioGiocatore.Media, Punteggio = m  },
            new() { Tipo = TipoPunteggioGiocatore.Velocità, Punteggio = dati[offset+0] },
            new() { Tipo = TipoPunteggioGiocatore.Resistenza, Punteggio = dati[offset+1] },
            new() { Tipo = TipoPunteggioGiocatore.Aggressività, Punteggio = dati[offset+2] },
            new() { Tipo = TipoPunteggioGiocatore.Qualità,Punteggio = dati[offset+3] },
            new() { Tipo = TipoPunteggioGiocatore.Rifinitura, Punteggio = dati[offset+4] },
            new() { Tipo = TipoPunteggioGiocatore.Dribbling, Punteggio = dati[offset+5]},
            new() { Tipo = TipoPunteggioGiocatore.Passaggio, Punteggio = dati[offset+6]},
            new() { Tipo = TipoPunteggioGiocatore.Tiro, Punteggio = dati[offset+7]},
            new() { Tipo = TipoPunteggioGiocatore.Entrate,Punteggio = dati[offset+8]},
            new() { Tipo = TipoPunteggioGiocatore.GiocoMani, Punteggio = dati[offset+9]},
            new() { Tipo = TipoPunteggioGiocatore.PiedePreferito,Punteggio = (int) (PiedePreferito) dati[offset+10]},
            new() { Tipo = TipoPunteggioGiocatore.Rigori,Punteggio = dati[offset+11]},
            new() { Tipo = TipoPunteggioGiocatore.CornerSX,Punteggio = dati[offset+12]},
            new() { Tipo = TipoPunteggioGiocatore.CornerDX,Punteggio = dati[offset+13]},
            new() { Tipo = TipoPunteggioGiocatore.FalloSX,Punteggio = dati[offset+14]},
            new() { Tipo = TipoPunteggioGiocatore.FalloDX,Punteggio = dati[offset+15]}
        };

        return g;
    }

    public static Allenatore LeggiAllenatore(ElementoArchivio elemento)
    {

        Allenatore a = new();



        //var lt = BitConverter.ToInt16(dati.GetRange(0, 2).ToArray(), 0);

        var vfdi = BitConverter.ToInt16(dati.GetRange(2, 2).ToArray(), 0);

        //TODO Commento if temporaneo per test 7+ ITA/SPA, decommentare successivamente
        //if (vfdi != Versione) return a;

        a.Giocabile = dati[4] == 0;

        a.Id = (uint)BitConverter.ToInt16(dati.GetRange(5, 2).ToArray(), 0);

        var lnc = BitConverter.ToInt16(dati.GetRange(7, 2).ToArray(), 0);

        a.Nome = Utils.decodificaTesto(dati.GetRange(9, lnc));

        if (!a.Giocabile) return a;

        var lnl = BitConverter.ToInt16(dati.GetRange(9 + lnc, 2).ToArray(), 0);

        a.NomeCompleto = Utils.decodificaTesto(dati.GetRange(11 + lnc, lnl));

        //#if DEBUG
        //        return a;
        //#endif

        //TODO Ricontrollare lettura testi allenatore, oppure valutare se ignorarli in lettura
        int lunghezzaTesti = 0;
        for (int txt = 1; txt <= 6; txt++)
        {
            var lt = BitConverter.ToInt16(dati.GetRange(11 + lnc + lnl + lunghezzaTesti, 2).ToArray(), 0);
            a.Testi.Add(Utils.decodificaTesto(dati.GetRange(11 + lnc + lnl + lunghezzaTesti, lt)));
            lunghezzaTesti += 2 + lt;
        }

        int offset = 11 + lnc + lnl + lunghezzaTesti;

        int byteTestGiocatore = dati[offset];

        a.exGiocatore = byteTestGiocatore == 3;

        if (a.exGiocatore)
        {
            int ldg = BitConverter.ToInt16(dati.GetRange(offset + 1, 2).ToArray(), 0);
            a.Testi.Add(Utils.decodificaTesto(dati.GetRange(offset + 3, ldg)));
            offset += 3 + ldg;
        }

        int ld = BitConverter.ToInt16(dati.GetRange(offset, 2).ToArray(), 0);
        if (ld <= dati.Count - offset) a.Testi.Add(Utils.decodificaTesto(dati.GetRange(offset + 2, ld)));

        return a;
    }

    public static Stadio LeggiStadio(ElementoArchivio elemento)
    {

        Stadio st = new();



        if (Versione == 800)
        {
            st.Id = (uint)elemento.Codice;
            var lns = BitConverter.ToInt16(dati.GetRange(0, 2).ToArray(), 0);
            st.Nome = Utils.decodificaTesto(dati.GetRange(2, lns));
            st.Larghezza = dati[lns + 2];
            st.Lunghezza = dati[lns + 3];
            st.NumeroBoh = dati[lns + 4];
            st.Nazione = (Paese)dati[lns + 5];
            st.AnnoCostruzione = BitConverter.ToInt16(dati.GetRange(lns + 6, 2).ToArray(), 0);
            st.Capienza = BitConverter.ToInt32(dati.GetRange(lns + 8, 4).ToArray(), 0);
            st.PostiInPiedi = BitConverter.ToInt32(dati.GetRange(lns + 12, 4).ToArray(), 0);
        }
        else
        {

            //var lt = BitConverter.ToInt16(dati.GetRange(36, 2).ToArray(), 0);

            var vfdi = BitConverter.ToInt16(dati.GetRange(38, 2).ToArray(), 0);

            //TODO Commento if temporaneo per test 7+ ITA/SPA, decommentare successivamente
            //if (vfdi != Versione) return st;


            st.Id = (uint)elemento.Codice;

            st.Giocabile = dati[41] == 0;

            var lnc_squadra = BitConverter.ToInt16(dati.GetRange(42, 2).ToArray(), 0);

            var lns = BitConverter.ToInt16(dati.GetRange(44 + lnc_squadra, 2).ToArray(), 0);

            st.Nome = Utils.decodificaTesto(dati.GetRange(44 + lnc_squadra + 2, lns));

            st.Nazione = (Paese)dati[47 + lnc_squadra + lns - 1];

            st.NumeroBoh = dati[47 + lnc_squadra + lns]; // byte dati[47 + lnc_squadra + lns] significato ignoto

            var lnsq = BitConverter.ToInt16(dati.GetRange(48 + lnc_squadra + lns, 2).ToArray(), 0);

            int offset = 50 + lnc_squadra + lns + lnsq;

            st.Capienza = BitConverter.ToInt32(dati.GetRange(offset, 4).ToArray(), 0);

            st.PostiInPiedi = BitConverter.ToInt32(dati.GetRange(offset + 4, 4).ToArray(), 0);

            st.Larghezza = BitConverter.ToInt16(dati.GetRange(offset + 8, 2).ToArray(), 0);
            st.Lunghezza = BitConverter.ToInt16(dati.GetRange(offset + 10, 2).ToArray(), 0);

            if (st.Giocabile) st.AnnoCostruzione = BitConverter.ToInt16(dati.GetRange(offset + 14, 2).ToArray(), 0);

        }

        return st;
    }

    public static List<Byte> ScriviSquadra(Squadra squadra)
    {
        ElementoArchivio e = new()
        {
            Codice = (int)squadra.Id,
            Offset = -1,
            Size = 0,

        };


        e.Codice = (int)squadra.Id;
        e.Offset = elementi.Any() ? elementi.Last().Offset + elementi.Last().Size : 0;


        e.Dat.AddRange(BitConverter.GetBytes(2037411651));
        e.Dat.AddRange(BitConverter.GetBytes(1751607666));
        e.Dat.AddRange(BitConverter.GetBytes(1663574132));
        e.Dat.Add(41);
        if (Versione == 800) e.Dat.AddRange(BitConverter.GetBytes(808464434));
        if (Versione == 700) e.Dat.AddRange(BitConverter.GetBytes(943274289));
        e.Dat.Add(32);
        e.Dat.Add(68);
        e.Dat.Add(105);
        e.Dat.AddRange(BitConverter.GetBytes(1952999273));
        e.Dat.AddRange(BitConverter.GetBytes(1967988835));
        e.Dat.AddRange(BitConverter.GetBytes(1835627628));
        e.Dat.AddRange(BitConverter.GetBytes(1634296933));

        int lt = 0;


        if (Versione == 800 && squadra.Giocabile)
        {
            lt = $"{squadra.Nome}{squadra.NomeCompleto}{squadra.NomePresidente}{squadra.NomeSponsor}{squadra.NomeSponsorTecnico}".Length + 5;
        }

        if (Versione == 800 && !squadra.Giocabile)
        {
            lt = $"{squadra.Nome}{squadra.NomeCompleto}".Length + 2;
        }

        if (Versione == 700 && squadra.Giocabile)
        {
            lt = $"{squadra.Nome}{squadra.NomeCompleto}{squadra.Stadio.Nome}{squadra.NomePresidente}{squadra.NomeSponsor}{squadra.NomeSponsorTecnico}".Length + 6;
        }

        if (Versione == 700 && !squadra.Giocabile)
        {
            lt = $"{squadra.Nome}{squadra.NomeCompleto}{squadra.Stadio.Nome}".Length + 3;
        }

        e.Dat.Add((byte)(lt % 256));
        e.Dat.Add((byte)(lt >> 8));

        e.Dat.Add((byte)(Versione % 256));
        e.Dat.Add((byte)(Versione >> 8));

        e.Dat.Add(0); //Lingua

        int g = squadra.Giocabile ? 0 : 1;

        e.Dat.Add((byte)g);

        e.Dat.Add((byte)(squadra.Nome.Length % 256));
        e.Dat.Add((byte)(squadra.Nome.Length >> 8));

        e.Dat.AddRange(Utils.codificaTesto(squadra.Nome));

        if (Versione == 800)
        {

            e.Dat.Add((byte)(squadra.Stadio.Id % 256));
            e.Dat.Add((byte)(squadra.Stadio.Id >> 8));
            e.Dat.Add((byte)squadra.Nazione);
            e.Dat.Add((byte)(squadra.NomeCompleto.Length % 256));
            e.Dat.Add((byte)(squadra.NomeCompleto.Length >> 8));
            e.Dat.AddRange(Utils.codificaTesto(squadra.NomeCompleto));
            e.Dat.Add((byte)(squadra.AnnoFondazione % 256));
            e.Dat.Add((byte)(squadra.AnnoFondazione >> 8));
            e.Dat.Add((byte)squadra.Boh);

            if (squadra.Giocabile)
            {
                e.Dat.AddRange(BitConverter.GetBytes((int)squadra.NumeroAbbonati));
                e.Dat.Add((byte)(squadra.NomePresidente.Length % 256));
                e.Dat.Add((byte)(squadra.NomePresidente.Length >> 8));
                e.Dat.AddRange(Utils.codificaTesto(squadra.NomePresidente));
                e.Dat.AddRange(BitConverter.GetBytes((int)squadra.CassaGioco));
                e.Dat.AddRange(BitConverter.GetBytes((int)squadra.CassaReale));
                e.Dat.Add((byte)(squadra.NomeSponsor.Length % 256));
                e.Dat.Add((byte)(squadra.NomeSponsor.Length >> 8));
                e.Dat.AddRange(Utils.codificaTesto(squadra.NomeSponsor));
                e.Dat.Add((byte)(squadra.NomeSponsorTecnico.Length % 256));
                e.Dat.Add((byte)(squadra.NomeSponsorTecnico.Length >> 8));
                e.Dat.AddRange(Utils.codificaTesto(squadra.NomeSponsorTecnico));
                e.Dat.Add((byte)(squadra.SquadraRiserve % 256));
                e.Dat.Add((byte)(squadra.SquadraRiserve >> 8));
                e.Dat.Add((byte)(squadra.Girone2B));
                e.Dat.Add((byte)(squadra.Girone3));
                if (squadra.StagioniPrecedenti == null || squadra.StagioniPrecedenti.Count != 20)
                {
                    for (int st = 1; st <= 20; st++) e.Dat.Add(0); //Stagioni precedenti
                }
                else for (int st = 1; st <= 20; st++) e.Dat.Add((byte)squadra.StagioniPrecedenti[st - 1]);

                e.Dat.Add((byte)squadra.StagioniA);
                e.Dat.Add((byte)(squadra.Giocate % 256));
                e.Dat.Add((byte)(squadra.Giocate >> 8));
                e.Dat.Add((byte)(squadra.Vinte % 256));
                e.Dat.Add((byte)(squadra.Vinte >> 8));
                e.Dat.Add((byte)(squadra.Pareggiate % 256));
                e.Dat.Add((byte)(squadra.Pareggiate >> 8));
                e.Dat.Add((byte)(squadra.GolSegnati % 256));
                e.Dat.Add((byte)(squadra.GolSegnati >> 8));
                e.Dat.Add((byte)(squadra.GolSubiti % 256));
                e.Dat.Add((byte)(squadra.GolSubiti >> 8));
                e.Dat.Add((byte)(squadra.PuntiTotali % 256));
                e.Dat.Add((byte)(squadra.PuntiTotali >> 8));
                e.Dat.Add((byte)squadra.Scudetti);
                e.Dat.Add((byte)squadra.SecondiPosti);
                foreach (var p in squadra.PosizioniCampionato) e.Dat.Add((byte)p);
                e.Dat.Add((byte)squadra.LTrofeiPalmares);
                e.Dat.AddRange(squadra.Palmares);

            }
        }
        else
        {

            e.Dat.Add((byte)(squadra.Stadio.Nome.Length % 256));
            e.Dat.Add((byte)(squadra.Stadio.Nome.Length >> 8));
            e.Dat.AddRange(Utils.codificaTesto(squadra.Stadio.Nome));
            e.Dat.Add((byte)squadra.Nazione);
            e.Dat.Add((byte)squadra.Boh);
            e.Dat.Add((byte)(squadra.NomeCompleto.Length % 256));
            e.Dat.Add((byte)(squadra.NomeCompleto.Length >> 8));
            e.Dat.AddRange(Utils.codificaTesto(squadra.NomeCompleto));
            e.Dat.AddRange(BitConverter.GetBytes((int)squadra.Stadio.Capienza));
            e.Dat.AddRange(BitConverter.GetBytes((int)squadra.Stadio.PostiInPiedi));
            e.Dat.AddRange(BitConverter.GetBytes((int)squadra.Stadio.Larghezza));
            e.Dat.AddRange(BitConverter.GetBytes((int)squadra.Stadio.Lunghezza));
            e.Dat.Add((byte)(squadra.AnnoFondazione % 256));
            e.Dat.Add((byte)(squadra.AnnoFondazione >> 8));

            if (squadra.Giocabile)
            {

                e.Dat.Add((byte)(squadra.Stadio.AnnoCostruzione % 256));
                e.Dat.Add((byte)(squadra.Stadio.AnnoCostruzione >> 8));
                e.Dat.AddRange(BitConverter.GetBytes((int)squadra.NumeroAbbonati));
                e.Dat.Add((byte)(squadra.NomePresidente.Length % 256));
                e.Dat.Add((byte)(squadra.NomePresidente.Length >> 8));
                e.Dat.AddRange(Utils.codificaTesto(squadra.NomePresidente));
                e.Dat.AddRange(BitConverter.GetBytes((int)squadra.CassaGioco));
                e.Dat.AddRange(BitConverter.GetBytes((int)squadra.CassaReale));
                e.Dat.Add((byte)(squadra.NomeSponsor.Length % 256));
                e.Dat.Add((byte)(squadra.NomeSponsor.Length >> 8));
                e.Dat.AddRange(Utils.codificaTesto(squadra.NomeSponsor));
                e.Dat.Add((byte)(squadra.NomeSponsorTecnico.Length % 256));
                e.Dat.Add((byte)(squadra.NomeSponsorTecnico.Length >> 8));
                e.Dat.AddRange(Utils.codificaTesto(squadra.NomeSponsorTecnico));
                e.Dat.Add((byte)(squadra.SquadraRiserve % 256));
                e.Dat.Add((byte)(squadra.SquadraRiserve >> 8));
                e.Dat.Add((byte)(squadra.Girone2B));
                if (squadra.StagioniPrecedenti == null || squadra.StagioniPrecedenti.Count != 20)
                {
                    for (int st = 1; st <= 20; st++) e.Dat.Add(0); //Stagioni precedenti
                }
                else for (int st = 1; st <= 20; st++) e.Dat.Add((byte)squadra.StagioniPrecedenti[st - 1]);

                e.Dat.Add((byte)squadra.StagioniA);
                e.Dat.Add((byte)(squadra.Giocate % 256));
                e.Dat.Add((byte)(squadra.Giocate >> 8));
                e.Dat.Add((byte)(squadra.Vinte % 256));
                e.Dat.Add((byte)(squadra.Vinte >> 8));
                e.Dat.Add((byte)(squadra.Pareggiate % 256));
                e.Dat.Add((byte)(squadra.Pareggiate >> 8));
                e.Dat.Add((byte)(squadra.GolSegnati % 256));
                e.Dat.Add((byte)(squadra.GolSegnati >> 8));
                e.Dat.Add((byte)(squadra.GolSubiti % 256));
                e.Dat.Add((byte)(squadra.GolSubiti >> 8));
                e.Dat.Add((byte)(squadra.PuntiTotali % 256));
                e.Dat.Add((byte)(squadra.PuntiTotali >> 8));
                e.Dat.Add((byte)squadra.Scudetti);
                e.Dat.Add((byte)squadra.SecondiPosti);
                foreach (var p in squadra.PosizioniCampionato) e.Dat.Add((byte)p);
                e.Dat.Add((byte)squadra.LTrofeiPalmares);
                e.Dat.AddRange(squadra.Palmares);


            }

        }

        e.Dat.AddRange(squadra.Tattica.TatticaCompleta);
        e.Dat.Add((byte)squadra.Tattica.PercentualeToccoDiPrima);
        e.Dat.Add((byte)squadra.Tattica.PercentualeContropiede);
        e.Dat.Add((byte)squadra.Tattica.TipoAttacco);
        e.Dat.Add((byte)squadra.Tattica.TipoEntrata);
        e.Dat.Add((byte)squadra.Tattica.TipoMarcatura);
        e.Dat.Add((byte)squadra.Tattica.TipoRinvii);
        e.Dat.Add((byte)squadra.Tattica.PressingDa);

        e.Dat.Add(1); //Numero Allenatori squadra
        e.Dat.AddRange(BitConverter.GetBytes((int)squadra.Allenatori.Last().Id));

        if (Versione == 800)
        {
            e.Dat.Add((byte)(squadra.Giocatori.Count % 256));
            e.Dat.Add((byte)(squadra.Giocatori.Count >> 8));
        }
        else e.Dat.Add((byte)squadra.Giocatori.Count);

        foreach (var gc in squadra.Giocatori)
        {
            int a = gc.AttivoInRosa ? 0 : 1;
            e.Dat.Add((byte)a);
            e.Dat.AddRange(BitConverter.GetBytes((int)gc.Id));
        }

        e.Size = e.Dat.Count;
        elementi.Add(e);
        return e.Dat;
    }

    public static List<Byte> ScriviGiocatore(Giocatore giocatore)
    {
        ElementoArchivio e = new();

        int lt = giocatore.Giocabile ?
            $"{giocatore.Nome}{giocatore.NomeCompleto}".Length + 8 :
            $"{giocatore.Nome}{giocatore.NomeCompleto}".Length + 2;

        e.Dat.Add((byte)(lt % 256));
        e.Dat.Add((byte)(lt >> 8));

        e.Dat.Add((byte)(Versione % 256));
        e.Dat.Add((byte)(Versione >> 8));

        int g = giocatore.Giocabile ? 0 : 1;
        e.Dat.Add((byte)g);

        e.Dat.Add((byte)(giocatore.Id % 256));
        e.Dat.Add((byte)(giocatore.Id >> 8));

        e.Dat.Add((byte)giocatore.Numero);

        e.Dat.Add((byte)(giocatore.Nome.Length % 256));
        e.Dat.Add((byte)(giocatore.Nome.Length >> 8));
        e.Dat.AddRange(Utils.codificaTesto(giocatore.Nome));

        e.Dat.Add((byte)(giocatore.NomeCompleto.Length % 256));
        e.Dat.Add((byte)(giocatore.NomeCompleto.Length >> 8));
        e.Dat.AddRange(Utils.codificaTesto(giocatore.NomeCompleto));

        e.Dat.Add((byte)giocatore.Slot);
        int d = giocatore.AltriDati ? 0 : 1;
        e.Dat.Add((byte)d);

        foreach (var r in giocatore.Ruoli) e.Dat.Add((byte)r);
        e.Dat.Add((byte)giocatore.Nazione);
        e.Dat.Add((byte)giocatore.codColorePelle);
        e.Dat.Add((byte)giocatore.codColoreCapelli);
        e.Dat.Add((byte)giocatore.Reparto);

        if (Versione == 800)
        {
            e.Dat.Add((byte)giocatore.codStileCapelli);
            e.Dat.Add((byte)giocatore.codStileBarba);
            int n = giocatore.Nazionalizzato ? 1 : 0;
            e.Dat.Add((byte)n);
        }

        e.Dat.Add((byte)giocatore.GiornoNascita);
        e.Dat.Add((byte)giocatore.MeseNascita);
        e.Dat.Add((byte)(giocatore.AnnoNascita % 256));
        e.Dat.Add((byte)(giocatore.AnnoNascita >> 8));
        e.Dat.Add((byte)giocatore.Altezza);
        e.Dat.Add((byte)giocatore.Peso);

        if (giocatore.Giocabile)
        {
            e.Dat.Add((byte)giocatore.PaeseNascita);
            for (int i = 1; i <= 10; i++)
            {
                e.Dat.Add(1);
                e.Dat.Add(0);
                e.Dat.AddRange(Utils.codificaTesto("-"));
            }
        }

        foreach (var p in giocatore.Punteggi) if (p.Tipo != TipoPunteggioGiocatore.Media) e.Dat.Add((byte)p.Punteggio);

        e.Size = e.Dat.Count;
        elementi.Add(e);
        return e.Dat;
    }


    public static List<Byte> ScriviAllenatore(Allenatore allenatore)
    {
        ElementoArchivio e = new();

        int lt = allenatore.Giocabile ?
            $"{allenatore.Nome}{allenatore.NomeCompleto}".Length + 2 :
            allenatore.Nome.Length + 1;

        e.Dat.Add((byte)(lt % 256));
        e.Dat.Add((byte)(lt >> 8));

        e.Dat.Add((byte)(Versione % 256));
        e.Dat.Add((byte)(Versione >> 8));

        int g = allenatore.Giocabile ? 0 : 1;

        e.Dat.Add((byte)g);

        e.Dat.Add((byte)(allenatore.Id % 256));
        e.Dat.Add((byte)(allenatore.Id >> 8));

        e.Dat.Add((byte)(allenatore.Nome.Length % 256));
        e.Dat.Add((byte)(allenatore.Nome.Length >> 8));
        e.Dat.AddRange(Utils.codificaTesto(allenatore.Nome));


        if (allenatore.Giocabile)
        {
            e.Dat.Add((byte)(allenatore.NomeCompleto.Length % 256));
            e.Dat.Add((byte)(allenatore.NomeCompleto.Length >> 8));
            e.Dat.AddRange(Utils.codificaTesto(allenatore.NomeCompleto));

            for (int t = 1; t <= 6; t++)
            {
                e.Dat.Add(1);
                e.Dat.Add(0);
                e.Dat.AddRange(Utils.codificaTesto("-"));
            }

            e.Dat.Add(0);
            e.Dat.Add(1);
            e.Dat.Add(0);
            e.Dat.AddRange(Utils.codificaTesto("-"));
        }

        e.Size = e.Dat.Count;
        elementi.Add(e);
        return e.Dat;
    }

    public static List<Byte> ScriviStadio(Stadio stadio)
    {
        ElementoArchivio e = new();

        e.Dat.Add((byte)(stadio.Nome.Length % 256));
        e.Dat.Add((byte)(stadio.Nome.Length >> 8));
        e.Dat.AddRange(Utils.codificaTesto(stadio.Nome));
        e.Dat.Add((byte)stadio.Larghezza);
        e.Dat.Add((byte)stadio.Lunghezza);
        e.Dat.Add((byte)stadio.NumeroBoh);
        e.Dat.Add((byte)stadio.Nazione);
        e.Dat.Add((byte)(stadio.AnnoCostruzione % 256));
        e.Dat.Add((byte)(stadio.AnnoCostruzione >> 8));
        e.Dat.AddRange(BitConverter.GetBytes((int)stadio.Capienza));
        e.Dat.AddRange(BitConverter.GetBytes((int)stadio.PostiInPiedi));

        e.Size = e.Dat.Count;
        elementi.Add(e);
        return e.Dat;
    }

    public static List<Byte> Scrivi()
    {
        dati.Clear();

        dati.AddRange(BitConverter.GetBytes(1229344068));
        dati.AddRange(BitConverter.GetBytes(808333686));
        dati.AddRange(BitConverter.GetBytes((UInt64)0));
        dati.AddRange(BitConverter.GetBytes(elementi.Count));

        int baseOffset = 13 * elementi.Count + 20;

        dati.AddRange(Enumerable.Repeat<Byte>(0, 13 * elementi.Count));

        int n = 0;

        foreach (var e in elementi)
        {
            var a = BitConverter.GetBytes(e.Codice);
            var b = BitConverter.GetBytes(e.Offset + baseOffset);
            var c = BitConverter.GetBytes(e.Size);
            for (int i = 0; i <= 3; i++)
            {
                dati[13 * n + 20 + i] = a[i];
                dati[13 * n + 25 + i] = b[i];
                dati[13 * n + 29 + i] = c[i];
            }
            dati[13 * n + 24] = 0;

            dati.AddRange(e.Dat);
            n++;
        }

        elementi.Clear();
        return dati;
    }

}
