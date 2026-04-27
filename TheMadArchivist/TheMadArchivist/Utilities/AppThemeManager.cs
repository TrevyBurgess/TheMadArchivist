using Microsoft.UI.Xaml;

namespace CyberFeedForward.TheMadArchivist.Utilities;

public static class AppThemeManager
{
    public static void ApplyDarkMode(FrameworkElement rootElement, bool isDarkModeEnabled)
    {
        rootElement.RequestedTheme = isDarkModeEnabled ? ElementTheme.Dark : ElementTheme.Light;
    }
}
