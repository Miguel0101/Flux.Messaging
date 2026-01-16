namespace Flux.Messaging.Abstractions.Message;

public interface IDynamicMessageHandler
{
    Task HandleAsync(object message, CancellationToken ct = default);
}