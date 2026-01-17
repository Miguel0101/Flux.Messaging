using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Message;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Publish.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Tests.Publish.Bus;

public class BusConcurrencyTests
{
    [Fact]
    public async Task PublishAsync_ShouldSupportConcurrentPublishing()
    {
        var handler = new CountingMessageHandler();

        var services = new ServiceCollection();

        services.AddSingleton<IMessageHandler<string>>(handler);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        var publishTasks = Enumerable.Range(0, 3)
            .Select(i => messageBus.PublishAsync($"Message {i}"));

        await Task.WhenAll(publishTasks);

        var count = await handler.ReceivedCount.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(3, count);
    }
}
