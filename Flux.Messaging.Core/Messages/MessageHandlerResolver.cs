using Flux.Messaging.Abstractions.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Core.Messages;

internal sealed class MessageHandlerResolver(IServiceProvider provider, MessageHandlerRegistry registry)
{
    public List<IDynamicMessageHandler> GetHandlers(Type messageType)
    {
        List<Type> handlerTypes = registry.Entries.GetValueOrDefault(messageType) ??
            throw new InvalidOperationException($"No handler registered for message type '{messageType.Name}'.");

        List<IDynamicMessageHandler> messageHandlers = [];

        foreach (var handlerType in handlerTypes)
        {
            foreach (var handler in provider.GetServices(handlerType))
            {
                messageHandlers.Add((IDynamicMessageHandler)handler!);
            }
        }

        return messageHandlers;
    }
}