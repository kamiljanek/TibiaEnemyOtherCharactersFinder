using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector.Validators;

public class NameDetectorValidator : INameDetectorValidator
{
    public bool IsCharacterChangedName(TibiaDataCharacterResult fechedCharacter, Character character)
    {
        return fechedCharacter?.Character?.Character.Name?.ToLower() != character.Name;
    }

    public bool IsCharacterTraded(TibiaDataCharacterResult fechedCharacter)
    {
        return fechedCharacter.Character.Character.Traded;
    }

    public bool IsCharacterExist(TibiaDataCharacterResult fechedCharacter)
    {
        return !string.IsNullOrWhiteSpace(fechedCharacter.Character.Character.Name);
    }
}