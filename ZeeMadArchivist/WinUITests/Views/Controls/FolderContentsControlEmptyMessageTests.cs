using CyberFeedForward.TheMadArchivist.Models;
using CyberFeedForward.TheMadArchivist.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Views.Controls;

[TestClass]
public sealed class FolderContentsControlEmptyMessageTests
{
    [TestMethod]
    public void WhenEntriesEmpty_ShowsEmptyMessage()
    {
        WinUiTestHelper.Run(() =>
        {
            var control = new FolderContentsControl();

            control.Entries.Clear();

            var emptyText = (TextBlock)control.FindName("EmptyMessageText");
            Assert.IsNotNull(emptyText);
            Assert.AreEqual(Visibility.Visible, emptyText.Visibility);
        });
    }

    [TestMethod]
    public void WhenEntriesNotEmpty_HidesEmptyMessage()
    {
        WinUiTestHelper.Run(() =>
        {
            var control = new FolderContentsControl();

            control.Entries.Clear();
            control.Entries.Add(new FileSystemEntry { Name = "Dir", FullPath = "C:\\Root\\Dir", IsFolder = true });

            var emptyText = (TextBlock)control.FindName("EmptyMessageText");
            Assert.IsNotNull(emptyText);
            Assert.AreEqual(Visibility.Collapsed, emptyText.Visibility);
        });
    }
}
