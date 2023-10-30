
namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class CharacterWithCorrelationsResult
{
    public string Name { get; set; }
    public string World { get; set; }
    public string Vocation { get; set; }
    public int Level { get; set; }
    public string LastLogin { get; set; }
    public IReadOnlyList<string> FormerNames { get; set; }
    public IReadOnlyList<string> FormerWorlds { get; set; }
    public bool Traded { get; set; }
    public IReadOnlyList<string> OtherVisibleCharacters { get; set; }
    public IReadOnlyList<CorrelationResult> PossibleInvisibleCharacters { get; set; }
}