using System.Collections.Concurrent;
using Flux.Messaging.Abstractions.RequestResponse;

namespace Flux.Messaging.Core.RequestResponse;

public sealed class PendingRequestsManager : IPendingRequestsManager
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _pending = [];

    public void Register(string requestId, TaskCompletionSource<object> tcs)
    {
        if (_pending.TryAdd(requestId, tcs) is false)
        {
            throw new InvalidOperationException($"Request {requestId} already registered.");
        }
    }

    public bool TryComplete(string requestId, object result)
    {
        if (_pending.TryRemove(requestId, out var tcs))
        {
            tcs.TrySetResult(result);
            return true;
        }
        return false;
    }

    public bool TryFail(string requestId, Exception exception)
    {
        if (_pending.TryRemove(requestId, out var tcs))
        {
            tcs.TrySetException(exception);
            return true;
        }
        return false;
    }

    public bool TryRemove(string requestId, out TaskCompletionSource<object>? tcs)
    {
        return _pending.TryRemove(requestId, out tcs);
    }

    public void CancelAll()
    {
        foreach (var kvp in _pending)
            kvp.Value.TrySetCanceled();

        _pending.Clear();
    }
}
