using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Send.Handlers;
using Flux.Messaging.Tests.Send.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Flux.Messaging.Tests.Send.Bus;

public class BusMultiHandlerTests
{
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenMultipleHandlersAreRegistered()
    {
        var services = new ServiceCollection();

        services.AddTransient<IRequestHandler<PingRequest, string>, PingRequestHandler>();
        services.AddTransient<IRequestHandler<PingRequest, string>, PingRequestHandler>();
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var bus = provider.GetRequiredService<IMessageBus>();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            bus.SendAsync(new PingRequest())
        );
    }
}