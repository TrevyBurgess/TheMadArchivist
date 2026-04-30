using CyberFeedForward.TheMadArchivist.Views.Pages;
using CyberFeedForward.TheMadArchivist.Utilities;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests.Tests.Navigation;

[TestClass]
public sealed class FrameBackForwardTests
{
    [TestMethod]
    public void Frame_CanGoBackAndForward_AfterMultipleNavigations()
    {
        WinUiTestHelper.Run(() =>
        {
            var frame = new Frame();

            Assert.IsFalse(frame.CanGoBack);
            Assert.IsFalse(frame.CanGoForward);

            Assert.IsTrue(frame.NavigateIfNotCurrent(typeof(SettingsPage)));
            Assert.IsFalse(frame.CanGoBack);
            Assert.IsFalse(frame.CanGoForward);

            Assert.IsFalse(frame.NavigateIfNotCurrent(typeof(SettingsPage)));
            Assert.IsFalse(frame.CanGoBack);
            Assert.IsFalse(frame.CanGoForward);

            Assert.IsTrue(frame.NavigateIfNotCurrent(typeof(AboutPage)));
            Assert.IsTrue(frame.CanGoBack);
            Assert.IsFalse(frame.CanGoForward);

            frame.GoBack();
            Assert.IsFalse(frame.CanGoBack);
            Assert.IsTrue(frame.CanGoForward);
        });
    }
}
