using Shiny.Beacons;

namespace ShinyApp.Delegates;


public partial class MyBeaconMonitorDelegate : IBeaconMonitorDelegate
{
    public MyBeaconMonitorDelegate() 
    {
    }


    public Task OnStatusChanged(BeaconRegionState newStatus, BeaconRegion region) 
    {
        return Task.CompletedTask;
    }
}

//-:cnd:noEmit
#if ANDROID
public partial class MyBeaconMonitorDelegate : IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif
//+:cnd:noEmit