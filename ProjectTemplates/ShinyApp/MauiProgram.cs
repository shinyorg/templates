//-:cnd:noEmit
#if PLATFORM
//+:cnd:noEmit
#if userdialogs
using Acr.UserDialogs;
#endif
#if debugrainbows
//-:cnd:noEmit
#if DEBUG
using Plugin.Maui.DebugRainbows;
#endif
//+:cnd:noEmit
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
#if usemauicontrols || music || contactstore
using Shiny;
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
#if useshinymediator
using Polly;
using Polly.Retry;
#endif
#if usedocumentdb || documentdbdiagnostics
using Shiny.DocumentDb;
#endif
#if usedocumentdb
using Shiny.DocumentDb.Sqlite;
#endif
#if aihealth
using Shiny.Health.Extensions.AI;
#endif
using ShinyApp.Services;

namespace ShinyApp;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
#if useconfig
        builder.Configuration.AddJsonPlatformBundle();
#endif

        builder    
            .UseMauiApp<App>()
            .UseShiny()
#if useshinymediator
            // pass false as second argument if you don't want to use built-in middleware
            .AddShinyMediator(x => x
                .AddMediatorRegistry() // optional - enables registration scanning
                .AddMauiPersistentCache()
                .AddDataAnnotations()
                .AddConnectivityBroadcaster()
#if prism
                .AddPrismSupport()
#endif
#if useblazor
                .UseBlazor()
#endif
#if aimediator
                // .AddGeneratedAITools() // uncomment once you have AI tool contracts - https://shinylib.net/mediator/extensions/ai/
#endif
            )
#endif
#if shinyshell
            .UseShinyShell(x => x
                //.AddGeneratedMaps() // uncomment once you have shellmaps
#if aishinyshell
                // .AddAiTools() // uncomment once you have shell maps with AI descriptions - https://shinylib.net/mauishell/ai/
#endif
#if uxdiversdialogs
                .UseUxDiversDialogs()
#endif
            )
#endif
#if usemauicontrols
            .UseShinyControls(cfg =>
            {
                cfg.AddDefaultMauiControlFeedback();
            })
#endif
#if shinydesktopcontrols
            .UseShinyDesktopControls()
#endif
#if devflow
//-:cnd:noEmit
#if DEBUG
            .UseMauiDevFlow()
#endif
//+:cnd:noEmit
#endif
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
#if fingerprint
            .UseBiometricAuthentication()
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

        // TODO: once you have localization, add these
        //builder.Services.AddLocalization();
        //builder.Services.AddStronglyTypedLocalizations();
        builder.Services.AddSingleton(TimeProvider.System);
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
#if appaction
        builder.Services.AddSingleton<ShinyApp.Delegates.AppActionDelegate>();
#endif
#if (usedeeplinks)
        builder.Services.AddShinyService<ShinyApp.Delegates.DeepLinkDelegate>();
#endif
#if (authservice)
        // AuthStore persists each property via [Bind(StoreKeys.Secure)] - register it like any other service
        builder.Services.AddSingleton<ShinyApp.Services.Impl.AuthStore>();
#if (usemsal)
        builder.Services.AddShinyService<ShinyApp.Services.Impl.MsalAuthenticationService>();
#elif (usewebauthenticator)
        builder.Services.AddShinyService<ShinyApp.Services.Impl.WebAuthenticatorAuthService>();
#endif
#endif
#if (usehttp)
        builder.Services.AddSingleton(sp =>
        {
#if (authservice)
            var auth = sp.GetRequiredService<ShinyApp.Services.IAuthenticationService>();
#endif
            return RestService.For<ShinyApp.Services.Impl.IApiClient>(
                builder.Configuration["ApiUri"]!,
                new RefitSettings()
                {
#if (authservice)
                    AuthorizationHeaderValueGetter = async (req, ct) =>
                    {
                        await auth.TryRefresh();
                        return auth.AuthenticationToken!;
                    }
#endif
                }
            );
        });
#endif
#if sqlite
//-:cnd:noEmit
#if ANDROID
        // required for sqlite android 16kb fix
        SQLitePCL.Batteries_V2.Init();
#endif
//+:cnd:noEmit
#endif
#if usedocumentdb
        builder.Services.AddDocumentStore(opts =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "docstore.db");
            opts.DatabaseProvider = new SqliteDatabaseProvider($"Data Source={dbPath}");
            opts.UseReflectionFallback = false;
            // TODO: JsonContext source generation so reflection is not needed
            //opts.JsonSerializerOptions = AppJsonContext.Default.Options;
        });
