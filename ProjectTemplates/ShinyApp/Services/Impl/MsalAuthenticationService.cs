using Microsoft.Identity.Client;

namespace ShinyApp.Services.Impl;


[Shiny.Stores.ObjectStoreBinder("secure")]
public class MsalAuthenticationService : NotifyPropertyChanged, IAuthenticationService
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
#if APPLE
            .WithIosKeychainSecurityGroup(this.options.KeyChainGroup)            
#endif
//+:cnd:noEmit
            .WithRedirectUri(this.options.RedirectUri)            
            .Build();
    }

    // this looks like a viewmodel type property as shiny binds it secure storage
    string? authToken;
    public string? AuthenticationToken
    {
        get => this.authToken;
        set => this.Set(ref this.authToken, value);
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
    public string KeyChainGroup { get; set; }
    public string RedirectUri { get; set; }
    public string B2CSigninSignupAuthority { get; set; }
}