using Flux.Messaging.Abstractions;

namespace Flux.Messaging.Tests.Handlers;

public sealed class IntMessageHandler : IMessageHandler<int>
{
    public TaskCompletionSource<int> ReceivedMessage { get; } = new();

    public override Task HandleAsync(int message, CancellationToken ct)
    {
        ReceivedMessage.TrySetResult(message);
        return Task.CompletedTask;
    }
}