using Flux.Messaging.Abstractions.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Core.Messages;

internal static class MessageHandlerRegistryBuilder
{
    public static MessageHandlerRegistry Build(IServiceCollection services)
    {
        MessageHandlerRegistry registry = new();

        var handlers = services
            .Where(descriptor => descriptor.ServiceType.IsGenericType &&
                        descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>));

        foreach (var descriptor in handlers)
        {
            var messageType = descriptor.ServiceType.GetGenericArguments()[0];
            registry.Register(messageType, descriptor.ServiceType);
        }

        return registry;
    }
}