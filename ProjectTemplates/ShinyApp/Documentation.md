# Documentation for Selected Components

## .NET MAUI
_Microsoft Application User Interface Library_

* [Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
* [GitHub](https://github.com/dotnet/maui)

## Shiny

_A cross platform framework designed to make working with device services and background processes easy, testable, and consistent while bringing things like dependency injection & logging in a structured way to your code! - Written by Allan Ritchie_

* [Documentation](https://shinylib.net/)
* [GitHub](https://github.com/shinyorg/shiny)

<!--#if (shinyframework)-->
## Shiny Framework

_Framework combines the best of MVVM using Prism & ReactiveUI while also combining Shiny. - Written by Allan Ritchie_

* [Documentation](https://github.com/shinyorg/framework)
* Third Party Libraries
    * [ReactiveUI](https://reactiveui.net)
    * [Prism](https://prismlibrary.com)
    * [DryIoc](https://github.com/dadhi/DryIoc)

<!--#endif-->
<!--#if (shinyframework || communitytoolkit)-->
## MAUI Community Toolkit

_A collection of reusable elements for application development with .NET MAUI, including animations, behaviors, converters, effects, and helpers. It simplifies and demonstrates common developer tasks when building iOS, Android, macOS and WinUI applications._

* [Documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/)
* [GitHub](https://github.com/CommunityToolkit/Maui)

<!--#endif-->
<!--#if (sharpnadovc)-->
## Sharpnado CollectionView

_A performant and feature rich collection view by Jean-Marie Alfonsi_

* [GitHub](https://github.com/roubachof/Sharpnado.CollectionView)

<!--#endif-->
<!--#if (sharpnadotabs)-->
## Sharpnado Tabs

_Feature rich tab control by Jean-Marie Alfonsi_

* [GitHub](https://github.com/roubachof/Sharpnado.Tabs)

<!--#endif-->
<!--#if (mediaelement)-->
## MAUI Community Toolkit - Media Element

MediaElement is a view for playing video and audio in your .NET MAUI app.

* [Documentation](https://learn.microsoft.com/en-ca/dotnet/communitytoolkit/maui/views/mediaelement)
* [GitHub](https://github.com/CommunityToolkit/Maui)

<!--#endif>
<!--#if (usecsharpmarkup)-->
## .NET MAUI Community Toolkit - C# Markup

_The C# Markup Extensions for .NET MAUI Community Toolkit is a set of extensions that allow you to write XAML in C#._

* [Documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/markup/markup)
* [GitHub](https://github.com/CommunityToolkit/Maui)

<!--#endif-->
<!--#if (usemsal)-->
### Microsoft Authentication Library (MSAL)

* [Microsoft Blog](https://devblogs.microsoft.com/dotnet/authentication-in-dotnet-maui-apps-msal/)
* [Documentation](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/blob/main/MauiApps.md)
* [GitHub](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet)

<!--#endif-->
<!--#if (barcodes)-->
## ZXing.Net MAUI

_Scan and render barcodes - Written by Jonathan Dick_

* [GitHub](https://github.com/Redth/ZXing.Net.Maui)

<!--#endif-->
<!--#if (storereview)-->
## Store Review Plugin

_Request store reviews from your users - Written by Jame Montemagno_

* [GitHub](https://github.com/jamesmontemagno/StoreReviewPlugin)

<!--#endif-->
<!--#if (inappbilling)-->
## In-App Billing

_A simple In-App Purchase plugin for .NET MAUI, Xamarin, and Windows to query item information, purchase items, restore items, and more. - Written by James Montemagno_
	
* [GitHub](https://github.com/jamesmontemagno/InAppBillingPlugin)

<!--#endif-->
<!--#if (audio)-->
## MAUI Audio Plugin

_Provides the ability to play audio inside a .NET MAUI application. - Written by Gerald Versluis_

* [GitHub](https://github.com/jfversluis/Plugin.Maui.Audio)

<!--#endif-->
<!--#if (fingerprint)-->
## Fingerprint Plugin

_A plugin for accessing the fingerprint, Face ID or other biometric sensors. - Written by Sven-Michael Stübe_

* [Fingerprint Plugin](https://github.com/smstuebe/xamarin-fingerprint) 

<!--#endif-->
<!--#if (usehttp)-->
## Refit HTTP Client

_The automatic type-safe REST library_

* [GitHub](https://github.com/reactiveui/refit)

<!--#endif-->
<!--#if (sqlite)-->
## SQLite .NET PCL

_SQLite-net is an open source, minimal library to allow .NET, .NET Core, and Mono applications to store data in SQLite 3 databases - Written by Frank Krueger_

[GitHub](https://github.com/praeclarum/sqlite-net)

<!--#endif-->

SELECTED UI TYPE: {MARKUP_TYPE}

<!--#if (markuptype == \"XAML\")-->
UI Type: XAML
<!--#endif-->
<!--#if (markuptype == \"CSharp\")-->
UI Type: C# Markup
<!--#endif-->
<!--#if (markuptype == \"Blazor\")-->
UI Type: Blazor
<!--#endif-->

<!--#if (usexaml)-->
USE XAML: TRUE
<!--#else-->
USE XAML: FALSE
<!--#endif-->

<!--#if (useblazor)-->
USE BLAZOR: TRUE
<!--#else-->
USE BLAZOR: FALSE
<!--#endif-->

<!--#if (usecsharpmarkup)-->
USE CSHARP MARKUP: TRUE
<!--#else-->
USE CSHARP MARKUP: FALSE
<!--#endif-->