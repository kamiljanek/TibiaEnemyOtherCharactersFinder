namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public interface ISeeder
{
    public Task Seed();
}

public interface ISeeder<T>
{
    Task Seed(T entity);
}
