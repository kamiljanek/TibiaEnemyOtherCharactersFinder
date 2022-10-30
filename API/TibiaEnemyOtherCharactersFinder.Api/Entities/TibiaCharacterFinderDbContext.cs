using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.EntityFrameworkCore;

namespace TibiaEnemyOtherCharactersFinder.Api.Entities
{
    public class TibiaCharacterFinderDbContext : DbContext
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=TibiaCharFinderDb; Trusted_Connection=True";

        public DbSet<World> Worlds { get; set; }
        public DbSet<WorldScan> WorldScans { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterCorrelation> CharacterCorrelations { get; set; }
        public DbSet<CharacterLogoutOrLogin> CharacterLogoutOrLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<World>(w =>
            {
                w.Property(wo => wo.Name).IsRequired();
                w.Property(wo => wo.Url).IsRequired();
                w.Property(wo => wo.IsAvailable).IsRequired();
                w.HasMany(wo => wo.WorldScans)
                    .WithOne(ws => ws.World)
                    .HasForeignKey(ws => ws.WorldId).OnDelete(DeleteBehavior.NoAction);
                w.HasMany(wo => wo.Characters)
                    .WithOne(wo => wo.World)
                    .HasForeignKey(wo => wo.WorldId).OnDelete(DeleteBehavior.NoAction);
                w.HasMany(wo => wo.CharacterLogoutOrLogins)
                    .WithOne(wo => wo.World)
                    .HasForeignKey(wo => wo.WorldId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<WorldScan>(ws =>
            {
                ws.Property(ws => ws.WorldScanId).IsRequired();
                ws.Property(ws => ws.CharactersOnline).IsRequired();
                ws.Property(ws => ws.WorldId).IsRequired();
                ws.Property(ws => ws.ScanCreateDateTime).IsRequired();
                ws.Property(ws => ws.IsDeleted).IsRequired().HasDefaultValue(false);
            });

            modelBuilder.Entity<Character>(c =>
            {
                c.Property(ch => ch.CharacterId).IsRequired();
                c.Property(ch => ch.Name).IsRequired();
                c.HasMany(ch => ch.LogoutWorldCorrelations)
                    .WithOne(ws => ws.LogoutCharacter)
                    .HasForeignKey(ch => ch.LogoutCharacterId).OnDelete(DeleteBehavior.NoAction);
                c.HasMany(ch => ch.LoginWorldCorrelations)
                    .WithOne(ws => ws.LoginCharacter)
                    .HasForeignKey(ch => ch.LoginCharacterId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CharacterLogoutOrLogin>(c =>
            {
                c.Property(ch => ch.CharacterLogoutOrLoginId).IsRequired();
                c.Property(ch => ch.CharacterName).IsRequired();
                c.Property(ch => ch.WorldScanId).IsRequired();
                c.Property(ch => ch.IsOnline).IsRequired();
                c.Property(ch => ch.WorldId).IsRequired();
            });

            modelBuilder.Entity<CharacterCorrelation>(o =>
            {
                o.HasKey(wc => wc.CorrelationId);
                o.Property(wc => wc.LogoutCharacterId).IsRequired();
                o.Property(wc => wc.LoginCharacterId).IsRequired();
                o.Property(wc => wc.NumberOfMatches).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public string GetConnectionString() => _connectionString;
    }
}
