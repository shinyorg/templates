---
applyTo: "**/*.cs,**/*.razor,**/*.xaml"
---

# Shiny Mediator Skill

You are an expert in Shiny Mediator, a mediator pattern library for .NET applications.

## When to Use This Skill

Invoke this skill when the user wants to:
- Create request handlers, command handlers, event handlers, or stream handlers
- Generate contracts (IRequest, ICommand, IEvent, IStreamRequest)
- Add middleware (caching, resilience, validation, offline)
- Scaffold ASP.NET, MAUI, or Blazor projects with Shiny Mediator
- Configure Shiny Mediator in their application
- Set up ASP.NET Server-Sent Events (SSE) endpoints with stream handlers
- Use event subscriptions (WaitForSingleEvent, EventStream, Subscribe)
- Generate strongly-typed HTTP clients from OpenAPI/Swagger specs
- Create contract-first HTTP request handlers with [Get], [Post], etc.
- Migrate from MediatR to Shiny Mediator

## Library Overview

**Documentation**: https://shinylib.net/mediator

Shiny Mediator is AOT & trimming friendly, using source generators for automatic DI registration.

### Core Patterns

| Pattern | Contract | Handler | Usage |
|---------|----------|---------|-------|
| Request | `IRequest<TResult>` | `IRequestHandler<TRequest, TResult>` | Queries returning data |
| Command | `ICommand` | `ICommandHandler<TCommand>` | Void state changes |
| Event | `IEvent` | `IEventHandler<TEvent>` | Pub/sub notifications |
| Stream | `IStreamRequest<TResult>` | `IStreamRequestHandler<TRequest, TResult>` | IAsyncEnumerable |

### Handler Registration

Always use registration attributes:
```csharp
[MediatorSingleton]  // Stateless handlers
[MediatorScoped]     // Handlers needing per-request services (DbContext)
```

**Critical: Partial Class Requirement**

When using **any middleware attribute** (`[Cache]`, `[OfflineAvailable]`, `[Resilient]`, `[MainThread]`, `[TimerRefresh]`, `[Sample]`, `[Throttle]`), the handler class **must be declared as `partial`**:
```csharp
[MediatorSingleton]
public partial class MyHandler : IRequestHandler<MyRequest, MyResult>  // partial required!
{
    [Cache(AbsoluteExpirationSeconds = 60)]
    public Task<MyResult> Handle(...) { }
}
```
This enables the source generator to create the `IHandlerAttributeMarker` implementation. Without `partial`, you'll get error `SHINY001`.

### Basic Setup

**ASP.NET:**
```csharp
builder.Services.AddShinyMediator(x => x
    .AddMediatorRegistry()
);
app.MapGeneratedMediatorEndpoints();
```

**MAUI:**
```csharp
builder.AddShinyMediator(x => x
    .AddMediatorRegistry()
    .UseMaui()
    .AddMauiPersistentCache()
    .PreventEventExceptions()
);
```

**Blazor:**
```csharp
builder.Services.AddShinyMediator(x => x
    .AddMediatorRegistry()
    .UseBlazor()
    .PreventEventExceptions()
);
```

## Code Generation Instructions

When generating Shiny Mediator code:

### 1. Contracts

Always use records for immutability:
```csharp
public record GetUserRequest(int UserId) : IRequest<UserDto>;
public record CreateUserCommand(string Name, string Email) : ICommand;
public record UserCreatedEvent(int UserId, string Name) : IEvent;
```

### 2. Handlers

Include all three parameters in Handle method:
```csharp
[MediatorScoped]
public class GetUserRequestHandler : IRequestHandler<GetUserRequest, UserDto>
{
    public Task<UserDto> Handle(
        GetUserRequest request,
        IMediatorContext context,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### 3. Middleware Attributes

Apply to handler methods as needed:
- `[Cache(AbsoluteExpirationSeconds = N)]` - Cacheable queries
- `[OfflineAvailable]` - Offline storage for mobile
- `[Resilient("policyName")]` - Retry/timeout policies
- `[MainThread]` - MAUI main thread execution
- `[TimerRefresh(milliseconds)]` - Auto-refresh streams
- `[Sample(milliseconds)]` - Fixed-window sampling (last event in window executes)
- `[Throttle(milliseconds)]` - True throttle (first event executes, cooldown discards rest)
- `[Validate]` - Data annotation validation

**When using ANY of these attributes, the handler class MUST be `partial`:**
```csharp
[MediatorSingleton]
public partial class CachedHandler : IRequestHandler<MyRequest, MyData>
{
    [Cache(AbsoluteExpirationSeconds = 60)]
    [OfflineAvailable]
    public Task<MyData> Handle(...) { }
}
```

### 4. Middleware Ordering

Use `[MiddlewareOrder(int)]` on custom middleware classes to control execution order. Lower values run first (outermost). Default is 0.
```csharp
[MiddlewareOrder(-100)]  // Runs before middleware with higher order values
[MediatorSingleton]
public class EarlyMiddleware<TRequest, TResult> : IRequestMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
{ ... }
```

### 5. File Organization

Place files in appropriate folders:
- Contracts: `Contracts/{Name}Request.cs`, `Contracts/{Name}Command.cs`
- Handlers: `Handlers/{Name}Handler.cs`
- Middleware: `Middleware/{Name}Middleware.cs`

## Usage Examples

**Request:**
```csharp
var response = await mediator.Request(new GetUserRequest(1));
var user = response.Result;
```

**Command:**
```csharp
await mediator.Send(new CreateUserCommand("John", "john@example.com"));
```

**Event:**
```csharp
await mediator.Publish(new UserCreatedEvent(1, "John"));
```

**Chaining via Context:**
```csharp
public async Task<UserDto> Handle(GetUserRequest request, IMediatorContext context, CancellationToken ct)
{
    // Use context to chain operations (shares scope)
    await context.Publish(new UserAccessedEvent(request.UserId));
    return new UserDto(...);
}
```

### Event Subscriptions & Streaming

**WaitForSingleEvent** - Await a single event occurrence (with optional filter):
```csharp
// Wait for a specific event (blocks until event fires or cancellation)
var evt = await mediator.WaitForSingleEvent<OrderCompletedEvent>(
    filter: e => e.OrderId == orderId,
    cancellationToken: ct
);
```

**EventStream** - Continuous IAsyncEnumerable stream of events (uses Channels internally):
```csharp
// Consume events as an async stream
await foreach (var evt in mediator.EventStream<PriceUpdatedEvent>(cancellationToken: ct))
{
    Console.WriteLine($"New price: {evt.Price}");
}
```

**Subscribe** - Manual subscription returning IDisposable:
```csharp
var sub = mediator.Subscribe<MyEvent>((ev, ctx, ct) =>
{
    Console.WriteLine($"Event received: {ev}");
    return Task.CompletedTask;
});
// Later: sub.Dispose() to unsubscribe
```

### ASP.NET Server-Sent Events (SSE)

Stream handlers decorated with `[MediatorHttpGet]` or `[MediatorHttpPost]` on an `IStreamRequestHandler` are **automatically generated as SSE endpoints** by the source generator via `MapGeneratedMediatorEndpoints()`.

**Manual SSE endpoint with EventStream:**
```csharp
app.MapGet("/events", ([FromServices] IMediator mediator) =>
    TypedResults.ServerSentEvents(mediator.EventStream<MyEvent>())
);
```

**Stream handler as auto-generated SSE endpoint:**
```csharp
public record TickerStreamRequest : IStreamRequest<int>;

