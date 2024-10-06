using System.Net.Http.Headers;
using Shiny.Mediator.Http;

namespace ShinyApp.Infrastructure;


public class AppHttpRequestDecorator<TRequest, TResult>(
    // IAuthenticationService authService,
    IAppInfo appInfo,
    IDeviceInfo deviceInfo
) : IHttpRequestDecorator<TRequest, TResult>
    where TRequest : IHttpRequest<TResult>
{
    public async Task Decorate(HttpRequestMessage httpMessage, TRequest request)
    {
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

