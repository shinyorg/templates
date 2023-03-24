namespace ShinyAspNet.Endpoints.Auth;

public record RefreshTokenRequest(string Value);

public record RefreshTokenResponse(bool Success, string? Jwt, string? RefreshToken)
{
    public static readonly RefreshTokenResponse Fail = new(false, null, null);
    public static RefreshTokenResponse Successful(string jwt, string refresh) => new(true, jwt, refresh);
}

public record SignInRequest(string Scheme);