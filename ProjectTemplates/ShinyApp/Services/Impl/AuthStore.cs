namespace ShinyApp.Services.Impl;

public class AuthStore : NotifyPropertyChanged
{
    string? authToken;
    public string? AuthenticationToken
    {
        get => this.authToken;
        set => this.Set(ref this.authToken, value);
    }

    string? refreshToken;
    public string? RefreshToken
    {
        get => this.refreshToken;
        set => this.Set(ref this.refreshToken, value);
    }

    DateTimeOffset? expiration;
    public DateTimeOffset? Expiration
    {
        get => this.expiration;
        set => this.Set(ref this.expiration, value);
    }

    public void Reset()
    {
        this.AuthenticationToken = null;
        this.RefreshToken = null;
        this.Expiration = null;
    }
}