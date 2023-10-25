namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class TibiaDataCharacterInformationResult
{
    public CharactersResult characters { get; init; }
}

public class CharactersResult
{
    public CharacterResult character { get; init; }
    public IReadOnlyList<OtherCharacterResult> other_characters { get; init; }
}

public class OtherCharacterResult
{
    public string name { get; init; }
}

public class CharacterResult
{
    public IReadOnlyList<string> former_names { get; init; }
    public IReadOnlyList<string> former_worlds { get; init; }
    public string last_login { get; init; }
    public int level { get; init; }
    public string name { get; init; }
    public bool traded { get; init; }
    public string vocation { get; init; }
    public string world { get; init; }
}