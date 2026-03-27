--
name: shiny-music
description: Generate code using Shiny.Music, a unified API for accessing the device music library on Android and iOS with permissions, metadata querying, filtering, playback, and file copy
auto_invoke: true
triggers:
  - music library
  - music player
  - device music
  - IMediaLibrary
  - IMusicPlayer
  - MusicMetadata
  - MusicFilter
  - GroupedCount
  - PlaylistInfo
  - media library
  - music permission
  - music playback
  - play music
  - copy track
  - audio library
  - MediaStore audio
  - MPMediaQuery
  - music genre
  - genre query
  - music year
  - music decade
  - year query
  - decade query
  - filter tracks
  - playlist
  - playlists
  - GetGenresAsync
  - GetYearsAsync
  - GetDecadesAsync
  - GetTracksAsync
  - GetPlaylistsAsync
  - GetPlaylistTracksAsync
  - Shiny.Music
  - music metadata
  - READ_MEDIA_AUDIO
  - NSAppleMusicUsageDescription
---

# Shiny Music Skill

You are an expert in Shiny.Music, a .NET library that provides a unified API for accessing the device music library on Android and iOS. It supports permission management, querying track metadata, playing music files, and copying tracks (where platform restrictions allow).

## When to Use This Skill

Invoke this skill when the user wants to:
- Access the device music library on Android or iOS
- Request permissions to read audio/music from the device
- Query or search music track metadata (title, artist, album, duration, year, explicit content, etc.)
- Get all distinct genres, years, or decades from the user's music library (with track counts)
- Browse playlists and get tracks within a playlist
- Filter tracks by genre, year, decade, and/or search text using `MusicFilter`
- Cross-query: get genres within a decade, years within a genre, etc.
- Play, pause, resume, stop, or seek within music tracks
- Play Apple Music subscription (DRM) tracks via `StoreId` and `MPMusicPlayerController` on iOS
- Check for an active streaming subscription via `HasStreamingSubscriptionAsync()`
- Copy music files from the device library to app storage
- Understand DRM limitations on iOS (Apple Music subscription tracks)
- Configure Android manifest permissions or iOS Info.plist for music access

## Library Overview

- **Repository**: https://github.com/shinyorg/music
- **Namespace**: `Shiny.Music`
- **NuGet**: `Shiny.Music`
- **Targets**: `net10.0-android`, `net10.0-ios`
- **Docs**: https://shinylib.net/client/music

## Setup

### Dependency Injection (MAUI)

```csharp
using Shiny.Music;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddShinyMusic();

        return builder.Build();
    }
}
```

## Platform Configuration

### Android — AndroidManifest.xml

```xml
<!-- Android 13+ (API 33+) -->
<uses-permission android:name="android.permission.READ_MEDIA_AUDIO" />

<!-- Android 12 and below (API < 33) -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"
                 android:maxSdkVersion="32" />
```

- **API 33+**: Uses the granular `READ_MEDIA_AUDIO` permission (audio files only).
- **API < 33**: Falls back to `READ_EXTERNAL_STORAGE`.
- Minimum supported API level: 24 (Android 7.0).

### iOS — Info.plist

```xml
<key>NSAppleMusicUsageDescription</key>
<string>This app needs access to your music library to browse and play your music.</string>
```

**This key is mandatory.** The app will crash on launch without it. No special entitlements are required.

## Core API Reference

### IMediaLibrary

Provides access to the device music library including permissions, querying, and file operations.

#### RequestPermissionAsync

```csharp
Task<PermissionStatus> RequestPermissionAsync();
```

Prompts the user for music library access. On Android, requests `READ_MEDIA_AUDIO` (API 33+) or `READ_EXTERNAL_STORAGE` (older). On iOS, calls `MPMediaLibrary.RequestAuthorization`.

Returns: `PermissionStatus` — `Granted`, `Denied`, `Restricted` (iOS only), or `Unknown`.

#### CheckPermissionAsync

```csharp
Task<PermissionStatus> CheckPermissionAsync();
```

Checks the current permission status without prompting the user.

#### GetAllTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetAllTracksAsync();
```

Returns all music tracks on the device. Permission must be granted first. On Android, queries `MediaStore.Audio.Media` with `IsMusic != 0`. On iOS, uses `MPMediaQuery` filtered to `MPMediaType.Music`. Only music is returned — no videos, ringtones, podcasts, or audiobooks.

#### SearchTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> SearchTracksAsync(string query);
```

