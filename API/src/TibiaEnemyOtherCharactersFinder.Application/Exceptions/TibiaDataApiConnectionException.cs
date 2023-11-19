namespace TibiaEnemyOtherCharactersFinder.Application.Exceptions;

public class TibiaDataApiConnectionException : TibiaEnemyOtherCharacterFinderException
{
    public TibiaDataApiConnectionException() : base("Error occurred while attempting to connect to TibiaData API. Please check https://tibiadata.com .")
    {
    }
}