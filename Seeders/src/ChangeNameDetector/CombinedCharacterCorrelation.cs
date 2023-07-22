using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector;

public class CombinedCharacterCorrelation
{
    public CharacterCorrelation FirstCombinedCorrelation { get; set; }
    public CharacterCorrelation SecondCombinedCorrelation { get; set; }
}