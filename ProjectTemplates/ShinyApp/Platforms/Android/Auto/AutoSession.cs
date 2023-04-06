using System;
using Android.Content;
using AndroidX.Car.App;

namespace ShinyApp.Platforms.Android.Auto;


public class AutoSession : Session
{
    public override Screen OnCreateScreen(Intent p0)
        => new AndroidAutoMainScreen(this.CarContext);
}

