namespace Shared.RabbitMQ.Conventions;

public class QueueBinding
{
    public string Key => $"{Exchange}:{Queue}:{RoutingKey}";
    public QueueBinding(string routingKey, string exchange, string queue)
    {
        RoutingKey = routingKey;
        Exchange = exchange;
        Queue = queue;
    }

    public string RoutingKey { get; set; }
    public string Exchange { get; set; }
    public string Queue { get; set; }
}