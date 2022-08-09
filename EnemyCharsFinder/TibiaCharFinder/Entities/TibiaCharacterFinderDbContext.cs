using Microsoft.EntityFrameworkCore;

namespace TibiaCharacterFinderAPI.Entities
{
    public class TibiaCharacterFinderDbContext : DbContext
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=TibiaCharFinderDb; Trusted_Connection=True";

        public DbSet<World> Worlds { get; set; }
        public DbSet<WorldScan> WorldScans { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<WorldCorrelation> WorldCorrelations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<World>(w =>
            {
                w.Property(wo => wo.Name).IsRequired();
                w.Property(wo => wo.Url).IsRequired();
                w.Property(wo => wo.IsAvailable).IsRequired();
                w.HasMany(wo => wo.WorldScans)
                    .WithOne(ws => ws.World)
                    .HasForeignKey(ws => ws.WorldId);
            });

            modelBuilder.Entity<WorldScan>(ws =>
            {
                ws.Property(ws => ws.CharactersOnline).IsRequired();
            });

            modelBuilder.Entity<Character>(c =>
            {
                c.Property(ch => ch.Name).IsRequired();
                c.HasMany(ch => ch.WorldCorrelations)
                    .WithOne(ch => ch.Character)
                    .HasForeignKey(ch => ch.CharacterId);
            });

            modelBuilder.Entity<WorldCorrelation>(o =>
            {
                o.Property(wc => wc.CharacterId).IsRequired();
                o.Property(wc => wc.PossibleOtherCharactersId).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
