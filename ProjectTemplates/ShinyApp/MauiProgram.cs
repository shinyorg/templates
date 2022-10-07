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
#if startup
        s.AddShinyService<AppStartup>();
#endif
#if settings
        s.AddShinyService<AppSettings>();
#endif
#if sqlite
        s.AddSingleton<MySqliteConnection>();
#endif
#if jobs
        // TODO: additional configuration in the signature
        s.AddJob(typeof(MyJob));
#endif
#if bluetoothle
        s.AddBluetoothLE();
#endif
#if blehosting
        s.AddBluetoothLEHosting();
#endif
#if beacons
        s.AddBeaconRanging();
        s.AddBeaconMonitoring<MyBeaconDelegate>();
#endif
#if gps
        s.AddGps<MyGpsDelegate>();
#endif
#if geofencing
        s.AddGeofencing<MyGeofenceDelegate>();
#endif
#if motionactivity
        s.AddMotionActivity();
#endif
#if usepushnative
        s.AddPush<MyPushDelegate>();
#endif
#if usepushanh
        // TODO: configure
        s.AddPushAzureNotificationHubs<MyPushDelegate>(new ("YourApiKey", "YourHubName"));
#endif
#if usepushfirebase
        // TODO: configure
        s.AddFirebaseMessaging<MyPushDelegate>();
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