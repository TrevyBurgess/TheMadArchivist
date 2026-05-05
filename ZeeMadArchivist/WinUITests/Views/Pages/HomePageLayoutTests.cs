using CyberFeedForward.TheMadArchivist.Views.Pages;
using CyberFeedForward.TheMadArchivist.Views.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Views.Pages;

[TestClass]
public sealed class HomePageLayoutTests
{
    [TestMethod]
    public void HomePage_HasMovableDividerBetweenPanels()
    {
        WinUiTestHelper.Run(() =>
        {
            var page = new HomePage();

            var divider = (ResizeCursorBorder)page.FindName("FolderContentsDivider");
            Assert.IsNotNull(divider);
            Assert.IsNotNull(divider.ResizeCursor);
        });
    }
}
