namespace ShinyApp;

public partial class App : Application
{
#if (shinyshell)
	public App()
	{
		this.InitializeComponent();
	}


	protected override Window CreateWindow(IActivationState? activationState)
        => new(new AppShell());
#elif (prism)
	public App()
	{
		this.InitializeComponent();
	}
#else
	public App(MainPage mainPage)
	{
		this.InitializeComponent();
		this.MainPage = mainPage;
	}
#endif
}
