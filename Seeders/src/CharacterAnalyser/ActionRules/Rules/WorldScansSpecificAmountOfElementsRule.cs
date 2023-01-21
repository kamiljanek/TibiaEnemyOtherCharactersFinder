using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace CharacterAnalyser.ActionRules.Rules;

public class WorldScansSpecificAmountOfElementsRule : IRule
{
    private const int _numberOfElements = 2;
    private readonly List<WorldScan> _worldScans;

    public WorldScansSpecificAmountOfElementsRule(List<WorldScan> worldScans)
    {
        _worldScans = worldScans;
    }

    public bool IsBroken() => _worldScans.Count != _numberOfElements;
}