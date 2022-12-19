using Microsoft.EntityFrameworkCore;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
public interface ITibiaCharacterFinderDbContext
{
    DbSet<CharacterAction> CharacterActions { get; set; }
    DbSet<CharacterCorrelation> CharacterCorrelations { get; set; }
    DbSet<Character> Characters { get; set; }
    DbSet<World> Worlds { get; set; }
    DbSet<WorldScan> WorldScans { get; set; }
}