namespace TibiaEnemyOtherCharactersFinder.Application.Interfaces;

public interface ISeederService
{
    public Task Seed();
}

public interface ISeeder<T>
{
    Task Seed(T entity);
}
