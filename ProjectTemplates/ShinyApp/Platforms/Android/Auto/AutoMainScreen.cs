using System;
using Android.Runtime;
using AndroidX.Car.App;
using AndroidX.Car.App.Model;

namespace ShinyApp.Platforms.Android.Auto;


public class AutoMainScreen : Screen
{
    public AutoMainScreen(CarContext context) : base(context) { }


    public override ITemplate OnGetTemplate()
    {
        // TODO: implement stuff here
        var itemList = new ItemList.Builder()
            .SetNoItemsMessage("TODO: No Charging Stations")
            .Build();

        return new ListTemplate.Builder()
            .SetTitle("EVS")
            .SetSingleList(itemList)
            .Build();
    }
}