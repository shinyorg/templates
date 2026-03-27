---
name: shiny-aspire-orleans
description: Generate code using Shiny Aspire integrations — Orleans ADO.NET hosting and Gluetun VPN container routing
auto_invoke: true
triggers:
  - aspire orleans
  - orleans aspire
  - WithDatabaseSetup
  - UseAdoNet
  - UseAdoNetClient
  - orleans database setup
  - orleans schema
  - orleans clustering
  - orleans grain storage
  - orleans reminders
  - OrleansFeature
  - Shiny.Aspire.Orleans
  - aspire orleans hosting
  - aspire orleans server
  - aspire orleans client
  - orleans adonet
  - orleans ado.net
  - gluetun
  - gluetun vpn
  - AddGluetun
  - WithRoutedContainer
  - WithVpnProvider
  - WithWireGuard
  - WithOpenVpn
  - vpn container
  - aspire vpn
  - Shiny.Aspire.Hosting.Gluetun
  - GluetunResource
  - network_mode
  - vpn routing
---

# Shiny Aspire Skill

You are an expert in Shiny's .NET Aspire integrations:

1. **Shiny.Aspire.Orleans** — Zero-friction integration between .NET Aspire and Microsoft Orleans for ADO.NET storage backends. Automatically provisions Orleans database schemas and wires up clustering, grain persistence, and reminders from Aspire configuration.
2. **Shiny.Aspire.Hosting.Gluetun** — Aspire hosting integration for Gluetun VPN containers. Models Gluetun as a first-class Aspire resource and lets other containers route their traffic through the VPN tunnel.

## When to Use This Skill

Invoke this skill when the user wants to:
- Set up Orleans with .NET Aspire using ADO.NET storage (PostgreSQL, SQL Server, or MySQL)
- Automatically create Orleans database schemas from Aspire
- Configure an Orleans silo with ADO.NET providers from Aspire-injected config
- Configure an Orleans client with ADO.NET clustering from Aspire-injected config
- Use `WithDatabaseSetup` to auto-provision Orleans tables
- Use `silo.UseAdoNet()` to configure a silo inside `UseOrleans`
- Use `client.UseAdoNetClient()` to configure a client inside `UseOrleansClient`
- Select which Orleans features to provision (clustering, persistence, reminders)
- Set up multiple named grain storage providers
- Add a Gluetun VPN container to an Aspire app
- Route container traffic through a VPN tunnel
- Configure VPN providers, WireGuard, or OpenVPN in Aspire
- Use `AddGluetun`, `WithRoutedContainer`, `WithVpnProvider`, `WithWireGuard`, or `WithOpenVpn`
- Set up Docker Compose publishing with VPN network mode and port transfer

## Library Overview

- **Repository**: https://github.com/shinyorg/aspire
- **Target**: `net10.0`
- **Aspire**: 13.1+
- **Orleans**: 10.0+ (Orleans packages only)

### Packages

| Package | NuGet | Usage |
|---|---|---|
| `Shiny.Aspire.Orleans.Hosting` | Install in Aspire AppHost | Auto-runs Orleans schema scripts when the database becomes ready |
| `Shiny.Aspire.Orleans.Server` | Install in Orleans silo | Registers ADO.NET provider builders for clustering, grain storage, and reminders |
| `Shiny.Aspire.Orleans.Client` | Install in Orleans client | Registers ADO.NET provider builder for clustering |
| `Shiny.Aspire.Hosting.Gluetun` | Install in Aspire AppHost | Adds a Gluetun VPN container and routes other containers through it |

### Supported Databases

| Database | ADO.NET Invariant | ProviderType Value |
|---|---|---|
| PostgreSQL | `Npgsql` | `PostgresDatabase` |
| SQL Server | `Microsoft.Data.SqlClient` | `SqlServerDatabase` |
| MySQL | `MySql.Data.MySqlClient` | `MySqlDatabase` |

## Architecture: Provider Registration

The Server and Client packages register Orleans provider builders via `[assembly: RegisterProvider]` attributes. When Orleans calls `ApplyConfiguration` during `UseOrleans()`, it reads the Aspire-injected configuration (e.g. `Orleans:Clustering:ProviderType = "PostgresDatabase"`) and resolves the matching provider builder automatically. The provider builder maps the database type to the correct ADO.NET invariant and resolves the connection string from the `ServiceKey`.

This means:
- **No manual configuration** — providers are resolved automatically from Aspire config.
- **Extension methods** — `UseAdoNet()` on `ISiloBuilder` and `UseAdoNetClient()` on `IClientBuilder` are identity methods for discoverability. The actual wiring happens through the registered provider builders.
- **Composable** — because configuration happens inside `UseOrleans(silo => { ... })` or `UseOrleansClient(client => { ... })`, users can add other features alongside.

