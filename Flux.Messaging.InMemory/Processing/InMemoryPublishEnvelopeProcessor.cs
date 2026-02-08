using Flux.Messaging.Abstractions.Dispatcher;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.InMemory.Processing;

internal sealed class InMemoryPublishEnvelopeProcessor(
    [FromKeyedServices(MessagingProviders.InMemory)] IMessageDispatcher dispatcher,
    ILogger<InMemoryPublishEnvelopeProcessor> logger) : IEnvelopeProcessor
{
    public MessageEnvelopeType Type { get; } = MessageEnvelopeType.Publish;

    public async Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        try
        {
            await dispatcher.DispatchPublishAsync(envelope, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the envelope {EnvelopeId}", envelope.Id);
        }
    }
}