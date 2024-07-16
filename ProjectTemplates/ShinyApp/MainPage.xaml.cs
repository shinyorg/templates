namespace ShinyApp;


public partial class MainPage : ContentPage
{
#if (shinyframework || prism)
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

