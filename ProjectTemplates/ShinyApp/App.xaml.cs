﻿namespace ShinyApp;

public partial class App : Application
{
#if shinyframework
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
