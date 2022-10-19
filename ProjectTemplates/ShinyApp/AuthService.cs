//-:cnd:noEmit
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

#if ANDROID
    readonly AndroidPlatform platform;

    public MsalAuthenticationService(AndroidPlatform platform. IConfiguration config)
    {
        this.platform = platform;
#else
    public MsalAuthenticationService(IConfiguration config)
    {
#endif
        this.options = config.GetSection("Msal").Get<MsalOptions>();

        this.pca = PublicClientApplicationBuilder
            .Create(this.options.ClientId)
            //.WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
            //.WithIosKeychainSecurityGroup(B2CConstants.IOSKeyChainGroup)
#if ANDROID
            .WithRedirectUri(this.options.AndroidRedirectUri)
#else
            .WithRedirectUri(this.options.AppleRedirectUri)
#endif
            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
            .Build();
    }



    public async Task<bool> Authenticate()
    {
        //            if (UseEmbedded)
        //            {

        //                return await PCA.AcquireTokenInteractive(scopes)
        //                                        .WithUseEmbeddedWebView(true)
        //                                        .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
        //                                        .ExecuteAsync()
        //                                        .ConfigureAwait(false);
        //            }

        var result = await this.pca
            .AcquireTokenInteractive(SCOPES)
            .WithSystemWebViewOptions(new SystemWebViewOptions
            {
                iOSHidePrivacyPrompt = true
            })
#if ANDROID
            .WithParentActivityOrWindow(this.platform.CurrentActivity)
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
    public string AppleRedirectUri { get; set; }
    public string AndroidRedirectUri { get; set; }
}
//+:cnd:noEmit