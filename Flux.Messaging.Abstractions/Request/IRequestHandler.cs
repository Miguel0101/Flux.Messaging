namespace Flux.Messaging.Abstractions.Request;

public interface IRequestHandler<TRequest, TResponse> : IGenericRequestHandler<TRequest>
where TRequest : IRequest<TResponse>
{
    new Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
    
    async Task<object> IGenericRequestHandler<TRequest>.HandleAsync(TRequest request, CancellationToken ct)
    {
        return (await HandleAsync(request, ct))!;
    }
}