Searches tracks by title, artist, or album. Case-insensitive partial string matching.

#### GetGenresAsync

```csharp
Task<IReadOnlyList<GroupedCount<string>>> GetGenresAsync(MusicFilter? filter = null);
```

Returns all distinct, non-null genre names from the user's music library with track counts, sorted alphabetically. When a `MusicFilter` is provided, only tracks matching the filter criteria are considered for grouping. Permission must be granted first.

#### GetYearsAsync

```csharp
Task<IReadOnlyList<GroupedCount<int>>> GetYearsAsync(MusicFilter? filter = null);
```

Returns all distinct, non-zero release years from the user's music library with track counts, sorted in ascending order. When a `MusicFilter` is provided, only tracks matching the filter criteria are considered. On Android, uses `MediaStore.Audio.Media.YEAR`; on iOS, derives year from `MPMediaItem.ReleaseDate`.

#### GetDecadesAsync

```csharp
Task<IReadOnlyList<GroupedCount<int>>> GetDecadesAsync(MusicFilter? filter = null);
```

Returns all distinct decades with track counts, sorted in ascending order. Each decade is its starting year (e.g., 1990 for the 1990s). When a `MusicFilter` is provided, only tracks matching the filter criteria are considered.

#### GetTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetTracksAsync(MusicFilter filter);
```

Returns tracks matching the specified filter criteria. All non-null filter properties are combined with AND logic. On Android, genre filtering queries via `MediaStore.Audio.Genres.Members`; year/decade/search use SQL WHERE clauses. On iOS, genre uses `MPMediaQuery` predicates; year/decade/search use LINQ filtering.

#### GetPlaylistsAsync

```csharp
Task<IReadOnlyList<PlaylistInfo>> GetPlaylistsAsync();
```

Returns all playlists from the device music library with their song counts, sorted alphabetically by name. On Android, reads from `MediaStore.Audio.Playlists`. On iOS, reads from `MPMediaQuery.PlaylistsQuery`. Permission must be granted first.

#### GetPlaylistTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetPlaylistTracksAsync(string playlistId);
```

Returns all tracks in the specified playlist, in playlist order. The `playlistId` is the platform-specific identifier returned by `GetPlaylistsAsync`. On Android, queries `MediaStore.Audio.Playlists.Members`. On iOS, retrieves tracks from the `MPMediaPlaylist` with the matching persistent ID.

#### CopyTrackAsync

```csharp
Task<bool> CopyTrackAsync(MusicMetadata track, string destinationPath);
```

Copies a music file to the specified path. Creates parent directories if needed. Returns `false` for DRM-protected tracks or on failure.

- **Android**: Reads from ContentResolver input stream. All local files can be copied. Original format preserved.
- **iOS**: Exports via `AVAssetExportSession` in M4A format. DRM-protected Apple Music subscription tracks **cannot** be copied (`AssetURL` is null).

### HasStreamingSubscriptionAsync

```csharp
Task<bool> HasStreamingSubscriptionAsync();
```

Checks whether the user has an active music streaming subscription that allows catalog playback. On iOS, this queries `SKCloudServiceController` for the `MusicCatalogPlayback` capability. On Android, this always returns `false`.

### IMusicPlayer

Controls playback of music files from the device library. Implements `IDisposable`.

On iOS, the player supports two modes:
- **Local playback** via `AVAudioPlayer` when `ContentUri` is available (purchased/synced tracks).
- **Streaming playback** via `MPMusicPlayerController.SystemMusicPlayer` when `StoreId` is available (Apple Music subscription tracks).

`PlayAsync` automatically selects the appropriate mode based on the track's properties.

#### PlayAsync

```csharp
Task PlayAsync(MusicMetadata track);
```

Stops any current track, loads the specified one, and begins playback. Throws `InvalidOperationException` if both `ContentUri` and `StoreId` are empty or the platform player fails.

- **Android**: Uses `Android.Media.MediaPlayer` with content URIs.
- **iOS (local)**: Uses `AVAudioPlayer` with `ipod-library://` asset URLs when `ContentUri` is available.
- **iOS (streaming)**: Uses `MPMusicPlayerController.SystemMusicPlayer` with the Apple Music catalog `StoreId` when `ContentUri` is empty but `StoreId` is available.

