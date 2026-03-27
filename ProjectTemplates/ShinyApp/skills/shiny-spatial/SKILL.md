---
name: shiny-spatial
description: Build geospatial applications with Shiny.Spatial - a dependency-free, AOT-compatible .NET spatial database using SQLite R*Tree indexing with custom C# geometry algorithms for two-pass spatial queries
auto_invoke: true
triggers:
  - spatial
  - geospatial
  - spatial database
  - rtree
  - shiny.spatial
  - shiny spatial
  - geofence
  - geofencing
  - region change
---

# Shiny.Spatial Skill

You are an expert in Shiny.Spatial, a dependency-free, cross-platform .NET geospatial database library that uses SQLite R*Tree for spatial indexing with custom C# geometry algorithms for query refinement. No SpatiaLite, no NetTopologySuite — only SQLite via `Microsoft.Data.Sqlite`.

## When to Use This Skill

Invoke this skill when the user wants to:
- Create or query a spatial database with geospatial data
- Store and retrieve geometry objects (Points, LineStrings, Polygons, Multi*, GeometryCollection)
- Perform spatial queries: distance, intersection, containment, bounding box
- Use the fluent query builder with property filters and distance ordering
- Work with WKB (Well-Known Binary) serialization
- Use pre-built databases (US/Canadian states, provinces, cities)
- Build location-aware applications on iOS, Android, or any .NET platform
- Set up GPS-driven geofence monitoring with enter/exit events
- Implement an `ISpatialGeofenceDelegate` for region change handling
- Configure `ISpatialGeofenceManager` to start/stop geofence detection

## Library Overview

**Repository**: https://github.com/shinyorg/geospatialdb
**Namespace**: `Shiny.Spatial`
**AOT compatible and trimmable.**

### Target Frameworks

| Package | Framework | Notes |
|---|---|---|
| `Shiny.Spatial` | `net10.0` | AOT compatible and trimmable |
| `Shiny.Spatial.Geofencing` | `net10.0-ios`, `net10.0-android` | iOS/Android GPS geofencing |

### Dependencies

- `Microsoft.Data.Sqlite` — brings `SQLitePCLRaw.bundle_e_sqlite3` with R*Tree enabled
- `Shiny.Locations` — geofencing package only (background GPS)

## Setup

### Install NuGet Package
```bash
dotnet add package Shiny.Spatial
```

## Architecture

### Two-Pass Query Pipeline

1. **R*Tree bounding box filter** (SQL, O(log n)) — eliminates most candidates using the SQLite R*Tree index
2. **C# geometry refinement** — exact Contains/Intersects/WithinDistance checks on survivors

### SQLite Schema

Each spatial table creates a single R*Tree virtual table with auxiliary columns:

```sql
CREATE VIRTUAL TABLE {name}_rtree USING rtree(
    id, min_x, max_x, min_y, max_y,
    +geometry BLOB,              -- WKB-encoded geometry
    +prop_{name} {type}, ...     -- user-defined property columns
);
```

Metadata is tracked in `__spatial_meta` and `__spatial_columns` tables.

## Geometry Types

All geometry classes are immutable and sealed, extending the abstract `Geometry` base class. Namespace: `Shiny.Spatial.Geometry`.

| Type | Description |
|---|---|
| `Coordinate` | Readonly struct with `X`/`Y` (aliased as `Longitude`/`Latitude`) |
| `Envelope` | Readonly struct — bounding box with `MinX`, `MaxX`, `MinY`, `MaxY` |
| `Point` | Single coordinate |
| `LineString` | Ordered sequence of coordinates (minimum 2) |
| `Polygon` | Exterior ring + optional interior rings (holes) |
| `MultiPoint` | Collection of Points |
| `MultiLineString` | Collection of LineStrings |
| `MultiPolygon` | Collection of Polygons |
| `GeometryCollection` | Collection of mixed Geometry types |

### Creating Geometries

