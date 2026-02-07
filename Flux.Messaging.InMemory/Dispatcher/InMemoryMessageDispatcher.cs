using Microsoft.Extensions.DependencyInjection;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Abstractions.Message;
using Flux.Messaging.Abstractions.Dispatcher;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.InMemory.Dispatcher;

internal sealed class InMemoryMessageDispatcher(IServiceProvider provider, ILogger<InMemoryMessageDispatcher> logger) : IMessageDispatcher
{
    public Task DispatchPublishAsync(MessageEnvelope envelope, CancellationToken ct)
    {
        var messageType = envelope.Payload.GetType();
        var handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);

        using var scope = provider.CreateScope();

        var handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .OfType<IDynamicMessageHandler>()
            .ToArray();

        if (handlers.Length == 0)
            return Task.CompletedTask;

        var tasks = handlers.Select(h =>
        {
            try
            {
                return h.HandleAsync(envelope.Payload, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Message ID: {Id}\nMessage type: {Type}\nMessage Timestamp: {Timestamp}", envelope.Id, envelope.PayloadType, envelope.Timestamp);
                return Task.CompletedTask;
            }
        });

        return Task.WhenAll(tasks);
    }

    public Task<object> DispatchRequestAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        var payloadType = envelope.Payload.GetType();

        var requestInterface = payloadType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)) ??
                throw new InvalidOperationException($"{payloadType.Name} does not implement IRequest<out TResponse>");

        var responseType = requestInterface.GetGenericArguments()[0];
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(payloadType, responseType);

        using var scope = provider.CreateScope();

        var handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .Cast<IDynamicRequestHandler>()
            .ToArray();

        return handlers.Length switch
        {
            0 => throw new InvalidOperationException($"No handler registered found for {payloadType.Name}."),
            1 => handlers[0].HandleAsync(envelope.Payload, ct),
            _ => throw new InvalidOperationException($"Multiple handlers found for {payloadType.Name}.")
        };
    }
}
