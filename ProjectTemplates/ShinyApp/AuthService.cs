using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace ShinyApp;


public interface IAuthenticationService
{
    Task<bool> TryRefresh();
    Task<bool> Authenticate();
    Task SignOut();
}


// THIS IS NOT MEANT TO BE A COMPLETE SERVICE.  IT IS MEANT ONLY AS A GOOD STARTING POINT
public class MsalAuthenticationService : IAuthenticationService
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
            .WithB2CAuthority(this.AuthoritySignInSignUp)            
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

#if (usemsalb2c)
    public string AuthoritySignInSignUp => $"https://{this.options.AzureADB2CHostname}/tfp/{this.options.TenantId}/{this.options.PolicySignUpSignIn}";

#endif
    public async Task<bool> Authenticate()
    {
        var result = await this.pca
            .AcquireTokenInteractive(SCOPES)

//-:cnd:noEmit
#if ANDROID
            .WithParentActivityOrWindow(this.platform.CurrentActivity)
#elif IOS || MACCATALYST
            .WithUseEmbeddedWebView(this.options.AppleUseEmbeddedView)
            .WithSystemWebViewOptions(new SystemWebViewOptions
            {
                iOSHidePrivacyPrompt = true
            })
#endif
//+:cnd:noEmit
#if (usemsalb2c)
            .WithB2CAuthority(this.AuthoritySignInSignUp)
#endif
            .ExecuteAsync()
            .ConfigureAwait(false);

        if (result == null)
            return false;

        this.SetAuth(result);
        return true;
    }


    public async Task<bool> TryRefresh()
    {
        try
        {
            var accts = await this.pca.GetAccountsAsync().ConfigureAwait(false);
            var acct = accts.FirstOrDefault();

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
        var accounts = await this.pca
            .GetAccountsAsync()
            .ConfigureAwait(false);

        foreach (var acct in accounts)
            await this.pca.RemoveAsync(acct).ConfigureAwait(false);
    }


    void SetAuth(AuthenticationResult result)
    {
        Console.WriteLine("Token received");
    }
}


class MsalOptions
{
    public string ClientId { get; set; }
    public string TenantId { get; set; }
    public bool AppleUseEmbeddedView { get; set; }
    public string AppleRedirectUri { get; set; }
    public string AppleKeyChainGroup { get; set; }
    public string AndroidRedirectUri { get; set; }

    public string PolicySignInSignUp { get; set; }
    public string AzureADB2CHostname { get; set; }
}