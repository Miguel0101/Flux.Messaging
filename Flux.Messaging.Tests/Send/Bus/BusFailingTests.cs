using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Send.Handlers;
using Flux.Messaging.Tests.Send.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.Tests.Send.Bus;

public class BusFailingTests
{
    [Fact]
    public async Task SendAsync_ShouldNotThrow_WhenHandlerThrowsAnError()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddTransient<ICommandHandler<CountCommand>, FailingCommandHandler>();
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        await messageBus.SendAsync(new CountCommand());
        await messageBus.DisposeAsync();
    }
}
