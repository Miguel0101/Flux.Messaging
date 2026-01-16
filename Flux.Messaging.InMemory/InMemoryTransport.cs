using System.Threading.Channels;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Transport;

namespace Flux.Messaging.InMemory;

internal sealed class InMemoryTransport : ITransport
{
    private Action<IMessageEnvelope>? _receiver;
    private readonly Channel<IMessageEnvelope> _channel;
    private readonly Task _processingTask;

    public InMemoryTransport()
    {
        _channel = Channel.CreateUnbounded<IMessageEnvelope>();
        _processingTask = ProcessAsync();
    }

    public async Task SendAsync(IMessageEnvelope envelope, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(envelope, ct);
    }

    public void SetReceiver(Action<IMessageEnvelope> receiver)
    {
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
    }

    private async Task ProcessAsync()
    {
        await foreach (var envelope in _channel.Reader.ReadAllAsync())
        {
            try
            {
                _receiver?.Invoke(envelope);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InMemoryTransport receiver: {ex.Message}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.TryComplete();
        await _processingTask;
    }
}