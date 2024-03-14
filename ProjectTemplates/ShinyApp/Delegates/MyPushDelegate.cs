using Shiny.Push;

namespace ShinyApp.Delegates;


public class MyPushDelegate : PushDelegate
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
