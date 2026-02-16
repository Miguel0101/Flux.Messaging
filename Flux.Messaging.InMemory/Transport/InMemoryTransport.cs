using System.Threading.Channels;
using Flux.Messaging.Abstractions.Envelopes;
using Flux.Messaging.Abstractions.Transports;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.InMemory.Transport;

internal sealed class InMemoryTransport : ITransport
{
    private Func<MessageEnvelope, CancellationToken, Task>? _receiver;
    private readonly Channel<MessageEnvelope> _channel;
    private readonly Task _processingTask;
    private readonly ILogger _logger;
    private bool _disposed;

    public InMemoryTransport(ILogger<InMemoryTransport> logger)
    {
        _channel = Channel.CreateUnbounded<MessageEnvelope>();
        _processingTask = ProcessAsync();
        _logger = logger;
    }

    public async Task SendAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(envelope, ct);
    }

    public void SetReceiver(Func<MessageEnvelope, CancellationToken, Task> receiver)
    {
        _receiver = receiver;
    }

    private async Task ProcessAsync()
    {
        await foreach (var envelope in _channel.Reader.ReadAllAsync())
        {
            try
            {
                if (_receiver != null)
                    await _receiver(envelope, default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing envelope {EnvelopeId}.", envelope.Id);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        _channel.Writer.TryComplete();

        try
        {
            await _processingTask;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Transport shutting down...");
        }
    }
}