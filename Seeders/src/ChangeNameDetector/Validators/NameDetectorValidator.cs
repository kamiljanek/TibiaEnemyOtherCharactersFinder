using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector.Validators;

public class NameDetectorValidator : INameDetectorValidator
{
    public bool IsCharacterFoundInFormerNames(TibiaDataCharacterInformationResult fechedCharacter, Character character)
    {
        return fechedCharacter.characters.character.former_names.Any(n => string.Equals(n, character.Name, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsCharacterChangedName(TibiaDataCharacterInformationResult fechedCharacter, Character character)
    {
        return fechedCharacter.characters.character.name?.ToLower() != character.Name;
    }

    public bool IsCharacterTraded(TibiaDataCharacterInformationResult fechedCharacter)
    {
        return fechedCharacter.characters.character.traded;
    }

    public bool IsCharacterExist(TibiaDataCharacterInformationResult fechedCharacter)
    {
        return !string.IsNullOrWhiteSpace(fechedCharacter.characters.character.name);
    }
}