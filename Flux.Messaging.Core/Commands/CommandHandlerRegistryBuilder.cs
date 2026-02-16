using Flux.Messaging.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Core.Commands;

internal static class CommandHandlerRegistryBuilder
{
    public static CommandHandlerRegistry Build(IServiceCollection services)
    {
        CommandHandlerRegistry registry = new();

        var handlers = services
            .Where(descriptor => descriptor.ServiceType.IsGenericType &&
                        descriptor.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

        foreach (var descriptor in handlers)
        {
            var commandType = descriptor.ServiceType.GetGenericArguments()[0];
            registry.Register(commandType, descriptor.ServiceType);
        }

        return registry;
    }
}