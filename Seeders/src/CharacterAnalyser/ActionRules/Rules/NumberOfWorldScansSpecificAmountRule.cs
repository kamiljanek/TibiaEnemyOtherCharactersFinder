using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace CharacterAnalyser.ActionRules.Rules;

public class NumberOfWorldScansSpecificAmountRule : IRule
{
    private const int _numberOfElements = 2;
    private readonly List<WorldScan> _worldScans;

    public NumberOfWorldScansSpecificAmountRule(List<WorldScan> worldScans)
    {
        _worldScans = worldScans;
    }

    public bool IsBroken() => _worldScans.Count != _numberOfElements;
}