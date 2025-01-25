using System.Text;
using System.Security.Claims;
#if (signalr)
using Microsoft.AspNetCore.SignalR;
#endif
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
#if (orleans)
using Orleans.Configuration;
#endif
#if (efpostgres)
using Npgsql;
#endif
using ShinyAspNet;
using ShinyAspNet.Data;
using ShinyAspNet.Services;
using ShinyAspNet.Services.Impl;
#if (signalr)
using ShinyAspNet.Hubs;
#endif

var builder = WebApplication.CreateBuilder(args);
// .UseServiceProviderFactory(new AutofacServiceProviderFactory())

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

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddShinyMediator();
builder.Services.AddDiscoveredMediatorHandlersFromShinyApp();

#if (swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

#if (efpostgres)
var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Main"));
#if (efspatial)
dataSourceBuilder.UseNetTopologySuite();
#endif
var dataSource = dataSourceBuilder.Build();
#endif

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
//#if (signalr)
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
//#endif

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

app.UseShinyMediatorEndpointHandlers(builder.Services);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

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
if (app.Environment.IsDevelopment())
{
    // if (app.Configuration.GetValue<bool>("EnsureDatabase", false))
    // {
    //     using (var scope = app.Services.CreateScope())
    //         scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    // }
    #if (swagger)
    app.UseSwagger();
    app.UseSwaggerUI();
    #endif
}

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

app
    .MapHealthChecks("/health")
    .RequireHost("*:5001");
    
app.Run();