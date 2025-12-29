using Flux.Messaging.Abstractions;
using Flux.Messaging.Tests.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Tests.Bus;

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
        var messageBus = provider.GetRequiredService<IMessageBus>();

        var publishTasks = Enumerable.Range(0, 3)
            .Select(i => messageBus.PublishAsync($"Message {i}"));

        await Task.WhenAll(publishTasks);

        var count = await handler.ReceivedCount.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(3, count);
    }
}
