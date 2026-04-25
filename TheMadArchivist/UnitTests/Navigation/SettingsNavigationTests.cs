using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using TheMadArchivist.Views.Pages;

namespace UnitTests.Navigation;

[TestClass]
public sealed class SettingsNavigationTests
{
    [UITestMethod]
    public void Navigate_To_SettingsPage_Succeeds()
    {
        var frame = new Frame();

        var navigated = frame.Navigate(typeof(SettingsPage));

        Assert.IsTrue(navigated);
        Assert.IsNotNull(frame.Content);
        Assert.IsInstanceOfType<SettingsPage>(frame.Content);
    }
}
