namespace CharacterAnalyser.ActionRules.Rules;

public class CharacterNameListCannotBeEmpty : IRule
{
    private readonly List<string> _names;

    public CharacterNameListCannotBeEmpty(List<string> names)
	{
        _names = names;
    }

    public bool IsBroken() => !_names.Any();
}