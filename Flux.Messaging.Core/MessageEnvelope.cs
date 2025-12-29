using Flux.Messaging.Abstractions;

namespace Flux.Messaging.Core;

internal sealed class MessageEnvelope : IMessageEnvelope
{
    public string Id { get; }
    public string Type { get; }
    public object Payload { get; }
    public DateTimeOffset Timestamp { get; }

    private MessageEnvelope(object payload)
    {
        Id = Guid.NewGuid().ToString("N");
        Type = payload.GetType().Name;
        Payload = payload;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public static MessageEnvelope Create(object payload)
    {
        return new(payload);
    }
}