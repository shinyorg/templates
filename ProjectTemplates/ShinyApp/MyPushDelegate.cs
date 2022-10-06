using Shiny.Push;

namespace ShinyApp;


public class MyPushDelegate : IPushDelegate
{
    public Task OnEntry(PushNotification data) => Task.CompletedTask;
    public Task OnReceived(PushNotification data) => Task.CompletedTask;
    public Task OnTokenRefreshed(string token) => Task.CompletedTask;
}
