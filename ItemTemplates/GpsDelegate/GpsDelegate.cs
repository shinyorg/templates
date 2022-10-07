using Shiny.Locations;

namespace $rootnamespace$


public partial class $safeitemname$ : IGpsDelegate
{
    public $safeitemname$()
    {
    }

    public async Task OnReading(GpsReading reading)
    {
        throw new NotImplementedException();
    }
}

#if ANDROID
public partial class $safeitemname$ : Shiny.IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif
