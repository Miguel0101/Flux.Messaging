using Flux.Messaging.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Core.Commands;

internal sealed class CommandHandlerResolver(IServiceScopeFactory scopeFactory, CommandHandlerRegistry registry)
{
    public IDynamicCommandHandler GetHandler(Type commandType)
    {
        Type handlerType = registry.Entries.GetValueOrDefault(commandType) ??
            throw new InvalidOperationException($"No handler registered for command type '{commandType.Name}'.");

        using var scope = scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;

        return (IDynamicCommandHandler)provider.GetRequiredService(handlerType);
    }
}