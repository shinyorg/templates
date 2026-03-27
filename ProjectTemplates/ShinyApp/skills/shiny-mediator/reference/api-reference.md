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
