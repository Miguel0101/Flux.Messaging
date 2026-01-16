using Microsoft.Extensions.DependencyInjection;
using Flux.Messaging.Abstractions.Envelope;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Abstractions.Message;
using Flux.Messaging.Abstractions.Dispatcher;

namespace Flux.Messaging.InMemory;

internal sealed class InMemoryMessageDispatcher : IMessageDispatcher
{
    private readonly IServiceProvider _provider;

    public InMemoryMessageDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchPublishAsync(IMessageEnvelope envelope, CancellationToken ct)
    {
        var messageType = envelope.Payload.GetType();
        var handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);

        using var scope = _provider.CreateScope();

        var handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .OfType<IDynamicMessageHandler>()
            .ToArray();

        if (handlers.Length == 0)
            return;

        var tasks = handlers.Select(async h =>
        {
            try
            {
                await h.HandleAsync(envelope.Payload, ct);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Message ID: {envelope.Id}\nMessage type: {envelope.Type}\nMessage Timestamp: {envelope.Timestamp}\nException Message: {e.Message}");
            }
        });

        await Task.WhenAll(tasks);
    }

    public async Task<object> DispatchRequestAsync(IMessageEnvelope envelope, CancellationToken ct = default)
    {
        var payloadType = envelope.Payload.GetType();

        var requestInterface = payloadType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)) ??
                throw new InvalidOperationException($"{payloadType.Name} does not implement IRequest<out TResponse>");

        var responseType = requestInterface.GetGenericArguments()[0];
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(payloadType, responseType);

        using var scope = _provider.CreateScope();

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
