namespace CharacterAnalyser.ActionRules;

public abstract class ActionRule
{
    public static bool IsBroken<T>(T rule) where T : IRule
    {
        return rule.IsBroken();
    }
}