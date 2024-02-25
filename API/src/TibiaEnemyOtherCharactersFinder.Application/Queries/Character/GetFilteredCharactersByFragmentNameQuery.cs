using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Application.Validations;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

public record GetFilteredCharactersByFragmentNameQuery(
    string SearchText,
    int Page,
    int PageSize) : IRequest<FilteredCharactersDto>;

public class GetFilteredCharacterListByFragmentNameQueryHandler : IRequestHandler<GetFilteredCharactersByFragmentNameQuery,
        FilteredCharactersDto>
{
    private readonly IDapperConnectionProvider _connectionProvider;
    private readonly IRequestValidator _validator;

    public GetFilteredCharacterListByFragmentNameQueryHandler(IDapperConnectionProvider connectionProvider,
        IRequestValidator validator)
    {
        _connectionProvider = connectionProvider;
        _validator = validator;
    }

    public async Task<FilteredCharactersDto> Handle(GetFilteredCharactersByFragmentNameQuery request,
        CancellationToken cancellationToken)
    {
        _validator.ValidSearchTextLenght(request.SearchText);
        _validator.ValidSearchTextCharacters(request.SearchText);
        _validator.ValidNumberParameterRange(request.Page, nameof(request.Page), 1);
        _validator.ValidNumberParameterRange(request.PageSize, nameof(request.PageSize), 1, 100);

        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        var parameters = new
        {
            SearchText = request.SearchText.ToLower(),
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result =
            (await connection.QueryAsync<(string name, int totalCount)>(GenerateQueries.GetFilteredCharactersWithCount,
                parameters)).ToList();

        var characterMatching = result.Count == 0
            ? new FilteredCharactersDto { TotalCount = 0, Names = Array.Empty<string>() }
            : new FilteredCharactersDto { TotalCount = result.First().totalCount, Names = result.Select(n => n.name).ToArray() };

        return characterMatching;
    }
}