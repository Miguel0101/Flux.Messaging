using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Abstractions.RequestResponse;
using Flux.Messaging.Abstractions.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.InMemory.Bus;

internal sealed class InMemoryMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly ITransport _transport;
    private readonly IEnvelopeProcessingStrategy _envelopeProcessing;
    private readonly IPendingRequestsManager _pendingRequests;
    private readonly ILogger _logger;

    public InMemoryMessageBus(
        [FromKeyedServices(MessagingProviders.InMemory)] ITransport transport,
        [FromKeyedServices(MessagingProviders.InMemory)] IEnvelopeProcessingStrategy envelopeProcessing,
        [FromKeyedServices(MessagingProviders.InMemory)] IPendingRequestsManager pendingRequests,
        ILogger<InMemoryMessageBus> logger)
    {
        _transport = transport;
        _envelopeProcessing = envelopeProcessing;
        _pendingRequests = pendingRequests;
        _logger = logger;

        _transport.SetReceiver(ReceiveEnvelopeAsync);
    }

    public Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var envelope = MessageEnvelope.CreatePublish(message);

        return _transport.SendAsync(envelope, ct);
    }

    public async Task<TResult> SendAsync<TResult>(IRequest<TResult> message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var replyTo = Guid.NewGuid().ToString("N");
        var envelope = MessageEnvelope.CreateRequest(message, replyTo);
        var tcs = new TaskCompletionSource<object>();

        _pendingRequests.Register(envelope.Id, tcs);

        await _transport.SendAsync(envelope, ct);

        var result = await tcs.Task;

        return (TResult)result;
    }

    private async Task ReceiveEnvelopeAsync(MessageEnvelope envelope)
    {
        try
        {
            await _envelopeProcessing.ProcessAsync(envelope);
        }
        catch (Exception ex)
        {
            if (_pendingRequests.TryRemove(envelope.Id, out var tcs))
            {
                tcs!.TrySetException(ex);
            }
            else
            {
                _logger.LogError(ex, "An error occurred processing the envelope {EnvelopeId}", envelope.Id);
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        return _transport.DisposeAsync();
    }
}
