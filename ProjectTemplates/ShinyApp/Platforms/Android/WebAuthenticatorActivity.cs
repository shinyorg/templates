using Android.App;
using Android.Content;
using Android.Content.PM;

namespace ShinyApp;


[Activity(
    NoHistory = true, 
    LaunchMode = LaunchMode.SingleTop, 
    Exported = true
)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [
        Intent.CategoryDefault, 
        Intent.CategoryBrowsable 
    ],
    DataScheme = "myapp"
)]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
}