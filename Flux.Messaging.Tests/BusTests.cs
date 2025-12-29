using Flux.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Tests;

public class BusTests
{
    public sealed class CapturingMessageHandler : IMessageHandler<string>
    {
        public TaskCompletionSource<string> ReceivedMessage { get; } = new();

        public override Task HandleAsync(string message, CancellationToken ct)
        {
            ReceivedMessage.TrySetResult(message);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task PublishAsync_ShouldDispatchMessageToRegisteredHandler()
    {
        var handler = new CapturingMessageHandler();

        var services = new ServiceCollection();

        services.AddSingleton<IMessageHandler<string>>(handler);
        services.AddFluxMessaging().UseInMemory();

        await using var provider = services.BuildServiceProvider();
        var messageBus = provider.GetRequiredService<IMessageBus>();

        const string publishedMessage = "Hello World!";

        await messageBus.PublishAsync(publishedMessage);

        var receivedMessage = await handler.ReceivedMessage.Task
            .WaitAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(publishedMessage, receivedMessage);
    }
}
