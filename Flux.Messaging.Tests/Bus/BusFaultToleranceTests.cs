using Flux.Messaging.Abstractions;
using Flux.Messaging.Tests.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Tests.Bus;

public class BusFaultToleranceTests
{
    [Fact]
    public async Task PublishAsync_ShouldContinueDispatching_WhenHandlerThrowsException()
    {
        var failingHandler = new FailingMessageHandler();
        var capturingHandler = new CapturingMessageHandler();

        var services = new ServiceCollection();

        services.AddSingleton<IMessageHandler<string>>(failingHandler);
        services.AddSingleton<IMessageHandler<string>>(capturingHandler);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredService<IMessageBus>();

        const string publishedMessage = "Resilience";

        await messageBus.PublishAsync(publishedMessage);

        var received = await capturingHandler.ReceivedMessage.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(publishedMessage, received);
    }
}
