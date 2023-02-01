using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

public class TibiaCharacterFinderDbContext : DbContext, ITibiaCharacterFinderDbContext
{
    public TibiaCharacterFinderDbContext(DbContextOptions<TibiaCharacterFinderDbContext> options) : base(options)
    {
    }

    public DbSet<World> Worlds { get; set; }
    public DbSet<WorldScan> WorldScans { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterCorrelation> CharacterCorrelations { get; set; }
    public DbSet<CharacterAction> CharacterActions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);
        // UNDONE: może odkomentować
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

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

        modelBuilder.Entity<CharacterAction>(c =>
        {
            c.Property(ch => ch.CharacterActionId).IsRequired();
            c.Property(ch => ch.CharacterName).IsRequired();
            c.Property(ch => ch.WorldScanId).IsRequired();
            c.Property(ch => ch.IsOnline).IsRequired();
            c.Property(ch => ch.WorldId).IsRequired();
            c.Property(ch => ch.LogoutOrLoginDate).IsRequired();
        });

        modelBuilder.Entity<CharacterCorrelation>(o =>
        {
            o.HasKey(wc => wc.CorrelationId);
            o.Property(wc => wc.LogoutCharacterId).IsRequired();
            o.Property(wc => wc.LoginCharacterId).IsRequired();
            o.Property(wc => wc.NumberOfMatches).IsRequired();
            o.Property(wc => wc.CreateDate).IsRequired().HasDefaultValue(new DateOnly(2022, 12, 06));
            o.Property(wc => wc.LastMatchDate).IsRequired().HasDefaultValue(new DateOnly(2022, 12, 06));
        });
    }

    // UNDONE: mozliwe ze do usuniecia
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ApplicationSettingsSections.FileName)
                .Build();

            var connectionString = configuration.GetConnectionString(ApplicationSettingsSections.ConnectionStringsSection);
        optionsBuilder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
        }
    }
}