```csharp
using Shiny.Spatial.Geometry;

// Point
var point = new Point(-104.99, 39.74);

// LineString (minimum 2 coordinates)
var line = new LineString(new[]
{
    new Coordinate(-104.99, 39.74),
    new Coordinate(-104.82, 38.83)
});

// Polygon (exterior ring, closed — first == last coordinate)
var polygon = new Polygon(new[]
{
    new Coordinate(-109.05, 37.0), new Coordinate(-102.05, 37.0),
    new Coordinate(-102.05, 41.0), new Coordinate(-109.05, 41.0),
    new Coordinate(-109.05, 37.0)
});

// Polygon with holes
var polygonWithHole = new Polygon(
    exteriorRing: new[] { /* outer coords */ },
    interiorRings: new[] { new[] { /* hole coords */ } }
);

// Multi types
var multiPoint = new MultiPoint(new[] { point1, point2 });
var multiLine = new MultiLineString(new[] { line1, line2 });
var multiPoly = new MultiPolygon(new[] { polygon1, polygon2 });
var collection = new GeometryCollection(new Geometry[] { point, line, polygon });
```

## Database API

### SpatialDatabase (IDisposable)

```csharp
using Shiny.Spatial.Database;

var db = new SpatialDatabase("path.db");    // file-backed
var db = new SpatialDatabase(":memory:");   // in-memory

SpatialTable table = db.CreateTable(name, coordinateSystem, properties...);
SpatialTable table = db.GetTable(name);
bool exists       = db.TableExists(name);
db.DropTable(name);
db.Dispose();
```

### Creating Tables with Properties

```csharp
var table = db.CreateTable("cities", CoordinateSystem.Wgs84,
    new PropertyDefinition("name", PropertyType.Text),
    new PropertyDefinition("population", PropertyType.Integer),
    new PropertyDefinition("area", PropertyType.Real));
```

| PropertyType | Description |
|---|---|
| `Text` | String values |
| `Integer` | Long integer values |
| `Real` | Double floating point values |
| `Blob` | Binary data |

### SpatialTable CRUD

| Method | Returns | Description |
|---|---|---|
| `Insert(feature)` | `long` | Insert a feature, returns its ID |
| `BulkInsert(features)` | `void` | Insert many features in a single transaction |
| `Update(feature)` | `void` | Update a feature by ID |
| `Delete(id)` | `bool` | Delete a feature by ID |
| `GetById(id)` | `SpatialFeature?` | Retrieve a single feature |
| `Count()` | `long` | Total feature count |

### SpatialFeature

```csharp
var feature = new SpatialFeature(new Point(-104.99, 39.74))
{
    Properties = { ["name"] = "Denver", ["population"] = 715000L }
};

long id = feature.Id;              // set after Insert
Geometry geom = feature.Geometry;
Dictionary<string, object?> props = feature.Properties;
```

### Spatial Queries

| Method | Description |
|---|---|
| `FindInEnvelope(envelope)` | R*Tree bounding box query |
| `FindIntersecting(geometry)` | Two-pass intersection query |
| `FindContainedBy(geometry)` | Two-pass containment query |
| `FindWithinDistance(center, meters)` | Two-pass distance query |
| `Query()` | Returns a fluent `SpatialQuery` builder |

```csharp
// Distance query
var nearby = table.FindWithinDistance(
    new Coordinate(-104.99, 39.74),
    distanceMeters: 150_000
);

// Shape intersection
var inState = table.FindIntersecting(coloradoPolygon);

// Bounding box
var envelope = new Envelope(-110, -100, 35, 42);
var inBox = table.FindInEnvelope(envelope);
```

### Fluent Query Builder

