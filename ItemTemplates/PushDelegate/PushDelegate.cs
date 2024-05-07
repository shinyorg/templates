using Shiny.Push;
#if APPLE
using UIKit;
using UserNotifications;
#endif

namespace ShinyApp;


public partial class MyPushDelegate : PushDelegate
{
    public override async Task OnEntry(PushNotification notification)
    {
    }

    public override async Task OnReceived(PushNotification notification)
    {
    }

    public override async Task OnNewToken(string token)
    {
    }

    public override async Task OnUnRegistered(string token)
    {
    }
}

#if APPLE
public partial class MyPushDelegate : IApplePushDelegate
{
    public UNNotificationPresentationOptions? GetPresentationOptions(PushNotification notification) => null;

    public UIBackgroundFetchResult? GetFetchResult(PushNotification notification) => null;
}
#endif