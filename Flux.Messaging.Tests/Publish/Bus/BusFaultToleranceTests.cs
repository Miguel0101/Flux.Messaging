using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Message;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Publish.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.Tests.Publish.Bus;

public class BusFaultToleranceTests
{
    [Fact]
    public async Task PublishAsync_ShouldContinueDispatching_WhenHandlerThrowsException()
    {
        var failingHandler = new FailingMessageHandler();
        var capturingHandler = new CapturingMessageHandler();

        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<IMessageHandler<string>>(failingHandler);
        services.AddSingleton<IMessageHandler<string>>(capturingHandler);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        const string publishedMessage = "Resilience";

        await messageBus.PublishAsync(publishedMessage);

        var received = await capturingHandler.ReceivedMessage.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(publishedMessage, received);
    }
}
