using RabbitMQ.Client;

namespace Shared.RabbitMQ.Configuration;

public class RabbitMqConnection
{
    public IConnection Connection { get; }
    public bool IsConnected => Connection.IsOpen;

    public RabbitMqConnection(IConnection connection)
    {
        Connection = connection;
    }
}