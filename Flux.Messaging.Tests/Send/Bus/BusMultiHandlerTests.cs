using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Extensions.DependencyInjection;
using Flux.Messaging.Tests.Send.Commands;
using Flux.Messaging.Tests.Send.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Messaging.Tests.Send.Bus;

public class BusMultiHandlerTests
{
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenMultipleHandlersAreRegistered()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddTransient<ICommandHandler<CountCommand>, CountingCommandHandler>();
        services.AddTransient<ICommandHandler<CountCommand>, CountingCommandHandler>();
        
        Assert.Throws<InvalidOperationException>(services.AddFluxMessaging);
    }
}
