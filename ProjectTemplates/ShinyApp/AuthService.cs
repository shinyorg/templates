#if usemsal
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

#endif
namespace ShinyApp;

// THIS IS NOT MEANT TO BE A COMPLETE SERVICE.  IT IS MEANT ONLY AS A GOOD STARTING POINT
public interface IAuthService
{
    string? AuthenticationToken { get; }
    Task<bool> Authenticate();
    Task SignOut();    
    Task<bool> TryRefresh();
}


#if usewebauthenticator
[Shiny.Stores.ObjectStoreBinder("secure")]
public class WebAuthenticatorAuthService : NotifyPropertyChanged, IAuthService 
{
    // this looks like a viewmodel type property as shiny binds it secure storage
    string? authToken;
    public string? AuthenticationToken
    {
        get => this.authToken;
        private set => this.Set(ref this.authToken, value);
    }
    
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

        return true;
    }


    public Task SignOut() 
    {
        this.AuthenticationToken = null;
        return Task.CompletedTask;
    }


    public Task<bool> TryRefresh() 
    {
        return Task.FromResult(false);
    }
}
#endif
#if usemsal
[Shiny.Stores.ObjectStoreBinder("secure")]
public class MsalAuthenticationService : NotifyPropertyChanged, IAuthService
{
    static readonly string[] SCOPES = new [] { "User.Read" };
    readonly MsalOptions options;
    readonly IPublicClientApplication pca;
//-:cnd:noEmit
#if ANDROID
    readonly AndroidPlatform platform;

    public MsalAuthenticationService(AndroidPlatform platform, IConfiguration config)
    {
        this.platform = platform;
#else
    public MsalAuthenticationService(IConfiguration config)
    {
#endif
//+:cnd:noEmit
        this.options = config.GetSection("Msal").Get<MsalOptions>();

        this.pca = PublicClientApplicationBuilder
            .Create(this.options.ClientId)
#if (usemsalb2c)
            .WithB2CAuthority(this.options.B2CSigninSignupAuthority)            
#endif
#if (usemsalbroker)
            .WithTenantId(this.options.TenantId)
            .WithBroker()
#endif
//-:cnd:noEmit
#if ANDROID
            .WithRedirectUri(this.options.AndroidRedirectUri)
#else
            .WithRedirectUri(this.options.AppleRedirectUri)
#endif
//+:cnd:noEmit
            .WithIosKeychainSecurityGroup(this.options.AppleKeyChainGroup)
            .Build();
    }

    // this looks like a viewmodel type property as shiny binds it secure storage
    string? authToken;
    public string? AuthenticationToken
    {
        get => this.authToken;
        private set => this.Set(ref this.authToken, value);
    }
    

    public async Task<bool> Authenticate()
    {
        try
        {
            var result = await this.pca
                .AcquireTokenInteractive(SCOPES)

//-:cnd:noEmit
#if ANDROID
                .WithParentActivityOrWindow(this.platform.CurrentActivity)
#elif IOS || MACCATALYST
                .WithSystemWebViewOptions(new SystemWebViewOptions
                {
                    iOSHidePrivacyPrompt = true
                })
#endif
//+:cnd:noEmit
#if (usemsalb2c)
                .WithB2CAuthority(this.options.B2CSigninSignupAuthority)
#endif
                .ExecuteAsync()
                .ConfigureAwait(false);

            if (result == null)
                return false;

            this.SetAuth(result);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public async Task<bool> TryRefresh()
    {
        try
        {
            var accts = await this.pca.GetAccountsAsync().ConfigureAwait(false);
            var acct = accts.FirstOrDefault();
            if (acct == null)
                return false;
                
            var authResult = await this.pca
                .AcquireTokenSilent(SCOPES, acct)
                .ExecuteAsync()
                .ConfigureAwait(false);

            this.SetAuth(authResult);
            return true;
        }
        catch (MsalUiRequiredException)
        {
            return false;
        }
    }


    public async Task SignOut()
    {
        this.AuthenticationToken = null;
        var accounts = await this.pca
            .GetAccountsAsync()
            .ConfigureAwait(false);

        foreach (var acct in accounts)
            await this.pca.RemoveAsync(acct).ConfigureAwait(false);
    }


    void SetAuth(AuthenticationResult result)
    {
        // this.AuthenticationToken = result.IdToken;
        Console.WriteLine("Token received");
    }
}


class MsalOptions
{
    public string ClientId { get; set; }
    public string TenantId { get; set; }
    public string AppleRedirectUri { get; set; }
    public string AppleKeyChainGroup { get; set; }
    public string AndroidRedirectUri { get; set; }
    public string B2CSigninSignupAuthority { get; set; }
}
#endif