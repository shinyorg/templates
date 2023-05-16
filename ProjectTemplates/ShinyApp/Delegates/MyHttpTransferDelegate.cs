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
public partial class MyHttpTransferDelegate : IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif
//+:cnd:noEmit