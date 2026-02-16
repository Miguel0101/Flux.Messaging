using BenchmarkDotNet.Attributes;
using Flux.Messaging.Abstractions.Bus;
using Flux.Messaging.Abstractions.Messages;
using Flux.Messaging.Abstractions.Providers;
using Flux.Messaging.Abstractions.Commands;
using Flux.Messaging.Benchmarks.Handlers;
using Flux.Messaging.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Flux.Messaging.Benchmarks.Commands;

namespace Flux.Messaging.Benchmarks.Bus;

[MemoryDiagnoser]
public class MessageBus
{
    private IMessageBus _bus = default!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddTransient<IMessageHandler<string>, PublishHandler>();
        services.AddTransient<ICommandHandler<SendCommand>, SendHandler>();

        services.AddLogging();
        services.AddFluxMessaging()
            .UseInMemory();

        _bus = services.BuildServiceProvider()
            .GetRequiredKeyedService<IMessageBus>(MessagingProviders.InMemory);
    }

    [Benchmark]
    public Task Publish()
        => _bus.PublishAsync("Benchmark");

    [Benchmark]
    public Task Send()
        => _bus.SendAsync(new SendCommand("Benchmark"));

}