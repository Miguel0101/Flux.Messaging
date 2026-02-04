namespace Flux.Messaging.Abstractions.Envelope;

public interface IMessageEnvelope
{
    string Id { get; }
    string? CorrelationId { get; }
    string Type { get; }
    object Payload { get; }
    string? ReplyTo { get; }
    Dictionary<string, object> Headers { get; set; }
    DateTimeOffset Timestamp { get; }
}