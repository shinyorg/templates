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
#if calendar
using Plugin.Maui.CalendarStore;
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
#if sharpnadocv
using Sharpnado.CollectionView;
#endif
#if sharpnadotabs
using Sharpnado.Tabs;
#endif
#if usegooglemaps
using Maui.GoogleMaps.Hosting;
#endif
#if skia || skiaextended
using SkiaSharp.Views.Maui.Controls.Hosting;
#endif
#if pureween
using Maui.FixesAndWorkarounds;
#endif
#if uraniumui
using UraniumUI;
#endif

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp
        .CreateBuilder()
        .UseMauiApp<App>()
#if barcodes
        .UseBarcodeReader()
#endif
#if uraniumui
        .UseUraniumUI()
        .UseUraniumUIMaterial()
        .UseUraniumUIBlurs()
#endif
#if shinyframework || communitytoolkit
        .UseMauiCommunityToolkit()
#endif
#if mediaelement
        .UseMauiCommunityToolkitMediaElement()
#endif
#if usecsharpmarkup
        .UseMauiCommunityToolkitMarkup()
#endif
#if sharpnadotabs
        .UseSharpnadoTabs(false)
#endif
#if sharpnadocv
        .UseSharpnadoCollectionView(false)
#endif
#if skia || skiaextended
        .UseSkiaSharp()
#endif
#if shinyframework            
        .UseShinyFramework(
            new DryIocContainerExtension(),
            prism => prism.OnAppStart("NavigationPage/MainPage")
        )
#else
        .UseShiny()
#endif
#if usemauimaps
        .UseMauiMaps()
#endif
#if sentry
        .UseSentry(options =>
        {
            // The DSN is the only required setting.
            options.Dsn = "https://examplePublicKey@o0.ingest.sentry.io/0";

            // Use debug mode if you want to see what the SDK is doing.
            // Debug messages are written to stdout with Console.Writeline,
            // and are viewable in your IDE's debug console or with 'adb logcat', etc.
            // This option is not recommended when deploying your application.
            options.Debug = true;

            // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
            // We recommend adjusting this value in production.
            options.TracesSampleRate = 1.0;

            // Other Sentry options can be set here.
        })
#endif
#if usegooglemaps
//-:cnd:noEmit
#if ANDROID
        .UseGoogleMaps()
#elif IOS
        .UseGoogleMaps("YOUR_IOS_GOOGLE_MAPS_KEY")
#endif
//+:cnd:noEmit
#endif
#if appactions
        .ConfigureEssentials(x => x
        //     .AddAppAction("app_info", "App Info", "Subtitle", "app_info_action_icon")
        //     .AddAppAction("battery_info", "Battery Info")
            .OnAppAction(y => Shiny.Hosting.Host.GetService<AppActionDelegate>().Handle(y))
        )
#endif
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
#if uraniumui
            fonts.AddMaterialIconFonts();
#endif
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
#if flipper
//-:cnd:noEmit
#if IOS && DEBUG
        global::Flipper.FlipperProxy.Shared.InitializeProxy();
#endif
//+:cnd:noEmit
#endif
#if pureween
        builder.ConfigureMauiWorkarounds();

        // OR optional picks
        // builder.ConfigureShellWorkarounds();
        // builder.ConfigureTabbedPageWorkarounds();
        // builder.ConfigureEntryNextWorkaround();
        // builder.ConfigureKeyboardAutoScroll();
        // builder.ConfigureFlyoutPageWorkarounds();
//-:cnd:noEmit
        // #if ANDROID
        // builder.ConfigureEntryFocusOpensKeyboard();
        // #endif
//+:cnd:noEmit
#endif
#if useconfig
        builder.Configuration.AddJsonPlatformBundle();
#endif
#if sqlitelogging
        builder.Logging.AddSqlite(Path.Combine(FileSystem.AppDataDirectory, "logging.db"));
