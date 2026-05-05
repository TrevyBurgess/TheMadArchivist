using CyberFeedForward.TheMadArchivist.Views.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Views.Controls;

[TestClass]
public sealed class ResizeCursorBorderTests
{
    [TestMethod]
    public void Control_CreatesWithResizeCursor()
    {
        WinUiTestHelper.Run(() =>
        {
            var control = new ResizeCursorBorder();
            Assert.IsNotNull(control.ResizeCursor);
            Assert.IsNotNull(control.Content as Border);
        });
    }
}
