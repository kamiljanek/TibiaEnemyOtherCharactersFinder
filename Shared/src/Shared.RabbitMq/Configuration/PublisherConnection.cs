using RabbitMQ.Client;

namespace Shared.RabbitMQ.Configuration;

public class PublisherConnection
{
    public IConnection Connection { get; }
    public bool IsConnected => Connection.IsOpen;

    public PublisherConnection(IConnection connection)
    {
        Connection = connection;
    }
}