using Flux.Messaging.Abstractions.Envelopes;

namespace Flux.Messaging.Abstractions.Processing;

public interface IEnvelopeProcessor
{
    MessageEnvelopeType Type { get; }
    Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default);
}