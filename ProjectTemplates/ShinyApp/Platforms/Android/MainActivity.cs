using Android.App;
using Android.Content.PM;
#if (usemsal)
using Android.Content;
using Android.Runtime;
using Microsoft.Identity.Client;
#endif

namespace ShinyApp;


[Activity(
    Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
    ConfigurationChanges = 
        ConfigChanges.ScreenSize | 
        ConfigChanges.Orientation | 
        ConfigChanges.UiMode | 
        ConfigChanges.ScreenLayout | 
        ConfigChanges.SmallestScreenSize | 
        ConfigChanges.Density
)]
#if (usepush && notifications)
[IntentFilter(
    new[] {
        ShinyPushIntents.NotificationClickAction,
        ShinyNotificationIntents.NotificationClickAction
    }
)]
#elif (usepush)
[IntentFilter(
    new[] { ShinyPushIntents.NotificationClickAction }
)]
#elif (notifications)
[IntentFilter(
    new[] { ShinyNotificationIntents.NotificationClickAction }
)]
#endif
public class MainActivity : MauiAppCompatActivity
{
#if (usemsal)
        // TODO: this should move to DI section
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
