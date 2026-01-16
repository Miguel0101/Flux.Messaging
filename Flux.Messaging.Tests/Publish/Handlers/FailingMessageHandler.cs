using Flux.Messaging.Abstractions.Message;

namespace Flux.Messaging.Tests.Publish.Handlers;

public sealed class FailingMessageHandler : IMessageHandler<string>
{
    public Task HandleAsync(string message, CancellationToken ct)
        => throw new InvalidOperationException("Handler failure");
}