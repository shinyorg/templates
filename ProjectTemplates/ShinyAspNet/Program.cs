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


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.AddInfrastructure(typeof(Program).Assembly);

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
app.UseHttpsRedirection();
app.UseInfrastructure();
app.Run();