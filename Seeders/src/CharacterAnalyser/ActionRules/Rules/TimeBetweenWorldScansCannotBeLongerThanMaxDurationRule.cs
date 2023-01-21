using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace CharacterAnalyser.ActionRules.Rules;

public class TimeBetweenWorldScansCannotBeLongerThanMaxDurationRule : IRule
{
    private const int _maxDurationMinutes = 6;
    private readonly List<WorldScan> _worldScans;

    public TimeBetweenWorldScansCannotBeLongerThanMaxDurationRule(List<WorldScan> worldScans)
    {
        _worldScans = worldScans;
    }
    public bool IsBroken()
    {
        var timeDifference = _worldScans[1].ScanCreateDateTime - _worldScans[0].ScanCreateDateTime;

        return timeDifference.TotalMinutes > _maxDurationMinutes;
    }
}