#endif
#if useappcenter
//-:cnd:noEmit
#if !MACCATALYST
        builder.Logging.AddAppCenter(builder.Configuration["AppCenterKey"]);
#endif
//+:cnd:noEmit
#endif
//-:cnd:noEmit
#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddDebug();
#endif
//+:cnd:noEmit
        var s = builder.Services;
#if (audio)
        s.AddSingleton(AudioManager.Current);
#endif
#if (calendar)
        s.AddSingleton(CalendarStore.Default);
#endif
#if appaction
        s.AddSingleton<AppActionDelegate>();
#endif
#if (authservice)
#if (usemsal)
        s.AddShinyService<ShinyApp.Services.Impl.MsalAuthenticationService>();
#elif (usewebauthenticator)
        s.AddShinyService<ShinyApp.Services.Impl.WebAuthenticatorAuthService>();
#endif
#endif
#if (usehttp)
        s.AddTransient<ShinyApp.Services.Impl.AuthHttpDelegatingHandler>();
        s
            .AddRefitClient<ShinyApp.Services.Impl.IApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ApiUri"]!))
            .AddHttpMessageHandler<ShinyApp.Services.Impl.AuthHttpDelegatingHandler>();
            
#endif
#if essentialsmedia
        s.AddSingleton(MediaPicker.Default);
#endif
#if essentialsfilepicker
        s.AddSingleton(FilePicker.Default);
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
        s.AddSingleton<ShinyApp.Services.MySqliteConnection>();
#endif
#if jobs
        s.AddJob(typeof(ShinyApp.Delegates.MyJob));
#endif
#if bluetoothle
        s.AddBluetoothLE();
#endif
#if blehosting
        s.AddBluetoothLeHosting();
        s.AddBleHostedCharacteristic<ShinyApp.Delegates.MyBleGattCharacteristic>();
#endif
#if beacons
        s.AddBeaconRanging();
        s.AddBeaconMonitoring<ShinyApp.Delegates.MyBeaconMonitorDelegate>();
#endif
#if gps
        s.AddGps<ShinyApp.Delegates.MyGpsDelegate>();
#endif
#if geofencing
        s.AddGeofencing<ShinyApp.Delegates.MyGeofenceDelegate>();
#endif
#if (httptransfers)
        s.AddHttpTransfers<ShinyApp.Delegates.MyHttpTransferDelegate>();
//-:cnd:noEmit
#if ANDROID
        // if you want http transfers to also show up as progress notifications, include this
        s.AddShinyService<Shiny.Net.Http.PerTransferNotificationStrategy>();
#endif
//+:cnd:noEmit
#endif
#if notifications
        s.AddNotifications<ShinyApp.Delegates.MyLocalNotificationDelegate>();
#endif
#if speechrecognition
        s.AddSpeechRecognition();
#endif
#if usepushnative
        s.AddPush<ShinyApp.Delegates.MyPushDelegate>();
#endif
#if usepushanh
        // TODO: Please make sure to add your proper connection string and hub name to appsettings.json or this will error on startup
        s.AddPushAzureNotificationHubs<ShinyApp.Delegates.MyPushDelegate>(                                   
            builder.Configuration["AzureNotificationHubs:ListenerConnectionString"], 
            builder.Configuration["AzureNotificationHubs:HubName"]
        );
#endif
#if usepushfirebase
        s.AddPushFirebaseMessaging<ShinyApp.Delegates.MyPushDelegate>();
#endif
#if health
//-:cnd:noEmit
#if !MACCATALYST
        s.AddHealthIntegration();
#endif
//+:cnd:noEmit
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
//+:cnd:noEmit

#endif

#if usexaml || usecsharpmarkup
#if shinyframework
        s.RegisterForNavigation<MainPage, MainViewModel>();
#else
        s.AddTransient<MainPage>();
        s.AddTransient<MainViewModel>();
#endif
#endif
        return builder;
    }
}