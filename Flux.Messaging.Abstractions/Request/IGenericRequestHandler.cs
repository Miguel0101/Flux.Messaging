namespace Flux.Messaging.Abstractions.Request;

public interface IGenericRequestHandler<TRequest> : IDynamicRequestHandler
{
    Task<object> HandleAsync(TRequest request, CancellationToken ct = default);
    
    async Task<object> IDynamicRequestHandler.HandleAsync(object request, CancellationToken ct)
    {
        return await HandleAsync((TRequest)request, ct);
    }
}