namespace ShinyApp.Services.Impl;


[Singleton]
public class WebAuthenticatorAuthService(AuthStore authStore) : NotifyPropertyChanged, IAuthenticationService 
{
    // this looks like a viewmodel type property as shiny binds it secure storage
    public string? AuthenticationToken => authStore.AuthenticationToken;
    
    public async Task<bool> Authenticate()
    {
        var scheme = "..."; // Apple, Microsoft, Google, Facebook, etc.
        var authUrlRoot = "https://mysite.com/mobileauth/";
        WebAuthenticatorResult result = null;

        if (scheme.Equals("Apple")
            && DeviceInfo.Platform == DevicePlatform.iOS
            && DeviceInfo.Version.Major >= 13)
        {
            // Use Native Apple Sign In API's
            result = await AppleSignInAuthenticator.AuthenticateAsync();
        }
        else
        {
            // Web Authentication flow
            var authUrl = new Uri($"{authUrlRoot}{scheme}");
            var callbackUrl = new Uri("myapp://");

            result = await WebAuthenticator.Default.AuthenticateAsync(authUrl, callbackUrl);
        }

        var authToken = string.Empty;

        if (result.Properties.TryGetValue("name", out string name) && !string.IsNullOrEmpty(name))
            authToken += $"Name: {name}{Environment.NewLine}";

        if (result.Properties.TryGetValue("email", out string email) && !string.IsNullOrEmpty(email))
            authToken += $"Email: {email}{Environment.NewLine}";

        // Note that Apple Sign In has an IdToken and not an AccessToken
        authToken += result?.AccessToken ?? result?.IdToken;
        authStore.AuthenticationToken = authToken;

        return true;
    }


    public Task SignOut() 
    {
        authStore.Reset();
        return Task.CompletedTask;
    }


    public Task<bool> TryRefresh() 
    {
        return Task.FromResult(false);
    }
}