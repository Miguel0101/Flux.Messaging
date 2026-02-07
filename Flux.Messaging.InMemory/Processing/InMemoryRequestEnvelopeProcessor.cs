using Flux.Messaging.Abstractions.Dispatcher;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.InMemory.Processing;

internal sealed class InMemoryRequestEnvelopeProcessor(
    [FromKeyedServices(MessagingProviders.InMemory)] ITransport transport,
    [FromKeyedServices(MessagingProviders.InMemory)] IMessageDispatcher dispatcher) : IEnvelopeProcessor
{
    public MessageEnvelopeType Type { get; } = MessageEnvelopeType.Request;

    public async Task ProcessAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        var response = await dispatcher.DispatchRequestAsync(envelope, ct);
        var replyEnvelope = MessageEnvelope.CreateResponse(response, envelope.Id);

        await transport.SendAsync(replyEnvelope, ct);
    }
}