[MediatorScoped]
public class TickerStreamHandler : IStreamRequestHandler<TickerStreamRequest, int>
{
    [MediatorHttpGet("/ticker")]
    public async IAsyncEnumerable<int> Handle(
        TickerStreamRequest request,
        IMediatorContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return i++;
            await Task.Delay(1000, cancellationToken);
        }
    }
}
```

**HTTP client-side SSE consumption:** Implement `IServerSentEventsStream` marker on the contract to indicate the server returns SSE format. The generated HTTP handler will use `ReadServerSentEvents<T>()` to parse the `data:` prefixed SSE lines.
```csharp
public record TickerStreamRequest : IStreamRequest<int>, IServerSentEventsStream;
```

## Contract-First HTTP Clients

Shiny Mediator generates strongly-typed HTTP client handlers from contract classes decorated with HTTP method attributes. No manual `HttpClient` code needed.

### Manual HTTP Contracts

Decorate request classes with `[Get]`, `[Post]`, `[Put]`, `[Delete]`, `[Patch]` and use `[Query]`, `[Header]`, `[Body]` on properties:
```csharp
[Get("/api/orders/{OrderId}")]
public class GetOrderRequest : IRequest<OrderDto>
{
    public int OrderId { get; set; }          // Route parameter (matches {OrderId})

    [Query("status")]
    public string? Status { get; set; }        // ?status=value

    [Header("Authorization")]
    public string? AuthToken { get; set; }     // HTTP header
}

[Post("/api/orders")]
public class CreateOrderRequest : IRequest<OrderDto>
{
    [Body]
    public CreateOrderBody? Body { get; set; } // JSON request body
}
```

The source generator creates handler classes inheriting `BaseHttpRequestHandler` that build routes, add query/header parameters, serialize bodies, and call `IHttpClientFactory`.

**Registration:**
```csharp
builder.Services.AddShinyMediator(x => x
    .AddMediatorRegistry()
    .AddStrongTypedHttpClient()   // Registers generated HTTP handlers
);
```

### OpenAPI Client Generation

Generate contracts, models, and handlers directly from OpenAPI/Swagger specs. Add a `<MediatorHttp>` item in your `.csproj`:

```xml
<ItemGroup>
    <!-- Remote URL: Include is a logical name, Uri points to the spec -->
    <MediatorHttp Include="MyApi"
                  Uri="https://api.example.com/swagger/v1/swagger.json"
                  Namespace="MyApp.ExternalApi"
                  ContractPostfix="HttpRequest"
                  GenerateJsonConverters="true"
                  Visible="false" />

    <!-- Local file: Include is the file path, no Uri needed -->
    <MediatorHttp Include="./specs/openapi.yaml"
                  Namespace="MyApp.LocalApi"
                  Visible="false" />
</ItemGroup>
```

`Include` can be a **file path** (for local specs) or a **logical name** when `Uri` is set separately.

**MediatorHttp metadata options:**

| Metadata | Description |
|----------|-------------|
| `Uri` | URL or local path to OpenAPI JSON/YAML spec |
| `Namespace` | C# namespace for generated types |
| `ContractPrefix` | Prefix for generated contract class names |
| `ContractPostfix` | Postfix for generated contract class names |
| `UseInternalClasses` | Generate internal classes instead of public |
| `GenerateModelsOnly` | Only generate models, no handlers |
| `GenerateJsonConverters` | Generate `JsonConverter` implementations for enums |

**Registration of generated OpenAPI client:**
```csharp
builder.Services.AddShinyMediator(x => x
    .AddMediatorRegistry()
    .AddGeneratedOpenApiClient()   // Registers all OpenAPI-generated handlers
);
```

Then use like any other mediator request:
```csharp
var result = await mediator.Request(new GetPetsHttpRequest { Status = "available" });
```

## Best Practices

1. **Use records for contracts** - Immutable, value equality
2. **One handler per file** - Focused and testable
3. **Appropriate lifetime** - Singleton for stateless, Scoped for DbContext
4. **Use `partial` with middleware attributes** - Required for `[Cache]`, `[OfflineAvailable]`, `[Resilient]`, etc.
5. **Chain via context** - Use `context.Request()` not injecting IMediator
6. **Implement IContractKey** - For custom cache/offline keys
7. **Always pass CancellationToken** - Respect cancellation
8. **Use IServerSentEventsStream marker** - On stream contracts consumed via HTTP SSE
9. **Stream handlers only support GET/POST** - Other HTTP methods are not valid for SSE endpoints
10. **Use EventStream for SSE push endpoints** - Combine `mediator.EventStream<T>()` with `TypedResults.ServerSentEvents()` for event-driven SSE

## Reference Files

For detailed templates and examples, see:
- `reference/templates.md` - Code generation templates (includes SSE endpoint templates)
- `reference/scaffolding.md` - Project structure templates
- `reference/middleware.md` - Middleware configuration
- `reference/api-reference.md` - Full API, event subscriptions, SSE, and NuGet packages

## Common Packages

```bash
dotnet add package Shiny.Mediator                              # Core
dotnet add package Shiny.Mediator.Maui                         # MAUI
dotnet add package Shiny.Mediator.Blazor                       # Blazor
dotnet add package Shiny.Mediator.AspNet                       # ASP.NET
dotnet add package Shiny.Mediator.Resilience                   # Polly
dotnet add package Shiny.Mediator.Caching.MicrosoftMemoryCache # Caching
dotnet add package Shiny.Mediator.AppSupport                   # Offline
```

# API Reference

## Installation

```bash
# Core package
dotnet add package Shiny.Mediator

