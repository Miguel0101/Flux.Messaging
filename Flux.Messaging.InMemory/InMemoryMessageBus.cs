using System.Collections.Concurrent;
using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Dispatcher;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Abstractions.Transport;
using Flux.Messaging.Core;

namespace Flux.Messaging.InMemory;

internal sealed class InMemoryMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly ITransport _transport;
    private readonly IMessageDispatcher _dispatcher;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _pending;

    public InMemoryMessageBus(ITransport transport, IMessageDispatcher dispatcher)
    {
        _transport = transport;
        _dispatcher = dispatcher;
        _pending = new ConcurrentDictionary<string, TaskCompletionSource<object>>();

        _transport.SetReceiver(ReceiveEnvelopeAsync);
    }

    public async Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var envelope = MessageEnvelope.Create(message);
        await _transport.SendAsync(envelope, ct);
    }

    public async Task<TResult> SendAsync<TResult>(IRequest<TResult> message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var replyTo = Guid.NewGuid().ToString("N");
        var envelope = MessageEnvelope.CreateRequest(message, replyTo);
        var tcs = new TaskCompletionSource<object>();

        _pending[envelope.Id] = tcs;

        await _transport.SendAsync(envelope, ct);

        var result = await tcs.Task;
        return (TResult)result;
    }

    private async void ReceiveEnvelopeAsync(IMessageEnvelope envelope)
    {
        try
        {
            if (!string.IsNullOrEmpty(envelope.CorrelationId))
            {
                if (_pending.TryRemove(envelope.CorrelationId, out var tcs))
                {
                    tcs.SetResult(envelope.Payload);
                    return;
                }
            }

            if (string.IsNullOrEmpty(envelope.ReplyTo))
            {
                await _dispatcher.DispatchPublishAsync(envelope);
                return;
            }

            var response = await _dispatcher.DispatchRequestAsync(envelope);
            var replyEnvelope = MessageEnvelope.CreateResponse(response, envelope.Id);

            await _transport.SendAsync(replyEnvelope);
        }
        catch (Exception ex)
        {
            if (_pending.TryRemove(envelope.Id, out var tcs))
            {
                tcs.TrySetException(ex);
            }
            else
            {
                Console.WriteLine($"Error in MessageBus.ReceiveEnvelopeAsync: {ex.Message}");
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        return _transport.DisposeAsync();
    }
}
