using System.Net.Http.Headers;

namespace ShinyApp.Services.Impl;


public class AuthHttpDelegatingHandler : DelegatingHandler
{
	readonly IAuthService authService;
	public AuthHttpDelegatingHandler(IAuthService authService)
		=> this.authService = authService;


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (this.authService.AuthenticationToken != null)
        {
            await this.authService.TryRefresh().ConfigureAwait(false);
            if (this.authService.AuthenticationToken != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.authService.AuthenticationToken);
        }
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}