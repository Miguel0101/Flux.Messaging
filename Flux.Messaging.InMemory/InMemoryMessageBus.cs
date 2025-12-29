using System.Threading.Channels;
using Flux.Messaging.Abstractions;
using Flux.Messaging.Core;

namespace Flux.Messaging.InMemory;

internal sealed class InMemoryMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly Channel<MessageEnvelope> _channel;
    private readonly IMessageDispatcher _dispatcher;
    private readonly Task _processingTask;

    public InMemoryMessageBus(IMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _channel = Channel.CreateUnbounded<MessageEnvelope>();
        _processingTask = ProcessAsync();
    }

    public async Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        await _channel.Writer.WriteAsync(MessageEnvelope.Create(message), ct);
    }

    private async Task ProcessAsync()
    {
        await foreach (var envelope in _channel.Reader.ReadAllAsync())
        {
            await _dispatcher.DispatchAsync(envelope);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        await _processingTask;
    }
}