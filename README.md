# Flux.Messaging

**Flux.Messaging** is a messaging framework for .NET designed for **Clean Architecture**, **DDD**, and **distributed systems**.  
It provides a unified model for:

- **Publish / Subscribe** (fire-and-forget)
- **Distributed Request / Response**
- **Strongly-typed handlers**
- **Pluggable transports** (InMemory, AMQP, etc.)

Flux.Messaging does not try to hide messaging complexity — it **structures it properly**.

---

## Key Features

- Clear separation between **Bus**, **Dispatcher**, and **Transport**
- First-class support for **distributed request-response**
- Contract-first design
- DDD / CQRS / Event-Driven friendly
- No dependency on ASP.NET
- Ready for **AMQP 1.0** (Qpid, Azure Service Bus, etc.)
- InMemory transport for testing and local development

---

## Packages

| Package | Description |
|------|-------------|
| `Flux.Messaging.Abstractions` | Public contracts (Bus, Handlers, Envelope, Transport) |
| `Flux.Messaging.Core` | Core implementations |
| `Flux.Messaging.InMemory` | InMemory transport and dispatcher |
| `Flux.Messaging.Extensions.DependencyInjection` | Developer Experience (DX) |

---

## Core Concepts

### Publish (Event)
- Fire-and-forget
- Multiple handlers supported
- No response expected

### Send (Request)
- Request / Response
- Exactly one handler
- Uses `CorrelationId` and `ReplyTo`
- Can cross process and service boundaries

---

## Installation

```bash
dotnet add package Flux.Messaging
```

---

## Basic Setup (InMemory)

```csharp
services.AddFluxMessaging()
        .UseInMemory();
```

---

## Publish (Event)

### Event definition

```csharp
public record ProductAdded(Guid ProductId);
```

### Handler

```csharp
public sealed class ProductAddedHandler : IMessageHandler<ProductAdded>
{
    public Task HandleAsync(ProductAdded message, CancellationToken ct = default)
    {
        Console.WriteLine($"Product added: {message.ProductId}");
        return Task.CompletedTask;
    }
}
```

### Publish

```csharp
await messageBus.PublishAsync(new ProductAdded(productId));
```

---

## 📥 Send (Request / Response)

### Request

```csharp
public sealed record ReserveStock(Guid ProductId, int Quantity)
    : IRequest<ReserveStockResult>;
```

### Response

```csharp
public sealed record ReserveStockResult(bool Success);
```

### Handler

```csharp
public sealed class ReserveStockHandler
    : IRequestHandler<ReserveStock, ReserveStockResult>
{
    public Task<ReserveStockResult> HandleAsync(
        ReserveStock request,
        CancellationToken ct = default)
    {
        return Task.FromResult(new ReserveStockResult(true));
    }
}
```

### Send

```csharp
var result = await messageBus.SendAsync<ReserveStockResult>(
    new ReserveStock(productId, 2));
```

---

## Architecture Overview

```
Application
 └─ IMessageBus
      ├─ Send (Request)
      └─ Publish (Event)

Infrastructure
 ├─ ITransport (AMQP, InMemory, etc.)
 └─ IMessageDispatcher
```

- **Bus**: public API  
- **Dispatcher**: resolves handlers  
- **Transport**: delivers envelopes  

The transport does not know handlers.  
The dispatcher does not know the transport.

---

## Cross-language interoperability

Flux.Messaging is **.NET-first**, but transport-agnostic.

Other languages can:
- Consume AMQP envelopes
- Deserialize the payload
- Reply using `ReplyTo` and `CorrelationId`

No framework dependency is required.

---

## Testing

The solution includes behavior-focused tests:

- Concurrency
- Multiple handlers
- Fault tolerance
- Dispatch correctness

---

## License

This project is licensed under the Apache License, Version 2.0.  
See the [LICENSE](LICENSE) file for details.
