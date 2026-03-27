---
applyTo: "**/*.cs,**/*.razor,**/*.xaml"
---

# Shiny Jobs

Shiny.Jobs provides cross-platform background job scheduling and execution for .NET MAUI applications targeting iOS and Android. Jobs run periodically in the background with support for constraints such as network availability, charging status, and battery level.

## When to Use This Skill

- The user wants to schedule background work that runs periodically
- The user needs to run tasks when the app is not in the foreground
- The user asks about job constraints (network, charging, battery)
- The user wants to register, cancel, or manage background jobs
- The user needs foreground job execution on a timer
- The user asks about `IJobManager`, `IJob`, `JobInfo`, or related types

## Library Overview

| Item       | Value                  |
|------------|------------------------|
| NuGet      | `Shiny.Jobs`           |
| Namespace  | `Shiny.Jobs`           |
| Platforms  | iOS, Android           |

## Setup

Register jobs during service configuration in `MauiProgram.cs`:

```csharp
using Shiny;
using Shiny.Jobs;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Register the job infrastructure and a specific job
        builder.Services.AddJob(
            typeof(MySyncJob),
            identifier: "MySync",
            requiredNetwork: InternetAccess.Any,
            runInForeground: true
        );

        // Or register with a full JobInfo record
        builder.Services.AddJob(new JobInfo(
            Identifier: "MyDataSync",
            JobType: typeof(MySyncJob),
            RunOnForeground: true,
            RequiredInternetAccess: InternetAccess.Any,
            DeviceCharging: false,
            BatteryNotLow: true
        ));

        return builder.Build();
    }
}
```

### iOS Setup

Add the following background task identifiers to your `Info.plist` under `BGTaskSchedulerPermittedIdentifiers`:

- `com.shiny.job`
- `com.shiny.jobpower`
- `com.shiny.jobnet`
- `com.shiny.jobpowernet`

Also enable the `processing` background mode.

### Android Setup

No additional manifest setup is required. Shiny.Jobs uses AndroidX WorkManager under the hood. If you want wake-lock support for `RunTask`, add the `WAKE_LOCK` permission to your `AndroidManifest.xml`.

## Code Generation Instructions

When generating job-related code, follow these conventions:

1. **Implement `IJob` directly** for simple jobs, or **inherit from `Job`** (abstract base class) for jobs that need minimum time tracking between runs.
2. **Always provide a unique `Identifier`** for each `JobInfo` registration.
3. **Use constructor injection** -- job classes are resolved via DI, so inject any services you need through the constructor.
4. **Respect cancellation tokens** -- always pass and honor the `CancellationToken` provided to `Run`.
5. **Use `RunOnForeground = true`** when the job should also execute while the app is in the foreground on a timer (via `JobLifecycleTask`).
6. **Use `InternetAccess` constraints** to prevent jobs from running without the correct network state.
7. **Call `RequestAccess()`** before relying on background jobs to verify the platform supports and has granted background execution.
8. **Observe `JobStarted` and `JobFinished`** for monitoring or UI updates -- these are `IObservable<T>` streams.
9. **Register jobs at startup** using `services.AddJob(...)` so they are treated as system jobs and automatically re-registered on app restart.
10. **Do not call `CancelAll()` lightly** -- it cancels all non-system jobs. System jobs (registered at startup) are preserved.

### Conventions

- Job class names should end with `Job` (e.g., `DataSyncJob`, `CleanupJob`).
- Place job classes in a `Jobs/` folder or namespace within the project.
- Always handle exceptions inside `Run` gracefully; unhandled exceptions are logged but the job is marked as failed.
- Use `JobInfo.Parameters` dictionary for passing simple configuration data to jobs.
- `JobInfo` does NOT have a `LastRunUtc` property. Last run tracking is on the `Job` abstract base class (`LastRunTime` property), not on `JobInfo`.

## Best Practices

- **Check `RequestAccess()` on startup** and guide the user if background jobs are not available (`AccessState.NotSetup`, `AccessState.Disabled`, etc.).
- **Keep jobs short-lived** -- background execution time is limited by the OS, especially on iOS.
- **Use constraints wisely** -- setting `DeviceCharging = true` or `BatteryNotLow = true` means the job may not run for extended periods if conditions are not met.
- **Prefer `InternetAccess.Any` over `InternetAccess.Unmetered`** unless the job transfers large amounts of data.
- **Use the `Job` base class** when you need built-in minimum time spacing between runs via `MinimumTime`.
- **Monitor jobs** by subscribing to `IJobManager.JobStarted` and `IJobManager.JobFinished` for logging or UI feedback.
- **Use `RunJobAsTask`** extension method to bridge a registered job into a `Task` for event-driven scenarios.

## Reference Files

- [API Reference](reference/api-reference.md)

# Shiny.Jobs API Reference

## Installation

