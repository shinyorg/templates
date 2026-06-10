using System;
using System.Threading.Tasks;
using Shiny.Data.Sync;

namespace ShinyApp.Delegates;


public class MyDataSyncDelegate : IDataSyncDelegate
{
    public Task OnSent(SyncOperation operation, string? responseBody)
    {
        // Server accepted the op. responseBody may carry a server-assigned id / ETag / etc.
        return Task.CompletedTask;
    }


    public Task OnError(SyncOperation operation, int statusCode, Exception ex)
    {
        // The op exhausted its retry budget. Persist to a dead-letter store, surface to the user, or re-queue.
        return Task.CompletedTask;
    }


    public Task OnReceived(SyncReceivedItem item)
    {
        // The engine already deserialized item.Entity via Shiny.Json.Default.
        // Apply the change to your local store; item.Verb is Create / Update / Delete.
        return Task.CompletedTask;
    }


    public Task<ConflictResolution> OnConflict(SyncOperation operation, string remotePayload)
        // Server already has a newer version. Decide how to resolve.
        => Task.FromResult(ConflictResolution.AcceptRemote);
}
