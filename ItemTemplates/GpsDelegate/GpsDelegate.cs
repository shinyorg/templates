using Shiny.Locations;

namespace ShinyApp;


public partial class GpsDelegate : Shiny.Locations.GpsDelegate
{
    public MyGpsDelegate()
    {
    }

    public override async Task OnGpsReading(GpsReading reading)
    {
    }
}

#if ANDROID
public partial class GpsDelegate : Shiny.IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif
