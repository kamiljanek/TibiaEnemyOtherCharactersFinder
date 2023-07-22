using RabbitMQ.Client;

namespace Shared.RabbitMQ.Configuration;

public class SubscriberConnection
{
    public SubscriberConnection(IConnection connection)
    {
        Connection = connection;
    }

    public IConnection Connection { get; }
}