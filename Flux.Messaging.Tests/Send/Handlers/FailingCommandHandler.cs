using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Tests.Send.Commands;

namespace Flux.Messaging.Tests.Send.Handlers;

public sealed class FailingCommandHandler : ICommandHandler<CountCommand>
{
    public Task HandleAsync(CountCommand command, CancellationToken ct)
        => throw new InvalidOperationException("boom");
}
