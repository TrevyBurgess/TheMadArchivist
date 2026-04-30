using CyberFeedForward.TheMadArchivist.Utilities;
using CyberFeedForward.TheMadArchivist.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Tests.Navigation;

[TestClass]
public sealed class FrameNavigateIfNotCurrentTests
{
    [TestMethod]
    public void NavigateIfNotCurrent_DoesNotAddToBackStack_WhenNavigatingToSamePage()
    {
        WinUiTestHelper.Run(() =>
        {
            var frame = new Frame();

            Assert.IsTrue(frame.NavigateIfNotCurrent(typeof(SettingsPage)));
            Assert.AreEqual(typeof(SettingsPage), frame.CurrentSourcePageType);
            Assert.IsFalse(frame.CanGoBack);

            Assert.IsFalse(frame.NavigateIfNotCurrent(typeof(SettingsPage)));
            Assert.IsFalse(frame.CanGoBack);
        });
    }
}
