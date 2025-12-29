using Flux.Messaging.Abstractions;

namespace Flux.Messaging.Tests.Handlers;

public sealed class FailingMessageHandler : IMessageHandler<string>
{
    public override Task HandleAsync(string message, CancellationToken ct)
        => throw new InvalidOperationException("Handler failure");
}