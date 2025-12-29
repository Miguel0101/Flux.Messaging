using Microsoft.Extensions.DependencyInjection;
using Flux.Messaging.Abstractions;

namespace Flux.Messaging.Core;

internal sealed class MessageDispatcher : IMessageDispatcher
{
    private readonly IServiceProvider _provider;

    public MessageDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync(IMessageEnvelope envelope, CancellationToken ct)
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

}
