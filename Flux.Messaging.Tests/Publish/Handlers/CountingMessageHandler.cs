using Flux.Messaging.Abstractions.Message;

namespace Flux.Messaging.Tests.Publish.Handlers;

public sealed class CountingMessageHandler : IMessageHandler<string>
{
    private int _count;
    public TaskCompletionSource<int> ReceivedCount { get; } = new();

    public Task HandleAsync(string message, CancellationToken ct)
    {
        if (Interlocked.Increment(ref _count) == 3)
            ReceivedCount.TrySetResult(_count);

        return Task.CompletedTask;
    }
}