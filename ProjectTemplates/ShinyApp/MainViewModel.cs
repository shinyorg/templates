namespace ShinyApp;

#if reactiveui
public class MainViewModel(BaseServices services) : ReactiveObject
{
    [Reactive] string property;
}
#elif ctmvvm
public partial class MainViewModel(BaseServices services) : ObservableObject
{
    [ObservableProperty] 
    public partial string Property { get; set; }

    [RelayCommand]
    async Task DoSomething()
    {
        
    }
}
#else
#if shinyshell
// we don't register routes in the Shell xaml as navigation will crash within MAUI
[ShellMap<MainPage>(registerRoute: false)]
#endif
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