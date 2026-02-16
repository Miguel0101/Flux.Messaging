namespace Flux.Messaging.Abstractions.Bus;

public interface IMessageBus : IAsyncDisposable
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default);
    Task PublishAsync<TMessage>(TMessage message, CancellationToken ct = default);
}