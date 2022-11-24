#if shinyframework || communitytoolkit
using CommunityToolkit.Maui;
#endif
#if shinyframework
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
#if fingerprint
using Plugin.Fingerprint;
#endif
#if usehttp
using Refit;
#endif

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp
        .CreateBuilder()
        .UseMauiApp<App>()
#if usecsharpmarkup
        .UseMauiCommunityToolkitMarkup()
#endif
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
#if usemaps
        .UseMauiMaps()
#endif
        .ConfigureFonts(fonts =>
        {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); 
        })
        .RegisterInfrastructure()
        .RegisterAppServices()
        .RegisterViews()
        .Build();


    static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder) 
    {
        // register your own services here!
        return builder;
    }


    static MauiAppBuilder RegisterInfrastructure(this MauiAppBuilder builder)
    {
#if useconfig
        builder.Configuration.AddJsonPlatformBundle();
#endif
#if useappcenter
        builder.Logging.AddAppCenter(builder.Configuration["AppCenterKey"]);
#endif

        var s = builder.Services;

#if (audio)
        s.AddSingleton(AudioManager.Current);
#endif
#if (authservice)
#if (usemsal)
        s.AddShinyService<MsalAuthenticationService>();
#elif (usewebauthenticator)
        s.AddShinyService<WebAuthenticatorAuthService>();
#endif
#if (usehttp)
        s.AddTransient<AuthHttpDelegatingHandler>();
        s
            .AddRefitClient<IApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ApiUri"]!))
            .AddHttpMessageHandler<AuthHttpDelegatingHandler>();
            
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
        s.AddSingleton(sp =>
        {
//-:cnd:noEmit
#if ANDROID
            CrossFingerprint.SetCurrentActivityResolver(() => sp.GetRequiredService<AndroidPlatform>().CurrentActivity);
#endif
//+:cnd:noEmit
            return CrossFingerprint.Current;
        });
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
#if (httptransfers)
        s.AddHttpTransfers<MyHttpTransferDelegate>();
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
        return builder;
    }


    static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        var s = builder.Services;

#if useblazor
        s.AddMauiBlazorWebView();
//-:cnd:noEmit
#if DEBUG
        s.AddBlazorWebViewDeveloperTools();
#endif
#endif
//+:cnd:noEmit

#if shinyframework
        s.RegisterForNavigation<MainPage, MainViewModel>();
#else
        s.AddTransient<MainPage>();
        s.AddTransient<MainViewModel>();
#endif
        return builder;
    }
}