## Setup

### 1. Aspire AppHost

Install `Shiny.Aspire.Orleans.Hosting` in the AppHost project.

```csharp
using Shiny.Aspire.Orleans.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("pg")
    .WithPgAdmin()
    .AddDatabase("orleans-db");

var orleans = builder.AddOrleans("cluster")
    .WithClustering(db)
    .WithGrainStorage("Default", db)
    .WithReminders(db)
    .WithDatabaseSetup(db); // creates all Orleans tables automatically

builder.AddProject<Projects.MySilo>("silo")
    .WithReference(orleans)
    .WaitFor(db);

builder.AddProject<Projects.MyApi>("api")
    .WithReference(orleans.AsClient())
    .WaitFor(db);

builder.Build().Run();
```

### 2. Orleans Silo

Install `Shiny.Aspire.Orleans.Server` in the silo project. Call `silo.UseAdoNet()` inside `UseOrleans`.

```csharp
using Shiny.Aspire.Orleans.Server;

var builder = WebApplication.CreateBuilder(args);

builder.UseOrleans(silo =>
{
    silo.UseAdoNet();
});

var app = builder.Build();
app.Run();
```

### 3. Orleans Client

Install `Shiny.Aspire.Orleans.Client` in the client project (e.g. an API gateway). Call `client.UseAdoNetClient()` inside `UseOrleansClient`.

```csharp
using Shiny.Aspire.Orleans.Client;

var builder = WebApplication.CreateBuilder(args);

builder.UseOrleansClient(client =>
{
    client.UseAdoNetClient();
});

var app = builder.Build();

app.MapGet("/counter/{name}", async (string name, IClusterClient client) =>
{
    var grain = client.GetGrain<ICounterGrain>(name);
    var count = await grain.GetCount();
    return Results.Ok(new { name, count });
});

app.Run();
```

## API Reference

### Hosting Package — `Shiny.Aspire.Orleans.Hosting`

#### WithDatabaseSetup

```csharp
public static OrleansService WithDatabaseSetup(
    this OrleansService orleans,
    IResourceBuilder<IResourceWithConnectionString> database,
    OrleansFeature features = OrleansFeature.All
)
```

Subscribes to Aspire's `ResourceReadyEvent` for the database resource. When the database is up and accepting connections, it executes embedded Orleans SQL schema scripts. The database type (PostgreSQL, SQL Server, MySQL) is auto-detected from the Aspire resource.

**Parameters:**
- `orleans` — The Orleans service from `builder.AddOrleans()`
- `database` — An Aspire database resource (e.g. from `AddPostgres`, `AddSqlServer`, `AddMySql`)
- `features` — Which Orleans features to provision (default: `OrleansFeature.All`)

#### OrleansFeature (Flags Enum)

```csharp
[Flags]
public enum OrleansFeature
{
    Clustering = 1,   // Membership tables for silo discovery
    Persistence = 2,  // Grain storage tables
    Reminders = 4,    // Reminder tables
    All = Clustering | Persistence | Reminders
}
```

Use to limit which schemas are provisioned:

```csharp
// Only clustering and persistence (no reminders)
orleans.WithDatabaseSetup(db, OrleansFeature.Clustering | OrleansFeature.Persistence);

// Only clustering
orleans.WithDatabaseSetup(db, OrleansFeature.Clustering);
```

#### DatabaseType (Enum)

```csharp
public enum DatabaseType
{
    SqlServer,
    PostgreSQL,
    MySql
}
```

Auto-detected from the Aspire resource — you do not need to specify this directly.

### Server Package — `Shiny.Aspire.Orleans.Server`

#### UseAdoNet (ISiloBuilder extension)

```csharp
public static ISiloBuilder UseAdoNet(this ISiloBuilder siloBuilder)
```

Marker extension for discoverability. The actual provider registration happens automatically via `[assembly: RegisterProvider]` attributes when the package is referenced. Providers are registered for all three database types across Clustering, GrainStorage, and Reminders.

Call inside `UseOrleans`:

```csharp
builder.UseOrleans(silo =>
{
    silo.UseAdoNet();
    // compose with other silo features here
});
```

### Client Package — `Shiny.Aspire.Orleans.Client`

#### UseAdoNetClient (IClientBuilder extension)

```csharp
public static IClientBuilder UseAdoNetClient(this IClientBuilder clientBuilder)
```

Marker extension for discoverability. Registers ADO.NET clustering provider builders for both Silo and Client targets. Clients do not need grain storage or reminders.

Call inside `UseOrleansClient`:

```csharp
builder.UseOrleansClient(client =>
{
    client.UseAdoNetClient();
});
```

## Configuration Flow

Aspire injects the following configuration when you use `.WithReference(orleans)`:

