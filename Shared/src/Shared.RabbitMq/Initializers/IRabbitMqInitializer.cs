namespace Shared.RabbitMQ.Initializers;

public interface IRabbitMqInitializer
{
     Task InitializeAsync(CancellationToken cancellationToken);
}