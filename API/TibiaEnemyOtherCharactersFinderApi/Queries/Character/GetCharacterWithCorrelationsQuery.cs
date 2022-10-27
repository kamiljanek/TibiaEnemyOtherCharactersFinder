using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinderApi.Dtos;
using TibiaEnemyOtherCharactersFinderApi.Providers;

namespace TibiaEnemyOtherCharactersFinderApi.Queries.Character
{
    public class GetCharacterWithCorrelationsQuery : IRequest<List<CharacterWithCorrelationsResult>>
    {
        public string Name { get; }
        public GetCharacterWithCorrelationsQuery(string name)
        {
            Name = name;
        }
    }
    public class GetCharacterWithCorrelationsQueryHandler : IRequestHandler<GetCharacterWithCorrelationsQuery, List<CharacterWithCorrelationsResult>>
    {
        private readonly IDapperConnectionProvider _connectionProvider;

        public GetCharacterWithCorrelationsQueryHandler(IDapperConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<List<CharacterWithCorrelationsResult>> Handle(GetCharacterWithCorrelationsQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.TibiaDB))
            {
                var parameters = new { CharacterName = request.Name };
                var result = await connection.QueryAsync<CharacterWithCorrelationsResult>(GenerateQueries.GetOtherPossibleCharacters, parameters);
                return result.ToList();
            }
        }
    }
}
