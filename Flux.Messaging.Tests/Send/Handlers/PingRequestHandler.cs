using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Tests.Send.Requests;

namespace Flux.Messaging.Tests.Send.Handlers;

public sealed class PingRequestHandler : IRequestHandler<PingRequest, string>
{
    public Task<string> HandleAsync(PingRequest request, CancellationToken ct)
        => Task.FromResult("pong");
}
