using Microsoft.Extensions.DependencyInjection;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Abstractions.Message;
using Flux.Messaging.Abstractions.Dispatcher;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.InMemory.Dispatcher;

internal sealed class InMemoryMessageDispatcher(IServiceProvider provider, ILogger<InMemoryMessageDispatcher> logger) : IMessageDispatcher
{
    public async Task DispatchPublishAsync(MessageEnvelope envelope, CancellationToken ct)
    {
        var messageType = envelope.Payload.GetType();
        var handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);

        await using var scope = provider.CreateAsyncScope();

        var handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .OfType<IDynamicMessageHandler>()
            .ToArray();

        if (handlers.Length == 0)
        {
            logger.LogWarning("No handler registered found for {MessageType}.", messageType.Name);
            return;
        }

        var tasks = handlers.Select(handler => ExecuteHandlerAsync(handler, envelope, ct)).ToList();

        await Task.WhenAll(tasks);
    }

    private async Task ExecuteHandlerAsync(IDynamicMessageHandler handler, MessageEnvelope envelope, CancellationToken ct)
    {
        try
        {
            await handler.HandleAsync(envelope.Payload, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Message ID: {Id}\nMessage type: {Type}\nMessage Timestamp: {Timestamp}", envelope.Id, envelope.PayloadType, envelope.Timestamp);
        }
    }

    public async Task<object> DispatchRequestAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        var payloadType = envelope.Payload.GetType();

        var requestInterface = payloadType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)) ??
                throw new InvalidOperationException($"{payloadType.Name} does not implement IRequest<out TResponse>");

        var responseType = requestInterface.GetGenericArguments()[0];
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(payloadType, responseType);

        await using var scope = provider.CreateAsyncScope();

        var handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .Cast<IDynamicRequestHandler>()
            .ToArray();

        return handlers.Length switch
        {
            0 => throw new InvalidOperationException($"No handler registered found for {payloadType.Name}."),
            1 => await handlers[0].HandleAsync(envelope.Payload, ct),
            _ => throw new InvalidOperationException($"Multiple handlers found for {payloadType.Name}.")
        };
    }
}
