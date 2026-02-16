using Flux.Messaging.Abstractions.Envelopes;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.InMemory.Processing;

internal sealed class InMemoryEnvelopeProcessingStrategy([FromKeyedServices(MessagingProviders.InMemory)] IEnumerable<IEnvelopeProcessor> processors) : IEnvelopeProcessingStrategy
{
    private readonly Dictionary<MessageEnvelopeType, IEnvelopeProcessor> _processors = processors.ToDictionary(p => p.Type, p => p);

    public Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        if (!_processors.TryGetValue(envelope.Type, out var processor))
            throw new InvalidOperationException($"Invalid envelope processor: {envelope.Type}.");

        return processor.ProcessAsync(envelope, ct);
    }
}