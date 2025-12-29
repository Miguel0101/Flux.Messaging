namespace Flux.Messaging.Abstractions;

public interface IDynamicMessageHandler
{
    Task HandleAsync(object message, CancellationToken ct = default);
}