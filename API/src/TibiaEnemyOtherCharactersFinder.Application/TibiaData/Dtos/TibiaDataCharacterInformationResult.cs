namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class TibiaDataCharacterInformationResult
{
    public CharactersResult characters { get; set; }
}

public class CharactersResult
{
    public CharacterResult character { get; set; }
    public IReadOnlyList<OtherCharacterResult> other_characters { get; set; }
}

public class OtherCharacterResult
{
    public string name { get; set; }
}

public class CharacterResult
{
    public IReadOnlyList<string> former_names { get; set; }
    public IReadOnlyList<string> former_worlds { get; set; }
    public string last_login { get; set; }
    public int level { get; set; }
    public string name { get; set; }
    public bool traded { get; set; }
    public string vocation { get; set; }
    public string world { get; set; }
}