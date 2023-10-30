using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

public interface ITibiaCharacterFinderDbContext
{
    DatabaseFacade Database { get; }
    ChangeTracker ChangeTracker { get; }
    EntityEntry<TEntry> Entry<TEntry>(TEntry entry) where TEntry : class;

    DbSet<CharacterAction> CharacterActions { get; set; }
    DbSet<CharacterCorrelation> CharacterCorrelations { get; set; }
    DbSet<Character> Characters { get; set; }
    DbSet<World> Worlds { get; set; }
    DbSet<WorldScan> WorldScans { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}