| Method | Type | Description |
|---|---|---|
| `InEnvelope(envelope)` | Filter | Bounding box filter |
| `Intersecting(geometry)` | Filter | Geometry intersection |
| `ContainedBy(geometry)` | Filter | Geometry containment |
| `WithinDistance(center, meters)` | Filter | Distance radius |
| `WhereProperty(name, op, value)` | Filter | Property comparison (`=`, `!=`, `<`, `<=`, `>`, `>=`, `LIKE`) |
| `OrderByDistance(center)` | Sort | Order by distance from coordinate |
| `Limit(count)` | Paging | Limit result count |
| `Offset(count)` | Paging | Skip first N results |
| `ToList()` | Terminal | Execute and return results |
| `Count()` | Terminal | Execute and return count |
| `FirstOrDefault()` | Terminal | Execute and return first or null |

```csharp
var center = new Coordinate(-104.99, 39.74);

var results = table.Query()
    .WithinDistance(center, 150_000)
    .WhereProperty("population", ">", 200000L)
    .OrderByDistance(center)
    .Limit(10)
    .ToList();

int count = table.Query().InEnvelope(envelope).Count();
var first = table.Query().WithinDistance(center, 1000).FirstOrDefault();
```

## Algorithms

Namespace: `Shiny.Spatial.Algorithms`

| Class | Method | Description |
|---|---|---|
| `DistanceCalculator` | `Haversine(a, b)` | Great-circle distance in meters (WGS84) |
| `DistanceCalculator` | `Euclidean(a, b)` | Cartesian distance |
| `DistanceCalculator` | `DistanceToSegment(p, a, b)` | Perpendicular distance from point to segment |
| `PointInPolygon` | `Contains(polygon, point)` | Ray-casting with hole support |
| `SegmentIntersection` | `Intersects(a1, a2, b1, b2)` | Cross-product segment intersection test |
| `SpatialPredicates` | `Intersects(a, b)` | Dispatch for all geometry type combinations |
| `SpatialPredicates` | `Contains(container, contained)` | Dispatch for all geometry type combinations |
| `EnvelopeExpander` | `ExpandByDistance(env, meters, cs)` | Expand envelope by distance (WGS84 or Cartesian) |

## WKB Serialization

```csharp
using Shiny.Spatial.Serialization;

byte[] wkb = WkbWriter.Write(geometry);
Geometry restored = WkbReader.Read(wkb);
```

Full roundtrip support for all geometry types using the WKB (Well-Known Binary) format.

## Pre-Built Databases

Located in `databases/`:

| Database | Table | Geometry | Records | Properties |
|---|---|---|---|---|
| `us-states.db` | `states` | Polygon | 51 (50 states + DC) | `name`, `abbreviation`, `population` |
| `us-cities.db` | `cities` | Point | 100 (top 100 by pop.) | `name`, `state`, `population` |
| `ca-provinces.db` | `provinces` | Polygon | 13 (all provinces/territories) | `name`, `abbreviation`, `population` |
| `ca-cities.db` | `cities` | Point | 50 (top 50 by pop.) | `name`, `province`, `population` |

All use `CoordinateSystem.Wgs84` (longitude/latitude).

```csharp
using var db = new SpatialDatabase("databases/us-states.db");
var states = db.GetTable("states");

// Find which state Denver is in
var denver = new Point(-104.99, 39.74);
var results = states.FindIntersecting(denver);
// results[0].Properties["name"] == "Colorado"
```

## Coordinate Systems

| System | Enum | Distance Algorithm | Use Case |
|---|---|---|---|
| WGS84 | `CoordinateSystem.Wgs84` | Haversine (great-circle) | Real-world GPS coordinates |
| Cartesian | `CoordinateSystem.Cartesian` | Euclidean | Flat/projected coordinate systems |

## Code Generation Instructions

When generating code with Shiny.Spatial:

### 1. Database Lifecycle
- Always wrap `SpatialDatabase` in a `using` statement or `IDisposable` pattern
- Use `:memory:` for tests, file paths for production
- Call `CreateTable` for new tables, `GetTable` for existing tables

