
namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class CharacterWithCorrelationsResult
{
    public string Name { get; set; }
    public string World { get; set; }
    public string Vocation { get; set; }
    public int Level { get; set; }
    public string LastLogin { get; set; }
    public List<string> FormerNames { get; set; }
    public List<string> FormerWorlds { get; set; }
    public bool Traded { get; set; }
    public List<CorrelationResult> Correlations { get; set; }
}