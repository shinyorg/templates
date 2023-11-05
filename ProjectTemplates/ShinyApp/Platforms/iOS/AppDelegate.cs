using Foundation;
using UIKit;
#if (usemsalbroker)
using Microsoft.Identity.Client;
#endif

namespace ShinyApp;


[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() 
    {
//#if (useappcenter)
        ObjCRuntime.Runtime.MarshalManagedException += (s, a) =>
            a.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
//#endif
        return MauiProgram.CreateMauiApp();
    }

#if (usepush)
    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        => global::Shiny.Hosting.Host.Lifecycle.OnRegisteredForRemoteNotifications(deviceToken);

    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        => global::Shiny.Hosting.Host.Lifecycle.OnFailedToRegisterForRemoteNotifications(error);

    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        => global::Shiny.Hosting.Host.Lifecycle.OnDidReceiveRemoteNotification(userInfo, completionHandler);

#endif
#if (usemsalbroker)
    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        if (AuthenticationContinuationHelper.IsBrokerResponse(null))
        {
            // Done on different thread to allow return in no time.
            _ = Task.Factory.StartNew(() => AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url));

            return true;
        }
        else if (!AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url))
        {
            return false;
        }

        return true;
    }
#endif
}
