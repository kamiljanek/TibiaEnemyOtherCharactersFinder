namespace CharacterAnalyser.ActionRules.Rules;

public class CharacterNameListCannotBeEmptyRule : IRule
{
    private readonly List<string> _names;

    public CharacterNameListCannotBeEmptyRule(List<string> names)
	{
        _names = names;
    }

    public bool IsBroken() => !_names.Any();
}