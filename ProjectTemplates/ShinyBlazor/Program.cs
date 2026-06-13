using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShinyBlazor;
//#if (documentdb)
using Shiny.DocumentDb;
using Shiny.DocumentDb.IndexedDb;
//#endif
//#if (blazorhost)
using Shiny;
//#endif
//#if (mudblazor)
using MudBlazor.Services;
//#endif
//#if (radzen)
using Radzen;
//#endif
//#if (fluentui)
using Microsoft.FluentUI.AspNetCore.Components;
//#endif

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//#if (shinymediator)
builder.Services.AddShinyMediator(cfg => cfg.UseBlazor());
//#endif

//#if (blazorhost)
// Device/browser info + culture & time-zone change notifications (IAppSupport).
// Requires the shiny-appsupport.js script in wwwroot/index.html.
builder.Services.AddAppSupport(ThisAssembly.AssemblyVersion);
//#endif

//#if (shinycontrols)
builder.Services.AddShinyBlazorControls();
//#endif

//#if (shinykiosk)
builder.Services.AddShinyDocking();
//#endif

//#if (bluetoothle)
builder.Services.AddBluetoothLE();
//#endif

//#if (gps)
builder.Services.AddGps();
//#endif

//#if (push)
// TODO: replace ApplicationServerKey with your VAPID public key — see https://shinylib.net/client/push/
builder.Services.AddPush(new Shiny.Push.WebPushOptions
{
    ApplicationServerKey = "REPLACE_WITH_VAPID_PUBLIC_KEY"
});
//#endif

//#if (useshinyspeech)
builder.Services.AddSpeechServices();
//#endif

//#if (aiconversation)
// TODO: register an IChatClient or IChatClientProvider before this — see https://shinylib.net/aiconversation/
builder.Services.AddShinyAiConversation(opts =>
{
    // opts.SetChatClientProvider<MyChatClientProvider>();
    // opts.SetMessageStore<MyMessageStore>(addAiLookupTool: true); // optional
});
//#endif

//#if (documentdb)
var documentDbOptions = new IndexedDbDocumentStoreOptions
{
    DatabaseName = "ShinyBlazor",
    UseReflectionFallback = true
};
builder.Services.AddSingleton(documentDbOptions);
builder.Services.AddSingleton<IDocumentStore, IndexedDbDocumentStore>();
//#if (documentdbdiagnostics)
// OpenTelemetry metrics + tracing — registered after the store.
// Subscribe from your OTel pipeline with .AddMeter("Shiny.DocumentDb") / .AddSource("Shiny.DocumentDb")
builder.Services.AddDocumentStoreInstrumentation();
//#endif
//#endif

//#if (stores)
builder.Services.AddShinyStores();
//#endif

//#if (mudblazor)
builder.Services.AddMudServices();
//#endif

//#if (radzen)
builder.Services.AddRadzenComponents();
//#endif

//#if (fluentui)
builder.Services.AddFluentUIComponents();
//#endif

await builder.Build().RunAsync();
