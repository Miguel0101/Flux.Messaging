namespace Flux.Messaging.Abstractions;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken ct = default);
}