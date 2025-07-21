using Android.App;
using AndroidX.Car.App;
using AndroidX.Car.App.Validation;

namespace ShinyApp.Platforms.Android.Auto;


[global::Android.App.Service(Exported = true)]
[IntentFilter(
	["androidx.car.app.CarAppService"],
	Categories = ["androidx.car.app.category.POI"]
)]
public class AutoService : CarAppService
{
    public override HostValidator CreateHostValidator()
        => HostValidator.AllowAllHostsValidator;


	public override Session OnCreateSession()
		=> new AutoSession();
}

