using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using TheMadArchivist.Utilities;

namespace UnitTests.Utilities;

[TestClass]
public sealed class AppThemeManagerTests
{
    [UITestMethod]
    public void ApplyDarkMode_WhenEnabled_SetsRequestedThemeToDark()
    {
        var root = new Grid();

        AppThemeManager.ApplyDarkMode(root, isDarkModeEnabled: true);

        Assert.AreEqual(ElementTheme.Dark, root.RequestedTheme);
    }

    [UITestMethod]
    public void ApplyDarkMode_WhenDisabled_SetsRequestedThemeToLight()
    {
        var root = new Grid();

        AppThemeManager.ApplyDarkMode(root, isDarkModeEnabled: false);

        Assert.AreEqual(ElementTheme.Light, root.RequestedTheme);
    }
}
