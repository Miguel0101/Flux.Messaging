using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Request;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Send.Handlers;
using Flux.Messaging.Tests.Send.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Tests.Send.Bus;

public class BusDispatchTests
{
    [Fact]
    public async Task SendAsync_ShouldReturnResponse_FromRequestHandler()
    {
        var services = new ServiceCollection();

        services.AddTransient<IRequestHandler<PingRequest, string>, PingRequestHandler>();
        services.AddFluxMessaging()
            .UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var bus = provider.GetRequiredService<IMessageBus>();

        var result = await bus.SendAsync(new PingRequest());

        Assert.Equal("pong", result);
    }
}
