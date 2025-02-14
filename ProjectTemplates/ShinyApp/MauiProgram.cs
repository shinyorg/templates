#if userdialogs
using Acr.UserDialogs;
#endif
#if debugrainbows
using Plugin.Maui.DebugRainbows;
#endif
#if screenrecord
using Plugin.Maui.ScreenRecording;
#endif
#if ocr
using Plugin.Maui.OCR;
#endif
#if fingerprint
using Maui.Biometric;
#endif
#if barcodes
using BarcodeScanning;
#endif
#if usegooglemaps
using Maui.GoogleMaps.Hosting;
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
#if settingsview
using AiForms.Settings;
#endif
#if cards
using PanCardView;
#endif
#if skia || skiaextended || livecharts
using SkiaSharp.Views.Maui.Controls.Hosting;
#endif
#if ffimageloading
using FFImageLoading.Maui;
#endif
#if uraniumui
using UraniumUI;
#endif
#if bottomsheet
using The49.Maui.BottomSheet;
#endif
#if contextmenu
using The49.Maui.ContextMenu;
#endif
#if camera
using Camera.MAUI;
#endif
#if communitytoolkit || mediaelement
using CommunityToolkit.Maui;
#endif
#if useblazor
#if mudblazor
using MudBlazor.Services;
#endif
#if radzen
using Radzen;
#endif
#if fluentui
using Microsoft.FluentUI.AspNetCore.Components;
#endif
#endif
#if shinymediator
using Polly;
using Polly.Retry;
#endif
using Microsoft.Extensions.Diagnostics.Enrichment;

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
            .UseShiny()
#if userdialogs
            .UseUserDialogs()
#endif
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
#if communitytoolkit
            .UseMauiCommunityToolkit()
#endif
#if mediaelement
            .UseMauiCommunityToolkitMediaElement()
#endif
#if usecsharpmarkup
            .UseMauiCommunityToolkitMarkup()
#endif
#if cameraview
            .UseMauiCommunityToolkitCamera()
#endif
#if bottomsheet
            .UseBottomSheet()
#endif
#if contextmenu
            .UseContextMenu()
#endif
#if camera
            .UseMauiCameraView()
#endif
#if fingerprint
            .UseBiometricAuthentication()
#endif
#if (settingsview)
            .UseSettingsView()
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
#if skia || skiaextended || livecharts
            .UseSkiaSharp()
#endif
#if prism
            .UsePrism(
                new DryIocContainerExtension(),
                prism => prism.CreateWindow("NavigationPage/MainPage")                
            )
#endif
#if screenrecord
            .UseScreenRecording()
#endif
#if usemauimaps
            .UseMauiMaps()
#endif
#if cards
            .UseCardsView()
#endif
#if ocr
            .UseOcr()
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
            });
#if useconfig
        builder.Configuration.AddJsonPlatformBundle();
#endif
#if (remoteconfig)
        builder.AddRemoteConfigurationMaui("https://todo");
        // builder.Services.AddOptions<MyConfig>().BindConfiguration("");
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
#if useblazor
        builder.Services.AddMauiBlazorWebView();

        #if mudblazor
        builder.Services.AddMudServices();
        #endif
        #if radzen
        builder.Services.AddRadzenComponents();
        #endif
        #if fluentui
        builder.Services.AddFluentUIComponents();
        #endif
//-:cnd:noEmit
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
//+:cnd:noEmit
#endif
        builder.Services.AddConnectivity();
        builder.Services.AddBattery();
#if shinymediator
        // pass false as second argument if you don't want to use built-in middleware
        builder.Services.AddShinyMediator(x => x 
            .AddMauiPersistentCache()
            .AddDataAnnotations()
            .AddConnectivityBroadcaster()
            .AddResiliencyMiddleware(
                ("Default", pipeline =>
                {
                    pipeline.AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 2,
                        MaxDelay = TimeSpan.FromSeconds(1.0),
                    });
                    pipeline.AddTimeout(TimeSpan.FromSeconds(5));
                })
            )
            .UseMaui() // pass false here if you don't want to use built-in offline support with MAUI
#if prism
            .AddPrismSupport()
#endif
#if useblazor
            .UseBlazor()
#endif
        );
        builder.Services.AddSingleton(
            typeof(Shiny.Mediator.Http.IHttpRequestDecorator<,>),
            typeof(ShinyApp.Infrastructure.AppHttpRequestDecorator<,>)
        );
#endif
#if appaction
        builder.Services.AddSingleton<ShinyApp.Delegates.AppActionDelegate>();
