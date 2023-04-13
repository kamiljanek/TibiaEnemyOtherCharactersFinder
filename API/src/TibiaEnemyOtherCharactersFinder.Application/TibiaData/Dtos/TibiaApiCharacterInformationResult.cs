namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class TibiaApiCharacterInformationResult
{
    public CharactersResult characters { get; set; }
}

public class CharactersResult
{
    public CharacterResult character { get; set; }
}

public class CharacterResult
{
    public List<string> former_names { get; set; }
    public List<string> former_worlds { get; set; }
    public string last_login { get; set; }
    public int level { get; set; }
    public string name { get; set; }
    public bool traded { get; set; }
    public string vocation { get; set; }
    public string world { get; set; }
}