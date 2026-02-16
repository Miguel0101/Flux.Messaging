using Flux.Messaging.Abstractions.Envelopes;
using Flux.Messaging.Abstractions.Messages;
using Flux.Messaging.Abstractions.Dispatcher;
using Microsoft.Extensions.Logging;
using Flux.Messaging.Core.Commands;
using Flux.Messaging.Core.Messages;

namespace Flux.Messaging.InMemory.Dispatcher;

internal sealed class InMemoryMessageDispatcher(
    MessageHandlerResolver messageHandlerResolver,
    CommandHandlerResolver commandHandlerResolver,
    ILogger<InMemoryMessageDispatcher> logger) : IMessageDispatcher
{
    public async Task DispatchMessageAsync(MessageEnvelope envelope, CancellationToken ct)
    {
        var messageType = envelope.Payload.GetType();
        var handlers = messageHandlerResolver.GetHandlers(messageType);

        var tasks = handlers.Select(handler => ExecuteHandlerAsync(handler, envelope, ct));

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

    public Task DispatchCommandAsync(MessageEnvelope envelope, CancellationToken ct = default)
    {
        var commandType = envelope.Payload.GetType();
        var handler = commandHandlerResolver.GetHandler(commandType);

        return handler.HandleAsync(envelope.Payload, ct);
    }
}
