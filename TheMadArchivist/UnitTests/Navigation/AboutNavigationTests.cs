using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using CyberFeedForward.TheMadArchivist.Views.Pages;

namespace UnitTests.Navigation;

[TestClass]
public sealed class AboutNavigationTests
{
    [UITestMethod]
    public void Navigate_To_AboutPage_Succeeds()
    {
        var frame = new Frame();

        var navigated = frame.Navigate(typeof(AboutPage));

        Assert.IsTrue(navigated);
        Assert.IsNotNull(frame.Content);
        Assert.IsInstanceOfType<AboutPage>(frame.Content);
    }
}
