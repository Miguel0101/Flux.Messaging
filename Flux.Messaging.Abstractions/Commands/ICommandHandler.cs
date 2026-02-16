namespace Flux.Messaging.Abstractions.Commands;

public interface ICommandHandler<TCommand> : IDynamicCommandHandler
{
    Task HandleAsync(TCommand command, CancellationToken ct = default);

    Task IDynamicCommandHandler.HandleAsync(object command, CancellationToken ct)
        => HandleAsync((TCommand)command, ct);
}
