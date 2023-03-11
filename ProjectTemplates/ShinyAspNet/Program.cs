using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
//#if (signalr)
using ShinyAspNet.Hubs;
//#endif

var builder = WebApplication.CreateBuilder(args);
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

//#if (google)
builder.Services.AddGoogle(options =>
{
    var cfg = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = cfg["ClientId"];
    options.ClientSecret = cfg["ClientSecret"];
});
//#endif
//#if (facebook)
builder.Services.AddFacebook(options =>
{
    var cfg = builder.Configuration.GetSection("Authentication:Facebook");
    options.ClientId = cfg["AppId"];
    options.ClientSecret = cfg["AppSecret"];
});
//#endif
//#if (apple)
builder.Services.AddApple(options =>
{
   options.ClientId = Configuration["Apple:ClientId"];
   options.KeyId = Configuration["Apple:KeyId"];
   options.TeamId = Configuration["Apple:TeamId"];
   options.PrivateKey = (keyId, _) =>
   {
       return Task.FromResult(Configuration[$"Apple:Key:{keyId}"].AsMemory());
   };
});
//#endif

#if (push)
builder.Services.AddPushManagement(x => x
   .AddApplePush(new AppleConfiguration
   {
       IsProduction = true, // prod or sandbox
       TeamId = "Your Team ID",
       AppBundleIdentifier = "com.yourcompany.yourapp",
       Key = "Your Key With NO new lines from Apple Dev Portal",
       KeyId = "The KeyID for your cert"
   })
   .AddGooglePush(new GoogleConfiguration
   {
       SenderId = "Your Firebase Sender ID",
       ServerKey = "Your Firebase Server Key",
       DefaultChannelId = "The Default Channel to use on Android",
       UseShinyAndroidPushIntent = true // this is for Shiny.Push.X v2.5+ if you use it on Xamarin Mobile apps
   })
   .UseEfRepository<AppDbContext>()
);
#endif
#if (email)
builder.Services.AddMailProcessor(x => x
   .UseSmtpSender(Configuration.GetSection("Mail:Smtp").Get<SmtpConfig>())
   .UseSqlServerTemplateLoader(Configuration.GetConnectionString("ConnectionString"))
);
#endif

builder.Services.AddDbContext<AppDbContext>(
    opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("Main"))
);
//#if (signalr)
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
//#endif
builder.Services.AddScoped<JwtService>();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(x =>
{
    x.DocumentName = "v1";
}, shortSchemaNames: true);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    // if (app.Configuration.GetValue<bool>("EnsureDatabase", false))
    // {
    //     using (var scope = app.Services.CreateScope())
    //         scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    // }
    app.UseSwaggerGen();
    app.MapCSharpClientEndpoint("/cs-client", "v1", s =>
    {
        s.ClassName = "ApiClient";
        s.GenerateClientClasses = true;
        s.GenerateBaseUrlProperty = true;
        s.GenerateDtoTypes = true;
        s.GenerateExceptionClasses = true;
        s.CSharpGeneratorSettings.Namespace = "ShinyAspNet.Services";
        s.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson;
        s.CSharpGeneratorSettings.ClassStyle = NJsonSchema.CodeGeneration.CSharp.CSharpClassStyle.Record;
    });
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
//#if (signalr)
app.MapHub<BizHub>("/biz");
//#endif

app.Run();