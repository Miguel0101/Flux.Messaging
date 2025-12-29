namespace Flux.Messaging.Abstractions;

public interface IMessageEnvelope
{
    string Id { get; }
    string Type { get; }
    object Payload { get; }
    DateTimeOffset Timestamp { get; }
}