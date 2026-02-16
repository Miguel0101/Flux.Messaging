using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Benchmarks.Commands;

namespace Flux.Messaging.Benchmarks.Handlers;

public class SendHandler : ICommandHandler<SendCommand>
{
    public Task HandleAsync(SendCommand request, CancellationToken ct = default)
    {
        return Task.FromResult(request.Message);
    }
}