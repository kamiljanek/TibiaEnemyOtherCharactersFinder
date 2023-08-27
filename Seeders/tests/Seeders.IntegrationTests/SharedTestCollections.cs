namespace Seeders.IntegrationTests;

[CollectionDefinition("Seeder test collection")]
public class SharedSeederTestCollection : ICollectionFixture<TibiaSeederFactory>
{
    
}

[CollectionDefinition("RabbitMq test collection")]
public class SharedRabbitMqTestCollection : ICollectionFixture<TibiaRabbitMqFactory>
{

}
