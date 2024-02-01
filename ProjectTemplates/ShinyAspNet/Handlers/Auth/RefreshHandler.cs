namespace ShinyAspNet.Handlers.Auth;


public record RefreshRequest(string Token) : IRequest<RefreshResponse>;

public record RefreshResponse(bool Success, string? Jwt, string? RefreshToken)
{
    public static readonly RefreshResponse Fail = new(false, null, null);
    public static RefreshResponse Successful(string jwt, string refresh) => new(true, jwt, refresh);
}


public class RefreshHandler(AppDbContext data, JwtService jwtService) : IRequestHandler<RefreshRequest, RefreshResponse>
{
    public async Task<RefreshResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await jwtService.ValidateRefreshToken(request.Token);
        if (!result)
            return RefreshResponse.Fail;

        var refreshToken = await data
            .RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.Token);

        var tokens = await jwtService.CreateJwt(refreshToken.User);

        return RefreshResponse.Successful(tokens.Jwt, tokens.RefreshToken);
    }
}