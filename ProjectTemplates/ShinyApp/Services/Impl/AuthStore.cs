namespace ShinyApp.Services.Impl;

public class AuthStore : NotifyPropertyChanged
{
    public string? AuthenticationToken
    {
        get;
        set => this.Set(ref field, value);
    }

    
    public string? RefreshToken
    {
        get;
        set => this.Set(ref field, value);
    }
    
    public DateTimeOffset? Expiration
    {
        get;
        set => this.Set(ref field, value);
    }

    public void Reset()
    {
        this.AuthenticationToken = null;
        this.RefreshToken = null;
        this.Expiration = null;
    }
}