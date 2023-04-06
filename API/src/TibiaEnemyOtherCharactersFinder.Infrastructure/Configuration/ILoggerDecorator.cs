namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public interface ILoggerDecorator
{
    Task Decorate(Func<Task> function);
    Task Decorate<T>(Func<T, Task> function, T parameter);
}