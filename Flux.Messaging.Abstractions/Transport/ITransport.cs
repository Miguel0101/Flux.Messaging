using Flux.Messaging.Abstractions.Envelope;

namespace Flux.Messaging.Abstractions.Transport;

public interface ITransport : IAsyncDisposable
{
    Task SendAsync(IMessageEnvelope envelope, CancellationToken ct = default);
    void SetReceiver(Action<IMessageEnvelope> receiver);
}