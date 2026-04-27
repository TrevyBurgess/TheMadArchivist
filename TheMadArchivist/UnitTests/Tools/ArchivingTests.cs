using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Tools;

[TestClass]
public sealed class ArchivingTests
{
    [TestMethod]
    public void MapDrive_WhenFolderPathIsEmpty_ReturnsInvalidParameter()
    {
        var result = global::CyberFeedForward.TheMadArchivist.AppTools.FolderTools.MapDrive(string.Empty, 'Z', "Test");
        Assert.AreEqual(87, result);
    }

    [TestMethod]
    public void MapDrive_WhenDriveLetterIsInvalid_ReturnsInvalidParameter()
    {
        var result = global::CyberFeedForward.TheMadArchivist.AppTools.FolderTools.MapDrive("\\\\server\\share", '1', "Test");
        Assert.AreEqual(87, result);
    }

    [TestMethod]
    public void MapDrive_WhenNameIsEmpty_ReturnsInvalidParameter()
    {
        var result = global::CyberFeedForward.TheMadArchivist.AppTools.FolderTools.MapDrive("\\\\server\\share", 'Z', string.Empty);
        Assert.AreEqual(87, result);
    }

    [TestMethod]
    public void UnmapDrive_WhenDriveLetterIsInvalid_ReturnsFalse()
    {
        var result = global::CyberFeedForward.TheMadArchivist.AppTools.FolderTools.UnmapDrive('1');
        Assert.IsFalse(result);
    }
}
