using System.Net.Http.Headers;
using System.Globalization;
using Shiny.Mediator.Http;

namespace ShinyApp.Infrastructure;


[Singleton]
public class AppHttpRequestDecorator(TimeProvider timeProvider) : IHttpRequestDecorator
{
    public async Task Decorate(
        HttpRequestMessage httpMessage, 
        IMediatorContext context, 
        CancellationToken cancellationToken
    )
    {
        var appInfo = AppInfo.Current;
        var deviceInfo = DeviceInfo.Current;

        var locale = CultureInfo.CurrentCulture?.Name ?? string.Empty;
        var timezone = TimeZoneInfo.Local?.Id ?? string.Empty;

        httpMessage.Headers.UserAgent.ParseAdd($"{appInfo.PackageName}/{appInfo.VersionString} ({deviceInfo.Platform}; {deviceInfo.Model}; {deviceInfo.Manufacturer})");
        httpMessage.Headers.Add("X-App-Build", appInfo.BuildString);
        httpMessage.Headers.Add("X-Device-Locale", locale);
        httpMessage.Headers.Add("X-Device-Timezone", timezone);

        // await authService.TryRefresh();

        // if (authService.AuthenticationToken != null)
        //     httpMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.AuthenticationToken);
    }
}

