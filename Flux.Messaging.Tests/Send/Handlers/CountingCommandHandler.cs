using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Tests.Send.Commands;

namespace Flux.Messaging.Tests.Send.Handlers;

public sealed class CountingCommandHandler : ICommandHandler<CountCommand>
{
    private int _count = 0;
    public TaskCompletionSource<int> ReceivedCount = new();

    public Task HandleAsync(CountCommand command, CancellationToken ct)
    {
        if (Interlocked.Increment(ref _count) == 100)
            ReceivedCount.TrySetResult(_count);

        return Task.CompletedTask;
    }
}
