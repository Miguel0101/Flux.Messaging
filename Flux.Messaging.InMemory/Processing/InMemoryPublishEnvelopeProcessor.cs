using Flux.Messaging.Abstractions.Dispatcher;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.InMemory.Processing;

internal sealed class InMemoryPublishEnvelopeProcessor([FromKeyedServices(MessagingProviders.InMemory)] IMessageDispatcher dispatcher) : IEnvelopeProcessor
{
    public MessageEnvelopeType Type { get; } = MessageEnvelopeType.Publish;

    public Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        return dispatcher.DispatchPublishAsync(envelope, ct);
    }
}