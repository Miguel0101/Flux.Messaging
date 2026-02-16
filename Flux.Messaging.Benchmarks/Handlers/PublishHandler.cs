using Flux.Messaging.Abstractions.Messages;

namespace Flux.Messaging.Benchmarks.Handlers;

public class PublishHandler : IMessageHandler<string>
{
    public Task HandleAsync(string message, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}