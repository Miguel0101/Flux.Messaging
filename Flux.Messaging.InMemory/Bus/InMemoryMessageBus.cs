using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Envelopes;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Transports;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.InMemory.Bus;

internal sealed class InMemoryMessageBus : IMessageBus
{
    private readonly ITransport _transport;
    private readonly IEnvelopeProcessingStrategy _envelopeProcessing;
    private bool _disposed = false;

    public InMemoryMessageBus(
        [FromKeyedServices(MessagingProviders.InMemory)] ITransport transport,
        [FromKeyedServices(MessagingProviders.InMemory)] IEnvelopeProcessingStrategy envelopeProcessing)
    {
        _transport = transport;
        _envelopeProcessing = envelopeProcessing;

        _transport.SetReceiver(ReceiveEnvelopeAsync);
    }

    public Task PublishAsync<TMessage>(TMessage message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var envelope = MessageEnvelope.CreateMessage(message);

        return _transport.SendAsync(envelope, ct);
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var envelope = MessageEnvelope.CreateCommand(command);

        return _transport.SendAsync(envelope, ct);
    }

    private Task ReceiveEnvelopeAsync(MessageEnvelope envelope, CancellationToken ct)
    {
        return _envelopeProcessing.ProcessAsync(envelope, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        await _transport.DisposeAsync();
    }
}
