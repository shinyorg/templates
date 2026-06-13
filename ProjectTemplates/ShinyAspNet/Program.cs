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
#if (usedocumentdb)
using Shiny.DocumentDb;
#if (docdbsqlite)
using Shiny.DocumentDb.Sqlite;
#endif
#if (docdbsqlcipher)
using Shiny.DocumentDb.Sqlite.SqlCipher;
#endif
#if (docdbsqlserver)
using Shiny.DocumentDb.SqlServer;
#endif
#if (docdbmysql)
using Shiny.DocumentDb.MySql;
#endif
#if (docdbpostgres)
using Shiny.DocumentDb.PostgreSql;
#endif
#if (docdbduckdb)
using Shiny.DocumentDb.DuckDb;
#endif
#if (docdbcosmos)
using Shiny.DocumentDb.CosmosDb;
#endif
#if (docdbmongo)
using Shiny.DocumentDb.MongoDb;
#endif
#if (docdblitedb)
using Shiny.DocumentDb.LiteDb;
#endif
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
    opts => opts.UseNpgsql(dataSource, o => o.UseNetTopologySuite())
#else
    opts => opts.UseNpgsql(dataSource)
#endif
#else
#if (efspatial)
    opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("Main"), o => o.UseNetTopologySuite())
#else
    opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("Main"))
#endif
#endif
);
#endif

#if (usedocumentdb)
builder.Services.AddDocumentStore(opts =>
{
#if (docdbsqlite)
    opts.DatabaseProvider = new SqliteDatabaseProvider(builder.Configuration.GetConnectionString("DocumentDb")!);
#endif
#if (docdbsqlcipher)
    opts.DatabaseProvider = new SqlCipherDatabaseProvider(builder.Configuration.GetConnectionString("DocumentDb")!);
#endif
#if (docdbsqlserver)
    opts.DatabaseProvider = new SqlServerDatabaseProvider(builder.Configuration.GetConnectionString("DocumentDb")!);
#endif
#if (docdbmysql)
    opts.DatabaseProvider = new MySqlDatabaseProvider(builder.Configuration.GetConnectionString("DocumentDb")!);
#endif
#if (docdbpostgres)
    opts.DatabaseProvider = new PostgreSqlDatabaseProvider(builder.Configuration.GetConnectionString("DocumentDb")!);
#endif
#if (docdbduckdb)
    opts.DatabaseProvider = new DuckDbDatabaseProvider(builder.Configuration.GetConnectionString("DocumentDb")!);
#endif
#if (docdblitedb)
    // TODO: configure LiteDb file path or stream — see https://shinylib.net/documentdb/litedb/
#endif
    opts.UseReflectionFallback = true;
});

#if (docdbcosmos)
// TODO: configure your Cosmos DB account endpoint, key, and database name — https://shinylib.net/documentdb/cosmosdb/
builder.Services.AddSingleton(new CosmosDbDocumentStoreOptions
{
    AccountEndpoint = builder.Configuration["CosmosDb:AccountEndpoint"]!,
    AccountKey = builder.Configuration["CosmosDb:AccountKey"]!,
    DatabaseName = builder.Configuration["CosmosDb:DatabaseName"] ?? "ShinyAspNet"
});
builder.Services.AddSingleton<IDocumentStore, CosmosDbDocumentStore>();
#endif

#if (docdbmongo)
// TODO: configure your MongoDB connection string and database — https://shinylib.net/documentdb/mongodb/
builder.Services.AddSingleton(new MongoDbDocumentStoreOptions
{
    ConnectionString = builder.Configuration.GetConnectionString("MongoDb")!,
    DatabaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "ShinyAspNet"
});
builder.Services.AddSingleton<IDocumentStore, MongoDbDocumentStore>();
#endif

#if (aidocumentdb)
// AI tools — exposes document store operations to IChatClient
// builder.Services.AddDocumentStoreAITools(tools =>
// {
//     // tools.AddType(jsonContext.SomeType, capabilities: DocumentAICapabilities.ReadOnly);
// });
#endif
#if (documentdbdiagnostics)
// OpenTelemetry metrics + tracing — registered after the store.
// Subscribe from your OTel pipeline with .AddMeter("Shiny.DocumentDb") / .AddSource("Shiny.DocumentDb")
builder.Services.AddDocumentStoreInstrumentation();
#endif
#endif

#if (orleans)
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
        .AddDashboard()
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
#endif

var app = builder.Build();
app.UseHttpsRedirection();
app.UseInfrastructure();
app.Run();
