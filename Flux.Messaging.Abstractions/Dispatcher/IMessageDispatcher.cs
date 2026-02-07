using Flux.Messaging.Abstractions.Envelope;

namespace Flux.Messaging.Abstractions.Dispatcher;

public interface IMessageDispatcher
{
    Task DispatchPublishAsync(MessageEnvelope envelope, CancellationToken ct = default);
    Task<object> DispatchRequestAsync(MessageEnvelope envelope, CancellationToken ct = default);
}
