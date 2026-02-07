namespace Flux.Messaging.Abstractions.RequestResponse;

public interface IPendingRequestsManager
{
    void Register(string requestId, TaskCompletionSource<object> tcs);
    bool TryComplete(string requestId, object result);
    bool TryFail(string requestId, Exception exception);
    bool TryRemove(string requestId, out TaskCompletionSource<object>? tcs);
    void CancelAll();
}