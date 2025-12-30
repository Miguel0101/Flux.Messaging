using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Extensions.DependencyInjection;

internal sealed class FluxMessagingBuilder(IServiceCollection services) : IFluxMessagingBuilder
{
    public IServiceCollection Services { get; } = services;
}
