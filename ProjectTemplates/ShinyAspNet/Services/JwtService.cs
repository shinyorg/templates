using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShinyAspNet.Services;


public class JwtService
{
    readonly int tokenExpiryMins;
    readonly int refreshExpiryHours;
    readonly string issuer;
    readonly string audience;
    readonly string signingKey;
    readonly AppDbContext data;


    public JwtService(IConfiguration cfg, AppDbContext data)
    {
        this.data = data;

        this.tokenExpiryMins = cfg.GetValue<int>("Authentication:Jwt:TokenValidityMinutes", 10);
        this.refreshExpiryHours = cfg.GetValue<int>("Authentication:Jwt:RefreshValidityHours", 720); // 30 days
        this.signingKey = cfg.GetValue<string>("Authentication:Jwt:Key") ?? throw new ArgumentException("No JWT Key set");
        this.issuer = cfg.GetValue<string>("Authentication:Jwt:Issuer") ?? throw new ArgumentException("No JWT Issuer set");
        this.audience = cfg.GetValue<string>("Authentication:Jwt:Audience") ?? throw new ArgumentException("No JWT Issuer set");
    }


    public async Task<(string Jwt, string RefreshToken)> CreateJwt(User user)
    {
        var jwtString = this.CreateJwtString(
            TimeSpan.FromMinutes(this.tokenExpiryMins),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        );
        var rtoken = await this.CreateRefreshToken(user);
        return (jwtString, rtoken);
    }


    public async Task<bool> ValidateRefreshToken(string token)
    {
        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = this.audience,
                ValidIssuer = this.issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.signingKey))
            }, out var fullToken);

            if (fullToken == null)
                return false;

            var storeToken = await this.data
                .RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == token);

            if (storeToken == null)
                return false;

            var idToken = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrWhiteSpace(idToken))
                return false;

            if (idToken != storeToken.UserId.ToString())
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }


    string CreateJwtString(TimeSpan untilExpiry, params Claim[] claims)
    {
        var expiryDate = DateTimeOffset.UtcNow.Add(untilExpiry).UtcDateTime;

        var securityToken = new JwtSecurityToken(
            this.issuer,
            this.audience,
            claims,
            signingCredentials: this.GetJwtKey(),
            expires: expiryDate
        );
        var value = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return value;
    }


    async Task<string> CreateRefreshToken(User user)
    {
        var jwtString = this.CreateJwtString(
            TimeSpan.FromHours(this.refreshExpiryHours),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        );

        this.data.RefreshTokens.Add(new RefreshToken
        {
            Id = jwtString,
            DateCreated = DateTimeOffset.UtcNow,
            UserId = user.Id
        });
        await data.SaveChangesAsync();
        return jwtString;
    }


    SymmetricSecurityKey GetSigningKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.signingKey));
    SigningCredentials GetJwtKey() => new SigningCredentials(this.GetSigningKey(), SecurityAlgorithms.HmacSha256);
}