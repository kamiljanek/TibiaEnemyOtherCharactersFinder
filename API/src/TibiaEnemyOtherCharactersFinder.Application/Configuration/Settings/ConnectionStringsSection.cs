namespace TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;

public class ConnectionStringsSection
{
    public const string SectionName = "ConnectionStrings";
    public string SqlServer { get; init; }
    public string PostgreSql { get; init; }
}