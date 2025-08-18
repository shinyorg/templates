namespace ShinyApp.Services;

#if prism
[Scoped]
#else
[Singleton]
#endif
public record BaseServices(
    IConfiguration Configuration,
#if prism
    INavigationService Navigator,
    IDialogService Dialogs,
#endif
#if shinyshell
    INavigator Navigator,
#endif
#if settings
    AppSettings Settings,
#endif
#if authservice
    IAuthenticationService Authentication,
#endif
    ILoggerFactory LoggerFactory
);