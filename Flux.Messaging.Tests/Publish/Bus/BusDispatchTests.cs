using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Message;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Publish.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Tests.Publish.Bus;

public class BusDispatchTests
{
    [Fact]
    public async Task PublishAsync_ShouldDispatchMessageToRegisteredHandler()
    {
        var handler = new CapturingMessageHandler();

        var services = new ServiceCollection();

        services.AddSingleton<IMessageHandler<string>>(handler);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        const string publishedMessage = "Hello World";

        await messageBus.PublishAsync(publishedMessage);

        var receivedMessage = await handler.ReceivedMessage.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(publishedMessage, receivedMessage);
    }
}
