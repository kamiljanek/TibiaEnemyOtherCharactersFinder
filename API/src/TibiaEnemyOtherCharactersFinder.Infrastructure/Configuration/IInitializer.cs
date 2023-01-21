using System.Threading.Tasks;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public interface IInitializer
{
    int? Order { get; }
    Task Initialize();
}
