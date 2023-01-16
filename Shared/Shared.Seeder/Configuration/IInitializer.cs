using System.Threading.Tasks;

namespace Shared.Seeder.Configuration;

public interface IInitializer
{
    int? Order { get; }
    Task Initialize();
}
