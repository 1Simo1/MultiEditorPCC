using Microsoft.Extensions.DependencyInjection;
using MultiEditorPCC.Dat.DbSet;
using MultiEditorPCC.Lib.Archivi;


namespace MultiEditorPCC.Lib;

public interface IDatSvc
{

    /// <summary>
    /// Legge un ElementoArchivio, precedentemente letto un file originale
    /// del gioco, e lo elabora memorizzando sul database il dato richiesto
    /// (es. se il fileGioco era di una squadra, salva una squadra)
    /// </summary>
    bool ScriviDB(ProgettoEditorPCC progettoEditor, String fileGioco, ElementoArchivio elemento, bool scriviDBC = false);

    /// <summary>
    /// Scrive i singoli file di informazioni estratti nella cartella dei files dell'editor
    /// (può ad esempio servire per scrivere i dati di una singola squadra di un FDI in un file DBC,
    /// non viene letto dal gioco, ma può essere tenuto per salvare su disco le informazioni di una singola squadra)
    /// </summary>
    bool EstraiFileEditor(ProgettoEditorPCC progettoEditor, String fileGioco, ElementoArchivio elemento);
    /// <summary>
    /// Legge un elemento da database editor, e lo rielabora ottenendo dati
    /// utili a poterlo poi riscrivere su un file (se usato senza modifiche,
    /// l'elemento ritornato dovrebbe risultare uguale od equivalente a quello passato 
    /// precedentemente al metodo ScriviDB)
    /// </summary>
    ElementoArchivio LeggiElementoDB(String versionePCC, String fileGioco);

    /// <summary>
    /// Legge un elemento di ArchivioElementi, seleziona e riceve l'elemento dal corretto gestore di archivi
    /// ed aggiorna i DatiProgettoAttivo di ArchivioSvc
    /// </summary>
    /// <param name="versionePCC">Versione del gioco usata per il progetto attivo</param>
    /// <param name="t">Tipi di dato elaborati in questo passaggio</param>
    /// <param name="elemento">Dati memorizzati da file di gioco, per l'elemento elaborato</param>
    void ElaboraInfoElementoDB(string versionePCC, List<ArchivioSvc.TipoDatoDB> t, ElementoArchivio elemento);

    Squadra ComponiInformazioniCompleteSquadra(Squadra sq);

    /// <summary>
    /// Esporta l'elemento modificato su file DBE, un formato di file apposito per questo editor,
    /// e che può essere condiviso e reimportato
    /// </summary>
    /// <typeparam name="T">Tipo di elemento esaminato (es. Squadra,Allenatore,Stadio,Giocatore)</typeparam>
    /// <param name="t">Tipo di dato dell'elemento</param>
    /// <param name="elemento">Elemento da scrivere</param>
    /// <returns>Byte che compongono il file (scrivendolo poi su disco si ottiene il file)</returns>
    bool EsportaElementoSuFileEditor<T>(ArchivioSvc.TipoDatoDB t, T elemento, ProgettoEditorPCC progettoEditor);

    T ImportaElementoDaFileEditor<T>(String PathDBE) where T : class;


    Task<bool> EsportaDBEditorSuFileCSV(ProgettoEditorPCC progettoEditor,
                                        bool aggiungiTatticaCompletaSquadra = true,
                                        List<Squadra> squadre = null,
                                        List<Giocatore> giocatori = null,
                                        List<Allenatore> allenatori = null,
                                        List<Stadio> stadi = null
                                       );
    T ImportaDBEditorDaFileCSV<T>(String PathCSV) where T : class;

}

public class DatSvc : IDatSvc
{

    public bool ScriviDB(ProgettoEditorPCC progettoEditor, String fileGioco, ElementoArchivio elemento, bool scriviDBC = false)
    {
        try
        {
            //TODO
        }
        catch (Exception e)
        {
            return false;
        }


        return true;
    }

