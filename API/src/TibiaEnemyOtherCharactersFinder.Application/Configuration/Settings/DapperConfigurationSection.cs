namespace TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;

public class DapperConfigurationSection
{
    public const string SectionName = "Dapper";
    public int CommandTimeout { get; init; }
}