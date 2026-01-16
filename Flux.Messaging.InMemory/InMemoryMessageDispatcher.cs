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
                await h.HandleAsync(envelope.Payload);
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
        var handlerType = typeof(IGenericRequestHandler<>).MakeGenericType(payloadType);

        using var scope = _provider.CreateScope();

        var handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .OfType<IDynamicRequestHandler>()
            .ToArray();

        return handlers.Length switch
        {
            0 => throw new InvalidOperationException($"No handler found for request type {payloadType.Name}."),
            1 => await handlers[0].HandleAsync(envelope.Payload, ct),
            _ => throw new InvalidOperationException($"Multiple handlers found for request type {payloadType.Name}. A request must have exactly one handler.")
        };
    }
}
