using System;
using Android.App;
using AndroidX.Car.App;
using AndroidX.Car.App.Validation;

namespace ShinyApp.Platforms.Android.Auto;


[Service(Exported = true)]
[IntentFilter(
	new[] { "androidx.car.app.CarAppService" },
	Categories = new[] { "androidx.car.app.category.POI" }
)]
public class AutoService : CarAppService
{
    public override HostValidator CreateHostValidator()
        => HostValidator.AllowAllHostsValidator;


	public override Session OnCreateSession()
		=> new AutoSession();
}

