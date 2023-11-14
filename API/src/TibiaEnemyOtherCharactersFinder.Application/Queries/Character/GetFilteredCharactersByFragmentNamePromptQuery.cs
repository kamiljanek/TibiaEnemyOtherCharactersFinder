using Dapper;
using FluentValidation.Results;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Exceptions;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

public record GetFilteredCharactersByFragmentNamePromptQuery(
    string SearchText,
    int Page,
    int PageSize) : IRequest<List<string>>;

public class
    GetFilteredCharacterListByFragmentNameQueryNewHandler : IRequestHandler<GetFilteredCharactersByFragmentNamePromptQuery, List<string>>
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public GetFilteredCharacterListByFragmentNameQueryNewHandler(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<List<string>> Handle(GetFilteredCharactersByFragmentNamePromptQuery request, CancellationToken cancellationToken)
    {
        var minLength = 2; // Minimum required length for a fragment name
        if (string.IsNullOrWhiteSpace(request.SearchText) || request.SearchText.Length < minLength)
        {
            throw new TibiaValidationException(new ValidationFailure(nameof(request.SearchText),
                $"Input is too short. It must be at least {minLength} characters long."));
        }

        if (request.PageSize < 1)
        {
            throw new TibiaValidationException(new ValidationFailure(nameof(request.PageSize),
                $"{nameof(request.PageSize)} must be greater than 0."));
        }

        if (request.Page < 1)
        {
            throw new TibiaValidationException(new ValidationFailure(nameof(request.Page),
                $"{nameof(request.Page)} must be greater than 0."));
        }

        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        var parameters = new
        {
            SearchText = request.SearchText.ToLower(),
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = (await connection.QueryAsync<string>(GenerateQueries.GetFilteredCharactersStartsAtSearchText, parameters)).ToList();

        if (result.Count == 0)
        {
            throw new NotFoundException(nameof(Character), request.SearchText);
        }

        return result;
    }
}
