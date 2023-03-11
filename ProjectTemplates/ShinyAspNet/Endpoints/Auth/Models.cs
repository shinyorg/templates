namespace ShinyAspNet.Endpoints.Auth;

public class RefreshTokenRequest
{
    public string Value { get; set; }
}

public record RefreshTokenResponse(bool Success, string? Jwt, string? RefreshToken)
{
    public static readonly RefreshTokenResponse Fail = new(false, null, null);
    public static RefreshTokenResponse Successful(string jwt, string refresh) => new(true, jwt, refresh);
}

public class SignInRequest
{
    public string Scheme { get; set; }
}

public class SignOutRequest
{
    public string? PushToken { get; set; }
    public string RefreshToken { get; set; }
}