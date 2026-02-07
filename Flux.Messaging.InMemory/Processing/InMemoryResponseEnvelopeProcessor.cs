using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.RequestResponse;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.InMemory.Processing;

internal sealed class InMemoryResponseEnvelopeProcessor([FromKeyedServices(MessagingProviders.InMemory)] IPendingRequestsManager pendingRequests) : IEnvelopeProcessor
{
    public MessageEnvelopeType Type { get; } = MessageEnvelopeType.Response;

    public Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        if (pendingRequests.TryRemove(envelope.CorrelationId!, out var tcs))
        {
            tcs!.SetResult(envelope.Payload);
        }

        return Task.CompletedTask;
    }
}