    public bool EstraiFileEditor(ProgettoEditorPCC progettoEditor, String fileGioco, ElementoArchivio elemento)
    {

        try
        {
            String nomeFileEditor = $"{AppDomain.CurrentDomain.BaseDirectory}files{Path.DirectorySeparatorChar}";
            nomeFileEditor += $"{progettoEditor.Nome}{Path.DirectorySeparatorChar}";

            if (fileGioco.EndsWith(".FDI"))
            {
                Directory.CreateDirectory(nomeFileEditor + "/DBDAT/");
                nomeFileEditor += $"{fileGioco.Substring(0, fileGioco.Length - 7)}{elemento.Codice.ToString().PadLeft(4, '0')}.DBC";

            }
            else
            {
                if (progettoEditor.VersionePCC.Equals("PC Calcio 6"))
                {
                    Directory.CreateDirectory(nomeFileEditor + "/Dbdat/");
                    Directory.CreateDirectory(nomeFileEditor + "/Dbdat/EQ036036");
                    nomeFileEditor += $"Dbdat{Path.DirectorySeparatorChar}EQ036036{Path.DirectorySeparatorChar}{elemento.Nome}";
                }

                if (progettoEditor.VersionePCC.Equals("PC Calcio 5"))
                {
                    Directory.CreateDirectory(nomeFileEditor + "/DBDAT/");
                    Directory.CreateDirectory(nomeFileEditor + "/DBDAT/EQUIPOS");
                    nomeFileEditor += $"DBDAT{Path.DirectorySeparatorChar}EQUIPOS{Path.DirectorySeparatorChar}{elemento.Nome}";
                }
            }

            File.WriteAllBytes(nomeFileEditor, elemento.Dat.ToArray());
        }
        catch (Exception _)
        {
            return false;
        }

        return true;
    }

    public ElementoArchivio LeggiElementoDB(String versionePCC, String fileGioco)
    {
        throw new NotImplementedException();
    }

    public void ElaboraInfoElementoDB(String versionePCC, List<ArchivioSvc.TipoDatoDB> t, ElementoArchivio elemento)
    {
        var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

        int versioneFDI = -1;

        if (elemento.Codice != -1)
        {
            versioneFDI = elemento.Dat.Count >= 40 ?
                          leggiByteVersioneFDI(elemento.Dat.GetRange(0, 40)) :
                          leggiByteVersioneFDI(elemento.Dat.GetRange(0, 4));

            if (versioneFDI != 700 && versioneFDI != 800) versioneFDI = 800;

        }


        if (versionePCC.Contains("PC Futbol 7+")) versioneFDI = 700;

        if (versioneFDI != -1) //versionePCC 7 o successiva (esamino un FDI)
        {
            FDI.Versione = versioneFDI;
            FDI.dati = elemento.Dat;

            foreach (var tipo in t)
            {

                switch (tipo)
                {
                    ///Per velocizzare i tempi di caricamento iniziali, la squadra
                    ///viene caricata con i riferimenti minimi a giocatori, allenatori
                    ///e stadi, che ho già completi nei rispettivi archivi 
                    ///in memoria (DatiProgettoAttivo)
                    ///Nel caso in cui occorrano le informazioni complete della squadra
                    ///(quindi che si possa ricavare tutto guardando solo la Squadra)
                    ///si può richiamare il metodo ComponiInformazioniCompleteSquadra
                    case ArchivioSvc.TipoDatoDB.SQUADRA:
                        Squadra sq = FDI.LeggiSquadra(elemento);
                        a.DatiProgettoAttivo.Squadre.Add(sq);
                        break;
                    case ArchivioSvc.TipoDatoDB.GIOCATORE:
                        Giocatore g = FDI.LeggiGiocatore(elemento);
                        a.DatiProgettoAttivo.Giocatori.Add(g);
                        break;
                    case ArchivioSvc.TipoDatoDB.ALLENATORE:
                        Allenatore e = FDI.LeggiAllenatore(elemento);
                        a.DatiProgettoAttivo.Allenatori.Add(e);
                        break;
                    case ArchivioSvc.TipoDatoDB.STADIO:
                        Stadio s = FDI.LeggiStadio(elemento);
                        a.DatiProgettoAttivo.Stadi.Add(s);
                        break;
                    default: break;
                }


            } // foreach


        }
        else //versionePCC 6 o precedente (esamino un archivio NON FDI)
        {
            //TODO
        }


        //a.DatiProgettoAttivo.Archivi = new();

    }

    private int leggiByteVersioneFDI(List<byte> list)
    {
        int vfdi = BitConverter.ToInt16(list.GetRange(2, 2).ToArray(), 0);

        return vfdi != 31088 ? vfdi : BitConverter.ToInt16(list.GetRange(38, 2).ToArray(), 0);

    }

