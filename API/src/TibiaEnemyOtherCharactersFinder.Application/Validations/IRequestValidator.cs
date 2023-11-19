using System.Runtime.CompilerServices;

namespace TibiaEnemyOtherCharactersFinder.Application.Validations;

public interface IRequestValidator
{
    void ValidSearchTextLenght(string searchText);
    void ValidSearchTextCharacters(string searchText);
    void ValidNumberParameterRange(int parameterInput, string parameterName, int lowerLimit);
    void ValidNumberParameterRange(int parameterInput, string parameterName, int lowerLimit, int upperLimit);
}