#endif
#if (deeplink)
        builder.Services.AddShinyService<ShinyApp.Delegates.DeepLinkDelegate>();
#endif
#if (authservice)
#if (usemsal)
        builder.Services.AddShinyService<ShinyApp.Services.Impl.MsalAuthenticationService>();
#elif (usewebauthenticator)
        builder.Services.AddShinyService<ShinyApp.Services.Impl.WebAuthenticatorAuthService>();
#endif
#endif
#if (usehttp)
        builder.Services.AddSingleton(sp =>
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
#if ocr
        builder.Services.AddSingleton(OcrPlugin.Default);
#endif
#if essentialsmedia
        builder.Services.AddSingleton(MediaPicker.Default);
#endif
#if essentialsfilepicker
        builder.Services.AddSingleton(FilePicker.Default);
#endif
#if startup
        builder.Services.AddShinyService<AppStartup>();
#endif
#if settings
        builder.Services.AddShinyService<AppSettings>();
#endif
#if sqlite
        builder.Services.AddSingleton<ShinyApp.Services.MySqliteConnection>();
#endif
#if jobs
        builder.Services.AddJob(typeof(ShinyApp.Delegates.MyJob));
#endif
#if bluetoothle
        builder.Services.AddBluetoothLE();
#endif
#if blehosting
        builder.Services.AddBluetoothLeHosting();
        builder.Services.AddBleHostedCharacteristic<ShinyApp.Delegates.MyBleGattCharacteristic>();
#endif
#if beacons
        builder.Services.AddBeaconRanging();
        builder.Services.AddBeaconMonitoring<ShinyApp.Delegates.MyBeaconMonitorDelegate>();
#endif
#if gps
        builder.Services.AddGps<ShinyApp.Delegates.MyGpsDelegate>();
#endif
#if geofencing
        builder.Services.AddGeofencing<ShinyApp.Delegates.MyGeofenceDelegate>();
#endif
#if httptransfers
        builder.Services.AddHttpTransfers<ShinyApp.Delegates.MyHttpTransferDelegate>();
//-:cnd:noEmit
#if ANDROID
        // if you want http transfers to also show up as progress notifications, include this
        builder.Services.AddShinyService<Shiny.Net.Http.PerTransferNotificationStrategy>();
#endif
//+:cnd:noEmit
#endif
#if notifications
        builder.Services.AddNotifications<ShinyApp.Delegates.MyLocalNotificationDelegate>();
#endif
#if usepushnative
        builder.Services.AddPush<ShinyApp.Delegates.MyPushDelegate>();
#endif
#if usepushanh
        // TODO: Please make sure to add your proper connection string and hub name to appsettings.json or this will error on startup
        builder.Services.AddPushAzureNotificationHubs<ShinyApp.Delegates.MyPushDelegate>(                                   
            builder.Configuration["AzureNotificationHubs:ListenerConnectionString"], 
            builder.Configuration["AzureNotificationHubs:HubName"]
        );
#endif
#if usepushfirebase
        builder.Services.AddPushFirebaseMessaging<ShinyApp.Delegates.MyPushDelegate>();
#endif
#if speechrecognition
        builder.Services.AddSingleton(CommunityToolkit.Maui.Media.SpeechToText.Default);
#endif
#if (screenrecord)
        builder.Services.AddSingleton(ScreenRecording.Default);
#endif
#if (audio)
        builder.Services.AddSingleton(Plugin.Maui.Audio.AudioManager.Current);
#endif
#if (calendar)
        builder.Services.AddSingleton(Plugin.Maui.CalendarStore.CalendarStore.Default);
#endif
#if (screenbrightness)
        builder.Services.AddSingleton(Plugin.Maui.ScreenBrightness.ScreenBrightness.Default);
#endif
#if inappbilling
        builder.Services.AddSingleton(Plugin.InAppBilling.CrossInAppBilling.Current);
#endif
#if storereview
        builder.Services.AddSingleton(Plugin.StoreReview.CrossStoreReview.Current);
#endif

#if (prism)
        builder.Services.AddScoped<BaseServices>();
        builder.Services.RegisterForNavigation<MainPage, MainViewModel>();
#else
        builder.Services.AddSingleton<BaseServices>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();
#endif
        builder.Logging.EnableEnrichment();
        builder.Services.AddSingleton<ILogEnricher, ShinyApp.Infrastructure.AppLogEnricher>();
        builder.Services.AddSingleton(TimeProvider.System);
        var app = builder.Build();

        return app;
    }
}