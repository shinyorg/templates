using Shiny.Locations;

namespace ShinyApp.Delegates;


public partial class MyGpsDelegate : IGpsDelegate
{
    public MyGpsDelegate()
    {

    }


    public Task OnReading(GpsReading reading)
    {
        throw new NotImplementedException();
    }
}

//-:cnd:noEmit
#if ANDROID
public partial class MyGpsDelegate : IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif
//+:cnd:noEmit