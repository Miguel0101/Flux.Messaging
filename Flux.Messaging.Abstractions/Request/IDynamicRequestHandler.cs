namespace Flux.Messaging.Abstractions.Request;

public interface IDynamicRequestHandler
{
    Task<object> HandleAsync(object request, CancellationToken ct = default);
}