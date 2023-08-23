global using Shiny;
global using System;
global using System.Windows.Input;
global using System.Collections.ObjectModel;
global using System.Reactive;
global using System.Reactive.Linq;
global using System.Reactive.Threading.Tasks;
global using Microsoft.Extensions.Logging;
#if (useconfig)
global using Microsoft.Extensions.Configuration;
#endif
#if shinyframework
global using ReactiveUI;
global using ReactiveUI.Fody.Helpers;
global using Prism.Navigation;
global using Prism.Services;
#endif
#if shinyframework || localization
global using Microsoft.Extensions.Localization;
#endif
#if shinyframework || communitytoolkit
global using CommunityToolkit.Maui;
#endif
#if usecsharpmarkup
global using CommunityToolkit.Maui.Markup;
#endif