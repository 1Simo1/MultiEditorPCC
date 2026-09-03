namespace MultiEditorPCC.Shared.DTO;

public class Squadra
{
    public uint Id { get; set; } = 0;
    public String Nome { get; set; } = String.Empty;
    public String NomeCompleto { get; set; } = String.Empty;

    public bool Giocabile { get; set; }
    public Paese Nazione { get; set; } = Paese.ITALIA;

    public UInt16 AnnoFondazione { get; set; } = 2021;

    public int Boh { get; set; } = 0;

    public int? NumeroAbbonati { get; set; } = 0;

    public String? NomePresidente { get; set; } = String.Empty;

    public int? CassaGioco { get; set; } = 121;

    public int? CassaReale { get; set; } = 121;

    public String? NomeSponsor { get; set; } = String.Empty;

    public String? NomeSponsorTecnico { get; set; } = String.Empty;

    public int? SquadraRiserve { get; set; } = 65535;

    public int? TerzaSquadra { get; set; } = 65535;

    public Girone2B Girone2B { get; set; } = Girone2B.Nessuno;

    public int Girone3 { get; set; } = 0;

    public int PercentualeToccoDiPrima { get; set; } = 50;
    public int PercentualeContropiede { get; set; } = 50;

    public TipoAttacco TipoAttacco { get; set; } = TipoAttacco.Offensivo;

    public TipoEntrata TipoEntrata { get; set; } = TipoEntrata.Morbida;

    public TipoMarcatura TipoMarcatura { get; set; } = TipoMarcatura.Zona;

    public TipoRinvii TipoRinvii { get; set; } = TipoRinvii.Lunghi;
    public PressingDa PressingDa { get; set; } = PressingDa.Proprio;

    public List<Byte> TatticaCompleta { get; set; } = new();

    public uint CodiceAllenatore { get; set; } = 0;

    public String NomeAllenatore { get; set; } = String.Empty;
    public String NomeCompletoAllenatore { get; set; } = String.Empty;

    public bool ExGiocatore { get; set; } = false;

    public String NomeStadio { get; set; } = String.Empty;
    public int Capienza { get; set; } = -1;
    public int PostiInPiedi { get; set; } = -1;
    public Int16 Larghezza { get; set; } = -1;
    public Int16 Lunghezza { get; set; } = -1;

    public int NumeroBoh { get; set; } = 0;

    public UInt16 AnnoCostruzione { get; set; } = 2021;

    public String Note { get; set; } = String.Empty;

    public int TipoInfo { get; set; } = 1;
    public int V { get; set; } = 3;

    public override string ToString()
    {
        string header = string.Empty;
        string st = string.Empty;

        var fl = typeof(Squadra).GetProperties();

        foreach (var f in fl)
        {
            header += $"{f.Name};";
            if (f.Name != "TatticaCompleta")
            {
                st += $"{f.GetValue(this)};";

            }
            else st += Convert.ToBase64String(((List<Byte>)f.GetValue(this)).ToArray()) + ";";

        }

        return $"{header.TrimEnd(';')}{Environment.NewLine}{st.TrimEnd(';')}";
    }

}
public enum TipoAttacco
{
    Offensivo = 0,
    Opportunistico = 1,
    Misto = 2
}

public enum TipoEntrata
{
    Morbida = 0,
    Mediana = 1,
    Aggressiva = 2
}

public enum TipoMarcatura
{
    Zona = 0,
    Uomo = 1
}

public enum TipoRinvii
{
    Corti = 0,
    Lunghi = 1
}


public enum PressingDa
{
    Proprio = 0,
    Metà = 1,
    Rivale = 2
}



public enum Girone2B
{
    Nessuno = 0,
    Primo = 1,
    Secondo = 2,
    Terzo = 3,
    Quarto = 4
}

