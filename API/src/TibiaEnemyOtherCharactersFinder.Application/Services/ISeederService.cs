namespace TibiaEnemyOtherCharactersFinder.Application.Services;

public interface ISeederService
{
    public Task Seed();
}

public interface ISeeder<T>
{
    Task Seed(T entity);
}
