using Shiny.Hosting;

#if APPLE
using UIKit;
#elif ANDROID
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
#endif

namespace ShinyApp.Delegates;


public partial class DeepLinkDelegate
{
    //https://github.com/Redth/MAUI.AppLinks.Sample/tree/main

    public void Handle(string uri) 
    {

    }
}

#if APPLE
public partial class DeepLinkDelegate : IosLifecycle.IContinueActivity
{
 // ios.FinishedLaunching((app, data)
        //     => HandleAppLink(app.UserActivity));

        // ios.ContinueUserActivity((app, userActivity, handler)
        //     => HandleAppLink(userActivity));

        // if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
        // {
        //     ios.SceneWillConnect((scene, sceneSession, sceneConnectionOptions)
        //         => HandleAppLink(sceneConnectionOptions.UserActivities.ToArray()
        //             .FirstOrDefault(a => a.ActivityType == NSUserActivityType.BrowsingWeb)));

        //     ios.SceneContinueUserActivity((scene, userActivity)
        //         => HandleAppLink(userActivity));
        // }    

    public bool Handle(NSUserActivity activity, UIApplicationRestorationHandler completionHandler) 
    {
        return false;
    }
}
#elif ANDROID
public partial class DeepLinkDelegate : IAndroidLifecycle.IOnActivityOnCreate
{
    public void ActivityOnCreate(Activity activity, Bundle? savedInstanceState)
    {
            var action = activity.Intent?.Action;
            var data = activity.Intent?.Data?.ToString();

            if (action == Intent.ActionView && data != null) 
                this.Handle(data);
    }
}
#endif