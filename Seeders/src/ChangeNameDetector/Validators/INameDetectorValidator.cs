using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector.Validators;

public interface INameDetectorValidator
{
    public bool IsCharacterChangedName(TibiaDataCharacterResult fechedCharacter, Character character);
    public bool IsCharacterTraded(TibiaDataCharacterResult fechedCharacter);
    public bool IsCharacterExist(TibiaDataCharacterResult fechedCharacter);
}