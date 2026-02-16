using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Send.Handlers;
using Flux.Messaging.Tests.Send.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.Tests.Send.Bus;

public class BusConcurrencyTests
{
    [Fact]
    public async Task SendAsync_ShouldHandleConcurrentCommands()
    {
        var services = new ServiceCollection();
        var handler = new CountingCommandHandler();

        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<ICommandHandler<CountCommand>>(handler);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        var tasks = Enumerable.Range(0, 100)
            .Select(task => messageBus.SendAsync(new CountCommand()));

        await Task.WhenAll(tasks);

        var count = await handler.ReceivedCount.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(100, count);
    }
}
