using Flux.Messaging.Abstractions.Message;

namespace Flux.Messaging.Tests.Publish.Handlers;

public sealed class CapturingMessageHandler : IMessageHandler<string>
{
    public TaskCompletionSource<string> ReceivedMessage { get; } = new();

    public Task HandleAsync(string message, CancellationToken ct)
    {
        ReceivedMessage.TrySetResult(message);
        return Task.CompletedTask;
    }
}