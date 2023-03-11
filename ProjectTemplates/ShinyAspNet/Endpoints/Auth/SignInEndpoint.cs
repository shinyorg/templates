using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ShinyAspNet.Endpoints.Auth;


public class SignInEndpoint : Endpoint<SignInRequest>
{
    readonly AppDbContext data;
    readonly JwtService jwtService;
    
    public SignInEndpoint(AppDbContext data, JwtService jwtService)
        => (this.data, this.jwtService) = (data, jwtService);


    public override void Configure()
    {
        this.Get("/signin/{Scheme}");
        this.Group<AuthGroup>();
    }


    public override async Task HandleAsync(SignInRequest req, CancellationToken ct)
    {
#if DEBUG
        if (req.Scheme.StartsWith("HACK:", StringComparison.InvariantCultureIgnoreCase))
        {
            var email = req.Scheme.Split(":")[1];
            var user = await this.data.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                await this.SendForbiddenAsync();
            }
            else
            {
                await this.CreateTokenToApp(user!, false);
            }
            return;
        }
#endif

        var auth = await this.HttpContext.AuthenticateAsync(req.Scheme);

        if (!auth.Succeeded
            || auth?.Principal == null
            || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
            || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
        {
            // Not authenticated, challenge
            await this.HttpContext.ChallengeAsync(req.Scheme);
        }
        else
        {
            var newUser = false;
            var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await data.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                newUser = true;
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email
                };
                data.Users.Add(user);
            }
            user.FirstName = claims!.First(x => x.Type == ClaimTypes.GivenName).Value;
            user.LastName = claims!.First(x => x.Type == ClaimTypes.Surname).Value;
            await this.data.SaveChangesAsync();

            await this.CreateTokenToApp(user, newUser);
        }
    }


    async Task CreateTokenToApp(User user, bool newUser)
    {
        var tokens = await this.jwtService.CreateJwt(user);
        var url = $"myapp://#newuser={newUser}&access_token={tokens.Jwt}&refresh_token={tokens.RefreshToken}";

        await this.SendRedirectAsync(url);
    }
}