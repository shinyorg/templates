namespace ShinyApp;

//[Shiny.Stores.ObjectStoreBinder("secure")] // defaults to standard platform preferences
// any public get/set values that notify will automatically be saved to the configured store
public class AppSettings : NotifyPropertyChanged, IShinyStartupTask
{
    AppTheme currentTheme;
    public AppTheme CurrentTheme
    {
        get => this.currentTheme;
        set => this.Set(ref this.currentTheme, value);
    }


    public void Start()
    {
        this.WhenAnyProperty(x => x.CurrentTheme)
            .Skip(1)
            .Subscribe(x => Application.Current!.UserAppTheme = x);
    }
}