using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
//#if (signalr)
using ShinyAspNet.Hubs;
using Microsoft.AspNetCore.SignalR;
//#endif
#if (swagger)
using FastEndpoints.Swagger;
using FastEndpoints.ClientGen;
#endif
#if (push)
using Shiny.Extensions.Push;
#endif
#if (email)
using Shiny.Extensions.Mail;
#endif

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

#if (push)
var appleCfg = builder.Configuration.GetSection("Push:Apple");
var googleCfg = builder.Configuration.GetSection("Push:Google");

builder.Services.AddPushManagement(x => x
    .AddApple(new AppleConfiguration
    {
        AppBundleIdentifier = appleCfg["AppBundleIdentifier"]!,
        TeamId = appleCfg["TeamId"]!,
        Key = appleCfg["Key"]!,
        KeyId = appleCfg["KeyId"]!,
        IsProduction = false
        //JwtExpiryMinutes
    })
    .AddGoogleFirebase(new GoogleConfiguration
    {
        ServerKey = googleCfg["ServerKey"]!,
        SenderId = googleCfg["SenderId"]!,
        DefaultChannelId = googleCfg["DefaultChannelId"]!
    })
    .UseAdoNetRepository<Microsoft.Data.SqlClient.SqlConnection>(new DbRepositoryConfig(
        builder.Configuration.GetConnectionString("Main"),
        "@",
        "PushRegistrations",
        true
    ))
    .AddShinyAndroidClickAction()
);

#endif
#if (email)
builder.Services.AddMail(mail =>
{
    var cfg = builder.Configuration.GetSection("Mail");
    mail
        .UseSmtpSender(new SmtpConfig
        {
            EnableSsl = cfg.GetValue("EnableSsl", true),
            Host = cfg["Host"],
            Port = cfg.GetValue("Port", 587)
        })
        //.UseSendGridSender("SendGridApiKey")
        //.UseFileTemplateLoader("File Path to templates")
        .UseAdoNetTemplateLoader<Microsoft.Data.SqlClient.SqlConnection>(
            builder.Configuration.GetConnectionString("Main")!,
            "@",
            "MailTemplates",
            true
        );
});
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
//#if (swagger)
builder.Services.AddSwaggerDoc(x =>
{
    x.DocumentName = "v1";
}, shortSchemaNames: true);
//#endif
//#if (olreans)
builder.Host.UseOrleans(x => x
    .UseLocalhostClustering()
    .AddBroadcastChannel("myapp", cfg => cfg.FireAndForgetDelivery = true)
    .AddMemoryGrainStorage("myapp")
);
//#endif

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//#if (push)
app.MapPushEndpoints("push", true, x => x.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
//#endif
app.UseFastEndpoints();

//#if (signalr)
app.MapHub<BizHub>("/biz");
//#endif

#if DEBUG
// easier debugging
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    //.AllowCredentials()
);
#endif

if (app.Environment.IsDevelopment())
{
    // if (app.Configuration.GetValue<bool>("EnsureDatabase", false))
    // {
    //     using (var scope = app.Services.CreateScope())
    //         scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    // }
    #if (swagger)
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
    #endif
}

app.Run();