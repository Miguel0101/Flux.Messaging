using Flux.Messaging.Abstractions;
using Flux.Messaging.Core;
using Flux.Messaging.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IFluxMessagingBuilder AddFluxMessaging(this IServiceCollection services)
    {
        var builder = new FluxMessagingBuilder(services);

        builder.Services.AddSingleton<IMessageDispatcher, MessageDispatcher>();

        return builder;
    }

    public static IFluxMessagingBuilder UseInMemory(this IFluxMessagingBuilder builder)
    {
        builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();

        return builder;
    }
}
