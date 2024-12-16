#if ctmvvm
public abstract partial class ViewModel(BaseServices services) : ObservableValidator
#elif reactiveui
public abstract partial class ViewModel(BaseServices services) : ReactiveObject
#else
public abstract class ViewModel(BaseServices services) : Shiny.NotifyPropertyChanged
#endif
#if prism
    , IInitialize,
    IPageLifecycleAware, 
    IApplicationLifecycleAware, 
    INavigationAware, 
    IConfirmNavigationAsync,
    IDestructible
#endif
{
    protected BaseServices Services => services;

#if prism
    public virtual void Initialize(INavigationParameters parameters) {}
    public virtual void OnAppearing() {}
    public virtual void OnDisappearing() => this.Deactivate();
    
    public virtual void OnResume() => this.OnAppearing();
    public virtual void OnSleep() => this.OnDisappearing();
    public virtual void OnNavigatedFrom(INavigationParameters parameters) {}
    public virtual void OnNavigatedTo(INavigationParameters parameters) {}

    public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters)
        => Task.FromResult(true);

    public virtual void Destroy()
    {
        this.destroyWith?.Dispose();
        this.destroyToken?.Cancel();
        this.destroyToken?.Dispose();
        this.Deactivate();
    }
    
    CompositeDisposable? deactivateWith;
    protected CompositeDisposable DeactivateWith => this.deactivateWith ??= new CompositeDisposable();

    CompositeDisposable? destroyWith;
    protected CompositeDisposable DestroyWith => this.destroyWith ??= new CompositeDisposable();


    CancellationTokenSource? deactiveToken;
    /// <summary>
    /// The destroy cancellation token - called when your model is deactivated
    /// </summary>
    protected CancellationToken DeactivateToken
    {
        get
        {
            this.deactiveToken ??= new CancellationTokenSource();
            return this.deactiveToken.Token;
        }
    }


    CancellationTokenSource? destroyToken;
    /// <summary>
    /// The destroy cancellation token - called when your model is destroyed
    /// </summary>
    protected CancellationToken DestroyToken
    {
        get
        {
            this.destroyToken ??= new CancellationTokenSource();
            return this.destroyToken.Token;
        }
    }
    

    /// <summary>
    /// This can be called manually, generally used when your viewmodel is going to the background in the nav stack
    /// </summary>
    protected virtual void Deactivate()
    {
        this.deactivateWith?.Dispose();
        this.deactivateWith = null;
        
        this.deactiveToken?.Cancel();
        this.deactiveToken?.Dispose();
        this.deactiveToken = null;
    }
#endif
}