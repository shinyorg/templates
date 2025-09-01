using Android.App;
using Android.OS;
using Android.Content.PM;
using Microsoft.Maui.ApplicationModel;
#if (usemsal)
using Android.Content;
using Android.Runtime;
using Microsoft.Identity.Client;
#endif

namespace ShinyApp;


[Activity(
#if (usepush || notifications || mediaelement)
    LaunchMode = LaunchMode.SingleTop,
#endif
#if (mediaelement)
    ResizeableActivity = true,
#endif
    Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
#if (usedeeplinks)
    Exported = true,
#endif
    ConfigurationChanges = 
        ConfigChanges.ScreenSize | 
        ConfigChanges.Orientation | 
        ConfigChanges.UiMode | 
        ConfigChanges.ScreenLayout | 
        ConfigChanges.SmallestScreenSize | 
        ConfigChanges.Density
)]
[IntentFilter(
    [
#if (usepush)
        ShinyPushIntents.NotificationClickAction,
#endif
#if (notifications)
        ShinyNotificationIntents.NotificationClickAction,
#endif
        Platform.Intent.ActionAppAction,
        global::Android.Content.Intent.ActionView
    ],    
#if (usedeeplinks)
    AutoVerify = true,
    DataScheme = "https",
    DataHost = "{DEEPLINK_HOST}",
#endif
    Categories = [ 
        global::Android.Content.Intent.CategoryDefault,
        global::Android.Content.Intent.CategoryBrowsable
    ]
)]
public class MainActivity : MauiAppCompatActivity
{
#if (usemsal)
    /// <summary>
    /// This is a callback to continue with the broker base authentication
    /// Info abour redirect URI: https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-client-application-configuration#redirect-uri
    /// </summary>
    /// <param name="requestCode">request code </param>
    /// <param name="resultCode">result code</param>
    /// <param name="data">intent of the actvity</param>
    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }
#endif
}
