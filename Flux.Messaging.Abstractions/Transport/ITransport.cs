using Flux.Messaging.Abstractions.Envelope;

namespace Flux.Messaging.Abstractions.Transport;

public interface ITransport : IAsyncDisposable
{
    Task SendAsync(MessageEnvelope envelope, CancellationToken ct = default);
    void SetReceiver(Func<MessageEnvelope, CancellationToken, Task> receiver);
}