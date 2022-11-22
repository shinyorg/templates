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
* Entitlements for iOS/MacCatalyst if necessary (ie. NFC, Push, MSAL)
* Info.plist & Entitlements.plist permissions for iOS/MacCatalyst
* AndroidManifest.xml Permissions & Features for Android
* Preps all of the necessary functionality for a SQLite Database	
* All of your MauiProgram.cs Dependency Injection
* Setup AppCenter Logging
* Create AppSettings.json
* Create & auto-configure a strongly-typed settings class that can be bound to preferences or secure storage using Shiny
* Create & auto-configure a Shiny startup service
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
	* [Refit](https://github.com/reactiveui/refit)
	* [SQLite.NET-pcl](https://github.com/praeclarum/sqlite-net) by Frank Krueger
	* [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui) by Jon Dick
	* [Store Review Plugin](https://github.com/jamesmontemagno/StoreReviewPlugin) by James Montemagno
	* [In-App Billing Plugin](https://github.com/jamesmontemagno/InAppBillingPlugin) by James Montemagno
	* [MAUI Audio Plugin](https://github.com/jfversluis/Plugin.Maui.Audio) by Gerald Versluis
	* [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/)
	* [Fingerprint Plugin](https://github.com/smstuebe/xamarin-fingerprint) by Sven-Michael Stübe

<img src="vs4win.png" />
<img src="vs4mac.png" />

#### MAUI XUnit Device Runner Project

_Sets up a platform unit test project using [Shiny.Xunit.Runners.Maui](https://github.com/shinyorg/xunit-maui)_

#### iOS Extension for .NET 7
	
_The Microsoft iOS Extension template is currently broken on VS4win & VS4mac, so this is in place for now. Instructions on how to wire this up to your MAUI project_

#### Shiny.NET Server Extensions (Work In Progress)

* Email Templating
* Push Notification Setup
* Entity Framework Basic Setup
* MSAL & WebAuthenticator Setup /w Google & Facebook

#### Cross Platform Library


### Item Templates
* Shiny BluetoothLE Hosted Managed Characteristic
* Shiny Job

---


---
## Roadmap
* Shiny ASP.NET application setup for Shiny Extensions push & email tempating extensions
	* Web Authenticator controller
* Solution templates that brings MAUI app and ASP.NET together along with build scripts?