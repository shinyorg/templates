namespace ShinyApp.Services.Impl;


public class ThemeService : NotifyPropertyChanged, IThemeService
{
    public AppTheme Current
    {
        get => Application.Current!.UserAppTheme;
        set
        {
            var theme = Application.Current!.UserAppTheme;
            if (this.Set(ref theme, value))
                Application.Current!.UserAppTheme = value;
        }
    }
}