using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Send.Handlers;
using Flux.Messaging.Tests.Send.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.Tests.Send.Bus;

public class BusFailingTests
{
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenHandlerThrowsAnError()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddTransient<IRequestHandler<PingRequest, string>, FailingRequestHandler>();
        services.AddFluxMessaging()
            .UseInMemory();

        await using var scope = services.BuildServiceProvider();
        var messageBus = scope.GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            messageBus.SendAsync(new PingRequest())
        );
    }
}