# Platform-specific packages
dotnet add package Shiny.Mediator.Maui          # For .NET MAUI
dotnet add package Shiny.Mediator.Blazor        # For Blazor
dotnet add package Shiny.Mediator.AspNet        # For ASP.NET
dotnet add package Shiny.Mediator.Uno           # For Uno Platform

# Middleware packages
dotnet add package Shiny.Mediator.Resilience    # Polly resilience
dotnet add package Shiny.Mediator.FluentValidation
dotnet add package Shiny.Mediator.Caching.MicrosoftMemoryCache
dotnet add package Shiny.Mediator.AppSupport    # Offline, replay, user notifications
```

## NuGet Packages Reference

| Package | Description |
|---------|-------------|
| `Shiny.Mediator` | Core mediator |
| `Shiny.Mediator.Contracts` | Contract interfaces only |
| `Shiny.Mediator.Maui` | MAUI integration |
| `Shiny.Mediator.Blazor` | Blazor integration |
| `Shiny.Mediator.AspNet` | ASP.NET integration |
| `Shiny.Mediator.Uno` | Uno Platform integration |
| `Shiny.Mediator.Prism` | Prism MVVM integration |
| `Shiny.Mediator.AppSupport` | Offline, replay, user notifications |
| `Shiny.Mediator.Resilience` | Polly resilience middleware |
| `Shiny.Mediator.FluentValidation` | FluentValidation middleware |
| `Shiny.Mediator.Caching.MicrosoftMemoryCache` | Memory caching |
| `Shiny.Mediator.DapperRequests` | Dapper SQL query handlers |
| `Shiny.Mediator.Sentry` | Sentry error tracking |
| `Shiny.Mediator.Testing` | Testing utilities |

## Core Interfaces

### IRequest<TResult>
```csharp
public interface IRequest<TResult> { }
```

### ICommand
```csharp
public interface ICommand { }
```

### IEvent
```csharp
public interface IEvent { }
```

### IStreamRequest<TResult>
```csharp
public interface IStreamRequest<TResult> { }
```

### IContractKey
```csharp
public interface IContractKey
{
    string GetKey();
}
```

## Handler Interfaces

### IRequestHandler<TRequest, TResult>
```csharp
public interface IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    Task<TResult> Handle(TRequest request, IMediatorContext context, CancellationToken cancellationToken);
}
```

### ICommandHandler<TCommand>
```csharp
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task Handle(TCommand command, IMediatorContext context, CancellationToken cancellationToken);
}
```

### IEventHandler<TEvent>
```csharp
public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task Handle(TEvent @event, IMediatorContext context, CancellationToken cancellationToken);
}
```

### IStreamRequestHandler<TRequest, TResult>
```csharp
public interface IStreamRequestHandler<TRequest, TResult> where TRequest : IStreamRequest<TResult>
{
    IAsyncEnumerable<TResult> Handle(TRequest request, IMediatorContext context, CancellationToken cancellationToken);
}
```

## Middleware Interfaces

### IRequestMiddleware<TRequest, TResult>
```csharp
public interface IRequestMiddleware<TRequest, TResult> where TRequest : IRequest<TResult>
{
    Task<TResult> Process(IMediatorContext context, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken);
}
```

### ICommandMiddleware<TCommand>
```csharp
public interface ICommandMiddleware<TCommand> where TCommand : ICommand
{
    Task Process(IMediatorContext context, CommandHandlerDelegate next, CancellationToken cancellationToken);
}
```

### IEventMiddleware<TEvent>
```csharp
public interface IEventMiddleware<TEvent> where TEvent : IEvent
{
    Task Process(IMediatorContext context, EventHandlerDelegate next, CancellationToken cancellationToken);
}
```

## IMediator Interface

```csharp
public interface IMediator
{
    // Requests
    Task<(IMediatorContext Context, TResult Result)> Request<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default, IMediatorContext? context = null);

    // Commands
    Task<IMediatorContext> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default, IMediatorContext? context = null) where TCommand : ICommand;

    // Events
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default, bool executeInParallel = false) where TEvent : IEvent;
    void PublishToBackground<TEvent>(TEvent @event) where TEvent : IEvent;

    // Subscriptions
    IDisposable Subscribe<TEvent>(Func<TEvent, IMediatorContext, CancellationToken, Task> handler) where TEvent : IEvent;
    Task<TEvent> WaitForSingleEvent<TEvent>(CancellationToken cancellationToken = default) where TEvent : IEvent;
    IAsyncEnumerable<TEvent> EventStream<TEvent>(CancellationToken cancellationToken = default) where TEvent : IEvent;

    // Stream Requests
    IAsyncEnumerable<(IMediatorContext Context, TResult Result)> Request<TResult>(IStreamRequest<TResult> request, CancellationToken cancellationToken = default);

    // Store Management
    Task FlushAllStores();
    Task FlushStores(string key, bool partialMatch = false);
}
```

### Event Subscription Methods

**WaitForSingleEvent** - Waits for a single event, optionally filtered. Uses `TaskCompletionSource` internally. Returns when the first matching event fires or cancellation is requested.
```csharp
// Extension method signature (supports optional filter)
Task<T> WaitForSingleEvent<T>(Func<T, bool>? filter = null, CancellationToken cancellationToken = default) where T : IEvent;

// Usage
var evt = await mediator.WaitForSingleEvent<OrderCompletedEvent>(
    filter: e => e.OrderId == myOrderId,
    cancellationToken: ct
);
```

**EventStream** - Returns an `IAsyncEnumerable<T>` of events using `System.Threading.Channels`. The stream continues until cancellation.
```csharp
// Extension method signature (supports optional filter)
IAsyncEnumerable<T> EventStream<T>(Func<T, bool>? filter = null, CancellationToken cancellationToken = default) where T : IEvent;

