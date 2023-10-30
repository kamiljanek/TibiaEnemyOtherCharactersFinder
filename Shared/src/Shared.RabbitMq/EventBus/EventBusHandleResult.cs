namespace Shared.RabbitMQ.EventBus;

public class EventBusHandleResult<T>
{
    public EventBusHandleResult(string messageId, string exchangeKey, string queue, string? type, string? data)
    {
        MessageId = messageId;
        ExchangeKey = exchangeKey;
        Queue = queue;
        Type = type;
        Data = data;
        Errors = Array.Empty<string>();
    }

    /// <summary>
    /// The Id of processed message
    /// </summary>
    public string MessageId { get; }

    /// <summary>
    /// The key of exchange
    /// </summary>
    public string ExchangeKey { get; }

    /// <summary>
    /// The name of the queue
    /// </summary>
    public string Queue { get; set; }

    /// <summary>
    /// The type of the message
    /// </summary>
    public string? Type { get; }

    /// <summary>
    /// The serialized input data
    /// </summary>
    public string? Data { get; }

    /// <summary>
    /// The date when the handling process started
    /// </summary>
    public DateTime StartedAt { get; private set; }

    /// <summary>
    /// The date when the handling process has been finished
    /// </summary>
    public DateTime StoppedAt { get; private set; }

    /// <summary>
    /// Calculated processing time in milliseconds
    /// </summary>
    public long ExecutionTime => (long)StoppedAt.Subtract(StartedAt).TotalMilliseconds;

    /// <summary>
    /// Indicates whether the handling has finished with success
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Indicates how many times handle method has been retried
    /// </summary>
    public int Retries { get; private set; }

    /// <summary>
    /// The errors from all retries if any
    /// </summary>
    public string[]? Errors { get; private set; }
    
    public T? CustomDetails { get; private set; }

    public void StartProcessing() => StartedAt = DateTime.UtcNow;

    public void StopProcessing(int retries, bool isSuccess, params string[]? errors)
    {
        StoppedAt = DateTime.UtcNow;
        Retries = retries;
        Success = isSuccess;
        Errors = errors ?? Array.Empty<string>();
    }
    
    public void SetDetails(T details)
    {
        CustomDetails = details;
    }
}