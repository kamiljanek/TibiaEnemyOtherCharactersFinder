using MediatR;
using TibiaEnemyOtherCharactersFinderApi.Dtos;
using TibiaEnemyOtherCharactersFinderApi.Providers;

namespace TibiaEnemyOtherCharactersFinderApi.Queries.Character
{
    public record GetCharacterQuery : IRequest<CharacterResult>;

    public class GetCharacterQueryHandler : IRequestHandler<GetCharacterQuery, CharacterResult>
    {
        private readonly IDapperConnectionProvider _connectionProvider;

        public GetCharacterQueryHandler(IDapperConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<CharacterResult> Handle(GetCharacterQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.TibiaDB))
            {

            }
            throw new NotImplementedException();
        }
    }
}