public enum Paese
{
    ITALIA = 36,
    SPAGNA = 22,
    INGHILTERRA = 30,
    GERMANIA = 2,
    FRANCIA = 24,
    ARGENTINA = 3,
    PORTOGALLO = 47,
    SCOZIA = 19,
    OLANDA = 27,
    BELGIO = 12,
    ALBANIA = 1,
    AUSTRALIA = 4,
    AUSTRIA,
    AZERBAIGIAN,
    BIELORUSSIA,
    BOLIVIA,
    BOSNIA,
    BRASILE = 10,
    BULGARIA,
    CAMERUN = 13,
    CILE,
    CIPRO,
    COLOMBIA,
    CROAZIA,
    DANIMARCA,
    SLOVACCHIA = 20,
    SLOVENIA = 21,
    FINLANDIA = 23,
    GHANA = 25,
    GRECIA = 26,
    HONDURAS = 28,
    UNGHERIA,
    IRLANDA = 31,
    IRLANDA_DEL_NORD = 32,
    ISLANDA,
    FAROE,
    ISRAELE = 35,
    LITUANIA = 37,
    LUSSEMBURGO,
    MACEDONIA,
    MALTA = 40,
    MAROCCO,
    MOLDAVIA,
    NIGERIA,
    NORVEGIA,
    GALLES,
    POLONIA,
    REPUBBLICA_CECA = 48,
    ROMANIA,
    RUSSIA,
    SERBIA,
    SUDAFRICA,
    SVEZIA,
    SVIZZERA = 54,
    TURCHIA,
    UCRAINA = 56,
    URUGUAY,
    YUGOSLAVIA,
    PERÙ,
    CANADA = 60,
    USA,
    GEORGIA,
    COSTARICA,
    PARAGUAY,
    GIAPPONE,
    ALGERIA,
    TRINIDAD_E_TOBAGO,
    SENEGAL,
    SURINAME,
    ZAMBIA = 70,
    CAPOVERDE,
    VENEZUELA,
    RHODESIA,
    SINGAPORE,
    ANDORRA,
    MOZAMBICO,
    LIECHTENSTEIN,
    LIBERIA,
    PANAMA,
    ZAIRE = 80,
    TAGIKISTAN,
    UZBEKISTAN,
    MESSICO,
    GUINEA,
    ANGOLA,
    ZIMBABWE,
    SIERRA_LEONE,
    GUADALUPE,
    ECUADOR,
    ESTONIA = 90,
    GUINEA_BISSAU,
    LIBIA,
    EGITTO,
    GIAMAICA,
    NUOVA_CALEDONIA,
    BERMUDA,
    NUOVA_ZELANDA,
    GUYANA_FRANCESE,
    SAINT_VINCENT,
    CIAD = 100,
    TOGO,
    GUINEA_CONAKRY,
    TANZANIA,
    BURKINA_FASO,
    GAMBIA,
    RUANDA,
    KENYA,
    MAURITANIA,
    MALI,
    UGANDA = 110,
    CONGO,
    LETTONIA,
    COSTA_DI_AVORIO,
    ARMENIA,
    NICARAGUA,
    CATALOGNA,
    NIGER = 117,
    BARBADOS = 118,
    IRAN,
    QATAR = 120,
    TUNISIA,
    GABON,
    TAHITI,
    MAURITIUS,
    MADAGASCAR,
    MARTINICA,
    VIETNAM,
    HAITI,
    ANTIGUA_E_BARBUDA,
    BAHREIN = 130,
    BANGLADESH,
    BELIZE,
    BENIN,
    BHUTAN,
    BOTSWANA,
    BRUNEI,
    BURUNDI,
    CAMBOGIA,
    REPUBBLICA_CENTRAFRICANA,
    CINA = 140,
    COMORE,
    YEMEN,
    CUBA,
    GIBUTI,
    DOMINICA,
    REPUBBLICA_DOMINICANA,
    EL_SALVADOR,
    GUINEA_EQUATORIALE,
    ERITREA,
    ETIOPIA = 150,
    MICRONESIA,
    FIJI,
    GRANADA,
    GUATEMALA,
    AFGHANISTAN,
    INDIA,
    INDONESIA,
    IRAQ,
    GIORDANIA,
    KAZAKISTAN = 160,
    KIRIBATI,
    KUWAIT,
    KYRGYZISTAN,
    LAOS,
    VATICANO,
    LIBANO,
    LESOTHO,
    MALAWI,
    MALESIA,
    MALDIVE = 170,
    ISOLE_MARSHALL,
    MONACO,
    MONGOLIA,
    BIRMANIA,
    NAMIBIA,
    NAURU,
    NEPAL,
    COREA_DEL_NORD,
    OMAN,
    PAKISTAN = 180,
    PALAU,
    PAPUA_NUOVA_GUINEA,
    FILIPPINE,
    SAINT_KITTS_E_NEVIS,
    SANTA_LUCIA,
    SAMOA,
    SAN_MARINO,
    SAO_TOME_E_PRINCIPE,
    ARABIA_SAUDITA,
    SEYCHELLES = 190,
    ISOLE_SALOMONE,
    SOMALIA,
    COREA_DEL_SUD,
    SRI_LANKA,
    SUDAN,
    SWAZILAND,
    SIRIA,
    TAIWAN,
    THAILANDIA,
    BAHAMAS = 200,
    TONGA,
    TURKMENISTAN,
    TUVALU,
    EMIRATI_ARABI,
    REGNO_UNITO = 205,
    VANUATU = 206
}