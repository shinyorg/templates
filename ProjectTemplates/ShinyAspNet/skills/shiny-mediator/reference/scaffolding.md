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
