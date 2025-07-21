using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace ShinyApp;


[Activity(Exported = true)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryBrowsable, Intent.CategoryDefault],
    DataHost = "auth",
    DataScheme = "msal{MSAL_CLIENT_ID}"
)]
public class MsalActivity : BrowserTabActivity
{
}