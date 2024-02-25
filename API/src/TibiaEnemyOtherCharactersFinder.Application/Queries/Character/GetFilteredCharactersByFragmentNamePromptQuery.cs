using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Exceptions;
using TibiaEnemyOtherCharactersFinder.Application.Validations;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

public record GetFilteredCharactersByFragmentNamePromptQuery(
    string SearchText,
    int Page,
    int PageSize) : IRequest<List<string>>;

public class GetFilteredCharacterListByFragmentNameQueryNewHandler : IRequestHandler<GetFilteredCharactersByFragmentNamePromptQuery, List<string>>
{
    private readonly IDapperConnectionProvider _connectionProvider;
    private readonly IRequestValidator _validator;

    public GetFilteredCharacterListByFragmentNameQueryNewHandler(IDapperConnectionProvider connectionProvider, IRequestValidator validator)
    {
        _connectionProvider = connectionProvider;
        _validator = validator;
    }

    public async Task<List<string>> Handle(GetFilteredCharactersByFragmentNamePromptQuery request, CancellationToken cancellationToken)
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

        var result = (await connection.QueryAsync<string>(GenerateQueries.GetFilteredCharactersStartsAtSearchText, parameters)).ToList();

        if (result.Count == 0)
        {
            throw new NotFoundException(nameof(Character), request.SearchText);
        }

        return result;
    }
}