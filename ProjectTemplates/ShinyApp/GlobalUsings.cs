global using ShinyApp.Services;
global using Shiny;
global using Shiny.Extensions.DependencyInjection;
#if (shinymediator)
global using Shiny.Mediator;
global using ICommand = System.Windows.Input.ICommand;
#endif
global using System;
global using System.Windows.Input;
global using System.Collections.ObjectModel;
global using System.Collections.Generic;
global using System.Reactive;
global using System.Reactive.Linq;
global using System.Reactive.Threading.Tasks;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
#if localization
global using Microsoft.Extensions.Localization;
#endif
#if reactiveui
global using ReactiveUI;
global using ReactiveUI.SourceGenerators;
#endif
#if prism
global using Prism.AppModel;
global using Prism.Navigation;
global using Prism.Services;
#endif
#if communitytoolkit
global using CommunityToolkit.Maui;
#endif
#if usecsharpmarkup
global using CommunityToolkit.Maui.Markup;
#endif
#if ctmvvm
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
#endif