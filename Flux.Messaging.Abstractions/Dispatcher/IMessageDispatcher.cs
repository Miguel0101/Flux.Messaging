using Flux.Messaging.Abstractions.Envelope;

namespace Flux.Messaging.Abstractions.Dispatcher;

public interface IMessageDispatcher
{
    Task DispatchPublishAsync(IMessageEnvelope envelope, CancellationToken ct = default);
    Task<object> DispatchRequestAsync(IMessageEnvelope envelope, CancellationToken ct = default);
}
