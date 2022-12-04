namespace ShinyWish.Services;


public interface IThemeService : INotifyPropertyChanged
{
	AppTheme Current { get; set; }
}