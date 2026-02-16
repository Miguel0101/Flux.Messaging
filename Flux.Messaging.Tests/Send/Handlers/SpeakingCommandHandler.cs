using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Tests.Send.Commands;

namespace Flux.Messaging.Tests.Send.Handlers;

public class SpeakingCommandHandler : ICommandHandler<SpeakCommand>
{
    public TaskCompletionSource<string> SpokenMessage = new();

    public Task HandleAsync(SpeakCommand command, CancellationToken ct = default)
    {
        SpokenMessage.TrySetResult(command.Something);
        return Task.CompletedTask;
    }
}