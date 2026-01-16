using Flux.Messaging.Abstractions.Message;

namespace Flux.Messaging.Tests.Publish.Handlers;

public sealed class IntMessageHandler : IMessageHandler<int>
{
    public TaskCompletionSource<int> ReceivedMessage { get; } = new();

    public Task HandleAsync(int message, CancellationToken ct)
    {
        ReceivedMessage.TrySetResult(message);
        return Task.CompletedTask;
    }
}