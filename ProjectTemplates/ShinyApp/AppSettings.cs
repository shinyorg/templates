namespace ShinyApp;

//[Shiny.Extensions.Stores.ObjectStoreBinder("secure")] // defaults to standard platform preferences
// any public get/set values that notify will automatically be saved to the configured store
public class AppSettings : NotifyPropertyChanged
{
}