```xml
<PackageReference Include="Shiny.Jobs" />
```

Project dependencies (pulled in automatically):

- `Shiny.Core`
- `Shiny.Support.DeviceMonitoring`
- `Shiny.Support.Repositories`

Android additionally uses:

- `Xamarin.AndroidX.Work.Runtime`

## Namespace

```csharp
using Shiny.Jobs;
```

Registration extension methods are in the `Shiny` namespace:

```csharp
using Shiny;
```


## Records

### JobInfo

The configuration record that describes a registered job. Implements `IRepositoryEntity` for persistence.

```csharp
namespace Shiny.Jobs;

public record JobInfo(
    /// <summary>
    /// Unique identifier for the job
    /// </summary>
    string Identifier,

    /// <summary>
    /// The Type of the IJob implementation class
    /// </summary>
    Type JobType,

    /// <summary>
    /// If true, the job will also run on a foreground timer while the app is active
    /// </summary>
    bool RunOnForeground = false,

    /// <summary>
    /// Optional key-value parameters passed to the job at runtime
    /// </summary>
    Dictionary<string, string>? Parameters = null,

    /// <summary>
    /// The required internet access level for the job to run
    /// </summary>
    InternetAccess RequiredInternetAccess = InternetAccess.None,

    /// <summary>
    /// If true, the job only runs when the device is charging
    /// </summary>
    bool DeviceCharging = false,

    /// <summary>
    /// If true, the job only runs when the battery is not low (above 20%)
    /// </summary>
    bool BatteryNotLow = false,

    /// <summary>
    /// If true, this is a system-managed job registered at startup. System jobs are not removed by CancelAll().
    /// </summary>
    bool IsSystemJob = false
) : IRepositoryEntity;
```

### JobRunResult

The result of a job execution.

```csharp
namespace Shiny.Jobs;

public record JobRunResult(
    /// <summary>
    /// The job info that was run, null if the job was not found
    /// </summary>
    JobInfo? Job,

    /// <summary>
    /// The exception if the job failed, null on success
    /// </summary>
    Exception? Exception
)
{
    /// <summary>
    /// True if the job completed without an exception
    /// </summary>
    public bool Success => this.Exception == null;
}
```

---

## Abstract Base Classes

### Job

An abstract base class for `IJob` that provides minimum time spacing between runs and persistent state tracking. Extends `NotifyPropertyChanged`.

```csharp
namespace Shiny.Jobs;

public abstract class Job : NotifyPropertyChanged, IJob
{
    protected ILogger Logger { get; }
    protected Job(ILogger logger);

    /// <summary>
    /// Implement this to provide your job logic
    /// </summary>
    protected abstract Task Run(CancellationToken cancelToken);

    /// <summary>
    /// The JobInfo associated with the current run
    /// </summary>
    protected JobInfo JobInfo { get; }

    /// <summary>
    /// Last runtime of this job. Null if never run before.
    /// This value persists between runs. It is not recommended to set this manually.
    /// </summary>
    public DateTimeOffset? LastRunTime { get; set; }

    /// <summary>
    /// Sets a minimum time between this job firing.
    /// CAREFUL: jobs tend to NOT run when the user's phone is not in use.
    /// This value persists between runs.
    /// </summary>
    public TimeSpan? MinimumTime { get; set; }
}
```

**Behavior:** When `MinimumTime` is set, the base class checks whether enough time has elapsed since `LastRunTime` before invoking `Run(CancellationToken)`. If the minimum interval has not elapsed, the job is skipped silently.

---

## Enums

### InternetAccess

Specifies the network requirement for a job.

```csharp
namespace Shiny.Jobs;

public enum InternetAccess
{
    /// <summary>No network requirement</summary>
    None = 0,

    /// <summary>Any internet connection (WiFi, cellular, etc.)</summary>
    Any = 1,

    /// <summary>Unmetered connection only (typically WiFi)</summary>
    Unmetered = 2
}
```

### JobState

Internal enum used for logging job lifecycle events.

```csharp
namespace Shiny.Jobs;

public enum JobState
{
    Start,
    Finish,
    Error
}
```

### AccessState

Defined in `Shiny` namespace (from Shiny.Core). Returned by `IJobManager.RequestAccess()`.

```csharp
namespace Shiny;

public enum AccessState
{
    /// <summary>Permission has not been checked or is in an unknown state</summary>
    Unknown,

    /// <summary>API is not supported on the current platform or device</summary>
    NotSupported,

    /// <summary>Platform manifest or plist configuration is missing</summary>
    NotSetup,

    /// <summary>The service has been disabled by the user</summary>
    Disabled,

    /// <summary>Permission granted in a limited fashion</summary>
    Restricted,

    /// <summary>The user denied permission</summary>
    Denied,

    /// <summary>All necessary permissions are granted</summary>
    Available
}
```

