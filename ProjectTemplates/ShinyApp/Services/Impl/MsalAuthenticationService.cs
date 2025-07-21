using Microsoft.Identity.Client;

namespace ShinyApp.Services.Impl;


[Singleton]
public class MsalAuthenticationService : IAuthenticationService
{
    readonly AuthStore authStore;
    static readonly string[] SCOPES = new[] { "User.Read" };
    readonly MsalOptions options;
    readonly IPublicClientApplication pca;
//-:cnd:noEmit
#if ANDROID
    readonly AndroidPlatform platform;

    public MsalAuthenticationService(AuthStore authStore, AndroidPlatform platform, IConfiguration config)
    {
        this.platform = platform;
#else
    public MsalAuthenticationService(AuthStore authStore, IConfiguration config)
    {
#endif
        //+:cnd:noEmit
        this.authStore = authStore;
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
#if APPLE
            .WithIosKeychainSecurityGroup(this.options.KeyChainGroup)            
#endif
//+:cnd:noEmit
            .WithRedirectUri(this.options.RedirectUri)            
            .Build();
    }


    public string? AuthenticationToken => this.authStore.AuthenticationToken;

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
        this.authStore.Reset();
        var accounts = await this.pca
            .GetAccountsAsync()
            .ConfigureAwait(false);

        foreach (var acct in accounts)
            await this.pca.RemoveAsync(acct).ConfigureAwait(false);
    }


    void SetAuth(AuthenticationResult result)
    {
        this.authStore.AuthenticatetionToken = result.IdToken;
        // TODO: refresh token and expiries and all that good stuff is for you :)
        Console.WriteLine("Token received");
    }
}


class MsalOptions
{
    public string ClientId { get; set; }
    public string TenantId { get; set; }
    public string KeyChainGroup { get; set; }
    public string RedirectUri { get; set; }
    public string B2CSigninSignupAuthority { get; set; }
}