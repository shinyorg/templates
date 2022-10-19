#if shinyframework
using CommunityToolkit.Maui;
using Prism.DryIoc;
#endif

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
#if shinyframework            
            .UseMauiCommunityToolkit()
            .UseShinyFramework(
                new DryIocContainerExtension(),
                prism => prism.OnAppStart("NavigationPage/MainPage")
            )
#else
            .UseShiny()
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); 
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
#if usemsal && msalservice
        s.AddSingleton<IAuthService, MsalAuthService>();
#endif
#if essentialsmedia
        s.AddSingleton(MediaPicker.Default);
#endif
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
#if notifications
        s.AddNotifications();
#endif
#if speechrecognition
        s.AddSpeechRecognition();
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

#if shinyframework
        s.AddGlobalCommandExceptionHandler(new(
//-:cnd:noEmit                
#if DEBUG
            ErrorAlertType.FullError
#else
            ErrorAlertType.NoLocalize
#endif
//+:cnd:noEmit
        ));
#endif
    }


    static void RegisterViews(IServiceCollection s)
    {
#if shinyframework
        s.RegisterForNavigation<MainPage, MainViewModel>();
#else
        s.AddTransient<MainPage>();
        s.AddTransient<MainViewModel>();
#endif
    }
}