/* Data una directory ed una dimensione in byte, legge
 * tutti i file di quella dimensione e torna l'elenco degli
 * offset a valore sempre costante in tutti i file
 */


//using System.Drawing;

//var b = new Bitmap();

String path = args.Length > 1 ? args[1] : String.Empty;
int bytes = args.Length > 2 ? int.Parse(args[2].Trim()) : 0;

while (path.Length == 0)
{
    Console.WriteLine("Inserisci percorso file (poi premi invio) : ");
    path = Console.ReadLine();
    path = path.Trim();
}

while (bytes == 0)
{
    Console.WriteLine("Inserisci la dimensione dei file da confrontare : ");
    try
    {
        String b = Console.ReadLine();
        b = b.Trim();
        if (String.IsNullOrEmpty(b))
        {
            bytes = 0;
        }
        else bytes = int.Parse(b);
    }
    catch
    {

        bytes = 0;
    }
}

Console.WriteLine("Lettura file in corso... ");

Dictionary<int, int> elenco = new();
for (int i = 0; i < bytes; i++) elenco.Add(i, -1);

var f = Directory.GetFiles($"{path}");

foreach (var file in f)
{
    var contenuto = File.ReadAllBytes(file);

    if (contenuto.Length == bytes)
    {
        foreach (var offset in elenco.Keys)
        {
            if (elenco[offset] == -1)
            {
                elenco[offset] = contenuto[offset];
            }
            else if (elenco[offset] != contenuto[offset])
            {
                elenco.Remove(offset);
                if (!elenco.Any())
                {
                    Console.WriteLine("Non ci sono byte a valore costante in comune ai file esaminati");
                    Console.WriteLine("Premi un tasto per terminare il programma");
                    Console.ReadKey();
                    return 0;
                }
            }
        }
        Console.WriteLine($"Letto il file {file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1)} => possibili byte ripetuti rimasti = {elenco.Count}");

    }
}

Console.WriteLine($"Esaminati tutti i file, ci sono {elenco.Count} byte ripetuti in tutti i file");
Console.WriteLine($"Elaborazione e scrittura dei dettagli nel file {path}{Path.DirectorySeparatorChar}RISULTATI.TXT");

var fs = File.AppendText($"{path}{Path.DirectorySeparatorChar}RISULTATI.TXT");
fs.AutoFlush = true;

fs.WriteLine("Offset con byte sempre ripetuti");
fs.WriteLine("Gli offset sono indicati con valore decimale seguito da valore esadecimale");
fs.WriteLine("Offset (0x)|Byte");

foreach (var b in elenco)
{
    fs.WriteLine($"{b.Key} (0x{b.Key.ToString("X")}) | {b.Value} (0x{b.Value.ToString("X")})");
}

fs.WriteLine();

fs.WriteLine("Offset con byte differenti");

if (elenco.Count == bytes)
{
    fs.WriteLine("NESSUNO, tutti i file esaminati sono identici");
}
else
{

    for (int i = 0; i < bytes; i++)
    {
        if (!elenco.ContainsKey(i))
        {
            fs.WriteLine($"{i} (0x{i.ToString("X")})");
        }
    }
}

fs.Close();
Console.WriteLine("Fatto!");
Console.WriteLine("Premi un tasto per terminare il programma");
Console.ReadKey();
return 0;