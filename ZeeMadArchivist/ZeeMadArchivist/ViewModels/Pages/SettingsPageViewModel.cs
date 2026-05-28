using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.Utilities;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CyberFeedForward.TheMadArchivist;

namespace CyberFeedForward.TheMadArchivist.ViewModels.Pages;

public sealed class SettingsPageViewModel : INotifyPropertyChanged
{
    private readonly ThemeSettingsService _themeSettingsService;
    private readonly CommandBarSettingsService _commandBarSettingsService;
    private readonly FrameworkElement? _themeRootElement;
    private AppThemeMode _themeMode;
    private bool _isCommandBarOnLeft;

    public SettingsPageViewModel()
        : this(
            new ThemeSettingsService(new LocalAppSettingsStore()),
            new CommandBarSettingsService(new LocalAppSettingsStore()),
            App.MainWindowInstance?.Content as FrameworkElement)
    {
    }

    public SettingsPageViewModel(
        ThemeSettingsService themeSettingsService,
        CommandBarSettingsService commandBarSettingsService,
        FrameworkElement? themeRootElement)
    {
        _themeSettingsService = themeSettingsService;
        _commandBarSettingsService = commandBarSettingsService;
        _themeRootElement = themeRootElement;

        _themeMode = _themeSettingsService.GetThemeMode();
        _isCommandBarOnLeft = _commandBarSettingsService.IsCommandBarOnLeft();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AppThemeMode ThemeMode
    {
        get => _themeMode;
        set
        {
            if (_themeMode == value)
            {
                return;
            }

            _themeMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsDarkModeEnabled));
            OnPropertyChanged(nameof(ThemeModeIndex));

            _themeSettingsService.SetThemeMode(value);

            var rootElement = _themeRootElement ?? (App.MainWindowInstance?.Content as FrameworkElement);
            if (rootElement is not null)
            {
                AppThemeManager.ApplyThemeMode(rootElement, value);
            }
        }
    }

    public bool IsDarkModeEnabled
    {
        get => ThemeMode == AppThemeMode.Dark;
        set => ThemeMode = value ? AppThemeMode.Dark : AppThemeMode.Light;
    }

    public int ThemeModeIndex
    {
        get => ThemeMode switch
        {
            AppThemeMode.SystemDefault => 0,
            AppThemeMode.Light => 1,
            _ => 2,
        };
        set => ThemeMode = value switch
        {
            1 => AppThemeMode.Light,
            2 => AppThemeMode.Dark,
            _ => AppThemeMode.SystemDefault,
        };
    }

    public bool IsCommandBarOnLeft
    {
        get => _isCommandBarOnLeft;
        set
        {
            if (_isCommandBarOnLeft == value)
            {
                return;
            }

            _isCommandBarOnLeft = value;
            OnPropertyChanged();

            _commandBarSettingsService.SetCommandBarOnLeft(value);

            if (App.MainWindowInstance is MainWindow mainWindow)
            {
                mainWindow.SetCommandBarOnLeft(value);
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
