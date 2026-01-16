using Flux.Messaging.Abstractions.Request;

namespace Flux.Messaging.Tests.Send.Requests;

public record PingRequest : IRequest<string>;