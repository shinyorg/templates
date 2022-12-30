using System.Runtime.CompilerServices;
using Shiny.Hosting;

namespace ShinyApp.Controls;

public partial class ConnectivityView : ContentView
{
	IDisposable? sub;


	public ConnectivityView()
	{
		this.InitializeComponent();

        // if not using Shiny.Framework or Shiny.Jobs, you must add shiny's connectivity stub using services.AddConnectivity()
        this.Loaded += (sender, args) =>
		{
            var conn = Host
				.Current
                .Services
                .GetRequiredService<Shiny.Net.IConnectivity>();

            this.SetInternet(conn);
            this.sub = conn
				.WhenChanged()
				.SubOnMainThread(x => this.SetInternet(x));
		};

        this.Unloaded += (sender, args) => this.sub?.Dispose();
	}


	void SetInternet(Shiny.Net.IConnectivity conn)
	{
		var result = conn.IsInternetAvailable();
		this.IsVisible = !result;
	}


    public static readonly BindableProperty OfflineTextProperty = BindableProperty.Create(
		nameof(OfflineText),
		typeof(string),
		typeof(ConnectivityView),
		"There is no internet connection detected"
	);
	public string OfflineText
	{
		get => (string)this.GetValue(OfflineTextProperty);
		set => this.SetValue(OfflineTextProperty, value);
	}	
}
