using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TheMadArchivist.Services;
using TheMadArchivist.Utilities;

namespace TheMadArchivist.ViewModels;

public sealed class SettingsPageViewModel : INotifyPropertyChanged
{
    private readonly ThemeSettingsService _themeSettingsService;
    private readonly FrameworkElement? _themeRootElement;
    private bool _isDarkModeEnabled;

    public SettingsPageViewModel()
        : this(new ThemeSettingsService(new LocalAppSettingsStore()), App.MainWindowInstance?.Content as FrameworkElement)
    {
    }

    public SettingsPageViewModel(ThemeSettingsService themeSettingsService, FrameworkElement? themeRootElement)
    {
        _themeSettingsService = themeSettingsService;
        _themeRootElement = themeRootElement;

        _isDarkModeEnabled = _themeSettingsService.IsDarkModeEnabled();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsDarkModeEnabled
    {
        get => _isDarkModeEnabled;
        set
        {
            if (_isDarkModeEnabled == value)
            {
                return;
            }

            _isDarkModeEnabled = value;
            OnPropertyChanged();

            _themeSettingsService.SetDarkModeEnabled(value);

            if (_themeRootElement is not null)
            {
                AppThemeManager.ApplyDarkMode(_themeRootElement, value);
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
