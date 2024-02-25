using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector.Validators;

public class NameDetectorValidator : INameDetectorValidator
{
    public bool IsCharacterChangedName(CharacterResult fechedCharacter, Character character)
    {
        return fechedCharacter?.Name?.ToLower() != character.Name;
    }

    public bool IsCharacterTraded(CharacterResult fechedCharacter)
    {
        return fechedCharacter.Traded;
    }

    public bool IsCharacterExist(CharacterResult fechedCharacter)
    {
        return !string.IsNullOrWhiteSpace(fechedCharacter.Name);
    }
}