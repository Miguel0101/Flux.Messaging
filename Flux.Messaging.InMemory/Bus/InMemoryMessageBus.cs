using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Abstractions.RequestResponse;
using Flux.Messaging.Abstractions.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.InMemory.Bus;

internal sealed class InMemoryMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly ITransport _transport;
    private readonly IEnvelopeProcessingStrategy _envelopeProcessing;
    private readonly IPendingRequestsManager _pendingRequests;
    private bool _disposed = false;

    public InMemoryMessageBus(
        [FromKeyedServices(MessagingProviders.InMemory)] ITransport transport,
        [FromKeyedServices(MessagingProviders.InMemory)] IEnvelopeProcessingStrategy envelopeProcessing,
        [FromKeyedServices(MessagingProviders.InMemory)] IPendingRequestsManager pendingRequests)
    {
        _transport = transport;
        _envelopeProcessing = envelopeProcessing;
        _pendingRequests = pendingRequests;

        _transport.SetReceiver(ReceiveEnvelopeAsync);
    }

    public Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var envelope = MessageEnvelope.CreatePublish(message);

        return _transport.SendAsync(envelope, ct);
    }

    public async Task<TResult> SendAsync<TResult>(IRequest<TResult> message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var replyTo = Guid.NewGuid().ToString("N");
        var envelope = MessageEnvelope.CreateRequest(message, replyTo);
        var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        _pendingRequests.Register(envelope.Id, tcs);

        await _transport.SendAsync(envelope, ct);

        var result = await tcs.Task;

        return (TResult)result;
    }

    private Task ReceiveEnvelopeAsync(MessageEnvelope envelope, CancellationToken ct)
    {
        return _envelopeProcessing.ProcessAsync(envelope, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _pendingRequests.CancelAll();
        await _transport.DisposeAsync();
    }
}