// Usage
await foreach (var evt in mediator.EventStream<PriceUpdatedEvent>(cancellationToken: ct))
{
    ProcessPriceUpdate(evt);
}
```

**Subscribe** - Manual event subscription returning `IDisposable` for cleanup.
```csharp
IDisposable Subscribe<TEvent>(Func<TEvent, IMediatorContext, CancellationToken, Task> handler) where TEvent : IEvent;
```

### Server-Sent Events (SSE) Interfaces

**IServerSentEventsStream** - Marker interface for stream request contracts consumed via HTTP SSE. Tells the generated HTTP handler to parse SSE `data:` lines.
```csharp
public interface IServerSentEventsStream;

// Usage on contract
public record MyStreamRequest : IStreamRequest<MyData>, IServerSentEventsStream;
```

### ASP.NET SSE Endpoint Builder Extensions

```csharp
// Map stream handlers as SSE endpoints (used by source generator, can also be called manually)
RouteHandlerBuilder MapMediatorServerSentEventsGet<TRequest, TResult>(string pattern, string? eventName = null)
    where TRequest : IStreamRequest<TResult>;

RouteHandlerBuilder MapMediatorServerSentEventsPost<TRequest, TResult>(string pattern, string? eventName = null)
    where TRequest : IStreamRequest<TResult>;
```

These are automatically called by `MapGeneratedMediatorEndpoints()` for stream handlers decorated with `[MediatorHttpGet]` or `[MediatorHttpPost]`. They use `TypedResults.ServerSentEvents()` internally.

## IMediatorContext Interface

```csharp
public interface IMediatorContext
{
    IServiceScope ServiceScope { get; }
    IMediatorContext? Parent { get; }
    bool BypassMiddlewareEnabled { get; set; }
    bool BypassExceptionHandlingEnabled { get; set; }

    void AddHeader(string key, object value);
    T? TryGetValue<T>(string key);

    // Chained operations
    Task<(IMediatorContext Context, TResult Result)> Request<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
    Task<IMediatorContext> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
}
```

## Attributes

### Registration Attributes
- `[MediatorSingleton]` - Register handler as singleton
- `[MediatorScoped]` - Register handler as scoped

### Middleware Attributes

> **Important:** When using any of these middleware attributes, the handler class **must be declared as `partial`**. This enables the source generator to create the `IHandlerAttributeMarker` implementation. Without `partial`, you'll get error `SHINY001`.

- `[Cache(AbsoluteExpirationSeconds, SlidingExpirationSeconds)]` - Enable caching
- `[OfflineAvailable]` - Enable offline storage
- `[Resilient("policyName")]` - Apply resilience policy
- `[MainThread]` - Execute on main thread (MAUI)
- `[TimerRefresh(milliseconds)]` - Auto-refresh streams
- `[Sample(milliseconds)]` - Fixed-window sampling (last event in window executes)
- `[Throttle(milliseconds)]` - True throttle (first event executes, cooldown discards rest)
- `[Validate]` - Enable validation

**Example with partial class:**
```csharp
[MediatorSingleton]
public partial class MyHandler : IRequestHandler<MyRequest, MyResult>
{
    [Cache(AbsoluteExpirationSeconds = 60)]
    public Task<MyResult> Handle(...) { }
}
```

### Middleware Class Attributes
- `[MiddlewareOrder(order)]` - Control middleware execution order (lower = runs first, default 0)

### HTTP Contract Attributes (Client-Side)
- `[Get("/route")]`, `[Post("/route")]`, `[Put("/route")]`, `[Delete("/route")]`, `[Patch("/route")]` - HTTP method and route
- `[Query("paramName")]` - Query string parameter (property name used if no explicit name)
- `[Header("headerName")]` - HTTP header
- `[Body]` - Request body (only one per contract, serialized as JSON)
- Route parameters: Properties matching `{ParamName}` in the route are interpolated automatically

**Route parameter resolution:** Properties whose names match `{placeholders}` in the route string are used as path parameters. Remaining properties need `[Query]`, `[Header]`, or `[Body]` attributes.

### ASP.NET Endpoint Attributes (Server-Side)
- `[MediatorHttpGroup("/route")]` - Group endpoints
- `[MediatorHttpGet("/route")]`, `[MediatorHttpPost("/route")]`, etc.

## HTTP Client Infrastructure

### BaseHttpRequestHandler
Base class for all generated HTTP handlers. Handles request execution, response deserialization, SSE parsing, and streaming.

```csharp
public record HttpHandlerServices(
    ILoggerFactory LoggerFactory,
    IConfiguration Configuration,
    ISerializerService Serializer,
    IHttpClientFactory HttpClientFactory,
    IEnumerable<IHttpRequestDecorator> Decorators
);
```

### IHttpRequestDecorator
Customize HTTP requests before they are sent (e.g., add auth headers):
```csharp
public interface IHttpRequestDecorator
{
    Task Decorate(HttpRequestMessage httpRequest, IMediatorContext context);
}
```

### Registration Methods
```csharp
// Register manually-written HTTP contract handlers
builder.Services.AddShinyMediator(x => x.AddStrongTypedHttpClient());

// Register OpenAPI-generated handlers
builder.Services.AddShinyMediator(x => x.AddGeneratedOpenApiClient());
```

## OpenAPI Client Generation

Generate strongly-typed mediator contracts and handlers from OpenAPI specs via `<MediatorHttp>` MSBuild items.

### MSBuild Configuration
```xml
<ItemGroup>
    <!-- Remote: Include is a logical name, Uri is the URL -->
    <MediatorHttp Include="MyApi"
                  Uri="https://api.example.com/swagger/v1/swagger.json"
                  Namespace="MyApp.Api"
                  ContractPostfix="HttpRequest"
                  GenerateJsonConverters="true"
                  Visible="false" />

    <!-- Local: Include is the file path, no Uri needed -->
    <MediatorHttp Include="./specs/openapi.yaml"
                  Namespace="MyApp.LocalApi"
                  Visible="false" />