    public Squadra ComponiInformazioniCompleteSquadra(Squadra sq)
    {
        var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

        uint idStadio = sq.Stadio.Id;
        sq.Stadio = a.DatiProgettoAttivo.Stadi.Find(e => e.Id == idStadio);
        List<uint> idAllenatori = new();
        foreach (var at in sq.Allenatori) idAllenatori.Add(at.Id);
        sq.Allenatori = new();
        foreach (var idAllenatore in idAllenatori)
        {
            sq.Allenatori.Add(a.DatiProgettoAttivo.Allenatori.Find(a => a.Id == idAllenatore));
        }

        Dictionary<int, bool> statoGiocatoriAttiviInRosa = new();

        foreach (var gt in sq.Giocatori) statoGiocatoriAttiviInRosa.Add(gt.Id, gt.AttivoInRosa);

        sq.Giocatori = new();

        foreach (var gt in statoGiocatoriAttiviInRosa)
        {
            Giocatore g = a.DatiProgettoAttivo.Giocatori.Find(gc => gc.Id == gt.Key);

            if (g != null)
            {
                g.AttivoInRosa = gt.Value;
            }
            else
            {
                g = new Giocatore() { Id = gt.Key };
            }

            sq.Giocatori.Add(g);
        }

        return sq;
    }

    public bool EsportaElementoSuFileEditor<T>(ArchivioSvc.TipoDatoDB t, T elemento, ProgettoEditorPCC progettoEditor)
    {
        var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

        List<byte> f = new();
        String nomeFile = "";

        FileEditor.Versione = 1;
        FileEditor.Dati = new();


        try
        {
            switch (t)
            {

                case ArchivioSvc.TipoDatoDB.SQUADRA:
                    Squadra sq = elemento as Squadra;
                    f = FileEditor.ScriviSquadra(sq);
                    nomeFile = $"SQ{sq.Id.ToString().PadLeft(5, '0')}";
                    //a.DatiProgettoAttivo.Squadre.Add(sq);
                    break;
                case ArchivioSvc.TipoDatoDB.GIOCATORE:
                    Giocatore g = elemento as Giocatore;
                    f = FileEditor.ScriviGiocatore(g);
                    nomeFile = $"GT{g.Id.ToString().PadLeft(5, '0')}";
                    //a.DatiProgettoAttivo.Giocatori.Add(g);
                    break;
                case ArchivioSvc.TipoDatoDB.ALLENATORE:
                    Allenatore al = elemento as Allenatore;
                    f = FileEditor.ScriviAllenatore(al);
                    nomeFile = $"AL{al.Id.ToString().PadLeft(5, '0')}";
                    //a.DatiProgettoAttivo.Allenatori.Add(al);
                    break;
                case ArchivioSvc.TipoDatoDB.STADIO:
                    Stadio st = elemento as Stadio;
                    f = FileEditor.ScriviStadio(st);
                    nomeFile = $"ST{st.Id.ToString().PadLeft(5, '0')}";
                    //a.DatiProgettoAttivo.Stadi.Add(s);
                    break;
                default: break;
            }

            String nomeFileEditor = $"{AppDomain.CurrentDomain.BaseDirectory}files{Path.DirectorySeparatorChar}";
            nomeFileEditor += $"{progettoEditor.Nome}{Path.DirectorySeparatorChar}{nomeFile}.DBE";

            File.WriteAllBytes(nomeFileEditor, f.ToArray());

        }
        catch (Exception ex)
        {

            return false;

        }

        return true;
    }

    public T ImportaElementoDaFileEditor<T>(String PathDBE) where T : class
    {
        var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();
        //var e = AppSvc.Services.GetRequiredService<EditorSvc>();
        FileEditor.Dati = File.ReadAllBytes(PathDBE).ToList();

        switch ((ArchivioSvc.TipoDatoDB)FileEditor.Dati.First())
        {

            case ArchivioSvc.TipoDatoDB.SQUADRA:
                Squadra sq = FileEditor.LeggiSquadra();

                //a.DatiProgettoAttivo.Squadre.Add(sq);
                return sq as T;
            case ArchivioSvc.TipoDatoDB.GIOCATORE:
                Giocatore g = FileEditor.LeggiGiocatore();
                //a.DatiProgettoAttivo.Giocatori.Add(g);
                return g as T;
            case ArchivioSvc.TipoDatoDB.ALLENATORE:
                Allenatore e = FileEditor.LeggiAllenatore();
                //a.DatiProgettoAttivo.Allenatori.Add(e);
                return e as T;
            case ArchivioSvc.TipoDatoDB.STADIO:
                Stadio st = FileEditor.LeggiStadio();
                //a.DatiProgettoAttivo.Stadi.Add(st);
                return st as T;
            default: break;
        }

        return null;


    }

