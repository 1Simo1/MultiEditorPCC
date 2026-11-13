using System.Globalization;
using System.Text;

namespace MultiEditorPCC.Shared.Lng;

public class LngService
{
    public String AppLng { get; set; }

    private Dictionary<String, String> Lng = new Dictionary<String, String>();

    private readonly String CSVHeaderLng = "Cod;Msg";

    public LngService()
    {
        if (!Directory.Exists("Lng")) Directory.CreateDirectory("Lng");

        if (!File.Exists("Lng/Lng.txt"))
        {
            File.WriteAllText("Lng/Lng.txt", CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper());
        }

        AppLng = File.ReadAllText("Lng/Lng.txt").Trim().ToUpper();
        InitDictionary();

    }
    public String Traduci(String Codice)
    {
        String Testo = Codice;

        if (Lng.ContainsKey(Codice)) return Lng[Codice].Trim('"');

        Lng.Add(Codice, Codice);

        StringBuilder sb = new();
        sb.Append(CSVHeaderLng);

        foreach (var riga in Lng)
        {
            sb.AppendLine();
            sb.Append($"{riga.Key};\"{riga.Value}\"");
        }

        File.WriteAllText($"Lng/{AppLng}.csv", sb.ToString());


        return Testo;
    }

    public bool SetAppLng(String AppLng)
    {
        if (this.AppLng != AppLng)
        {
            this.AppLng = AppLng;
            File.WriteAllText("Lng/Lng.txt", this.AppLng);
            Lng = InitDictionary();
            return true;
        }
        return false;
    }

    private Dictionary<string, string> InitDictionary()
    {
        Lng = new();

        if (!File.Exists($"Lng/{AppLng}.csv"))
        {
            File.WriteAllText($"Lng/{AppLng}.csv", CSVHeaderLng);
            return Lng;
        }

        foreach (var riga in File.ReadAllLines($"Lng/{AppLng}.csv"))
        {
            if (riga.Equals(CSVHeaderLng)) continue;
            var val = riga.Split(';');
            if (val.Length != 2) continue;
            Lng.Add(val[0], val[1]);
        }

        return Lng;
    }
}
