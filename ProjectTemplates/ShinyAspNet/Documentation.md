
## ASP.NET Documentation
* [Documentation](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-7.0)

## Entity Framework
* [Documentation](https://learn.microsoft.com/en-us/ef/core/)

## Fast Endpoints
_ASP.NET minimal APIs done right.  This also includes the ability to generate client code_

* [Documentation](https://fast-endpoints.com/)
* [GitHub](https://github.com/FastEndpoints/FastEndpoints)

<!--#if (orleans)-->
## Microsoft Orleans

_Orleans is a cross-platform framework for building robust, scalable distributed applications. Distributed applications are defined as apps that span more than a single process, often beyond hardware boundaries using peer-to-peer communication. Orleans scales from a single on-premises server to hundreds to thousands of distributed, highly available applications in the cloud. Orleans extends familiar concepts and C# idioms to multi-server environments. Orleans is designed to scale elastically. When a host joins a cluster, it can accept new activations. When a host leaves the cluster, either because of scale down or a machine failure, the previous activations on that host will be reactivated on the remaining hosts as needed. An Orleans cluster can be scaled down to a single host. The same properties that enable elastic scalability also enable fault tolerance. The cluster automatically detects and quickly recovers from failures.

One of the primary design objectives of Orleans is to simplify the complexities of distributed application development by providing a common set of patterns and APIs. Developers familiar with single-server application development can easily transition to building resilient, scalable cloud-native services and other distributed applications using Orleans. For this reason, Orleans has often been referred to as "Distributed .NET" and is the framework of choice when building cloud-native apps. Orleans runs anywhere that .NET is supported. This includes hosting on Linux, Windows, and macOS. Orleans apps can be deployed to Kubernetes, virtual machines, and PaaS services such as Azure App Service and Azure Container Apps._

* [Documentation](https://learn.microsoft.com/en-us/dotnet/orleans/)

<!--#endif-->
<!--#if (swagger)-->
## Swagger
* [Swagger Link (project must be running)](https://localhost:5001/swagger/v1/swagger.json)
* [C# Client Code Generation (project must be running)](https://localhost:5001/cs-client)

<!--#endif-->
<!--#if (apple)-->
## Apple Authentication
* [GitHub](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers)

<!--#endif-->
<!--#if (google)-->
## Google Authentication
* [Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-7.0)

<!--#endif-->
<!--#if (facebook)-->
## Facebook Authentication
* [Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-7.0)

<!--#endif-->
<!--#if (push)-->
## Shiny Push Notification Management
_Powerful push notification management_

* [Documentation](http://shinylib.net/extensions/push/index.html)
* [GitHub](https://github.com/shinyorg/apiservices)

<!--#endif-->
<!--#if (email)-->
## Shiny Email Templates
_Powerful email templates for your apps_

* [Documentation](http://shinylib.net/extensions/email.html)
* [GitHub](https://github.com/shinyorg/apiservices)

<!--#endif-->
<!--#if (ffimageloading)-->
## FFImageLoading
_Fast & Furious Image Loading for .NET MAUI_

Forked from the amazingly popular original FFImageLoading Library, this Compat version FFImageLoading.Compat aims to ease your migration from Xamarin.Forms to .NET MAUI with a compatible implementation to get you up and running without rewriting the parts of your app that relied on the original library.

This Maui version which merges all Transformations & SVG library parts into ONE and is migrated from FFImageLoading.Compat aims to fix some critical bugs and gives you a place to submit Maui releated issues.

The Google webp format image support. (It works in Xamarin.Forms version, but not in FFImageLoading.Compat)
Thanks to the Original Authors: Daniel Luberda, Fabien Molinet & Redth.

<!--#endif-->