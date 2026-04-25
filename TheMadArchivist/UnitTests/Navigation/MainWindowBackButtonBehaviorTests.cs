using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests.Navigation;

[TestClass]
public sealed class MainWindowBackButtonBehaviorTests
{
    [UITestMethod]
    public void MainWindow_BackStack_Exists_After_Navigating_To_Settings()
    {
        var window = new TheMadArchivist.MainWindow();

        // Simulate clicking the Settings button by calling the handler via reflection is brittle.
        // Instead, validate expected Frame behavior indirectly: startup should navigate to an initial page,
        // and then navigating to Settings should allow going back.
        var frame = (Microsoft.UI.Xaml.Controls.Frame)window.GetType()
            .GetField("MainFrame", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .GetValue(window)!;

        Assert.IsNotNull(frame.Content);
        Assert.IsFalse(frame.CanGoBack);

        frame.Navigate(typeof(TheMadArchivist.Views.Pages.SettingsPage));

        Assert.IsTrue(frame.CanGoBack);
    }
}
