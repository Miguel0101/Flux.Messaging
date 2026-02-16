using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Send.Handlers;
using Flux.Messaging.Tests.Send.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.Tests.Send.Bus;

public class BusDispatchTests
{
    [Fact]
    public async Task SendAsync_ShouldCount_FromCommandHandler()
    {
        var services = new ServiceCollection();
        var handler = new SpeakingCommandHandler();

        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<ICommandHandler<SpeakCommand>>(handler);
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        string message = "Hello Guy!";

        await messageBus.SendAsync(new SpeakCommand(message));

        string spokenMessage = await handler.SpokenMessage.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(message, spokenMessage);
    }
}