</ItemGroup>
```

`Include` can be a **file path** (for local specs) or a **logical name** when `Uri` is set separately.

### MediatorHttp Metadata

| Metadata | Description | Default |
|----------|-------------|---------|
| `Uri` | URL or local path to OpenAPI JSON/YAML spec | Required |
| `Namespace` | C# namespace for generated types | Required |
| `ContractPrefix` | Prefix for generated contract class names | (none) |
| `ContractPostfix` | Postfix for generated contract class names | (none) |
| `UseInternalClasses` | Generate `internal` classes instead of `public` | `false` |
| `GenerateModelsOnly` | Only generate model classes, skip handlers | `false` |
| `GenerateJsonConverters` | Generate `JsonConverter` for string enums | `false` |

### What Gets Generated
- **Models** from `components/schemas` with `[JsonPropertyName]` attributes
- **Enums** with optional `JsonStringEnumConverter`
- **Contract classes** implementing `IRequest<TResult>` with route/query/header/body properties
- **Handler classes** inheriting `BaseHttpRequestHandler`
- **Registration extension** `AddGeneratedOpenApiClient()`

## Migration from MediatR

| MediatR | Shiny Mediator |
|---------|----------------|
| `IRequest<T>` | `IRequest<T>` |
| `IRequestHandler<TRequest, TResponse>` | `IRequestHandler<TRequest, TResult>` |
| `INotification` | `IEvent` |
| `INotificationHandler<T>` | `IEventHandler<T>` |
| `IPipelineBehavior<,>` | `IRequestMiddleware<,>` |
| `services.AddMediatR()` | `services.AddShinyMediator()` |
| `await _mediator.Send(request)` | `await mediator.Request(request)` |
| `await _mediator.Publish(notification)` | `await mediator.Publish(@event)` |

**Key Differences:**
1. Add `[MediatorSingleton]` or `[MediatorScoped]` to handlers
2. Handler signature includes `IMediatorContext context` parameter
3. `Request()` returns `(IMediatorContext, TResult)` tuple
4. Use `Send()` for commands (void), `Request()` for queries (return value)

## Troubleshooting

### Error SHINY001: Handler must be partial
- **Cause:** You're using middleware attributes (`[Cache]`, `[OfflineAvailable]`, `[Resilient]`, `[MainThread]`, `[TimerRefresh]`, `[Sample]`, `[Throttle]`) but the handler class is not declared as `partial`
- **Fix:** Add `partial` keyword to the class declaration:
  ```csharp
  public partial class MyHandler : IRequestHandler<...>  // Add 'partial'
  ```

### Handler Not Found
- Ensure handler has `[MediatorSingleton]` or `[MediatorScoped]` attribute
- Verify `AddMediatorRegistry()` is called in setup
- Check handler implements correct interface

### Caching Not Working
- Verify cache middleware is registered (`AddMemoryCaching()` or `AddMauiPersistentCache()`)
- Check `[Cache]` attribute is on the handler method
- **Ensure handler class is `partial`**
- Implement `IContractKey` for custom cache keys

### Events Not Firing
- Ensure event handler is registered or implements `IEventHandler<T>` on a ViewModel
- For MAUI, call `UseMaui()` in setup
- For Blazor, call `UseBlazor()` in setup

### Middleware Not Executing
- Check middleware registration order (or use `[MiddlewareOrder]` to control explicitly)
- Verify open generic middleware is registered with `AddOpenRequestMiddleware()`
- Ensure middleware has `[MediatorSingleton]` attribute

### Middleware Running in Wrong Order
- Use `[MiddlewareOrder(n)]` on middleware classes to control execution order
- Lower values run first (outermost in pipeline), default is 0
- Middleware with the same order preserves DI registration order

# Middleware Reference

> **Critical: Partial Class Requirement**
>
> When using **any middleware attribute** on a handler method, the handler class **must be declared as `partial`**. This enables the source generator to create the `IHandlerAttributeMarker` implementation for runtime attribute lookup.
>
> **Without `partial`, you'll get compiler error `SHINY001`.**

## Built-in Middleware Attributes

Apply middleware to handlers using attributes. **Remember: class must be `partial`!**

```csharp
[MediatorSingleton]
public partial class MyRequestHandler : IRequestHandler<MyRequest, string>  // MUST be partial!
{
    [Cache(AbsoluteExpirationSeconds = 300)]  // Cache for 5 minutes
    [OfflineAvailable]                         // Store for offline access
    [Resilient("default")]                     // Apply resilience policy
    [MainThread]                               // Execute on main thread (MAUI)
    [TimerRefresh(5000)]                       // Auto-refresh stream every 5s
    public Task<string> Handle(MyRequest request, IMediatorContext context, CancellationToken ct)
    {
        return Task.FromResult("result");
    }
}
```

## Caching Middleware

```csharp
// Setup
builder.AddShinyMediator(x => x
    .AddMauiPersistentCache()  // or .AddMemoryCaching()
);

// Handler - MUST be partial when using [Cache]
[MediatorSingleton]
public partial class CachedHandler : IRequestHandler<MyRequest, MyData>
{
    [Cache(AbsoluteExpirationSeconds = 60, SlidingExpirationSeconds = 30)]
    public Task<MyData> Handle(MyRequest request, IMediatorContext context, CancellationToken ct)
    {
        return FetchFromApi();
    }
}

// Check if result came from cache
var response = await mediator.Request(new MyRequest());
var cacheInfo = response.Context.Cache();  // Returns CacheContext if cached
```

## Offline Availability (App Support)

```csharp
// Setup
builder.AddShinyMediator(x => x
    .AddStandardAppSupportMiddleware()  // Includes offline, replay, user notifications
);

// Handler
[MediatorSingleton]
public partial class OfflineHandler : IRequestHandler<MyRequest, MyData>
{
    [OfflineAvailable]
    public Task<MyData> Handle(MyRequest request, IMediatorContext context, CancellationToken ct)
    {
        return FetchFromApi();
    }
}

// Check if result came from offline storage
var response = await mediator.Request(new MyRequest());
var offlineInfo = response.Context.Offline();  // Returns OfflineAvailableContext if offline
```

## Resilience Middleware (Polly)

```csharp
// Setup via configuration
builder.AddShinyMediator(x => x
    .AddResiliencyMiddleware(builder.Configuration)
);

// appsettings.json
{
    "Resilience": {
        "default": {
            "TimeoutMilliseconds": 5000,
            "Retry": {
                "MaxAttempts": 3,
                "DelayMilliseconds": 1000,
                "BackoffType": "Exponential",
                "UseJitter": true
            }
        }
    }
}

// Handler - MUST be partial when using [Resilient]
[MediatorSingleton]
public partial class ResilientHandler : IRequestHandler<MyRequest, string>
{
    [Resilient("default")]
    public Task<string> Handle(MyRequest request, IMediatorContext context, CancellationToken ct)
    {
        return CallUnreliableApi();
    }
}

