using CyberFeedForward.TheMadArchivist.Models;
using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests.ViewModels.Controls;

[TestClass]
public sealed class BreadcrumbBarViewModelTests
{
    private sealed class FakeFileSystemService : IFileSystemService
    {
        private readonly Dictionary<string, IReadOnlyList<FileSystemEntry>> _entriesByPath;

        public FakeFileSystemService(Dictionary<string, IReadOnlyList<FileSystemEntry>> entriesByPath)
        {
            _entriesByPath = entriesByPath;
        }

        public IReadOnlyList<FileSystemEntry> GetEntries(string folderPath)
        {
            return _entriesByPath.TryGetValue(folderPath, out var entries)
                ? entries
                : new List<FileSystemEntry>();
        }
    }

    [TestMethod]
    public void BuildCumulativePaths_Null_ReturnsEmpty()
    {
        var paths = BreadcrumbBarViewModel.BuildCumulativePaths(null);
        Assert.AreEqual(0, paths.Count);
    }

    [TestMethod]
    public void BuildCumulativePaths_WindowsPath_ReturnsRootAndSegments()
    {
        var paths = BreadcrumbBarViewModel.BuildCumulativePaths("C:\\A\\B");

        Assert.AreEqual("C:\\", paths[0]);
        Assert.AreEqual("C:\\A", paths[1]);
        Assert.AreEqual("C:\\A\\B", paths[2]);
    }

    [TestMethod]
    public void SettingFolderPath_BuildsSegments_AndLoadsSubFolderNames()
    {
        var fs = new FakeFileSystemService(new Dictionary<string, IReadOnlyList<FileSystemEntry>>
        {
            ["C:\\"] = new List<FileSystemEntry>(),
            ["C:\\A"] = new List<FileSystemEntry>
            {
                new() { Name = "Sub1", FullPath = "C:\\A\\Sub1", IsFolder = true },
                new() { Name = "File1.txt", FullPath = "C:\\A\\File1.txt", IsFolder = false },
            },
            ["C:\\A\\B"] = new List<FileSystemEntry>(),
        });

        var vm = new BreadcrumbBarViewModel(fs);

        vm.FolderPath = "C:\\A\\B";

        Assert.AreEqual(3, vm.Segments.Count);
        Assert.AreEqual("C:\\", vm.Segments[0].FolderPath);
        Assert.AreEqual(0, vm.Segments[0].Items.Count);

        Assert.AreEqual("C:\\A", vm.Segments[1].FolderPath);
        Assert.AreEqual(1, vm.Segments[1].Items.Count);
        Assert.AreEqual("Sub1", vm.Segments[1].Items[0]);

        Assert.AreEqual("C:\\A\\B", vm.Segments[2].FolderPath);
        Assert.AreEqual(0, vm.Segments[2].Items.Count);
    }
}
