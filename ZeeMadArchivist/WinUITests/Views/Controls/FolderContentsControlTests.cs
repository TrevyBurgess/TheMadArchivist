using CyberFeedForward.TheMadArchivist.Models;
using CyberFeedForward.TheMadArchivist.Views.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Views.Controls;

[TestClass]
public sealed class FolderContentsControlTests
{
    [TestMethod]
    public void InvokeEntry_WithFolder_UpdatesFolderPath()
    {
        WinUiTestHelper.Run(() =>
        {
            var control = new FolderContentsControl
            {
                FolderPath = "C:\\Root",
            };

            control.InvokeEntry(new FileSystemEntry { FullPath = "C:\\Root\\Sub", IsFolder = true });

            Assert.AreEqual("C:\\Root\\Sub", control.FolderPath);
        });
    }

    [TestMethod]
    public void InvokeEntry_WithFile_DoesNotUpdateFolderPath()
    {
        WinUiTestHelper.Run(() =>
        {
            var control = new FolderContentsControl
            {
                FolderPath = "C:\\Root",
            };

            control.InvokeEntry(new FileSystemEntry { FullPath = "C:\\Root\\File.txt", IsFolder = false });

            Assert.AreEqual("C:\\Root", control.FolderPath);
        });
    }
}