### 2. Coordinate System Selection
- Use `CoordinateSystem.Wgs84` for real-world GPS latitude/longitude data
- Use `CoordinateSystem.Cartesian` for projected or flat coordinate systems
- WGS84 uses Haversine distance (meters), Cartesian uses Euclidean distance

### 3. Property Types
- Use `PropertyType.Text` for strings
- Use `PropertyType.Integer` for long values — always use `L` suffix (e.g., `715000L`)
- Use `PropertyType.Real` for doubles
- Use `PropertyType.Blob` for binary data

### 4. Query Strategy
- For simple spatial queries, use the direct methods (`FindWithinDistance`, `FindIntersecting`, etc.)
- For combined spatial + property filtering, use the fluent `Query()` builder
- Always apply spatial filters first (they use the R*Tree index) before property filters
- Use `Limit()` and `Offset()` for pagination
- Use `OrderByDistance()` for nearest-neighbor results

### 5. Bulk Operations
- Use `BulkInsert()` for inserting multiple features — it wraps in a transaction for performance
- Single `Insert()` is fine for one-off inserts

### 6. Geometry Construction
- Polygons must be closed (first coordinate == last coordinate)
- LineStrings require minimum 2 coordinates
- Use `Coordinate` struct for X/Y pairs (aliases: `Longitude`/`Latitude`)

### 7. Testing Pattern
```csharp
using var db = new SpatialDatabase(":memory:");
var table = db.CreateTable("test", CoordinateSystem.Wgs84,
    new PropertyDefinition("name", PropertyType.Text));

table.Insert(new SpatialFeature(new Point(-104.99, 39.74))
{
    Properties = { ["name"] = "Denver" }
});

var results = table.FindWithinDistance(new Coordinate(-104.99, 39.74), 1000);
results.Count.ShouldBe(1);
results[0].Properties["name"].ShouldBe("Denver");
```

## Geofencing (`Shiny.Spatial.Geofencing`)

