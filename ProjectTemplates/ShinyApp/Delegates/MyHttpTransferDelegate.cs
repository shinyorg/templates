using System.Threading.Tasks;
using Shiny.Net.Http;

namespace ShinyApp.Delegates;


public partial class MyHttpTransferDelegate : IHttpTransferDelegate
{
    public MyHttpTransferDelegate() 
    {
    }


    public Task OnCompleted(HttpTransferRequest request)
    {
        return Task.CompletedTask;
    }


    public Task OnError(HttpTransferRequest request, Exception ex)
    {
        return Task.CompletedTask;
    }
}

//-:cnd:noEmit
#if ANDROID
public partial class MyHttpTransferDelegate : IAndroidHttpTransferDelegate
{
    public void ConfigureNotification(AndroidX.Core.App.NotificationCompat.Builder builder, HttpTransfer transfer)
    {
        // switch (transfer.Status)
        // {
        //     case HttpTransferState.Pending:
        //         if (transfer.Request.IsUpload)
        //         {
        //             builder.SetContentText($"Starting Upload {Path.GetFileName(transfer.Request.LocalFilePath)} to {transfer.Request.Uri}");
        //         }
        //         else
        //         {
        //             builder.SetContentText($"Start Download from {transfer.Request.Uri}");
        //         }
        //         break;

        //     case HttpTransferState.Paused:
        //     case HttpTransferState.PausedByNoNetwork:
        //     case HttpTransferState.PausedByCostedNetwork:
        //         var type = transfer.Request.IsUpload ? "Upload" : "Download";
        //         builder.SetContentText($"Paused {type} for {transfer.Request.Uri}");
        //         break;

        //     case HttpTransferState.InProgress:
        //         if (transfer.Request.IsUpload)
        //         {
        //             builder.SetContentText($"Uploading {Path.GetFileName(transfer.Request.LocalFilePath)}");
        //         }
        //         else
        //         {
        //             builder.SetContentText($"Downloading file from {transfer.Request.Uri}");
        //         }
        //         break;
        // }
    }
}
#endif
//+:cnd:noEmit