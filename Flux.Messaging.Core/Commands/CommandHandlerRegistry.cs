namespace Flux.Messaging.Core.Commands;

internal sealed class CommandHandlerRegistry
{
    private readonly Dictionary<Type, Type> _handlers = [];

    public void Register(Type commandType, Type handlerType)
    {
        if (_handlers.ContainsKey(commandType))
            throw new InvalidOperationException($"Multiple command handlers found for {commandType}");

        _handlers[commandType] = handlerType;
    }

    public IReadOnlyDictionary<Type, Type> Entries => _handlers;
}