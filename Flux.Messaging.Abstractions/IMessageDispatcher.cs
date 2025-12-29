namespace Flux.Messaging.Abstractions;

public interface IMessageDispatcher
{
    Task DispatchAsync(IMessageEnvelope envelope, CancellationToken ct = default);
}
