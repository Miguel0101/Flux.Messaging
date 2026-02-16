namespace Flux.Messaging.Core.Messages;

internal sealed class MessageHandlerRegistry
{
    private readonly Dictionary<Type, List<Type>> _handlers = [];

    public void Register(Type messageType, Type handlerType)
    {
        if (_handlers.TryGetValue(messageType, out var handlers))
        {
            handlers.Add(handlerType);
        }
        else
        {
            _handlers.TryAdd(messageType, [handlerType]);
        }
    }

    public IReadOnlyDictionary<Type, List<Type>> Entries => _handlers;
}