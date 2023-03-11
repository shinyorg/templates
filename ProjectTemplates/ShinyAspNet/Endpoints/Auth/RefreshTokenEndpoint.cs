namespace ShinyAspNet.Endpoints.Auth;


public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, RefreshTokenResponse>
{
    readonly JwtService jwt;
    readonly AppDbContext data;
    public RefreshTokenEndpoint(JwtService jwt, AppDbContext data)
	    => (this.jwt, this.data) = (jwt, data);


    public override void Configure()
    {
        this.Post("/refresh");
        this.AllowAnonymous();
        this.Group<AuthGroup>();
    }


    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var result = await this.jwt.ValidateRefreshToken(req.Value);
        if (!result)
        {
            await this.SendOkAsync(RefreshTokenResponse.Fail);
            return;
        }

        var refreshToken = await this.data
            .RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == req.Value);

        var tokens = await this.jwt.CreateJwt(refreshToken!.User);
        this.data.RefreshTokens.Remove(refreshToken);
        await this.data.SaveChangesAsync();

        await this.SendOkAsync(RefreshTokenResponse.Successful(tokens.Jwt, tokens.RefreshToken));
    }
}