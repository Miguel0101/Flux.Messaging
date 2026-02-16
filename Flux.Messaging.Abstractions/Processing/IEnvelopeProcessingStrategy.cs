using Flux.Messaging.Abstractions.Envelopes;

namespace Flux.Messaging.Abstractions.Processing;

public interface IEnvelopeProcessingStrategy
{
    Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default);
}