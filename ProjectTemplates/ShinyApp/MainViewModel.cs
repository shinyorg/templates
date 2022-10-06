namespace ShinyApp;


public class MainViewModel : ViewModel
{
    public MainViewModel(BaseServices services) : base(services) {}


    [Reactive] public string Property { get; set; }
}