### NetworkAccess

```csharp
namespace Shiny.Jobs;

public enum NetworkAccess
{
    None,
    Unknown,
    Bluetooth,
    Ethernet,
    WiFi,
    Cellular
}
```

### NetworkReach

```csharp
namespace Shiny.Jobs;

public enum NetworkReach
{
    Unknown,
    None,
    Local,
    ConstrainedInternet,
    Internet
}
```

### PowerState

```csharp
namespace Shiny.Jobs;

public enum PowerState
{
    /// <summary>Power state hasn't been checked yet or is in an unknown state</summary>
    Unknown,

    /// <summary>Device is plugged in and charging</summary>
    Charging,

    /// <summary>Device is fully charged and plugged in</summary>
    Charged,

    /// <summary>No battery has been detected so device is plugged in</summary>
    NoBattery,

    /// <summary>Device is running on battery</summary>
    Discharging
}
```

---

## Extension Methods

### JobExtensions (on IJobManager)

```csharp
namespace Shiny.Jobs;

public static class JobExtensions
{
    /// <summary>
    /// Runs a registered job as a Task using RunTask internally.
    /// You don't need to use this if you are using foreground jobs (i.e., services.UseJobForegroundService).
    /// </summary>
    /// <param name="jobManager">The job manager instance</param>
    /// <param name="jobIdentifier">The unique identifier of the registered job</param>
    /// <returns>A task that completes when the job finishes</returns>
    public static Task RunJobAsTask(this IJobManager jobManager, string jobIdentifier);
}
```

### ServiceCollectionExtensions (on IServiceCollection)

Defined in the `Shiny` namespace. Available on platforms only.

```csharp
namespace Shiny;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register a job on the job manager using a full JobInfo record
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="jobInfo">The JobInfo describing the job</param>
    public static IServiceCollection AddJob(this IServiceCollection services, JobInfo jobInfo);

    /// <summary>
    /// Register a job on the job manager by type with optional configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="jobType">The Type of the IJob implementation</param>
    /// <param name="identifier">Optional unique identifier (defaults to jobType.FullName)</param>
    /// <param name="requiredNetwork">Network access constraint (default: None)</param>
    /// <param name="runInForeground">If true, also runs on foreground timer (default: false)</param>
    /// <param name="parameters">Optional key-value parameters</param>
    public static IServiceCollection AddJob(
        this IServiceCollection services,
        Type jobType,
        string? identifier = null,
        InternetAccess requiredNetwork = InternetAccess.None,
        bool runInForeground = false,
        params (string Key, string Value)[] parameters
    );

    /// <summary>
    /// Registers the job infrastructure (IJobManager, JobLifecycleTask, connectivity, battery).
    /// Called automatically by AddJob but can be called directly if you only want the infrastructure
    /// without registering a specific job at startup.
    /// </summary>
    public static IServiceCollection AddJobs(this IServiceCollection services);
}
```

---

## Usage Examples

### Basic Job Implementation

```csharp
using Shiny.Jobs;

public class DataSyncJob : IJob
{
    readonly IMyApiService api;
    readonly IMyLocalDb db;

    public DataSyncJob(IMyApiService api, IMyLocalDb db)
    {
        this.api = api;
        this.db = db;
    }

    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        var pending = await this.db.GetPendingUploads();
        foreach (var item in pending)
        {
            cancelToken.ThrowIfCancellationRequested();
            await this.api.Upload(item);
            await this.db.MarkUploaded(item.Id);
        }
    }
}
```

### Job with Minimum Time Spacing (Using Job Base Class)

```csharp
using Microsoft.Extensions.Logging;
using Shiny.Jobs;

public class PeriodicCleanupJob : Job
{
    readonly IMyLocalDb db;

    public PeriodicCleanupJob(ILogger<PeriodicCleanupJob> logger, IMyLocalDb db)
        : base(logger)
    {
        this.db = db;
        this.MinimumTime = TimeSpan.FromHours(6);
    }

    protected override async Task Run(CancellationToken cancelToken)
    {
        await this.db.PurgeOldRecords(cancelToken);
    }
}
```

### Registering Jobs at Startup

```csharp
// In MauiProgram.cs
builder.Services.AddJob(
    typeof(DataSyncJob),
    identifier: "DataSync",
    requiredNetwork: InternetAccess.Any,
    runInForeground: true
);

builder.Services.AddJob(new JobInfo(
    Identifier: "PeriodicCleanup",
    JobType: typeof(PeriodicCleanupJob),
    RunOnForeground: false,
    RequiredInternetAccess: InternetAccess.None,
    BatteryNotLow: true
));
```

### Runtime Job Management

