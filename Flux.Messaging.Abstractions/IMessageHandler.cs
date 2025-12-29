namespace Flux.Messaging.Abstractions;

public abstract class IMessageHandler<T> : IGenericMessageHandler<T>
{
    public abstract Task HandleAsync(T message, CancellationToken ct = default);

    async Task IDynamicMessageHandler.HandleAsync(object message, CancellationToken ct)
    {
        await HandleAsync((T)message, ct);
    }
}