## ASP.NET Documentation
* [Documentation](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-7.0)

## Entity Framework
* [Documentation](https://learn.microsoft.com/en-us/ef/core/)

## Shiny Mediator
Mediator is a behavioral design pattern that lets you reduce chaotic dependencies between objects. The pattern restricts direct communications between the objects and forces them to collaborate only via a mediator object.

Shiny Mediator is a mediator pattern implementation, works for server, but also works great for apps.  Apps have pages with lifecycles that don't necessarily participate in the standard
dependency injection lifecycle.  .NET MAUI generally tends to favor the Messenger pattern.  We hate this pattern for many reasons which we won't get into.  That being said, we do offer a messenger subscription in our Mediator for where interfacesand dependency injection can't reach.

Created by [Allan Ritchie](https://github.com/aritchie)

* [GitHub](https://github.com/shinyorg/mediator)
* [Documentation](https://shinylib.net/client/mediator)



<!--#if (orleans)-->
## Microsoft Orleans

_Orleans is a cross-platform framework for building robust, scalable distributed applications. Distributed applications are defined as apps that span more than a single process, often beyond hardware boundaries using peer-to-peer communication. Orleans scales from a single on-premises server to hundreds to thousands of distributed, highly available applications in the cloud. Orleans extends familiar concepts and C# idioms to multi-server environments. Orleans is designed to scale elastically. When a host joins a cluster, it can accept new activations. When a host leaves the cluster, either because of scale down or a machine failure, the previous activations on that host will be reactivated on the remaining hosts as needed. An Orleans cluster can be scaled down to a single host. The same properties that enable elastic scalability also enable fault tolerance. The cluster automatically detects and quickly recovers from failures.

One of the primary design objectives of Orleans is to simplify the complexities of distributed application development by providing a common set of patterns and APIs. Developers familiar with single-server application development can easily transition to building resilient, scalable cloud-native services and other distributed applications using Orleans. For this reason, Orleans has often been referred to as "Distributed .NET" and is the framework of choice when building cloud-native apps. Orleans runs anywhere that .NET is supported. This includes hosting on Linux, Windows, and macOS. Orleans apps can be deployed to Kubernetes, virtual machines, and PaaS services such as Azure App Service and Azure Container Apps._

* [Documentation](https://learn.microsoft.com/en-us/dotnet/orleans/)

<!--#endif-->
<!--#if (swagger)-->
## Swagger
* [Swagger Link (project must be running)](https://localhost:5001/swagger/v1/swagger.json)

### To generate a C# client
cd YourApiDirectoy
dotnet run --generateclients true

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
<!--#if (mediatr)-->
## MediatR
_MediatR is a simple, unambitious mediator implementation in .NET_

* [Documentation](https://github.com/jbogard/MediatR)
<!--#endif-->