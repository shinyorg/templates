namespace ShinyApp;


//[Shiny.Stores.ObjectStoreBinder("secure")] // defaults to standard platform preferences
public class AppSettings : ReactiveObject
{
    // any public get/set values marked with [Reactive] will automatically be saved to the configured store
    [Reactive] public string MySetting { get; set; }
}