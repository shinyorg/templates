# Shiny Firebase Push Notifications Skill

## Triggers
- firebase
- FCM
- firebase cloud messaging
- push notifications firebase
- AddPushFirebaseMessaging
- FirebaseConfiguration
- GoogleService-Info.plist
- google-services.json
- firebase push
- firebase messaging

## Overview

Shiny.Push.FirebaseMessaging provides Firebase Cloud Messaging (FCM) push notification support for .NET MAUI applications on iOS and Android. It wraps the native Firebase iOS SDK (via Slim Bindings) and Android FCM through the Shiny Push infrastructure.

## NuGet Package

```
Shiny.Push.FirebaseMessaging
```

## Setup

### MauiProgram.cs Configuration

```csharp
using Shiny;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShiny(); // Required

        // Option 1: Use embedded configuration (GoogleService-Info.plist / google-services.json)
        builder.Services.AddPushFirebaseMessaging();

        // Option 2: Use embedded configuration explicitly
        builder.Services.AddPushFirebaseMessaging(FirebaseConfiguration.Embedded);

        // Option 3: Manual configuration
        builder.Services.AddPushFirebaseMessaging(new FirebaseConfiguration(
            UseEmbeddedConfiguration: false,
            AppId: "your-app-id",
            SenderId: "your-sender-id",
            ProjectId: "your-project-id",
            ApiKey: "your-api-key"
        ));

        // With a custom push delegate
        builder.Services.AddPushFirebaseMessaging<MyPushDelegate>();

        return builder.Build();
    }
}
```

### Custom Push Delegate

```csharp
public class MyPushDelegate : IPushDelegate
{
    public Task OnReceived(PushNotification notification)
    {
        // Handle incoming push notification
        return Task.CompletedTask;
    }

    public Task OnTokenChanged(string token)
    {
        // Handle FCM token changes - send to your backend
        return Task.CompletedTask;
    }

    public Task OnEntry(PushNotificationResponse response)
    {
        // Handle when user taps on a notification
        return Task.CompletedTask;
    }
}
```

## FirebaseConfiguration Record

| Parameter | Type | Default | Description |
|---|---|---|---|
| `UseEmbeddedConfiguration` | `bool` | `true` | Use platform config files (GoogleService-Info.plist / google-services.json) |
| `AppId` | `string?` | `null` | Firebase App ID (required if not using embedded config) |
| `SenderId` | `string?` | `null` | Firebase Sender ID (required if not using embedded config) |
| `ProjectId` | `string?` | `null` | Firebase Project ID (required if not using embedded config) |
| `ApiKey` | `string?` | `null` | Firebase API Key (required if not using embedded config) |
| `DefaultChannel` | `NotificationChannel?` | `null` | Android only - default notification channel |
| `IntentAction` | `string?` | `null` | Android only - custom intent action |

## Platform-Specific Requirements

### iOS
- Add `GoogleService-Info.plist` to your iOS project (if using embedded configuration)
- Set Build Action to `BundleResource`
- Enable Push Notifications capability in Entitlements.plist
- Enable Remote Notifications background mode

### Android
- Add `google-services.json` to your Android project root (if using embedded configuration)
- The Shiny Push library handles the Android Firebase initialization automatically

## Topic Subscriptions (Tags)

The iOS implementation supports topic subscriptions through `IPushTagSupport`:

```csharp
// Inject IPushManager
var push = services.GetRequiredService<IPushManager>();

// Cast provider to access tag support
if (push is IPushTagSupport tagSupport)
{
    await tagSupport.AddTag("news");
    await tagSupport.RemoveTag("promotions");
    await tagSupport.SetTags("news", "updates");
    await tagSupport.ClearTags();

    var currentTags = tagSupport.RegisteredTags;
}
```

## Extension Methods

### `AddPushFirebaseMessaging(FirebaseConfiguration? config = null)`
Registers Firebase push notification services. Pass `null` or omit for embedded configuration.

### `AddPushFirebaseMessaging<TPushDelegate>(FirebaseConfiguration? config = null)`
Registers Firebase push with a custom `IPushDelegate` implementation that handles notification events.

## Key Source Files
- `Shiny.Push.FirebaseMessaging/FirebaseConfiguration.cs` - Configuration record
- `Shiny.Push.FirebaseMessaging/Platforms/Shared/ServiceCollectionExtensions.cs` - DI registration
- `Shiny.Push.FirebaseMessaging/Platforms/iOS/FirebasePushProvider.cs` - iOS FCM provider
