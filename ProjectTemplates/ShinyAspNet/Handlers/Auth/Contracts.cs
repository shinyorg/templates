namespace ShinyAspNet.Handlers.Auth;

public record SignOutRequest(
    string RefreshToken,
    string? PushToken
) : IRequest;


public record SignInRequest(string Scheme) : IRequest<SignInResponse> { }
public record SignInResponse(bool Success, string? Uri = null)
{
    public static SignInResponse Fail { get; } = new(false);
    public static SignInResponse Sucessful(string uri) => new(true, uri!);
}


public record RefreshRequest(string Token) : IRequest<RefreshResponse>;

public record RefreshResponse(bool Success, string? Jwt, string? RefreshToken)
{
    public static readonly RefreshResponse Fail = new(false, null, null);
    public static RefreshResponse Successful(string jwt, string refresh) => new(true, jwt, refresh);
}