```
Orleans:Clustering:ProviderType = "PostgresDatabase"
Orleans:Clustering:ServiceKey   = "orleans-db"
Orleans:GrainStorage:Default:ProviderType = "PostgresDatabase"
Orleans:GrainStorage:Default:ServiceKey   = "orleans-db"
Orleans:Reminders:ProviderType  = "PostgresDatabase"
Orleans:Reminders:ServiceKey    = "orleans-db"
ConnectionStrings:orleans-db    = "Host=...;Database=..."
```

Orleans' `ApplyConfiguration` reads these sections and delegates to the registered provider builders, which configure the ADO.NET providers with the correct connection strings and invariants.

## Schema Provisioning Order

`WithDatabaseSetup` runs embedded SQL scripts in order:

1. **Main** — creates the `OrleansQuery` table (query registry)
2. **Clustering** — creates `OrleansMembershipVersionTable`, `OrleansMembershipTable`, and stored procedures
3. **Persistence** — creates `OrleansStorage` table and stored procedures
4. **Reminders** — creates `OrleansRemindersTable` and stored procedures

## Switching Databases

Swap the Aspire resource builder — everything else stays the same:

```csharp
// PostgreSQL
var db = builder.AddPostgres("pg").AddDatabase("orleans-db");

// SQL Server
var db = builder.AddSqlServer("sql").AddDatabase("orleans-db");

// MySQL
var db = builder.AddMySql("mysql").AddDatabase("orleans-db");
```

## Multiple Grain Storage Providers

```csharp
// AppHost
var orleans = builder.AddOrleans("cluster")
    .WithClustering(db)
    .WithGrainStorage("Default", db)
    .WithGrainStorage("Archive", archiveDb)
    .WithDatabaseSetup(db);

// Grain
public class MyGrain(
    [PersistentState("state", "Default")] IPersistentState<MyState> state,
    [PersistentState("archive", "Archive")] IPersistentState<ArchiveState> archive
) : Grain, IMyGrain { }
```

Each named provider reads from `Orleans:GrainStorage:{Name}:ProviderType` and `Orleans:GrainStorage:{Name}:ServiceKey`.

## Code Generation Best Practices

1. **Always use `WithDatabaseSetup`** in the AppHost to auto-provision schemas — never require manual SQL scripts.
2. **Always call `WaitFor(db)`** on projects that reference Orleans, so the database is ready before the silo starts.
3. **Use `.AsClient()`** when wiring a client project — this provides only clustering config, not full silo config.
4. **Use `UseOrleans` and `UseOrleansClient`** — call `silo.UseAdoNet()` in silo projects inside `UseOrleans` and `client.UseAdoNetClient()` in client projects inside `UseOrleansClient`.
5. **Feature flags are optional** — only use `OrleansFeature` flags if the user explicitly wants to skip certain schemas.
6. **Don't hardcode connection strings** — Aspire injects them automatically via configuration.
7. **Don't manually configure ADO.NET invariants** — the packages auto-detect the correct invariant from the provider type.
8. **Use named grain storage** for multiple persistence stores — each name maps to a separate configuration section.

---

# Gluetun VPN — `Shiny.Aspire.Hosting.Gluetun`

## Setup

Install `Shiny.Aspire.Hosting.Gluetun` in the Aspire AppHost project.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var vpn = builder.AddGluetun("vpn")
    .WithVpnProvider("mullvad")
    .WithWireGuard(builder.AddParameter("wireguard-key", secret: true))
    .WithServerCountries("US", "Canada");

var scraper = builder.AddContainer("scraper", "my-scraper")
    .WithHttpEndpoint(targetPort: 8080);

vpn.WithRoutedContainer(scraper);

builder.Build().Run();
```

## API Reference

### AddGluetun

```csharp
public static IResourceBuilder<GluetunResource> AddGluetun(
    this IDistributedApplicationBuilder builder,
    string name,
    int? httpProxyPort = null,
    int? shadowsocksPort = null)
```

Creates a Gluetun container resource with:
- Image: `qmcgaw/gluetun:latest` from `docker.io`
- `--cap-add NET_ADMIN` runtime arg
- `--device /dev/net/tun` runtime arg
- Docker Compose publish callback that sets `cap_add`, `devices`, and transfers ports from routed containers

Optional port parameters expose Gluetun's built-in HTTP proxy (target port 8888) and Shadowsocks proxy (target port 8388).

### WithVpnProvider

```csharp
vpn.WithVpnProvider("mullvad");
```

Sets the `VPN_SERVICE_PROVIDER` environment variable. Required for all Gluetun setups.

### WithOpenVpn

```csharp
// String credentials
vpn.WithOpenVpn("username", "password");

