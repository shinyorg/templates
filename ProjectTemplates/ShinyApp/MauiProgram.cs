using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Prism.Controls;
using Prism.DryIoc;
using ShinyApp;
using UraniumUI;

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseShinyFramework(
                new DryIocContainerExtension(),
                prism => prism.OnAppStart("NavigationPage/MainPage")
            )
            .ConfigureMauiHandlers(handlers =>
            {
                //handlers.AddUraniumUIHandlers();
                //handlers.AddBarcodeScannerHandler();
            })
            .ConfigureFonts(fonts =>
            {
            });

        builder.Configuration.AddJsonPlatformBundle();
        builder.Logging.AddAppCenter(builder.Configuration["AppCenterKey"]);

        RegisterServices(builder.Services);
        RegisterViews(builder.Services);

        return builder.Build();
    }


    static void RegisterServices(IServiceCollection s)
    {
        s.AddGlobalCommandExceptionHandler(new(
#if DEBUG
            ErrorAlertType.FullError
#else
            ErrorAlertType.NoLocalize
#endif
        ));
    }


    static void RegisterViews(IServiceCollection s)
    {
        s.RegisterForNavigation<MainPage, MainViewModel>();
    }
}