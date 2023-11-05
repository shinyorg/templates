namespace ShinyApp.Delegates;


public class AppActionDelegate
{
    // dependency injection works here
    public AppActionDelegate() 
    {
    }


    public void Handle(AppAction appAction)
    {
        // App.Current.Dispatcher.Dispatch(async () =>
        // {
        //     var page = appAction.Id switch
        //     {
        //         "battery_info" => new SensorsPage(),
        //         "app_info" => new AppModelPage(),
        //         _ => default(Page)
        //     };

        //     if (page != null)
        //     {
        //         await Application.Current.MainPage.Navigation.PopToRootAsync();
        //         await Application.Current.MainPage.Navigation.PushAsync(page);
        //     }
        // });
    }
}
