using System.Reflection;
using TibiaEnemyOtherCharactersFinder.Application.Configuration;


namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration
{
    public static class Assemblies
    {
        public static Assembly ApplicationAssembly { get; } = typeof(AssembliesHook).Assembly;
    }
}