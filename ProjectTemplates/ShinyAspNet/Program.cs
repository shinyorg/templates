using System.Text;
#if (scalar)
using Scalar.AspNetCore;
#endif
#if (jwtauth)
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif
#if (signalr)
using Microsoft.AspNetCore.SignalR;
#endif
#if (otel)
using Microsoft.Extensions.FileProviders;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
#endif
#if (orleans)
using Orleans.Configuration;
#endif
#if (efpostgres)
using Npgsql;
#endif
using ShinyAspNet;
#if (!efnone)
using ShinyAspNet.Data;
#endif
#if (jwtauth)
using ShinyAspNet.Services;
using ShinyAspNet.Services.Impl;
#endif
#if (signalr)
using ShinyAspNet.Hubs;
#endif

var builder = WebApplication.CreateBuilder(args);
// .UseServiceProviderFactory(new AutofacServiceProviderFactory())

//#if (otel)
builder.Logging.AddOpenTelemetry(options =>
{
    // options.SetResourceBuilder(ResourceBuilder
    //     .CreateEmpty()
    //     .AddService(builder.Environment.ApplicationName)
    //     .AddAttributes(new Dictionary<string, object>
    //     {
    //         ["environment"] = builder.Environment.EnvironmentName
    //     })
    // );
    options.AddConsoleExporter();
    options.AddOtlpExporter(x => 
    {
        x.Endpoint = new Uri("http://localhost:4317/ingest/otlp/v1/logs");
        x.Protocol = OtlpExportProtocol.HttpProtobuf;
        x.Headers = "X-Seq-ApiKey=123";
    });
});

builder
    .Services
    .AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        // metrics
        //     .AddPrometheusExporter()
        //     .AddMeter("Microsoft.Orleans");
    })
    .WithTracing(tracing =>
    {
        var service = ResourceBuilder.CreateDefault().AddService("WorkflowSystem", "1.0");
        tracing.SetResourceBuilder(service);
        
        tracing.AddSource("Microsoft.Orleans.Runtime");
        tracing.AddSource("Microsoft.Orleans.Application");
        
        // tracing.AddZipkinExporter(zipkin =>
        // {
        //     zipkin.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
        // });
    });
//#endif
//#if (jwtauth)
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
//#endif
builder.Services.AddHttpContextAccessor();
builder.Services.AddShinyMediator();
builder.Services.AddDiscoveredMediatorHandlersFromShinyApp();

#if (efpostgres)
var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Main"));
#if (efspatial)
dataSourceBuilder.UseNetTopologySuite();
#endif
var dataSource = dataSourceBuilder.Build();
#endif

#if (!efnone)
builder.Services.AddDbContext<AppDbContext>(
#if (efpostgres)
#if (efspatial) 
    opts => opts.UseNpgsql(dataSource)
#else
    opts => opts.UseNpgsql(dataSource, o => o.UseNetTopologySuite())
#endif
#else
    opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("Main"))
#endif
);
#endif
builder.Services.AddOpenApi();
//#if (signalr)
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
//#endif
//#if (jwtauth)
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
//#endif

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
//#if (orleans)
builder.Host.UseOrleans((ctx, silo) =>
{
    // In order to support multiple hosts forming a cluster, they must listen on different ports.
    // Use the --InstanceId X option to launch subsequent hosts.
    
    // silo.AddMemoryStreams("StreamProvider");
    // silo.AddMemoryGrainStorage("MemoryStore");
    // var instanceId = ctx.Configuration.GetValue<int>("InstanceId");
    // silo.UseLocalhostClustering(
    //     siloPort: 11111 + instanceId,
    //     gatewayPort: 30000 + instanceId,
    //     primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11111)
    // );
    
    silo
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "";
            options.ServiceId = "";
        })
        .UseDashboard(x => {})
        .UseAdoNetReminderService(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("Orleans"); 
            options.Invariant = Constants.DatabaseInvariant;
        })
        .UseAdoNetClustering(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
            // options.Invariant = Constants.DatabaseInvariant;
        })
        .AddAdoNetGrainStorage("Default", options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
            //options.Invariant = Constants.DatabaseInvariant;
            // options.UseJsonFormat = true;
        })
        .AddAdoNetStreams("Default", options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
            //options.Invariant = Constants.DatabaseInvariant;
        });
});
//#endif

var app = builder.Build();

#if (jwtauth)
app.MapShinyMediatorEndpointHandlers(builder.Services);
#else
// uncomment when you have handlers with [ScopedHandler]
// app.MapShinyMediatorEndpointHandlers(builder.Services);
#endif
app.UseHttpsRedirection();
//#if (jwtauth)
app.UseAuthentication();
app.UseAuthorization();
//#endif
//#if (signalr)
app.MapHub<BizHub>("/biz");
//#endif

//-:cnd:noEmit
#if DEBUG
// easier debugging
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    // .WithOrigins("https://localhost:1234")
    //.AllowCredentials()
);
#endif
//+:cnd:noEmit
#if (swagger)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
#endif
#if (deeplinks)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/.well-known")
    ),
    RequestPath = "/.well-known",
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});
#endif

#if (efmssql || efpostgres)
app
    .MapHealthChecks("/health")
    .RequireHost("*:5001");
#endif

app.Run();