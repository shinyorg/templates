# Shiny Templates

![Nuget](https://img.shields.io/nuget/v/shiny.templates?style=for-the-badge)

**Please NOTE: Shiny v3 (used by these templates) & the templates are in a preview state.  Issues are anticipated.**

### To Install
> dotnet new --install Shiny.Templates

or update

> dotnet new --update-apply

---

## Features

### Project Templates

#### MAUI Shiny.NET App Project Template

* Creates a best practice MAUI application with best-in-class frameworks:
    * [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui)
	* [Prism](https://prismlibrary.com/)
	* [ReactiveUI](https://reactiveui.net/)
	* [Shiny.NET](https://shinylib.net)
* Select your markup type
	* XAML
	* Blazor
	* C# (courtesy of .NET MAUI Community Toolkit) [Documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/markup/markup)
* [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/) Setup Including:
	* [Media Element](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/)
	* [C# Markup](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/markup/markup))
* Entitlements for iOS/MacCatalyst if necessary (ie. Push, MSAL)
* Info.plist & Entitlements.plist permissions for iOS/MacCatalyst
* AndroidManifest.xml Permissions & Features for Android
* Preps all of the necessary functionality for a SQLite Database	
* All of your MauiProgram.cs Dependency Injection
* Setup Logging Options with AppCenter, SQLite, & Sentry.IO
* Create AppSettings.json
* Create & auto-configure a strongly-typed settings class that can be bound to preferences or secure storage using Shiny
* Create & auto-configure a Shiny startup service
* Setup Android Auto & iOS CarPlay
* Good practice setup for MSAL or Web Authenticator authentication service
	* Include a custom Refit HTTP client with authentication wired for best practices
* Setup everything needed for the .NET MAUI Essentials Media Capture service
* Setup .NET MAUI Maps
* Setup all of the necessary boilerplate for the following authentication providers:
	* MAUI Web Authentication
	* MSAL (Microsoft Authentication Library) Basic
	* MSAL (Microsoft Authentication Library) B2C
	* MSAL (Microsoft Authentication Library) Broker
* Full Setup for the following 3rd party components
	* [Uranium UI](https://github.com/enisn/UraniumUI) by Enis Necipoglu	
	* [Sharpnado Tabs](https://github.com/roubachof/Sharpnado.Tabs) by Jean-Marie Alfonsi
	* [Sharpnado CollectionView](https://github.com/roubachof/Sharpnado.CollectionView) by Jean-Marie Alfonsi
	* [MAUI Google Maps](https://github.com/themronion/Maui.GoogleMaps/tree/maui) by Pavlo Lukianets_
	* [Refit](https://github.com/reactiveui/refit)
	* [SQLite.NET-pcl](https://github.com/praeclarum/sqlite-net) by Frank Krueger
	* [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui) by Jon Dick
	* [Store Review Plugin](https://github.com/jamesmontemagno/StoreReviewPlugin) by James Montemagno
	* [In-App Billing Plugin](https://github.com/jamesmontemagno/InAppBillingPlugin) by James Montemagno
	* [MAUI Audio Plugin](https://github.com/jfversluis/Plugin.Maui.Audio) by Gerald Versluis
	* [Fingerprint Plugin](https://github.com/smstuebe/xamarin-fingerprint) by Sven-Michael Stübe
	* [Drastic Flipper](https://github.com/drasticactions/Drastic.Flipper) by Tim Miller


<img src="vs4win.png" />
<img src="vs4mac.png" />

#### MAUI XUnit Device Runner Project

_Sets up a platform unit test project using [Shiny.Xunit.Runners.Maui](https://github.com/shinyorg/xunit-maui)_

#### iOS Extension for .NET 7
	
_The Microsoft iOS Extension template is currently broken on VS4win & VS4mac, so this is in place for now. Instructions on how to wire this up to your MAUI project_

#### Shiny.NET Server Extensions

* Email Templating
* Push Notification Setup
* Entity Framework Basic Setup
* MSAL & WebAuthenticator Setup /w Google & Facebook


### Item Templates
* Shiny BluetoothLE Hosted Managed Characteristic
* Shiny Job
