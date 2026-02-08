//#if (jwtauth)
#if (jwtauth)
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif
#if (jwtauth)
using ShinyAspNet.Services;
using ShinyAspNet.Services.Impl;
#endif
using Shiny;

namespace ShinyAspNet.Infrastructure;

public class AuthModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<JwtService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddAuthorization();

        builder
            .Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = builder.Configuration.GetSection("Authentication:Jwt");
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jwt["Audience"],
                    ValidIssuer = jwt["Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt["Key"]!))
                };
            });

        //#if (google)
        builder.Services.AddAuthentication().AddGoogle(options =>
        {
            var cfg = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = cfg["ClientId"];
            options.ClientSecret = cfg["ClientSecret"];
        });
        //#endif

        //#if (facebook)
        builder.Services.AddAuthentication().AddFacebook(options =>
        {
            var cfg = builder.Configuration.GetSection("Authentication:Facebook");
            options.ClientId = cfg["AppId"];
            options.ClientSecret = cfg["AppSecret"];
        });

        //#endif
        //#if (apple)
        builder.Services.AddAuthentication().AddApple(options =>
        {
            var cfg = builder.Configuration.GetSection("Authentication:Apple");
            options.ClientId = cfg["ClientId"];
            options.TeamId = cfg["TeamId"];
            options.KeyId = cfg["KeyId"];
            options.PrivateKey = (keyId, _) => Task.FromResult(cfg[$"PrivateKey"].AsMemory());
        });
        //#endif
    }

    public void Use(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
//#endif