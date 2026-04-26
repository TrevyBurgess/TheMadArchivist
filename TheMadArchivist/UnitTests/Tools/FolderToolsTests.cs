using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;

namespace UnitTests.Tools;

[TestClass]
public sealed class FolderToolsTests
{
    [TestMethod]
    public void UpdateFolderIcon_WhenIconPathIsEmpty_ReturnsFalse()
    {
        var result = global::Tools.FolderTools.UpdateFolderIcon(string.Empty, "C:\\");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void UpdateFolderIcon_WhenFolderPathIsEmpty_ReturnsFalse()
    {
        var result = global::Tools.FolderTools.UpdateFolderIcon("C:\\temp.ico", string.Empty);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void UpdateFolderIcon_WhenPathsDoNotExist_ReturnsFalse()
    {
        var result = global::Tools.FolderTools.UpdateFolderIcon("C:\\this-file-should-not-exist-12345.ico", "C:\\this-folder-should-not-exist-12345");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void UpdateFolderIcon_WhenValid_CreatesDesktopIni()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), $"TheMadArchivist_{Guid.NewGuid():N}");
        var tempIcon = Path.Combine(tempFolder, "icon.ico");

        Directory.CreateDirectory(tempFolder);

        try
        {
            using (var icon = SystemIcons.Application)
            using (var fs = new FileStream(tempIcon, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                icon.Save(fs);
            }

            var result = global::Tools.FolderTools.UpdateFolderIcon(tempIcon, tempFolder);

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(Path.Combine(tempFolder, "desktop.ini")));
        }
        finally
        {
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, recursive: true);
            }
        }
    }
}
