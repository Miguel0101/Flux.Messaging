using Flux.Messaging.Abstractions.Envelopes;

namespace Flux.Messaging.Abstractions.Dispatcher;

public interface IMessageDispatcher
{
    Task DispatchMessageAsync(MessageEnvelope envelope, CancellationToken ct = default);
    Task DispatchCommandAsync(MessageEnvelope envelope, CancellationToken ct = default);
}
