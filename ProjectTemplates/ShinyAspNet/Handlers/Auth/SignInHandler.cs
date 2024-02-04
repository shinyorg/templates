using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ShinyAspNet.Handlers.Auth;


public record SignInRequest(string Scheme) : IRequest<SignInResponse> { }
public record SignInResponse(bool Success, string? Uri = null)
{
    public static SignInResponse Fail { get; } = new(false);
    public static SignInResponse Sucessful(string uri) => new(true, uri!);
}

public class SignInHandler(AppDbContext data, JwtService jwtService, IHttpContextAccessor httpAccessor) : IRequestHandler<SignInRequest, SignInResponse>
{
    public async Task<SignInResponse> Handle(SignInRequest request, CancellationToken cancellationToken)
    {
#if DEBUG
        if (request.Scheme.StartsWith("HACK:", StringComparison.InvariantCultureIgnoreCase))
        {
            var email1 = request.Scheme.Split(":")[1];
            var user1 = await data.Users.FirstOrDefaultAsync(x => x.Email == email1);

            if (user1 == null)
                return SignInResponse.Fail;

            var uritest = await this.CreateTokenToApp(user1!, false);
            return SignInResponse.Sucessful(uritest);
        }
#endif
        var auth = await httpAccessor.HttpContext!.AuthenticateAsync(request.Scheme);

        if (!auth.Succeeded
            || auth?.Principal == null
            || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
            || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))

            return SignInResponse.Fail;

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
        await data.SaveChangesAsync();

        var uri = await this.CreateTokenToApp(user, newUser);
        return SignInResponse.Sucessful(uri);
    }


    async Task<string> CreateTokenToApp(User user, bool newUser)
    {
        var tokens = await jwtService.CreateJwt(user);
        var url = $"myapp://#newuser={newUser}&access_token={tokens.Jwt}&refresh_token={tokens.RefreshToken}";
        
        return url;
    }
}