using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging;

internal sealed class FluxMessagingBuilder(IServiceCollection services) : IFluxMessagingBuilder
{
    public IServiceCollection Services { get; } = services;
}
