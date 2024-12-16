namespace ShinyApp;


public partial class MainPage : ContentPage
{
#if (prism)
	public MainPage()
	{
		this.InitializeComponent();
	}
#else
	public MainPage(MainViewModel viewModel)
	{
		this.InitializeComponent();
		this.BindingContext = viewModel;
	}
#endif
}