#### Pause / Resume / Stop

```csharp
void Pause();   // No effect if not Playing
void Resume();  // No effect if not Paused
void Stop();    // Stops and releases the current track
```

#### Seek

```csharp
void Seek(TimeSpan position);
```

Seeks to the specified position. Android uses millisecond precision; iOS uses second precision.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `State` | `PlaybackState` | Current state: `Stopped`, `Playing`, or `Paused` |
| `CurrentTrack` | `MusicMetadata?` | Currently loaded track, or `null` if stopped |
| `Position` | `TimeSpan` | Current playback position (`TimeSpan.Zero` if no track) |
| `Duration` | `TimeSpan` | Total duration of current track (`TimeSpan.Zero` if no track) |

#### Events

| Event | Description |
|-------|-------------|
| `StateChanged` | Raised on state transitions (e.g., Playing → Paused) |
| `PlaybackCompleted` | Raised when a track finishes naturally (not via `Stop()`) |

### MusicMetadata

```csharp
public record MusicMetadata(
    string Id,
    string? Title,
    string? Artist,
    string? Album,
    string? Genre,
    TimeSpan Duration,
    string? AlbumArtUri,
    bool? IsExplicit,
    string ContentUri,
    string? StoreId = null,
    int? Year = null
);
```

| Property | Description |
|----------|-------------|
| `Id` | Platform-specific unique ID. Android: MediaStore row ID. iOS: MPMediaItem persistent ID. |
| `Title` | Track title, or `null` if not available. |
| `Artist` | Artist or performer, or `null` if not available. |
| `Album` | Album name, or `null` if not available. |
| `Genre` | Genre, or `null` if unavailable. |
| `Duration` | Playback duration. |
| `AlbumArtUri` | Album art URI (Android only via MediaStore; `null` on iOS). |
| `IsExplicit` | Whether the track is marked as explicit content. iOS only via `MPMediaItem.IsExplicitItem`; always `null` on Android. |
| `ContentUri` | URI for playback/copy. Android: `content://` URI. iOS: `ipod-library://` asset URL. **Empty string for DRM-protected Apple Music tracks** — these cannot be played via AVAudioPlayer or copied. |
| `StoreId` | Optional Apple Music catalog ID (from `PlayParams.Id`). Enables streaming playback via `MPMusicPlayerController` on iOS. Always `null` on Android. |
| `Year` | Release year of the track, or `null` if not available. Android: `MediaStore.Audio.Media.YEAR`; iOS: derived from `MPMediaItem.ReleaseDate`. |

### PlaylistInfo

```csharp
public record PlaylistInfo(string Id, string Name, int SongCount);
```

| Property | Description |
|----------|-------------|
| `Id` | Platform-specific unique identifier. Android: MediaStore playlist row ID. iOS: persistent ID. |
| `Name` | The display name of the playlist. |
| `SongCount` | The number of tracks in the playlist. |

### MusicFilter

Defines optional criteria for filtering music tracks. All specified properties are combined with AND logic. Used with `GetTracksAsync`, `GetGenresAsync`, `GetYearsAsync`, and `GetDecadesAsync`.

```csharp
public class MusicFilter
{
    public string? Genre { get; init; }
    public int? Year { get; init; }
    public int? Decade { get; init; }
    public string? SearchQuery { get; init; }
}
```

| Property | Description |
|----------|-------------|
| `Genre` | Filter by genre name (case-insensitive match). |
| `Year` | Filter by exact release year. Takes precedence over `Decade` if both are set. |
| `Decade` | Filter by decade start year (e.g., 1990 for the 1990s). Ignored if `Year` is also set. |
| `SearchQuery` | Text search across title, artist, and album (case-insensitive, contains match). |

### GroupedCount&lt;T&gt;

Returned by `GetGenresAsync`, `GetYearsAsync`, and `GetDecadesAsync`.

```csharp
public record GroupedCount<T>(T Value, int Count);
```

| Property | Description |
|----------|-------------|
| `Value` | The grouped value (`string` for genres, `int` for years/decades). |
| `Count` | The number of tracks that belong to this group. |

### PermissionStatus

| Value | Description |
|-------|-------------|
| `Unknown` | User has not been prompted yet |
| `Denied` | User denied access |
| `Granted` | User granted access |
| `Restricted` | iOS only — blocked by system policy (parental controls, MDM) |

