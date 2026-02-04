using Flux.Messaging.Abstractions.Envelope;

namespace Flux.Messaging.Core;

internal sealed class MessageEnvelope : IMessageEnvelope
{
    public string Id { get; }
    public string? CorrelationId { get; }
    public string Type { get; }
    public object Payload { get; }
    public string? ReplyTo { get; }
    
    public Dictionary<string, object> Headers { get; set; } = []; 
    public DateTimeOffset Timestamp { get; }

    private MessageEnvelope(object payload, string? correlationId, string? replyTo)
    {
        Id = Guid.NewGuid().ToString("N");
        CorrelationId = correlationId;
        Type = payload.GetType().Name;
        Payload = payload;
        ReplyTo = replyTo;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public static MessageEnvelope Create(object payload) => new(payload, null, null);
    public static MessageEnvelope CreateRequest(object payload, string replyTo) => new(payload, null, replyTo);
    public static MessageEnvelope CreateResponse(object payload, string correlationId) => new(payload, correlationId, null);
}