using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Messages;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Publish.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.Tests.Publish.Bus;

public class BusMultiHandlerTests
{
    [Fact]
    public async Task PublishAsync_ShouldDispatchMessageToAllRegisteredHandlers()
    {
        var handler1 = new CapturingMessageHandler();
        var handler2 = new CapturingMessageHandler();

        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<IMessageHandler<string>>(handler1);
        services.AddSingleton<IMessageHandler<string>>(handler2);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        const string publishedMessage = "Broadcast";

        await messageBus.PublishAsync(publishedMessage);

        var received1 = await handler1.ReceivedMessage.Task.WaitAsync(TimeSpan.FromSeconds(1));
        var received2 = await handler2.ReceivedMessage.Task.WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(publishedMessage, received1);
        Assert.Equal(publishedMessage, received2);
    }
}
