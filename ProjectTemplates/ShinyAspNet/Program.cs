using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

//builder
//    .Services
//    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(
//        jwtOpts =>
//        {
//            //jwtOpts.SaveToken = true;
//            //jwtOpts.Authority

//        },
//        msftOpts =>
//        {
//            var cfg = builder.Configuration.GetSection("Authentication:AzureB2C");
//            msftOpts.Instance = cfg["Instance"];
//            msftOpts.TenantId = cfg["TenantId"];
//            msftOpts.ClientId = cfg["ClientId"];
//            msftOpts.ClientSecret = cfg["ClientSecret"];
//        }
//    );
//.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
//.EnableTokenAcquisitionToCallDownstreamApi()
//.AddInMemoryTokenCaches()
//.AddDownstreamWebApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
//.AddInMemoryTokenCaches();


builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        var cfg = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = cfg["ClientId"];
        options.ClientSecret = cfg["ClientSecret"];
    })
    .AddFacebook(options =>
    {
        var cfg = builder.Configuration.GetSection("Authentication:Facebook");
        options.ClientId = cfg["AppId"];
        options.ClientSecret = cfg["AppSecret"];
    });


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(
    opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("Main"))
);

#if DEBUG
ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endif

/app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();