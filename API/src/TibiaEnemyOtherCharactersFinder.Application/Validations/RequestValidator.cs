using FluentValidation.Results;
using TibiaEnemyOtherCharactersFinder.Application.Exceptions;

namespace TibiaEnemyOtherCharactersFinder.Application.Validations;

public class RequestValidator : IRequestValidator
{
    private const int MinLength = 2; // Minimum length for a fragment name
    private const int MaxLength = 50; // Maximum length for a fragment name
    public RequestValidator()
    {
    }

    public void ValidSearchTextLenght(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < MinLength || searchText.Length > MaxLength)
        {
            throw new TibiaValidationException(new ValidationFailure(nameof(searchText),
                $"Input length must be between {MinLength}-{MaxLength} characters long."));
        }
    }

    public void ValidSearchTextCharacters(string searchText)
    {
        string trimmedSearchText = searchText.Trim();

        if (!trimmedSearchText.All(c => (char.IsLetter(c) && (c is >= 'a' and <= 'z' || c is >= 'A' and <= 'Z')) || char.IsWhiteSpace(c) || c is '+'))
        {
            throw new TibiaValidationException(new ValidationFailure(nameof(searchText), "Input has unacceptable characters."));
        }
    }

    public void ValidNumberParameterRange(int parameterInput, string parameterName, int lowerLimit)
    {
        if (parameterInput < lowerLimit)
        {
            throw new TibiaValidationException(new ValidationFailure(parameterName,
                $"{parameterName} must be greater than {lowerLimit-1}."));
        }
    }

    public void ValidNumberParameterRange(int parameterInput, string parameterName, int lowerLimit, int upperLimit)
    {
        if (parameterInput < lowerLimit || parameterInput > upperLimit)
        {
            throw new TibiaValidationException(new ValidationFailure(parameterName,
                $"{parameterName} must be between {lowerLimit}-{upperLimit}."));
        }
    }
}