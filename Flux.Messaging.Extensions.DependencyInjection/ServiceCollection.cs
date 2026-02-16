using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Dispatcher;
using Flux.Messaging.Abstractions.Processing;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Transports;
using Flux.Messaging.Core.Commands;
using Flux.Messaging.Core.Messages;
using Flux.Messaging.InMemory.Bus;
using Flux.Messaging.InMemory.Dispatcher;
using Flux.Messaging.InMemory.Processing;
using Flux.Messaging.InMemory.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IFluxMessagingBuilder AddFluxMessaging(this IServiceCollection services)
    {
        var builder = new FluxMessagingBuilder(services);

        builder.Services.AddSingleton(CommandHandlerRegistryBuilder.Build(services));
        builder.Services.AddSingleton<CommandHandlerResolver>();

        builder.Services.AddSingleton(MessageHandlerRegistryBuilder.Build(services));
        builder.Services.AddSingleton<MessageHandlerResolver>();

        return builder;
    }

    public static IFluxMessagingBuilder UseInMemory(this IFluxMessagingBuilder builder)
    {
        builder.Services.AddKeyedSingleton<IEnvelopeProcessingStrategy, InMemoryEnvelopeProcessingStrategy>(MessagingProviders.InMemory);
        builder.Services.AddKeyedSingleton<IEnvelopeProcessor, InMemoryMessageEnvelopeProcessor>(MessagingProviders.InMemory);
        builder.Services.AddKeyedSingleton<IEnvelopeProcessor, InMemoryCommandEnvelopeProcessor>(MessagingProviders.InMemory);

        builder.Services.AddKeyedSingleton<IMessageBus, InMemoryMessageBus>(MessagingProviders.InMemory);
        builder.Services.AddKeyedSingleton<ITransport, InMemoryTransport>(MessagingProviders.InMemory);
        builder.Services.AddKeyedSingleton<IMessageDispatcher, InMemoryMessageDispatcher>(MessagingProviders.InMemory);

        return builder;
    }
}
