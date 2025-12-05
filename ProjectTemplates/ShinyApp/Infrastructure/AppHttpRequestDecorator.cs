using System.Net.Http.Headers;
using Shiny.Mediator.Http;

namespace ShinyApp.Infrastructure;


[Singleton]
public class AppHttpRequestDecorator : IHttpRequestDecorator
{
    public async Task Decorate(HttpRequestMessage httpMessage, IMediatorContext context, CancellationToken cancellationToken)
    {
        var appInfo = AppInfo.Current;
        var deviceInfo = DeviceInfo.Current;
        
        httpMessage.Headers.Add("X-App-Version", appInfo.VersionString);
        httpMessage.Headers.Add("X-App-Build", appInfo.BuildString);
        httpMessage.Headers.Add("X-Device-Platform", deviceInfo.Platform.ToString());
        httpMessage.Headers.Add("X-Device-Version", deviceInfo.Version.ToString());
        httpMessage.Headers.Add("X-Device-Model", deviceInfo.Model);
        httpMessage.Headers.Add("X-Device-Manufacturer", deviceInfo.Manufacturer);

        // await authService.TryRefresh();

        // if (authService.AuthenticationToken != null)
        //     httpMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.AuthenticationToken);
    }
}