// Or setup programmatically
builder.AddShinyMediator(x => x
    .AddResiliencyMiddleware(
        ("myPolicy", builder => builder
            .AddTimeout(TimeSpan.FromSeconds(10))
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 }))
    )
);
```

## Validation Middleware

```csharp
// Data Annotations
builder.AddShinyMediator(x => x.AddDataAnnotations());

// Contract with validation
[Validate]
public class CreateUserCommand : ICommand
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(2)]
    public string Name { get; set; }
}

// FluentValidation
builder.AddShinyMediator(x => x.AddFluentValidation());
```

## Event Sampling

Fixed-window sampling for rapid event firings. The first event starts a timer. Subsequent events within the window replace the pending delegate but do not reset the timer. When the timer fires, the **last** event received is executed.

```csharp
// Setup
builder.AddShinyMediator(x => x
    .AddSampleEventMiddleware()
);

// Handler - MUST be partial when using [Sample]
[MediatorSingleton]
public partial class SearchHandler : IEventHandler<SearchChangedEvent>
{
    [Sample(500)]  // 500ms fixed window
    public Task Handle(SearchChangedEvent @event, IMediatorContext context, CancellationToken ct)
    {
        // Executes once at the end of the 500ms window with the last event received
        return PerformSearch(@event.Query);
    }
}
```

## Event Throttling

True throttle for rapid event firings. The **first** event executes immediately, then all subsequent events within the cooldown window are discarded. After cooldown expires, the next event executes immediately again.

```csharp
// Setup
builder.AddShinyMediator(x => x
    .AddThrottleEventMiddleware()
);

// Handler - MUST be partial when using [Throttle]
[MediatorSingleton]
public partial class ButtonClickHandler : IEventHandler<ButtonClickedEvent>
{
    [Throttle(1000)]  // 1s cooldown
    public Task Handle(ButtonClickedEvent @event, IMediatorContext context, CancellationToken ct)
    {
        // Executes immediately on first event, ignores duplicates for 1s
        return ProcessClick();
    }
}
```

## Performance Logging

```csharp
builder.AddShinyMediator(x => x.AddPerformanceLoggingMiddleware());
```

## Middleware Ordering

Control middleware execution order with `[MiddlewareOrder]` on middleware classes. Lower values run first (outermost in pipeline). Default order is 0.

```csharp
[MiddlewareOrder(-100)]  // Runs first - validation before anything else
[MediatorSingleton]
public class ValidationMiddleware<TRequest, TResult> : IRequestMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    public async Task<TResult> Process(
        IMediatorContext context,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        Validate(context.Message);
        return await next();
    }
}

[MiddlewareOrder(100)]  // Runs last - closest to handler
[MediatorSingleton]
public class CachingMiddleware<TRequest, TResult> : IRequestMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    public async Task<TResult> Process(
        IMediatorContext context,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        // Check cache, call next() on miss
        return await next();
    }
}
```

Middleware without `[MiddlewareOrder]` defaults to 0. Middleware with the same order preserves DI registration order (stable sort).

## Custom Middleware

```csharp
// Request middleware
[MediatorSingleton]
public class LoggingMiddleware<TRequest, TResult> : IRequestMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    public async Task<TResult> Process(
        IMediatorContext context,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Before: {typeof(TRequest).Name}");
        var result = await next();
        Console.WriteLine($"After: {typeof(TRequest).Name}");
        return result;
    }
}

// Register open generic middleware
builder.AddShinyMediator(x => x
    .AddOpenRequestMiddleware(typeof(LoggingMiddleware<,>))
);
```

## Exception Handling

```csharp
// Global exception handler
public class MyExceptionHandler : IExceptionHandler
{
    public Task<bool> Handle(IMediatorContext context, Exception exception)
    {
        // Return true if handled, false to rethrow
        if (exception is MyExpectedException)
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

// Register
builder.AddShinyMediator(x => x
    .AddExceptionHandler<MyExceptionHandler>()
    .PreventEventExceptions()  // Prevents event handler exceptions from crashing
);

// ASP.NET JSON validation response
builder.AddShinyMediator(x => x.AddJsonValidationExceptionHandler());
```

# Project Scaffolding Templates

## ASP.NET Project Structure

```
{ProjectName}/
├── Program.cs
├── {ProjectName}.csproj
├── appsettings.json
├── Contracts/
│   ├── Requests/
│   ├── Commands/
│   └── Events/
├── Handlers/
│   ├── Requests/
│   ├── Commands/
│   ├── Events/
│   └── Endpoints/
├── Middleware/
├── Services/
└── Validators/
```

**Program.cs for ASP.NET:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Shiny Mediator
builder.Services.AddShinyMediator(x => x
    .AddMediatorRegistry()
    .AddMemoryCaching()
    .AddPerformanceLoggingMiddleware()
    .AddDataAnnotations()
    .AddJsonValidationExceptionHandler()
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGeneratedMediatorEndpoints();

app.Run();
```

**appsettings.json with Resilience:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Resilience": {
    "default": {
      "TimeoutMilliseconds": 30000,
      "Retry": {
        "MaxAttempts": 3,
        "DelayMilliseconds": 1000,
        "BackoffType": "Exponential",
        "UseJitter": true
      }
    },
    "fast": {
      "TimeoutMilliseconds": 5000,
      "Retry": {
        "MaxAttempts": 2,
        "DelayMilliseconds": 500,
        "BackoffType": "Linear"
      }
    }
  }
}
```

## MAUI Project Structure

```
{ProjectName}/
├── MauiProgram.cs
├── {ProjectName}.csproj
├── App.xaml / App.xaml.cs
├── AppShell.xaml / AppShell.xaml.cs
├── Contracts/
│   ├── Requests/
│   ├── Commands/
│   └── Events/
├── Handlers/
├── Middleware/
├── ViewModels/
├── Views/
└── Services/
```

**MauiProgram.cs:**
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Shiny Mediator
        builder.AddShinyMediator(x => x
            .AddMediatorRegistry()
            .UseMaui()
            .AddMauiPersistentCache()
            .AddStandardAppSupportMiddleware()
            .AddDataAnnotations()
            .PreventEventExceptions()
        );

        // Register ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
```

## Blazor Project Structure

