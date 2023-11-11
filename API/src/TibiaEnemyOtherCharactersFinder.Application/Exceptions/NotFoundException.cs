namespace TibiaEnemyOtherCharactersFinder.Application.Exceptions;

public class NotFoundException : TibiaEnemyOtherCharacterFinderException
{
    public NotFoundException(string propertyName, string entity) : base(BuildErrorMessage(propertyName, entity))
    {
    }

    private static string BuildErrorMessage(string propertyName, string entity)
    {
        return $"{propertyName} ({entity}) not found.";
    }
}