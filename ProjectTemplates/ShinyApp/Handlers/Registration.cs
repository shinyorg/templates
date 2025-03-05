namespace ShinyApp;


public static class HandlerRegistration 
{
	public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddShinyMediatorHandlers(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)
	{
		services.AddSingletonAsImplementedInterfaces<ShinyApp.Handlers.MyCommandHandler>();
		services.AddSingletonAsImplementedInterfaces<ShinyApp.Handlers.MyRequestHandler>();
		services.AddSingletonAsImplementedInterfaces<ShinyApp.Handlers.MyEventHandler>();
        services.AddSingletonAsImplementedInterfaces<ShinyApp.Handlers.MyStreamHandler>();
		return services;
	}
}