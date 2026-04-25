using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using TheMadArchivist.Services;
using TheMadArchivist.Utilities;

namespace TheMadArchivist.Views.Pages;

public sealed partial class SettingsPage : Page
{
    private readonly ThemeSettingsService _themeSettingsService = new(new LocalAppSettingsStore());
    private bool _isInitializing;

    public SettingsPage()
    {
        InitializeComponent();

        Loaded += SettingsPage_OnLoaded;
    }

    private void SettingsPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        _isInitializing = true;
        DarkModeToggleSwitch.IsOn = _themeSettingsService.IsDarkModeEnabled();
        _isInitializing = false;
    }

    private void DarkModeToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (_isInitializing)
        {
            return;
        }

        if (App.MainWindowInstance?.Content is not FrameworkElement rootElement)
        {
            return;
        }

        _themeSettingsService.SetDarkModeEnabled(DarkModeToggleSwitch.IsOn);

        AppThemeManager.ApplyDarkMode(rootElement, DarkModeToggleSwitch.IsOn);
    }
}
