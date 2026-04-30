using Microsoft.UI.Xaml;
using CyberFeedForward.TheMadArchivist.Services;

namespace CyberFeedForward.TheMadArchivist.Utilities;

public static class AppThemeManager
{
    public static void ApplyDarkMode(FrameworkElement rootElement, bool isDarkModeEnabled)
    {
        ApplyThemeMode(rootElement, isDarkModeEnabled ? AppThemeMode.Dark : AppThemeMode.Light);
    }

    public static void ApplyThemeMode(FrameworkElement rootElement, AppThemeMode mode)
    {
        rootElement.RequestedTheme = mode switch
        {
            AppThemeMode.Dark => ElementTheme.Dark,
            AppThemeMode.Light => ElementTheme.Light,
            _ => ElementTheme.Default,
        };
    }
}
