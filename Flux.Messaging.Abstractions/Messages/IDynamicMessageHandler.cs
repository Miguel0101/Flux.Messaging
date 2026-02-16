namespace Flux.Messaging.Abstractions.Messages;

public interface IDynamicMessageHandler
{
    Task HandleAsync(object message, CancellationToken ct = default);
}