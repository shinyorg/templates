namespace ShinyAspNet.Handlers.Auth;


[MediatorHttpPost(
    "RefreshAuth",
    "/auth/signin/refresh", 
    RequiresAuthorization = true
)]
public class RefreshHandler(AppDbContext data, JwtService jwtService) : IRequestHandler<RefreshRequest, RefreshResponse>
{
    public async Task<RefreshResponse> Handle(RefreshRequest request, IMediatorContext context, CancellationToken cancellationToken)
    {
        var result = await jwtService
            .ValidateRefreshToken(request.Token, cancellationToken)
            .ConfigureAwait(false);
            
        if (!result)
            return RefreshResponse.Fail;

        var refreshToken = await data
            .RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.Token, cancellationToken);

        var tokens = await jwtService.CreateJwt(refreshToken.User, cancellationToken);

        return RefreshResponse.Successful(tokens.Jwt, tokens.RefreshToken);
    }
}