// Aspire parameter resources (recommended for secrets)
vpn.WithOpenVpn(
    builder.AddParameter("openvpn-user"),
    builder.AddParameter("openvpn-pass", secret: true));
```

Sets `VPN_TYPE=openvpn`, `OPENVPN_USER`, and `OPENVPN_PASSWORD`.

### WithWireGuard

```csharp
// String key
vpn.WithWireGuard("my-private-key");

// Aspire parameter resource (recommended for secrets)
vpn.WithWireGuard(builder.AddParameter("wireguard-key", secret: true));
```

Sets `VPN_TYPE=wireguard` and `WIREGUARD_PRIVATE_KEY`.

### WithServerCountries / WithServerCities

```csharp
vpn.WithServerCountries("US", "Canada", "Germany");
vpn.WithServerCities("New York", "Toronto");
```

Values are comma-joined and set as `SERVER_COUNTRIES` / `SERVER_CITIES` environment variables.

### WithHttpProxy / WithShadowsocks

```csharp
vpn.WithHttpProxy();           // HTTPPROXY=on
vpn.WithHttpProxy(false);      // HTTPPROXY=off
vpn.WithShadowsocks();         // SHADOWSOCKS=on
vpn.WithShadowsocks(false);    // SHADOWSOCKS=off
```

### WithFirewallOutboundSubnets

```csharp
vpn.WithFirewallOutboundSubnets("10.0.0.0/8", "192.168.0.0/16");
```

Sets `FIREWALL_OUTBOUND_SUBNETS` (comma-joined). Useful for allowing traffic to local network resources outside the VPN tunnel.

### WithTimezone

```csharp
vpn.WithTimezone("America/New_York");
```

Sets the `TZ` environment variable.

### WithGluetunEnvironment

```csharp
// String value
vpn.WithGluetunEnvironment("DNS_ADDRESS", "1.1.1.1");

// Aspire parameter resource
vpn.WithGluetunEnvironment("UPDATER_PERIOD", builder.AddParameter("updater-period"));
```

Generic passthrough for any Gluetun environment variable not covered by the typed methods.

### WithRoutedContainer

```csharp
vpn.WithRoutedContainer(scraper);
vpn.WithRoutedContainer(downloader);
```

Routes a container's traffic through the Gluetun VPN tunnel. Each call:
1. Adds a `GluetunRoutedResourceAnnotation` to the Gluetun resource
2. Sets `--network container:<vpn-name>` runtime args on the routed container
3. On Docker Compose publish, sets `network_mode: "service:<vpn-name>"` and transfers port mappings to the Gluetun service

You can route multiple containers through the same VPN.

## Docker Compose Output

When published as Docker Compose, routed containers automatically get:

```yaml
services:
  vpn:
    image: qmcgaw/gluetun:latest
    cap_add:
      - NET_ADMIN
    devices:
      - /dev/net/tun
    environment:
      - VPN_SERVICE_PROVIDER=mullvad
      - VPN_TYPE=wireguard
      - WIREGUARD_PRIVATE_KEY=${wireguard-key}
      - SERVER_COUNTRIES=US,Canada
    ports:
      - "8080:8080"    # forwarded from scraper
  scraper:
    image: my-scraper
    network_mode: "service:vpn"
    # ports moved to vpn service
```

## Types

### GluetunResource

```csharp
namespace Aspire.Hosting.ApplicationModel;
public class GluetunResource(string name) : ContainerResource(name);
```

### GluetunRoutedResourceAnnotation

```csharp
namespace Aspire.Hosting.ApplicationModel;
public sealed record GluetunRoutedResourceAnnotation(
    GluetunResource GluetunResource,
    ContainerResource RoutedResource) : IResourceAnnotation;
```

Stored on the Gluetun resource. References each container that routes through it.

## Gluetun Code Generation Best Practices

1. **Always use `WithVpnProvider`** — it is required for Gluetun to connect to any VPN service.
2. **Use `ParameterResource` for secrets** — never hardcode private keys or passwords in the AppHost. Use `builder.AddParameter("key", secret: true)`.
3. **Call `WithRoutedContainer` on the VPN builder** — not on the container. The method is on `IResourceBuilder<GluetunResource>`.
4. **Ports transfer automatically** — when a container is routed through Gluetun, its endpoints are served on the Gluetun container in Docker Compose. Do not manually duplicate port mappings.
5. **Use `WithGluetunEnvironment` for provider-specific settings** — the typed methods cover common settings, but many providers have additional options documented in the Gluetun wiki.
6. **Use `WithFirewallOutboundSubnets` for local network access** — if routed containers need to reach local services (databases, APIs) outside the VPN, allow those subnets explicitly.
7. **Multiple containers can share one VPN** — call `WithRoutedContainer` multiple times on the same Gluetun resource.
