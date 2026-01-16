using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Dispatcher;
using Flux.Messaging.Abstractions.Transport;
using Flux.Messaging.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IFluxMessagingBuilder AddFluxMessaging(this IServiceCollection services)
    {
        var builder = new FluxMessagingBuilder(services);

        return builder;
    }

    public static IFluxMessagingBuilder UseInMemory(this IFluxMessagingBuilder builder)
    {
        builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();
        builder.Services.AddSingleton<ITransport, InMemoryTransport>();
        builder.Services.AddSingleton<IMessageDispatcher, InMemoryMessageDispatcher>();

        return builder;
    }
}
