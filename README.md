# Shiny Templates

[![Nuget](https://img.shields.io/nuget/v/shiny.templates?style=for-the-badge)](https://www.nuget.org/packages/Shiny.Templates/)


### To Install
> dotnet new install Shiny.Templates

---

## Features

### Project Templates

#### MAUI Shiny.NET App Project Template

* Supports Visual Studio for Windows 2022 and Visual Studio for Mac 2022
* Creates all the necessary permissions, boilerplate, & setup you'll need to get your .NET MAUI app up and running with Shiny & many other great community libraries
* Easy setup and choice for push notifications
	* Full Native
	* Azure Notification Hubs
	* Firebase (Coming Soon)
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
* Setup proper localization	
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
	* [Prism](https://prismlibrary.com) by Brian Lagunas & Dan Siegel
	* [ReactiveUI](https://reactiveui.net) from many contributors
	* 
	* [MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui) from many contributors
	* [Virtual List View](https://github.com/Redth/Maui.VirtualListView) by Jon Dick
	* [Uranium UI](https://github.com/enisn/UraniumUI) by Enis Necipoglu	
	* [FFImageLoading MAUI](https://github.com/microspaze/FFImageLoading.Maui) 
	* [Sharpnado Tabs](https://github.com/roubachof/Sharpnado.Tabs) by Jean-Marie Alfonsi
	* [Sharpnado CollectionView](https://github.com/roubachof/Sharpnado.CollectionView) by Jean-Marie Alfonsi
	* [MAUI Google Maps](https://github.com/themronion/Maui.GoogleMaps/tree/maui) by Pavlo Lukianets
	* [Refit](https://github.com/reactiveui/refit)
	* [SQLite.NET-pcl](https://github.com/praeclarum/sqlite-net) by Frank Krueger
	* [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui) by Jon Dick
	* [Store Review Plugin](https://github.com/jamesmontemagno/StoreReviewPlugin) by James Montemagno
	* [In-App Billing Plugin](https://github.com/jamesmontemagno/InAppBillingPlugin) by James Montemagno
	* [MAUI Screen Recording Plugin](https://github.com/jfversluis/Plugin.Maui.ScreenRecording) by Gerald Versluis
	* [MAUI Audio Plugin](https://github.com/jfversluis/Plugin.Maui.Audio) by Gerald Versluis
	* [MAUI Calendar Store Plugin](https://github.com/jfversluis/Plugin.Maui.CalendarStore) by Gerald Versluis	
	* [Fingerprint Plugin](https://github.com/smstuebe/xamarin-fingerprint) by Sven-Michael Stübe
	* [Drastic Flipper](https://github.com/drasticactions/Drastic.Flipper) by Tim Miller
	* [Embed.IO](https://unosquare.github.io/embedio/) by Unosquare
	* [SkiaSharp](https://github.com/mono/SkiaSharp) by Matthew Leibowitz
	* [MudBlazor](https://mudblazor.com)

<img src="vs4win.png" />
<img src="vs4mac.png" />

#### MAUI XUnit Device Runner Project

_Sets up a platform unit test project using [Shiny.Xunit.Runners.Maui](https://github.com/shinyorg/xunit-maui)_

#### iOS Extension for .NET 8
	
_The Microsoft iOS Extension template is currently broken on VS4win & VS4mac, so this is in place for now. Instructions on how to wire this up to your MAUI project_

#### Shiny.NET Server Extensions

* Email Templating
* Push Notification Setup
* .NET Orleans
* Entity Framework Basic Setup
* MSAL & WebAuthenticator Setup /w Google & Facebook
* Apple Domain Association Setup
* Exception Handler Setup


### Item Templates
* Shiny BluetoothLE Hosted Managed Characteristic
* Shiny Job
