using CyberFeedForward.TheMadArchivist.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests.Utilities;

[TestClass]
public sealed class AppThemeManagerTests
{
    [TestMethod]
    public void ApplyDarkMode_WhenEnabled_SetsRequestedThemeToDark()
    {
        WinUiTestHelper.Run(() =>
        {
            var root = new Grid();

            AppThemeManager.ApplyDarkMode(root, isDarkModeEnabled: true);

            Assert.AreEqual(ElementTheme.Dark, root.RequestedTheme);
        });
    }

    [TestMethod]
    public void ApplyDarkMode_WhenDisabled_SetsRequestedThemeToLight()
    {
        WinUiTestHelper.Run(() =>
        {
            var root = new Grid();

            AppThemeManager.ApplyDarkMode(root, isDarkModeEnabled: false);

            Assert.AreEqual(ElementTheme.Light, root.RequestedTheme);
        });
    }
}
