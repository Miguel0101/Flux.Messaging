using Flux.Messaging.Abstractions;
using Flux.Messaging.Tests.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Tests.Bus;

public class BusMultiHandlerTests
{
    [Fact]
    public async Task PublishAsync_ShouldDispatchMessageToAllRegisteredHandlers()
    {
        var handler1 = new CapturingMessageHandler();
        var handler2 = new CapturingMessageHandler();

        var services = new ServiceCollection();

        services.AddSingleton<IMessageHandler<string>>(handler1);
        services.AddSingleton<IMessageHandler<string>>(handler2);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredService<IMessageBus>();

        const string publishedMessage = "Broadcast";

        await messageBus.PublishAsync(publishedMessage);

        var received1 = await handler1.ReceivedMessage.Task.WaitAsync(TimeSpan.FromSeconds(1));
        var received2 = await handler2.ReceivedMessage.Task.WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(publishedMessage, received1);
        Assert.Equal(publishedMessage, received2);
    }
}
