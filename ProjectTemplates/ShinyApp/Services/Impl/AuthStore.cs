using Shiny.Extensions.Stores;

namespace ShinyApp.Services.Impl;

public partial class AuthStore
{
    [Bind(StoreKeys.Secure)]
    public partial string? AuthenticationToken { get; set; }

    [Bind(StoreKeys.Secure)]
    public partial string? RefreshToken { get; set; }

    [Bind(StoreKeys.Secure)]
    public partial DateTimeOffset? Expiration { get; set; }

    public void Reset()
    {
        this.AuthenticationToken = null;
        this.RefreshToken = null;
        this.Expiration = null;
    }
}