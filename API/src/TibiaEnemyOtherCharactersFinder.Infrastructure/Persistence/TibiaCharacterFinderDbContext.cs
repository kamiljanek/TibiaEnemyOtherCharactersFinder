using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

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

        modelBuilder.Entity<WorldScan>(w =>
        {
            w.Property(ws => ws.WorldScanId).IsRequired();
            w.Property(ws => ws.CharactersOnline).IsRequired();
            w.Property(ws => ws.WorldId).IsRequired();
            w.Property(ws => ws.ScanCreateDateTime).IsRequired();
            w.Property(ws => ws.IsDeleted).IsRequired().HasDefaultValue(false);
        });

        modelBuilder.Entity<Character>(c =>
        {
            c.HasIndex(ch => ch.Name);
            c.HasIndex(ch => ch.CharacterId);
            c.Property(ch => ch.CharacterId).IsRequired();
            c.Property(ch => ch.Name).IsRequired();
            c.Property(ch => ch.FoundInScan).IsRequired().HasDefaultValue(false);
            c.HasMany(ch => ch.LogoutCharacterCorrelations)
                .WithOne(ws => ws.LogoutCharacter)
                .HasForeignKey(ch => ch.LogoutCharacterId).OnDelete(DeleteBehavior.NoAction);
            c.HasMany(ch => ch.LoginCharacterCorrelations)
                .WithOne(ws => ws.LoginCharacter)
                .HasForeignKey(ch => ch.LoginCharacterId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CharacterAction>(c =>
        {
            c.HasIndex(ca => ca.CharacterName);
            c.Property(ca => ca.CharacterActionId).IsRequired();
            c.Property(ca => ca.CharacterName).IsRequired();
            c.Property(ca => ca.WorldScanId).IsRequired();
            c.Property(ca => ca.IsOnline).IsRequired();
            c.Property(ca => ca.WorldId).IsRequired();
            c.Property(ca => ca.LogoutOrLoginDate).IsRequired();
        });

        modelBuilder.Entity<CharacterCorrelation>(o =>
        {
            o.HasIndex(cc => cc.LogoutCharacterId);
            o.HasIndex(cc => cc.LoginCharacterId);
            o.HasKey(cc => cc.CorrelationId);
            o.Property(cc => cc.CorrelationId).IsRequired();
            o.Property(cc => cc.LogoutCharacterId).IsRequired();
            o.Property(cc => cc.LoginCharacterId).IsRequired();
            o.Property(cc => cc.NumberOfMatches).IsRequired();
            o.Property(cc => cc.CreateDate).IsRequired().HasDefaultValue(new DateOnly(2022, 12, 06));
            o.Property(cc => cc.LastMatchDate).IsRequired().HasDefaultValue(new DateOnly(2022, 12, 06));
        });
    }
}