using CyberFeedForward.TheMadArchivist.Models;
using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.Views.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Views.Controls;

[TestClass]
public sealed class FolderContentsControlFolderFilterTests
{
    private sealed class FakeFileSystemService : IFileSystemService
    {
        private readonly IReadOnlyList<FileSystemEntry> _entries;

        public FakeFileSystemService(IReadOnlyList<FileSystemEntry> entries)
        {
            _entries = entries;
        }

        public IReadOnlyList<FileSystemEntry> GetEntries(string folderPath)
        {
            return _entries;
        }
    }

    [TestMethod]
    public void WhenFolderPathChanges_EntriesOnlyContainFolders()
    {
        WinUiTestHelper.Run(() =>
        {
            var fs = new FakeFileSystemService(new List<FileSystemEntry>
            {
                new() { Name = "Dir", FullPath = "C:\\Root\\Dir", IsFolder = true },
                new() { Name = "File.txt", FullPath = "C:\\Root\\File.txt", IsFolder = false },
            });

            var control = new FolderContentsControl(fs);

            control.FolderPath = "C:\\Root";

            Assert.IsTrue(control.Entries.All(e => e.IsFolder));
            Assert.AreEqual(1, control.Entries.Count);
            Assert.AreEqual("Dir", control.Entries[0].Name);
        });
    }
}
