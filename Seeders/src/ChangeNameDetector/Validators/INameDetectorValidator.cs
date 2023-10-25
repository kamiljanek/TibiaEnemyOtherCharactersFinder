using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector.Validators;

public interface INameDetectorValidator
{
    public bool IsCharacterChangedName(TibiaDataCharacterInformationResult fechedCharacter, Character character);
    public bool IsCharacterTraded(TibiaDataCharacterInformationResult fechedCharacter);
    public bool IsCharacterExist(TibiaDataCharacterInformationResult fechedCharacter);
}