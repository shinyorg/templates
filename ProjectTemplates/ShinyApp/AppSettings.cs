namespace ShinyApp;

#if shinyframework
//[Shiny.Stores.ObjectStoreBinder("secure")] // defaults to standard platform preferences
public class AppSettings : ReactiveObject
{
    // any public get/set values marked with [Reactive] will automatically be saved to the configured store
    [Reactive] public string MySetting { get; set; }
}
#else
//[Shiny.Stores.ObjectStoreBinder("secure")] // defaults to standard platform preferences
public class AppSettings : NotifyPropertyChanged
{
    // any public get/set values that notify will automatically be saved to the configured store
    string mySetting;
    public string MySetting 
    { 
        get => this.mySetting;
        set => this.Set(ref this.mySetting, value);
    }
}

#endif