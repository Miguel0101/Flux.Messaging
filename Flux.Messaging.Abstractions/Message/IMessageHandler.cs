namespace Flux.Messaging.Abstractions.Message;

public interface IMessageHandler<T> : IDynamicMessageHandler
{
    Task HandleAsync(T message, CancellationToken ct = default);

    async Task IDynamicMessageHandler.HandleAsync(object message, CancellationToken ct)
    {
        await HandleAsync((T)message, ct);
    }
}