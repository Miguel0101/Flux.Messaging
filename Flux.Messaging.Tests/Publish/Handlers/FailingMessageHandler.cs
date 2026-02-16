using Flux.Messaging.Abstractions.Messages;

namespace Flux.Messaging.Tests.Publish.Handlers;

public sealed class FailingMessageHandler : IMessageHandler<int>
{
    public Task HandleAsync(int message, CancellationToken ct)
        => throw new InvalidOperationException("Handler failure");
}