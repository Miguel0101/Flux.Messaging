namespace Flux.Messaging.Abstractions.Envelopes;

public class MessageEnvelope
{
    public string Id { get; }
    public string? CorrelationId { get; }
    public string PayloadType { get; }
    public object Payload { get; }
    public MessageEnvelopeType Type { get; }
    public DateTimeOffset Timestamp { get; }

    private MessageEnvelope(object payload, MessageEnvelopeType type, string? correlationId)
    {
        Id = Guid.NewGuid().ToString("N");
        CorrelationId = correlationId;
        PayloadType = payload.GetType().Name;
        Payload = payload;
        Type = type;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public static MessageEnvelope CreateMessage(object payload, string? correlationId = null) => new(payload, MessageEnvelopeType.Message, correlationId);
    public static MessageEnvelope CreateCommand(object payload, string? correlationId = null) => new(payload, MessageEnvelopeType.Command, correlationId);
}