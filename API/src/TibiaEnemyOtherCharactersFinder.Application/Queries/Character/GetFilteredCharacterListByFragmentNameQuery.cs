using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Application.Exceptions;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

public record GetFilteredCharacterListByFragmentNameQuery(
    string SearchText,
    int Page,
    int PageSize,
    bool SearchInMiddle) : IRequest<List<string>>;

public class
    GetFilteredCharacterListByFragmentNameQueryHandler : IRequestHandler<GetFilteredCharacterListByFragmentNameQuery,
        List<string>>
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public GetFilteredCharacterListByFragmentNameQueryHandler(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<List<string>> Handle(GetFilteredCharacterListByFragmentNameQuery request,
        CancellationToken cancellationToken)
    {
        var minLength = 2; // Minimum required length for a fragment name
        if (string.IsNullOrWhiteSpace(request.SearchText) || request.SearchText.Length < minLength)
        {
            throw new NameTooShortException(nameof(request.SearchText), minLength);
        }

        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        var parameters = new
        {
            SearchText = request.SearchText.ToLower(),
            Page = request.Page,
            PageSize = request.PageSize
        };

        return request.SearchInMiddle
            ? (await connection.QueryAsync<string>(GenerateQueries.GetFilteredCharactersSearchInMiddle, parameters)).ToList()
            : (await connection.QueryAsync<string>(GenerateQueries.GetFilteredCharactersStartsAtSearchText, parameters)).ToList();
    }
}
