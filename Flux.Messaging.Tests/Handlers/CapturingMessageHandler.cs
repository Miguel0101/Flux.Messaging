using Flux.Messaging.Abstractions;

namespace Flux.Messaging.Tests.Handlers;

public sealed class CapturingMessageHandler : IMessageHandler<string>
{
    public TaskCompletionSource<string> ReceivedMessage { get; } = new();

    public override Task HandleAsync(string message, CancellationToken ct)
    {
        ReceivedMessage.TrySetResult(message);
        return Task.CompletedTask;
    }
}