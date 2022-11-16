﻿using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using Shared.Providers;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.Character
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
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                var parameters = new { CharacterName = request.Name };
                var result = await connection.QueryAsync<CharacterWithCorrelationsResult>(GenerateQueries.NpgsqlGetOtherPossibleCharacters, parameters);
                return result.ToList();
            }
        }
    }
}