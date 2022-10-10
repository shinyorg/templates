namespace ShinyApp;

#if shinyframework
public class MainViewModel : ViewModel
{
    public MainViewModel(BaseServices services) : base(services) {}


    [Reactive] public string Property { get; set; }
}
#else
public class MainViewModel : NotifyPropertyChanged
{
    public MainViewModel() 
    {
    }


    string property;
    public string Property 
    { 
        get => this.property;
        set => this.Set(ref this.property, value);
    }
}
#endif