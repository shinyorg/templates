using Shiny.Locations;

namespace ShinyApp;


public partial class GpsDelegate : IGpsDelegate
{
    public MyGpsDelegate()
    {
    }

    public async Task OnReading(GpsReading reading)
    {
        throw new NotImplementedException();
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
