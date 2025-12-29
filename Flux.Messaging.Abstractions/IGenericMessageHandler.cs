namespace Flux.Messaging.Abstractions;

public interface IGenericMessageHandler<T> : IDynamicMessageHandler
{
    Task HandleAsync(T message, CancellationToken ct = default);
}