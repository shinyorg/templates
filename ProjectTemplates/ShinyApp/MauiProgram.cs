#if shinyframework
using CommunityToolkit.Maui;
using Prism.DryIoc;
#endif
#if inappbilling
using Plugin.InAppBilling;
#endif
#if storereview
using Plugin.StoreReview;
#endif
#if barcodes
using ZXing.Net.Maui.Controls;
#endif
#if audio
using Plugin.Maui.Audio;
#endif

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
#if barcodes
            .UseBarcodeReader()
#endif
#if shinyframework || communitytoolkit
            .UseMauiCommunityToolkit()
#endif
#if shinyframework            
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
        RegisterServices(builder);
        RegisterViews(builder.Services);

        return builder.Build();
    }


    static void RegisterServices(MauiAppBuilder builder)
    {
        var s = builder.Services;

#if (audio)
        s.AddSingleton(AudioManager.Current);
#endif
#if (usemsal && msalservice)
        s.AddSingleton<IAuthService, MsalAuthService>();
#endif
#if essentialsmedia
        s.AddSingleton(MediaPicker.Default);
#endif
#if inappbilling
        s.AddSingleton(CrossInAppBilling.Current);
#endif
#if storereview
        s.AddSingleton(CrossStoreReview.Current);
#endif
#if fingerprint
        
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
        s.AddPushAzureNotificationHubs<MyPushDelegate>(new (
            builder.Configuration["AzureNotificationHubs:ListenerConnectionString"], 
            builder.Configuration["AzureNotificationHubs:HubName"]
        ));
#endif
#if usepushfirebase
        s.AddFirebaseMessaging<MyPushDelegate>();
#endif
#if shinyframework
        s.AddDataAnnotationValidation();
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