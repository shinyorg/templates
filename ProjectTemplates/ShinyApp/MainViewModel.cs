namespace ShinyApp;

#if reactiveui
public class MainViewModel : ViewModel
{
    public MainViewModel(BaseServices services) : base(services) {}


    [Reactive] string property;
}
#elif ctmvvm
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] string property;

    [RelayCommand]
    async Task DoSomething()
    {
        
    }
}
#else
public class MainViewModel : NotifyPropertyChanged
{
    string property;
    public string Property 
    { 
        get => this.property;
        set => this.Set(ref this.property, value);
    }
}
#endif