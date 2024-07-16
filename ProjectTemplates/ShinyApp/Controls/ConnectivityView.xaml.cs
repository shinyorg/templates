using Shiny.Hosting;

namespace ShinyApp.Controls;


public partial class ConnectivityView : ContentView
{
    readonly IConnectivity connectivity;


    public ConnectivityView()
    {
        this.InitializeComponent();
        this.connectivity = Host
            .Current
            .Services
            .GetRequiredService<IConnectivity>();
        
        this.Loaded += (_, _) =>
        {
            this.connectivity.ConnectivityChanged += this.OnConnectivityChanged;
            this.SetInternet();
        };

        this.Unloaded += (_, _) => this.connectivity.ConnectivityChanged -= this.OnConnectivityChanged;
    }


    void OnConnectivityChanged(object _, ConnectivityChangedEventArgs __) 
        => this.SetInternet();
    
    void SetInternet()
    {
        var result = this.connectivity.NetworkAccess == NetworkAccess.Internet || this.connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet;
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
