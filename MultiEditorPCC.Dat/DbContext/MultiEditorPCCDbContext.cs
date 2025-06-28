using Microsoft.EntityFrameworkCore;
using MultiEditorPCC.Dat.DbSet;

namespace MultiEditorPCC.Dat.DbContext;
public class MultiEditorPCCDbContext : Microsoft.EntityFrameworkCore.DbContext
{

    public DbSet<ProgettoEditorPCC> Progetti { get; set; }
    public DbSet<CartellaGioco> CartelleGioco { get; set; }

    public DbSet<Squadra> Squadre { get; set; }
    public DbSet<Giocatore> Giocatori { get; set; }
    public DbSet<Allenatore> Allenatori { get; set; }
    public DbSet<Stadio> Stadi { get; set; }

    /// <summary>
    /// Memorizzo se uso il database dell'editor o un singolo database di progetto,
    /// nel primo caso ho dbset dei Progetti e di varie impostazioni, non necessarie
    /// nei database del singolo progetto
    /// </summary>
    private bool dbEditor { get; set; }

    public MultiEditorPCCDbContext(DbContextOptions options, bool dbProgettoSingolo = false) : base(options)
    {
        dbEditor = !dbProgettoSingolo;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartellaGioco>().HasKey(c => c.Path);
        modelBuilder.Entity<PunteggioGiocatore>().HasKey(p => p.Tipo);
        base.OnModelCreating(modelBuilder);
    }


}
