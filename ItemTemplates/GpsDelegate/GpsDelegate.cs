using Shiny.Locations;
using Microsoft.Extensions.Logging;

namespace ShinyApp;


public partial class MyGpsDelegate : GpsDelegate
{
    public MyGpsDelegate(ILogger<MyGpsDelegate> logger) : base(logger)
    {
    }

    public override async Task OnGpsReading(GpsReading reading)
    {
    }
}

#if ANDROID
public partial class MyGpsDelegate : Shiny.IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif
