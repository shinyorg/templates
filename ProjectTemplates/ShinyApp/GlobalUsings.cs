global using Shiny;
#if (shinymediator)
global using Shiny.Mediator;
#endif
global using System;
global using System.Windows.Input;
global using System.Collections.ObjectModel;
global using System.Collections.Generic;
global using System.Reactive;
global using System.Reactive.Linq;
global using System.Reactive.Threading.Tasks;
global using Microsoft.Extensions.Logging;
#if (useconfig)
global using Microsoft.Extensions.Configuration;
#endif
#if (localization)
global using Microsoft.Extensions.Localization;
#endif
#if shinyframework || reactiveui
global using ReactiveUI;
global using ReactiveUI.Fody.Helpers;
#endif
#if shinyframework || prism
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
#if (ctmvvm)
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
#endif