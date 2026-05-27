using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShinyBlazor;
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

//#if (shinycontrols)
builder.Services.AddShinyBlazorControls();
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
