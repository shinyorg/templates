using System.Threading.Tasks;
using Shiny.Net.Http;

namespace ShinyApp.Delegates;


public partial class MyHttpTransferDelegate : IHttpTransferDelegate
{
    public MyHttpTransferDelegate() 
    {
    }

    public Task OnError(IHttpTransfer transfer, Exception ex)
    {
        throw new NotImplementedException();
    }


    public Task OnCompleted(IHttpTransfer transfer)
    {
        throw new NotImplementedException();
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