    public async Task<bool> EsportaDBEditorSuFileCSV(ProgettoEditorPCC progettoEditor,
                                                    bool aggiungiTatticaCompletaSquadra = false,
                                                    List<Squadra> squadre = null,
                                                    List<Giocatore> giocatori = null,
                                                    List<Allenatore> allenatori = null,
                                                    List<Stadio> stadi = null
                                                    )
    {
        try
        {
            var a = AppSvc.Services.GetRequiredService<ArchivioSvc>();

            String percorsoCSV = $"{AppDomain.CurrentDomain.BaseDirectory}files{Path.DirectorySeparatorChar}";
            percorsoCSV += $"{progettoEditor.Nome}{Path.DirectorySeparatorChar}";

            String nf = "";


            DatabaseCSV.scriviTatticaCompleta = aggiungiTatticaCompletaSquadra;

            if (squadre == null || !squadre.Any())
            {
                DatabaseCSV.ScriviCSVSquadre(a.DatiProgettoAttivo.Squadre);
            }
            else DatabaseCSV.ScriviCSVSquadre(squadre);

            nf = "Squadre";
            if (squadre.Count == 1)
            {
                nf = $"{squadre.First().Nome.Replace(" ", "_")}#{squadre.First().Id}";
            }

            File.WriteAllText($"{percorsoCSV}{nf}.csv", DatabaseCSV.contenutoCSV);

            if (allenatori == null || !allenatori.Any())
            {
                DatabaseCSV.ScriviCSVAllenatori(a.DatiProgettoAttivo.Allenatori);
            }
            else DatabaseCSV.ScriviCSVAllenatori(allenatori);

            nf = "Allenatori";
            if (squadre.Count == 1)
            {
                nf = $"Allenatore#{squadre.First().Nome.Replace(" ", "_")}#{squadre.First().Id}";
            }
            File.WriteAllText($"{percorsoCSV}{nf}.csv", DatabaseCSV.contenutoCSV);



            if (stadi == null || !stadi.Any())
            {
                DatabaseCSV.ScriviCSVStadi(a.DatiProgettoAttivo.Stadi);
            }
            else DatabaseCSV.ScriviCSVStadi(stadi);

            nf = "Stadi";
            if (squadre.Count == 1)
            {
                nf = $"Stadio#{squadre.First().Nome.Replace(" ", "_")}#{squadre.First().Id}";
            }
            File.WriteAllText($"{percorsoCSV}{nf}.csv", DatabaseCSV.contenutoCSV);


            List<Giocatore> elencoGiocatori = new();

            if (giocatori == null || !giocatori.Any())
            {
                elencoGiocatori = a.DatiProgettoAttivo.Giocatori;
            }
            else elencoGiocatori = giocatori;

            List<Squadra> elencoSquadre = new();

            if (squadre == null || !squadre.Any())
            {
                elencoSquadre = a.DatiProgettoAttivo.Squadre;
            }
            else elencoSquadre = squadre;


            foreach (var gc in elencoGiocatori)
            {
                int id = gc.Id;

                var sq = elencoSquadre.Where(sq => sq.Giocatori.Find(gc => gc.Id == id) != null).FirstOrDefault();
                String Squadra = String.Empty;
                if (sq != null) Squadra = sq.Nome;

                var nSquadra = sq != null ? sq.Id : 0;

                DatabaseCSV.dettagliSquadreGiocatore.Add(id, new((int)nSquadra, Squadra));
            }


            DatabaseCSV.ScriviCSVGiocatori(elencoGiocatori);

            nf = "Giocatori";
            if (squadre.Count == 1)
            {
                nf = $"Giocatori#{squadre.First().Nome.Replace(" ", "_")}#{squadre.First().Id}";
            }
            File.WriteAllText($"{percorsoCSV}{nf}.csv", DatabaseCSV.contenutoCSV);

            if (giocatori == null || !giocatori.Any()) a.DatiProgettoAttivo.Giocatori = elencoGiocatori;
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    public T ImportaDBEditorDaFileCSV<T>(string PathCSV) where T : class
    {
        //TODO Implementare import tipo db da file CSV
        throw new NotImplementedException();
    }
}
