namespace Flux.Messaging.Abstractions.Commands;

public interface IDynamicCommandHandler
{
    Task HandleAsync(object command, CancellationToken ct = default);
}