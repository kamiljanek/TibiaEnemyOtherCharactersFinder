namespace CharacterAnalyser.ActionRules.Rules;

public class CharacterNameListCannotBeEmptyRule : IRule
{
    private readonly IReadOnlyList<string> _names;

    public CharacterNameListCannotBeEmptyRule(IReadOnlyList<string> names)
	{
        _names = names;
    }

    public bool IsBroken() => !_names.Any();
}