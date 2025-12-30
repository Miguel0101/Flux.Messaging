using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging.Extensions.DependencyInjection;

public interface IFluxMessagingBuilder
{
    IServiceCollection Services { get; }
}