```
{ProjectName}/
├── Program.cs
├── {ProjectName}.csproj
├── App.razor
├── _Imports.razor
├── Contracts/
│   ├── Requests/
│   ├── Commands/
│   └── Events/
├── Handlers/
├── Middleware/
├── Components/
│   ├── Layout/
│   └── Pages/
└── Services/
```

**Program.cs for Blazor:**
```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Add HTTP client
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Shiny Mediator
builder.Services.AddShinyMediator(x => x
    .AddMediatorRegistry()
    .UseBlazor()
    .AddMemoryCaching()
    .PreventEventExceptions()
);

await builder.Build().RunAsync();
```

## Feature-Based Organization

For larger projects, organize by feature:

```
Features/
├── Users/
│   ├── GetUser/
│   │   ├── GetUserRequest.cs
│   │   └── GetUserHandler.cs
│   ├── CreateUser/
│   │   ├── CreateUserCommand.cs
│   │   └── CreateUserHandler.cs
│   └── UserCreated/
│       ├── UserCreatedEvent.cs
│       └── UserCreatedEventHandler.cs
├── Products/
│   ├── GetProducts/
│   ├── CreateProduct/
│   └── ProductUpdated/
└── Orders/
    ├── GetOrder/
    ├── CreateOrder/
    └── OrderPlaced/
```

# Shiny Mediator Code Templates

> **Important: Partial Class Requirement**
>
> When using **any middleware attribute** (`[Cache]`, `[OfflineAvailable]`, `[Resilient]`, `[MainThread]`, `[TimerRefresh]`, `[Sample]`, `[Throttle]`), the handler class **must be declared as `partial`**. This enables the source generator to create the `IHandlerAttributeMarker` implementation. Without `partial`, you'll get compiler error `SHINY001`.

## Request Handler Template

When generating a request handler, use this pattern:

```csharp
// Contract: Contracts/{Name}Request.cs
namespace {Namespace}.Contracts;

public record {Name}Request({Parameters}) : IRequest<{ResultType}>;

public record {ResultType}({ResultProperties});
```

```csharp
// Handler: Handlers/{Name}RequestHandler.cs
namespace {Namespace}.Handlers;

[Mediator{Lifetime}]  // Singleton or Scoped
// Use 'partial' when applying middleware attributes below
public partial class {Name}RequestHandler : IRequestHandler<{Name}Request, {ResultType}>
{
    private readonly {Dependencies};

    public {Name}RequestHandler({DependencyParameters})
    {
        {DependencyAssignments}
    }

    {MiddlewareAttributes}  // If any attributes used, class MUST be partial
    public async Task<{ResultType}> Handle(
        {Name}Request request,
        IMediatorContext context,
        CancellationToken cancellationToken)
    {
        {Implementation}
    }
}
```

**Middleware Attributes to Apply (requires `partial` class):**
- `[Cache(AbsoluteExpirationSeconds = N)]` - For cacheable queries
- `[OfflineAvailable]` - For offline-capable requests
- `[Resilient("policyName")]` - For resilience with retry/timeout
- `[MainThread]` - For MAUI UI thread execution
- `[TimerRefresh(milliseconds)]` - For auto-refresh streams
- `[Sample(milliseconds)]` - For fixed-window sampling of rapid event firings (last event in window wins)
- `[Throttle(milliseconds)]` - For throttling rapid event firings (first event executes, rest discarded during cooldown)

## Command Handler Template

```csharp
// Contract: Contracts/{Name}Command.cs
namespace {Namespace}.Contracts;

public record {Name}Command({Parameters}) : ICommand;
```

```csharp
// Handler: Handlers/{Name}CommandHandler.cs
namespace {Namespace}.Handlers;

[Mediator{Lifetime}]
// Use 'partial' when applying middleware attributes
public partial class {Name}CommandHandler : ICommandHandler<{Name}Command>
{
    private readonly {Dependencies};

    public {Name}CommandHandler({DependencyParameters})
    {
        {DependencyAssignments}
    }

    {MiddlewareAttributes}  // If any attributes used, class MUST be partial
    public async Task Handle(
        {Name}Command command,
        IMediatorContext context,
        CancellationToken cancellationToken)
    {
        {Implementation}
    }
}
```

## Event Handler Template

```csharp
// Contract: Contracts/{Name}Event.cs
namespace {Namespace}.Contracts;

public record {Name}Event({Parameters}) : IEvent;
```

```csharp
// Handler: Handlers/{Name}EventHandler.cs
namespace {Namespace}.Handlers;

[Mediator{Lifetime}]
// Use 'partial' when applying middleware attributes
public partial class {Name}EventHandler : IEventHandler<{Name}Event>
{
    private readonly ILogger<{Name}EventHandler> _logger;

    public {Name}EventHandler(ILogger<{Name}EventHandler> logger)
    {
        _logger = logger;
    }

    {MiddlewareAttributes}  // If any attributes used, class MUST be partial
    public Task Handle(
        {Name}Event @event,
        IMediatorContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("{EventName} received: {@Event}", nameof({Name}Event), @event);
        {Implementation}
        return Task.CompletedTask;
    }
}
```

## Stream Handler Template

```csharp
// Contract: Contracts/{Name}StreamRequest.cs
namespace {Namespace}.Contracts;

public record {Name}StreamRequest({Parameters}) : IStreamRequest<{ResultType}>;
```

```csharp
// Handler: Handlers/{Name}StreamHandler.cs
namespace {Namespace}.Handlers;

[MediatorSingleton]
// Use 'partial' when applying middleware attributes like [TimerRefresh]
public partial class {Name}StreamHandler : IStreamRequestHandler<{Name}StreamRequest, {ResultType}>
{
    {MiddlewareAttributes}  // If any attributes used (e.g., [TimerRefresh]), class MUST be partial
    public async IAsyncEnumerable<{ResultType}> Handle(
        {Name}StreamRequest request,
        IMediatorContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return {YieldValue};
            await Task.Delay({DelayMs}, cancellationToken);
        }
    }
}
```

## Custom Middleware Template

Use `[MiddlewareOrder(n)]` to control execution order. Lower values run first (outermost in pipeline). Default is 0.

