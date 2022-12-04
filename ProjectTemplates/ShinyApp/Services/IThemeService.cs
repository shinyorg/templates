using System.ComponentModel;

namespace ShinyApp.Services;


public interface IThemeService : INotifyPropertyChanged
{
	AppTheme Current { get; set; }
}