### PlaybackState

| Value | Description |
|-------|-------------|
| `Stopped` | No track playing; player is idle |
| `Playing` | A track is actively playing |
| `Paused` | Playback is paused and can be resumed |

## DRM and ContentUri

On iOS, Apple Music subscription tracks are DRM-protected. For these tracks:
- `MPMediaItem.AssetURL` is `null`
- `MusicMetadata.ContentUri` will be `string.Empty`
- `CopyTrackAsync` will return `false`

However, if the track has a `StoreId` (Apple Music catalog ID), it **can** be played via `MPMusicPlayerController.SystemMusicPlayer`. The player automatically uses this path when `StoreId` is available.

| Track Source | ContentUri | StoreId | Playable | Copyable |
|---|---|---|---|---|
| iTunes purchases (DRM-free) | ✅ populated | ⚠️ may exist | ✅ (AVAudioPlayer) | ✅ |
| Locally synced from computer | ✅ populated | ❌ | ✅ (AVAudioPlayer) | ✅ |
| Apple Music subscription | ❌ empty | ✅ populated | ✅ (SystemMusicPlayer) | ❌ |
| iTunes Match (cloud) | ⚠️ only if downloaded | ⚠️ may exist | ⚠️ | ⚠️ |
| Android local files | ✅ always populated | ❌ | ✅ | ✅ |

## Streaming Subscription Check

Use `HasStreamingSubscriptionAsync()` to determine if the user can play Apple Music catalog content:

```csharp
var canStream = await _library.HasStreamingSubscriptionAsync();
if (canStream)
{
    // User has an active Apple Music subscription
    // Tracks with StoreId can be played via MPMusicPlayerController
}
```

On Android, this always returns `false`.

## Code Generation Best Practices

1. **Always request permission first** — call `RequestPermissionAsync()` before any query or playback operation.
2. **Check playability before playing on iOS** — use `StoreId` for streaming or `ContentUri` for local playback. If both are empty, the track cannot be played.
3. **Register as singletons** — both `IMediaLibrary` and `IMusicPlayer` should be singletons in DI.
4. **Dispose the player** — `IMusicPlayer` implements `IDisposable`; call `Dispose()` or let the DI container handle it.
5. **Test on physical devices** — simulators/emulators have no music content.
6. **Handle `Restricted` on iOS** — distinct from `Denied`; means system policy blocks access.
7. **Copy format on iOS is M4A** — regardless of original encoding, `AVAssetExportSession` outputs M4A.
8. **Use `HasStreamingSubscriptionAsync()`** — check before presenting streaming playback UI to the user.
9. **Use `MusicFilter` for combined queries** — filter tracks by genre + year/decade in a single call rather than filtering in memory.
10. **Use grouping methods with filters for cross-queries** — e.g., `GetGenresAsync(new MusicFilter { Decade = 1990 })` to find genres represented in the 90s.
11. **Use `GetPlaylistsAsync` and `GetPlaylistTracksAsync`** — browse playlists and retrieve their contents in playlist order.

## Filtering Examples

```csharp
// All Rock tracks
var rockTracks = await library.GetTracksAsync(new MusicFilter { Genre = "Rock" });

// All tracks from the 1990s
var nineties = await library.GetTracksAsync(new MusicFilter { Decade = 1990 });

// Rock tracks from 1995
var rock95 = await library.GetTracksAsync(new MusicFilter { Genre = "Rock", Year = 1995 });

// Genres in the 2000s (with counts)
var genres2000s = await library.GetGenresAsync(new MusicFilter { Decade = 2000 });

// Years for Jazz (with counts)
var jazzYears = await library.GetYearsAsync(new MusicFilter { Genre = "Jazz" });

// Decades for Pop (with counts)
var popDecades = await library.GetDecadesAsync(new MusicFilter { Genre = "Pop" });

// Combined: genres matching "rock" search in the 1980s
var rock80s = await library.GetGenresAsync(new MusicFilter { Decade = 1980, SearchQuery = "rock" });

// Browse all playlists
var playlists = await library.GetPlaylistsAsync();
foreach (var p in playlists)
    Console.WriteLine($"{p.Name} ({p.SongCount} songs)");

// Get tracks in a playlist
var playlistTracks = await library.GetPlaylistTracksAsync(playlists[0].Id);
```