A separate NuGet package that adds GPS-driven geofence monitoring on top of `Shiny.Spatial`. Built on [Shiny.Locations](https://shinylib.net) for background GPS on iOS and Android.

The primary use case is monitoring preexisting spatial databases containing city and state/province polygons. There is currently no API to add individual geofences manually — you point the monitor at one or more spatial database tables and it detects region enter/exit automatically.

### Install

```bash
dotnet add package Shiny.Spatial.Geofencing
```

**Platforms**: iOS, Android (registration API is `#if IOS || ANDROID`)

### Setup (DI Registration)

`Add()` requires a file path on disk. For databases bundled as MAUI raw assets (`Resources/Raw`), copy the asset to `AppDataDirectory` first since SQLite cannot open files directly from the app package.

```csharp
// In MauiProgram.cs
builder.Services.AddSpatialGps<MyGeofenceDelegate>(config =>
{
    config.MinimumDistance = Distance.FromMeters(300); // default
    config.MinimumTime = TimeSpan.FromMinutes(1);     // default
    config
        .Add(CopyAssetToAppData("us-states.db"), "states")
        .Add(CopyAssetToAppData("us-cities.db"), "cities");
});

// Helper to copy a MAUI raw asset to a writable location
static string CopyAssetToAppData(string assetFileName)
{
    var destPath = Path.Combine(FileSystem.AppDataDirectory, assetFileName);
    if (!File.Exists(destPath))
    {
        using var source = FileSystem.OpenAppPackageFileAsync(assetFileName).GetAwaiter().GetResult();
        using var dest = File.Create(destPath);
        source.CopyTo(dest);
    }
    return destPath;
}
```

### Key Types

#### `ISpatialGeofenceManager`

The main interface for controlling geofence monitoring. Inject this to start/stop monitoring and query the current region.

```csharp
public interface ISpatialGeofenceManager
{
    bool IsStarted { get; }
    Task<AccessState> RequestAccess();
    Task Start();
    Task Stop();
    Task<IReadOnlyList<SpatialCurrentRegion>> GetCurrent(CancellationToken cancelToken = default);
}
```

| Method | Description |
|---|---|
| `IsStarted` | Whether geofence monitoring is active |
| `RequestAccess()` | Requests GPS permissions |
| `Start()` | Begins background GPS monitoring and region detection |
| `Stop()` | Stops monitoring |
| `GetCurrent()` | Gets the current GPS position and queries all monitored tables to determine which region(s) the device is in |

#### `ISpatialGeofenceDelegate`

Implement this interface to receive geofence enter/exit events. Register it with `AddSpatialGps<T>()`.

```csharp
public interface ISpatialGeofenceDelegate
{
    Task OnRegionChanged(SpatialRegionChange change);
}
```

#### `SpatialRegionChange`

Event data for geofence transitions. Each event represents either entering or exiting a single region.

```csharp
public record SpatialRegionChange(
    string TableName,    // the spatial table that was matched
    SpatialFeature Region, // the region being entered or exited
    bool Entered          // true = entered, false = exited
);
```

When transitioning directly from Region A to Region B, two events fire: an exit from A (`Entered = false`), then an entry into B (`Entered = true`).

#### `SpatialCurrentRegion`

Returned by `ISpatialGeofenceManager.GetCurrent()`.

```csharp
public record SpatialCurrentRegion(string TableName, SpatialFeature? Region);
```

#### `SpatialMonitorConfig`

Configuration for which databases/tables to monitor.

```csharp
public class SpatialMonitorConfig
{
    public List<SpatialMonitorEntry> Entries { get; }
    public Distance? MinimumDistance { get; set; } // default: 300m
    public TimeSpan? MinimumTime { get; set; }    // default: 1 minute
    public SpatialMonitorConfig Add(string databasePath, string tableName);
}
```

- `Add()` takes a file path on disk — for MAUI raw assets, copy to `AppDataDirectory` first (see setup example above)

### Delegate Example

```csharp
public class MyGeofenceDelegate(
    ILogger<MyGeofenceDelegate> logger,
    INotificationManager notifications
) : ISpatialGeofenceDelegate
{
    public async Task OnRegionChanged(SpatialRegionChange change)
    {
        var regionName = change.Region.Properties.GetValueOrDefault("name") ?? "Unknown";
        var action = change.Entered ? "Entered" : "Exited";

        logger.LogInformation("{Action} {Region} in {Table}", action, regionName, change.TableName);
        await notifications.Send("Geofence", $"{action}: {regionName}");
    }
}
```

### Usage in a Page/ViewModel

```csharp
public class MyViewModel(ISpatialGeofenceManager geofences)
{
    public async Task StartMonitoring()
    {
        await geofences.RequestAccess();
        await geofences.Start();
    }

    public async Task StopMonitoring()
    {
        await geofences.Stop();
    }

    public async Task CheckCurrentRegions()
    {
        var regions = await geofences.GetCurrent();
        foreach (var r in regions)
        {
            var name = r.Region?.Properties.GetValueOrDefault("name") ?? "None";
            Console.WriteLine($"{r.TableName}: {name}");
        }
    }
}
```

## Best Practices

1. **Always use `using` for SpatialDatabase** — it manages SQLite connections
2. **Use BulkInsert for batch operations** — transactions improve performance significantly
3. **Apply spatial filters first** — the R*Tree index makes spatial queries O(log n)
4. **Use the fluent query builder for complex queries** — combines spatial + property filters efficiently
5. **Use WGS84 for real-world coordinates** — Haversine distance is accurate for GPS data
6. **Close polygons** — first and last coordinates must match
7. **Use `L` suffix for integer properties** — `PropertyType.Integer` expects `long` values
8. **Prefer `FindWithinDistance` for proximity queries** — more intuitive than manual envelope expansion
9. **Use pre-built databases for testing** — `databases/` folder has ready-to-use US/Canadian geographic data
10. **Library is AOT-compatible** — safe to use in trimmed/NativeAOT applications
