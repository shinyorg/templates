#if debugrainbows
using Plugin.Maui.DebugRainbows;
#endif
#if barcodes
using BarcodeScanning;
#endif
#if usehttp
using Refit;
#endif
#if screenrecord
using Plugin.Maui.ScreenRecording;
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
#if ffimageloading
using FFImageLoading.Maui;
#endif
#if uraniumui
using UraniumUI;
#endif
#if useblazor
#if mudblazor
using MudBlazor.Services;
#endif
#if radzen
using Radzen;
#endif
#endif

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp
        .CreateBuilder()
        .UseMauiApp<App>()
#if virtuallist
        .UseVirtualListView()
#endif
#if barcodes
        .UseBarcodeScanning()
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
#if ffimageloading
        .UseFFImageLoading()
#endif
#if skia || skiaextended
        .UseSkiaSharp()
#endif
#if shinyframework            
        .UseShinyFramework(
            new DryIocContainerExtension(),
            prism => prism.CreateWindow("NavigationPage/MainPage"),
            new (
//-:cnd:noEmit
#if DEBUG
                ErrorAlertType.FullError
#else
                ErrorAlertType.NoLocalize
#endif
//+:cnd:noEmit
            )
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
            .OnAppAction(y => Shiny.Hosting.Host.GetService<ShinyApp.Delegates.AppActionDelegate>()!.Handle(y))
        )
#endif
#if debugrainbows
//-:cnd:noEmit
#if DEBUG
        .UseDebugRainbows(
        //     new DebugRainbowOptions{
        //         ShowRainbows = true,
        //         ShowGrid = true,
        //         HorizontalItemSize = 20,
        //         VerticalItemSize = 20,
        //         MajorGridLineInterval = 4,
        //         MajorGridLines = new GridLineOptions { Color = Color.FromRgb(255, 0, 0), Opacity = 1, Width = 4 },
        //         MinorGridLines = new GridLineOptions { Color = Color.FromRgb(255, 0, 0), Opacity = 1, Width = 1 },
        //         GridOrigin = DebugGridOrigin.TopLeft,
        //     }
        )
#endif
//+:cnd:noEmit  
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
#if useconfig
        builder.Configuration.AddJsonPlatformBundle();
#endif
#if sqlitelogging
        builder.Logging.AddSqlite(Path.Combine(FileSystem.AppDataDirectory, "logging.db"));
#endif
//-:cnd:noEmit
#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddDebug();
#endif
//+:cnd:noEmit
        var s = builder.Services;
#if appaction
        s.AddSingleton<ShinyApp.Delegates.AppActionDelegate>();
#endif
#if (deeplink)
        s.AddShinyService<ShinyApp.Delegates.DeepLinkDelegate>();
#endif
#if (authservice)
#if (usemsal)
        s.AddShinyService<ShinyApp.Services.Impl.MsalAuthenticationService>();
#elif (usewebauthenticator)
        s.AddShinyService<ShinyApp.Services.Impl.WebAuthenticatorAuthService>();
#endif
#endif
#if (usehttp)
        s.AddSingleton(sp =>
        {
            var auth = sp.GetRequiredService<ShinyApp.Services.IAuthenticationService>();
            return RestService.For<ShinyApp.Services.Impl.IApiClient>(
                builder.Configuration["ApiUri"]!,
                new RefitSettings()
                {
                    AuthorizationHeaderValueGetter = async (req, ct) =>
                    {
                        await auth.TryRefresh();
                        return auth.AuthenticationToken!;
                    }
                }
            );
        });
            
#endif
#if essentialsmedia
        s.AddSingleton(MediaPicker.Default);
#endif
#if essentialsfilepicker
        s.AddSingleton(FilePicker.Default);
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
#if httptransfers
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
#if speechrecognition
        s.AddSingleton(CommunityToolkit.Maui.Media.SpeechToText.Default);
#endif
#if (audio)
        s.AddSingleton(Plugin.Maui.AudioManager.Current);
#endif
#if (calendar)
        s.AddSingleton(Plugin.Maui.CalendarStore.CalendarStore.Default);
#endif
#if (screenrecord)
        builder.UseScreenRecording();
        s.AddSingleton(Plugin.Maui.ScreenRecording.ScreenRecording.Default);
#endif
#if inappbilling
        s.AddSingleton(Plugin.InAppBilling.CrossInAppBilling.Current);
#endif
#if storereview
        s.AddSingleton(Plugin.StoreReview.CrossStoreReview.Current);
#endif
#if fingerprint
        s.AddSingleton(sp =>
        {
//-:cnd:noEmit
#if ANDROID
            Plugin.Fingerprint.CrossFingerprint.SetCurrentActivityResolver(() => sp.GetRequiredService<AndroidPlatform>().CurrentActivity);
#endif
//+:cnd:noEmit
            return Plugin.Fingerprint.CrossFingerprint.Current;
        });
#endif
#if userdialogs
        s.AddSingleton(sp => 
        {
//-:cnd:noEmit
#if ANDROID
            Acr.UserDialogs.UserDialogs.Init(() => sp.GetRequiredService<AndroidPlatform>().CurrentActivity);
#endif
//+:cnd:noEmit            
            return Acr.UserDialogs.UserDialogs.Instance;
        });
#endif
#if shinyframework
        s.AddDataAnnotationValidation();
#endif
        return builder;
    }


    static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        var s = builder.Services;

#if useblazor
        s.AddMauiBlazorWebView();

        #if mudblazor
        builder.Services.AddMudServices();
        #endif
        #if radzen
        builder.Services.AddRadzenComponents();
        #endif
//-:cnd:noEmit
#if DEBUG
        s.AddBlazorWebViewDeveloperTools();
#endif
//+:cnd:noEmit

#endif

#if shinyframework
        s.RegisterForNavigation<MainPage, MainViewModel>();
#else
        s.AddTransient<MainPage>();
        s.AddTransient<MainViewModel>();
#endif
        return builder;
    }
}