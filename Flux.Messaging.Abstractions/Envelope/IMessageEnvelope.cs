namespace Flux.Messaging.Abstractions.Envelope;

public interface IMessageEnvelope
{
    string Id { get; }
    string? CorrelationId { get; }
    string Type { get; }
    object Payload { get; }
    string? ReplyTo { get; }
    DateTimeOffset Timestamp { get; }
}