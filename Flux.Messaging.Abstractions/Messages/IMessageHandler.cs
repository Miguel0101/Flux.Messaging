namespace Flux.Messaging.Abstractions.Messages;

public interface IMessageHandler<TMessage> : IDynamicMessageHandler
{
    Task HandleAsync(TMessage message, CancellationToken ct = default);

    Task IDynamicMessageHandler.HandleAsync(object message, CancellationToken ct)
        => HandleAsync((TMessage)message, ct);
}