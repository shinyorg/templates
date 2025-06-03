namespace ShinyApp.Services;

public record BaseServices(
    IConfiguration Configuration,
    #if prism
    INavigationService Navigator,
    IDialogService Dialogs,
    #endif
    #if settings
    AppSettings Settings,
    #endif
    #if authservice
    IAuthenticationService Authentication,
    #endif
    ILoggerFactory LoggerFactory
);