#endif
#if documentdbdiagnostics
        // OpenTelemetry metrics + tracing — call AFTER registering a store.
        // Subscribe from your OTel pipeline with .AddMeter("Shiny.DocumentDb") / .AddSource("Shiny.DocumentDb")
        builder.Services.AddDocumentStoreInstrumentation();
#endif
#if aidocumentdb
        // For AI tools setup, see https://shinylib.net/documentdb/ai-tools/
        // builder.Services.AddDocumentStoreAITools(tools =>
        // {
        //     // tools.AddType(jsonContext.SomeType, capabilities: DocumentAICapabilities.ReadOnly, configure: b => b.Description("...").MaxPageSize(20));
        // });
#endif
#if roomsharp
        builder.Services.AddDatabase();
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
#if datasync
        // Picks the right transport per target: NSURLSession on Apple, foreground service on Android, HttpClient elsewhere.
        // Implement IDataSyncDelegate, then register one endpoint per ISyncEntity type.
        // Docs: https://shinylib.net/datasync/
        builder.Services.AddDataSync<ShinyApp.Delegates.MyDataSyncDelegate>(opts =>
        {
            // opts.RegisterEndpoint<ShinyApp.Models.TodoItem>("https://api.example.com/todos");
        });
        builder.Services.AddHttpClient(Shiny.Data.Sync.Infrastructure.RestSyncTransport.HttpClientName, c =>
            c.BaseAddress = new Uri("https://api.example.com")
        );
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
#if usepushfcm
        builder.Services.AddPushFirebaseMessaging<ShinyApp.Delegates.MyPushDelegate>();
#endif
#if (screenrecord)
        builder.Services.AddSingleton(ScreenRecording.Default);
#endif
#if (audio)
        builder.Services.AddSingleton(Plugin.Maui.Audio.AudioManager.Current);
#endif
#if (music)
        builder.Services.AddShinyMusic();
#endif
#if (calendar)
        builder.Services.AddSingleton(Plugin.Maui.CalendarStore.CalendarStore.Default);
#endif
#if (contactstore)
        builder.Services.AddContactStore();
#endif
#if (usehealth)
        builder.Services.AddHealthIntegration();
#endif
#if (aihealth)
        // Exposes an opt-in slice of the health store to an LLM as Microsoft.Extensions.AI tools.
        // Resolve HealthAITools from DI and pass its Tools to your IChatClient (ChatOptions.Tools).
        // Tools assume permissions are already granted - call IHealthService.RequestPermissions first.
        // Docs: https://shinylib.net/health/ai-tools/
        builder.Services.AddHealthAITools(tools => tools
            .AddMetric(Shiny.Health.DataType.StepCount)
            .AddMetric(Shiny.Health.DataType.HeartRate)
            .AddWorkouts()
            // .AddBloodPressure()
            // .AddNutrition(Shiny.Health.Extensions.AI.HealthAICapabilities.ReadWrite)
        );
#endif
#if (shinyspeech)
        builder.Services.AddSpeechServices();
#endif
#if (aiconversation)
        // TODO: make sure IChatClientProvider is implemented or an IChatClient is registered in the DI container before enabling this
        // TODO: message store is optional and allow you to save user/AI conversations
        builder.Services.AddShinyAiConversation(opts =>
        {
            // TODO: implement IChatClientProvider and set it here
            // opts.SetChatClientProvider<MyChatClientProvider>();
            // opts.SetMessageStore<MyMessageStore>(addAiLookupTool: true); // optional
        });
#endif
#if (screenbrightness)
        builder.Services.AddSingleton(Plugin.Maui.ScreenBrightness.ScreenBrightness.Default);
#endif
#if storereview
        builder.Services.AddSingleton(Plugin.StoreReview.CrossStoreReview.Current);
#endif

#if (prism)
        builder.Services.RegisterForNavigation<MainPage, MainViewModel>();
#endif
        // this is source generated by Shiny.Extensions.DependencyInjection - simply add [Singleton], [Scoped], or [Transient] to your classes
        //builder.Services.AddGeneratedServices();
        // builder.Services.AddGeneratedAITools(); // https://shinylib.net/di/ai-tools/
        var app = builder.Build();

        return app;
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit