namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public interface ILoggerDecorator<in T>
{
    Task Decorate(Func<Task> function, T parameter);
}

public interface ILoggerDecorator
{
    Task Decorate(Func<Task> function);
}

public interface IParameterizedLoggerDecorator<T>
{
    Task Decorate(Func<T, Task> function, T parameter);
}

