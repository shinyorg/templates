namespace ShinyAspNet.Infrastructure;

public class AuthOptions
{
    public string JwtSecret { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = "ShinyFleet";
    public string JwtAudience { get; set; } = "ShinyFleet";
    public int TokenExpirationMinutes { get; set; } = 60;

    // Microsoft Entra (Azure AD) Configuration
    public string EntraClientId { get; set; } = string.Empty;
    public string EntraTenantId { get; set; } = string.Empty;
}