using CommunityToolkit.Maui;
using Prism.DryIoc;

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
            .ConfigureFonts(fonts =>
            {
            });

#if useconfig
        builder.Configuration.AddJsonPlatformBundle();
#if useappcenter
        builder.Logging.AddAppCenter(builder.Configuration["AppCenterKey"]);
#endif
#endif
        RegisterServices(builder.Services);
        RegisterViews(builder.Services);

        return builder.Build();
    }


    static void RegisterServices(IServiceCollection s)
    {
        // TODO: http transfers with delegate
#if bluetoothle
        s.AddBluetoothLE();
#endif
#if blehosting
        s.AddBluetoothLEHosting();
#endif
#if beacons
        s.AddBeaconRanging();
        // s.AddBeaconMonitoring<MyBeaconDelegate>();  // TODO
#endif
#if usepushnative
        s.AddPush<MyPushDelegate>(); // TODO: firebase config?
#endif
#if usepushanh
        // TODO: need config
        s.AddPushAzureNotificationHubs<MyPushDelegate>(new ("", ""));
#endif
#if usefirebase
        s.AddFirebaseMessaging<MyPushDelegate>(); // TODO: firebase config?
#endif
#if notifications
        s.AddNotifications();
#endif
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