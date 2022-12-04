using Shiny.Locations;

namespace ShinyApp.Delegates;


public partial class MyGeofenceDelegate : IGeofenceDelegate
{
    public MyGeofenceDelegate()
    {
    }


    public Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
    {
        throw new NotImplementedException();
    }
}


//-:cnd:noEmit
#if ANDROID
public partial class MyGeofenceDelegate : IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif
//+:cnd:noEmit