```csharp
public class JobDashboardViewModel
{
    readonly IJobManager jobManager;

    public JobDashboardViewModel(IJobManager jobManager)
    {
        this.jobManager = jobManager;
    }

    public async Task Initialize()
    {
        // Check access
        var access = await this.jobManager.RequestAccess();
        if (access != AccessState.Available)
        {
            // Handle: NotSupported, NotSetup, Disabled, Denied, etc.
            return;
        }

        // List all registered jobs
        var jobs = this.jobManager.GetJobs();

        // Get a specific job
        var syncJob = this.jobManager.GetJob("DataSync");

        // Run a specific job on demand
        var result = await this.jobManager.Run("DataSync");
        if (!result.Success)
        {
            // Handle result.Exception
        }

        // Run all jobs
        var results = await this.jobManager.RunAll(runSequentially: true);

        // Cancel a specific job
        this.jobManager.Cancel("DataSync");

        // Cancel all non-system jobs
        this.jobManager.CancelAll();
    }
}
```

### Observing Job Events

```csharp
// Subscribe to job lifecycle events
jobManager.JobStarted.Subscribe(jobInfo =>
{
    Console.WriteLine($"Job '{jobInfo.Identifier}' started");
});

jobManager.JobFinished.Subscribe(result =>
{
    if (result.Success)
        Console.WriteLine($"Job '{result.Job?.Identifier}' completed successfully");
    else
        Console.WriteLine($"Job '{result.Job?.Identifier}' failed: {result.Exception?.Message}");
});
```

### Running a Job as an Adhoc Task

```csharp
// Fire a one-time task (uses background task APIs on each platform)
jobManager.RunTask("QuickUpload", async cancelToken =>
{
    await myService.UploadPendingData(cancelToken);
});

// Bridge a registered job into a Task (useful for event-driven scenarios)
await jobManager.RunJobAsTask("DataSync");
```

### Using Job Parameters

```csharp
// Register with parameters
builder.Services.AddJob(
    typeof(RegionSyncJob),
    identifier: "RegionSync",
    requiredNetwork: InternetAccess.Any,
    parameters: ("Region", "US"), ("BatchSize", "100")
);

// Access parameters inside the job
public class RegionSyncJob : IJob
{
    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        var region = jobInfo.Parameters?["Region"] ?? "Default";
        var batchSize = int.Parse(jobInfo.Parameters?["BatchSize"] ?? "50");
        // Use region and batchSize...
    }
}
```

### Registering a Job at Runtime

```csharp
// Register a new job dynamically (not at startup)
jobManager.Register(new JobInfo(
    Identifier: "DynamicExport",
    JobType: typeof(ExportJob),
    RunOnForeground: false,
    RequiredInternetAccess: InternetAccess.Unmetered,
    DeviceCharging: true,
    BatteryNotLow: true
));
```

---

## Troubleshooting

### Jobs Not Running on iOS

- Ensure all four BGTask identifiers are registered in `Info.plist` under `BGTaskSchedulerPermittedIdentifiers`:
  - `com.shiny.job`
  - `com.shiny.jobpower`
  - `com.shiny.jobnet`
  - `com.shiny.jobpowernet`
- Ensure the `processing` background mode is enabled in `Info.plist`.
- `RequestAccess()` returns `AccessState.NotSetup` when configuration is missing.
- Jobs do not run on the iOS Simulator.

### Jobs Not Running on Android

- AndroidX WorkManager has a minimum periodic interval of **15 minutes**.
- Ensure the device is not in battery optimization / Doze mode during testing.
- Add `WAKE_LOCK` permission if you need `RunTask` to use wake locks.

### `RequestAccess()` Returns Unexpected State

| Return Value            | Meaning                                                      |
|-------------------------|--------------------------------------------------------------|
| `AccessState.Available` | Everything is configured correctly                           |
| `AccessState.NotSetup`  | iOS plist configuration is missing or registration failed    |
| `AccessState.NotSupported` | Platform or device does not support background processing |
| `AccessState.Disabled`  | User has disabled background processing for the app          |
| `AccessState.Denied`    | User denied the required permission                          |

### `CancelAll()` Does Not Remove My Startup Jobs

`CancelAll()` only removes non-system jobs. Jobs registered at startup via `services.AddJob(...)` are marked as `IsSystemJob = true` and are preserved. To remove system jobs, cancel them individually with `Cancel(identifier)`.

### Job Constructor Injection Not Working

Job instances are resolved via `ActivatorUtilities.GetServiceOrCreateInstance`. Make sure the services your job depends on are registered in the DI container before the job runs.

### Foreground Jobs Not Firing

- Ensure `RunOnForeground = true` is set on the `JobInfo`.
- Foreground jobs are managed by `JobLifecycleTask` with a default interval of **30 seconds** (configurable between 15 seconds and 5 minutes).
- Foreground jobs respect constraints: `RequiredInternetAccess`, `DeviceCharging`, and `BatteryNotLow` are all checked before each foreground run.
