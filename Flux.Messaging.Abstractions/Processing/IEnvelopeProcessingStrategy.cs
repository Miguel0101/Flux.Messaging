using Flux.Messaging.Abstractions.Envelope;

namespace Flux.Messaging.Abstractions.Processing;

public interface IEnvelopeProcessingStrategy
{
    Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default);
}