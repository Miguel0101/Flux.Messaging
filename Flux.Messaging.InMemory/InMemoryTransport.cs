using System.Threading.Channels;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Transport;

namespace Flux.Messaging.InMemory;

internal sealed class InMemoryTransport : ITransport
{
    private Func<IMessageEnvelope, Task>? _receiver;
    private readonly Channel<IMessageEnvelope> _channel;
    private readonly Task _processingTask;

    public InMemoryTransport()
    {
        _channel = Channel.CreateUnbounded<IMessageEnvelope>();
        _processingTask = ProcessAsync();
    }

    public async Task SendAsync(IMessageEnvelope envelope, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        envelope.Headers["InMemoryAck"] = tcs;

        await _channel.Writer.WriteAsync(envelope, ct);

        await tcs.Task;
    }

    public void SetReceiver(Func<IMessageEnvelope, Task> receiver)
    {
        _receiver = receiver;
    }

    private async Task ProcessAsync()
    {
        await foreach (var envelope in _channel.Reader.ReadAllAsync())
        {
            _ = Task.Run(async () =>
            {
                var ack = envelope.Headers["InMemoryAck"] as TaskCompletionSource<bool>;

                try
                {
                    if (_receiver != null)
                        await _receiver(envelope);

                    ack?.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    ack?.TrySetException(ex);
                }
            });
        }
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.TryComplete();
        await _processingTask;
    }
}