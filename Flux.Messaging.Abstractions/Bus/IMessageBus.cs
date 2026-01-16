using Flux.Messaging.Abstractions.Request;

namespace Flux.Messaging.Abstractions.Bus;

public interface IMessageBus
{
    Task<TResult> SendAsync<TResult>(IRequest<TResult> message, CancellationToken ct = default);
    Task PublishAsync<T>(T message, CancellationToken ct = default);
}