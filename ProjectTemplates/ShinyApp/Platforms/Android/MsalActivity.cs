using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace ShinyApp;


[Activity(Exported = true)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
    DataHost = "auth",
    DataScheme = "msal{MSAL_CLIENT_ID}"
)]
public class MsalActivity : BrowserTabActivity
{
}