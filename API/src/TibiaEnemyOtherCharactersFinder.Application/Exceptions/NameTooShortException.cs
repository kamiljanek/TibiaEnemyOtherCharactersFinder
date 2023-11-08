namespace TibiaEnemyOtherCharactersFinder.Application.Exceptions;

public class NameTooShortException : ArgumentException
{
    public NameTooShortException(string paramName, int minLength) : base("Name is too short. It must be at least " + minLength + " characters long.", paramName)
    {
    }
}