namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class CharacterResult
{
    public string Name { get; init; }
    public bool Traded { get; init; }
    public string World { get; init; }
    public string Vocation { get; init; }
    public int Level { get; init; }
    public string LastLogin { get; init; }
    public string[] FormerNames { get; init; }
    public string[] FormerWorlds { get; init; }
    public string[] OtherCharacters { get; init; }
}