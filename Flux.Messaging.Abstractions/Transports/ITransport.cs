using Flux.Messaging.Abstractions.Envelopes;

namespace Flux.Messaging.Abstractions.Transports;

public interface ITransport : IAsyncDisposable
{
    Task SendAsync(MessageEnvelope envelope, CancellationToken ct = default);
    void SetReceiver(Func<MessageEnvelope, CancellationToken, Task> receiver);
}