```csharp
// Middleware/{Name}Middleware.cs
namespace {Namespace}.Middleware;

[MiddlewareOrder({Order})]  // Optional: controls execution order (lower = runs first, default 0)
[MediatorSingleton]
public class {Name}Middleware<TRequest, TResult> : IRequestMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    private readonly ILogger<{Name}Middleware<TRequest, TResult>> _logger;

    public {Name}Middleware(ILogger<{Name}Middleware<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Process(
        IMediatorContext context,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        // Before handler execution
        _logger.LogInformation("Processing {RequestType}", typeof(TRequest).Name);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await next();

            // After handler execution
            stopwatch.Stop();
            _logger.LogInformation("{RequestType} completed in {ElapsedMs}ms",
                typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {RequestType}", typeof(TRequest).Name);
            throw;
        }
    }
}
```

## HTTP Contract Template

```csharp
// Contracts/Http/{Name}Request.cs
namespace {Namespace}.Contracts.Http;

using Shiny.Mediator.Http;

[{HttpMethod}("{Route}")]
public class {Name}Request : IRequest<{ResultType}>
{
    // Route parameters (from URL path)
    public {RouteParamType} {RouteParamName} { get; set; }

    // Query parameters
    [Query]
    public string? {QueryParamName} { get; set; }

    // Headers
    [Header("Authorization")]
    public string? AuthToken { get; set; }

    // Body (for POST/PUT/PATCH)
    [Body]
    public {BodyType}? {BodyName} { get; set; }
}
```

## ASP.NET Endpoint Handler Template

```csharp
// Handlers/Endpoints/{Name}EndpointHandler.cs
namespace {Namespace}.Handlers.Endpoints;

[MediatorScoped]
[MediatorHttpGroup("{GroupRoute}"{AuthRequired})]
public class {Name}EndpointHandler : IRequestHandler<{Name}Request, {ResultType}>
{
    private readonly {Dependencies};

    public {Name}EndpointHandler({DependencyParameters})
    {
        {DependencyAssignments}
    }

    [MediatorHttp{Method}("{Route}"{EndpointConfig})]
    public async Task<{ResultType}> Handle(
        {Name}Request request,
        IMediatorContext context,
        CancellationToken cancellationToken)
    {
        {Implementation}
    }
}
```

## ASP.NET SSE Stream Endpoint Template

Stream handlers decorated with `[MediatorHttpGet]` or `[MediatorHttpPost]` are auto-generated as SSE endpoints. Only GET and POST are supported for stream endpoints.

```csharp
// Contract: Contracts/{Name}StreamRequest.cs
namespace {Namespace}.Contracts;

public record {Name}StreamRequest({Parameters}) : IStreamRequest<{ResultType}>;
```

```csharp
// Handler: Handlers/Endpoints/{Name}StreamHandler.cs
namespace {Namespace}.Handlers.Endpoints;

[MediatorScoped]
public class {Name}StreamHandler : IStreamRequestHandler<{Name}StreamRequest, {ResultType}>
{
    [MediatorHttpGet("{Route}")]
    public async IAsyncEnumerable<{ResultType}> Handle(
        {Name}StreamRequest request,
        IMediatorContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return {YieldValue};
            await Task.Delay({DelayMs}, cancellationToken);
        }
    }
}
```

The source generator produces `MapMediatorServerSentEventsGet<TRequest, TResult>("/route")` for these handlers when `MapGeneratedMediatorEndpoints()` is called.

## ASP.NET Manual SSE with EventStream Template

For pushing mediator events as SSE (not from a stream handler):

```csharp
// In Program.cs or endpoint configuration
app.MapGet("/events/{eventType}", ([FromServices] IMediator mediator, CancellationToken ct) =>
    TypedResults.ServerSentEvents(mediator.EventStream<MyEvent>(cancellationToken: ct))
);
```

## HTTP Client SSE Contract Template

For client-side contracts that consume SSE endpoints, add the `IServerSentEventsStream` marker:

```csharp
// Contracts/Http/{Name}StreamRequest.cs
namespace {Namespace}.Contracts.Http;

using Shiny.Mediator.Http;

[Get("{Route}")]
public record {Name}StreamRequest({Parameters}) : IStreamRequest<{ResultType}>, IServerSentEventsStream;
```

The `IServerSentEventsStream` marker tells the generated HTTP handler to parse SSE format (`data:` prefixed lines) instead of raw JSON streaming.

## OpenAPI Client Generation Template

Generate contracts, models, and handlers from an OpenAPI/Swagger spec by adding a `<MediatorHttp>` item in your `.csproj`:

```xml
<!-- In .csproj -->
<ItemGroup>
    <!-- Remote spec: Include is a logical name, Uri is the URL -->
    <MediatorHttp Include="PetStoreApi"
                  Uri="https://petstore.swagger.io/v2/swagger.json"
                  Namespace="{Namespace}.PetStore"
                  ContractPostfix="HttpRequest"
                  GenerateJsonConverters="true"
                  Visible="false" />

    <!-- Local spec: Include is the file path, no Uri needed -->
    <MediatorHttp Include="./specs/openapi.yaml"
                  Namespace="{Namespace}.LocalApi"
                  GenerateModelsOnly="false"
                  UseInternalClasses="false"
                  Visible="false" />
</ItemGroup>
```

`Include` can be a **file path** (for local specs) or a **logical name** when `Uri` is set to a remote URL.

**MediatorHttp metadata options:**

| Metadata | Description | Default |
|----------|-------------|---------|
| `Uri` | URL or local path to OpenAPI JSON/YAML spec | Required |
| `Namespace` | C# namespace for generated types | Required |
| `ContractPrefix` | Prefix for generated contract class names | (none) |
| `ContractPostfix` | Postfix for generated contract class names | (none) |
| `UseInternalClasses` | Generate `internal` classes instead of `public` | `false` |
| `GenerateModelsOnly` | Only generate model classes, skip handlers | `false` |
| `GenerateJsonConverters` | Generate `JsonConverter` for string enums | `false` |

**The source generator produces:**
- Model classes from `components/schemas` with `[JsonPropertyName]` attributes
- Enum types with optional `JsonStringEnumConverter`
- Request contract classes implementing `IRequest<TResult>` with route/query/header/body properties
- Handler classes inheriting `BaseHttpRequestHandler` for each endpoint
- A registration extension method `AddGeneratedOpenApiClient()`

**Registration:**
```csharp
builder.Services.AddShinyMediator(x => x
    .AddMediatorRegistry()
    .AddGeneratedOpenApiClient()
);
```

**Usage (same as any mediator request):**
```csharp
var response = await mediator.Request(new GetPetByIdHttpRequest { PetId = 123 });
var pet = response.Result;
