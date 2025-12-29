using Microsoft.Extensions.DependencyInjection;

namespace Flux.Messaging;

public interface IFluxMessagingBuilder
{
    IServiceCollection Services { get; }
}