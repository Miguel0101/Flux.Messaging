namespace Flux.Messaging.Abstractions.Envelope;

public class MessageEnvelope
{
    public string Id { get; }
    public string? CorrelationId { get; }
    public string PayloadType { get; }
    public object Payload { get; }
    public string? ReplyTo { get; }
    public MessageEnvelopeType Type { get; }
    public DateTimeOffset Timestamp { get; }

    private MessageEnvelope(object payload, MessageEnvelopeType type, string? correlationId, string? replyTo)
    {
        Id = Guid.NewGuid().ToString("N");
        CorrelationId = correlationId;
        PayloadType = payload.GetType().Name;
        Payload = payload;
        ReplyTo = replyTo;
        Type = type;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public static MessageEnvelope CreatePublish(object payload) => new(payload, MessageEnvelopeType.Publish, null, null);
    public static MessageEnvelope CreateRequest(object payload, string replyTo) => new(payload, MessageEnvelopeType.Request, null, replyTo);
    public static MessageEnvelope CreateResponse(object payload, string correlationId) => new(payload, MessageEnvelopeType.Response, correlationId, null);
}