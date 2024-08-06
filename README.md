# Shiny Templates

[![Nuget](https://img.shields.io/nuget/v/shiny.templates?style=for-the-badge)](https://www.nuget.org/packages/Shiny.Templates/)


Works with Visual Studio 2022 & JetBrains Rider 2024+

### To Install
> dotnet new install Shiny.Templates

---

## Features

### Project Templates

#### MAUI Shiny.NET App Project Template

* Supports Visual Studio for Windows 2022, Visual Studio for Mac 2022, & JetBrains Rider 2024+
* Creates all the necessary permissions, boilerplate, & setup you'll need to get your .NET MAUI app up and running with Shiny & many other great community libraries
* Easy setup and choice for push notifications
	* Full Native
	* Azure Notification Hubs
	* Firebase
* Creates a best practice MAUI application with best-in-class frameworks:
    * [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui)
	* [Prism](https://prismlibrary.com/)
	* [ReactiveUI](https://reactiveui.net/)
	* [MVVM Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm)
	* [Shiny.NET](https://shinylib.net)
	* [Shiny Mediator](https://github.com/shinyorg/mediator)
* Select mutliple platform UI Markup types
	* XAML
	* Blazor
	* C# Markup (courtesy of .NET MAUI Community Toolkit) [Documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/markup/markup)
* [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/) Setup Including:
	* [Media Element](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/)
	* [C# Markup](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/markup/markup)
	* [Camera View](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/camera-view)
	* [MVVM](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm)
* Entitlements for iOS/MacCatalyst if necessary (ie. Push, MSAL)
* Info.plist & Entitlement permissions for iOS/MacCatalyst
* Helps generate Apple's "fun" PrivacyInfo.xcprivacy
* AndroidManifest.xml Permissions & Features for Android
* Preps all of the necessary functionality for a SQLite Database
* Setup proper localization	
* All of your MauiProgram.cs Dependency Injection
* Setup Logging Options with Local SQLite & Sentry.IO
* Create AppSettings.json
* Create & auto-configure a strongly-typed settings class that can be bound to preferences or secure storage using Shiny
* Create & auto-configure a Shiny startup service
* Setup Android Auto & iOS CarPlay
* Good practice setup for MSAL or Web Authenticator authentication service
	* Include a custom Refit HTTP client with authentication wired for best practices
* Setup everything needed for the .NET MAUI Essentials Media Capture service
* Mapping
	* Setup .NET MAUI Maps
	* Setup Google Maps
* Setup all of the necessary boilerplate for the following authentication providers:
	* MAUI Web Authentication
	* MSAL (Microsoft Authentication Library) Basic
	* MSAL (Microsoft Authentication Library) B2C
	* MSAL (Microsoft Authentication Library) Broker
* Full Setup for the following 3rd party components
	* [Prism](https://prismlibrary.com) by Brian Lagunas & Dan Siegel
	* [ReactiveUI](https://reactiveui.net) from many contributors
	* [MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui) from many contributors
	* [Virtual List View](https://github.com/Redth/Maui.VirtualListView) by Jon Dick
	* [Uranium UI](https://github.com/enisn/UraniumUI) by Enis Necipoglu
	* [FFImageLoading MAUI](https://github.com/microspaze/FFImageLoading.Maui) 
	* [Sharpnado Tabs](https://github.com/roubachof/Sharpnado.Tabs) by Jean-Marie Alfonsi
	* [Sharpnado CollectionView](https://github.com/roubachof/Sharpnado.CollectionView) by Jean-Marie Alfonsi
	* [MAUI Google Maps](https://github.com/themronion/Maui.GoogleMaps/tree/maui) by Pavlo Lukianets
	* [Camera.MAUI](https://github.com/hjam40/Camera.MAUI) by Hector (hjam40)
	* [Refit](https://github.com/reactiveui/refit) by ReactiveUI Maintainers
	* [SQLite.NET-pcl](https://github.com/praeclarum/sqlite-net) by Frank Krueger
	* [BarcodeScanning.Native.Maui](https://github.com/afriscic/BarcodeScanning.Native.Maui) by afriscic
	* [Store Review Plugin](https://github.com/jamesmontemagno/StoreReviewPlugin) by James Montemagno
	* [In-App Billing Plugin](https://github.com/jamesmontemagno/InAppBillingPlugin) by James Montemagno
	* [MAUI Screen Recording Plugin](https://github.com/jfversluis/Plugin.Maui.ScreenRecording) by Gerald Versluis
	* [MAUI Audio Plugin](https://github.com/jfversluis/Plugin.Maui.Audio) by Gerald Versluis
	* [MAUI Calendar Store Plugin](https://github.com/jfversluis/Plugin.Maui.CalendarStore) by Gerald Versluis
	* [MAUI Screen Brightness Plugin](https://github.com/jfversluis/Plugin.Maui.ScreenBrightness) by Gerald Versluis
	* [MAUI OCR Plugin](https://github.com/kfrancis/ocr) by Kori Francis
	* [MAUI Biometric Plugin](https://github.com/oscoreio/Maui.Biometric) by Konstantin S & Sven-Michael Stübe
	* [CardsView MAUI](https://github.com/AndreiMisiukevich/CardView.MAUI) by Andrei Misiukevich
	* [Drastic Flipper](https://github.com/drasticactions/Drastic.Flipper) by Tim Miller
	* [Embed.IO](https://unosquare.github.io/embedio/) by Unosquare
	* [SkiaSharp](https://github.com/mono/SkiaSharp) by Matthew Leibowitz
	* [MudBlazor](https://mudblazor.com)
	* [Radzen Blazor](https://blazor.radzen.com/)
	* [ACR User Dialogs](https://github.com/aritchie/userdialogs) by Allan Ritchie
	* [Debug Rainbows](https://github.com/sthewissen/Plugin.Maui.DebugRainbows) by Steven Thewissen
	* [Live Charts](https://livecharts.dev/) by Alberto Rodríguez
	* [Compiled Bindings](https://github.com/levitali/CompiledBindings) by Vitali Lesheniuk
	* [Settings View](https://github.com/muak/AiForms.Maui.SettingsView) by Satoshi NaKamura
	* [AlohaKit Animations](https://github.com/jsuarezruiz/AlohaKit.Animations) by Javier Suárez
	* [Skeleton](https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton) by Horus Software
	* [The 49 Bottom Sheet](https://github.com/the49ltd/The49.Maui.BottomSheet) by The 49 Ltd
	* [The 49 Context Menu](hhttps://github.com/the49ltd/The49.Maui.ContextMenu) by The 49 Ltd
	* [Bindable Property Generator](https://github.com/rrmanzano/maui-bindableproperty-generator) by rrmanzano

<img src="vs4win.png" />


#### Shiny.NET Server Extensions

* Email Templating
* Push Notification Setup
* Minimal APIs
* Shiny Mediator Endpoint Setup
* .NET Orleans
* Entity Framework Basic Setup
* MSAL & WebAuthenticator Setup /w Google & Facebook
* Apple Domain Association Setup


### Item Templates
* Shiny BluetoothLE Hosted Managed Characteristic
* Shiny Job
