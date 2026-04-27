using CyberFeedForward.TheMadArchivist.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CyberFeedForward.TheMadArchivist.Services;

public sealed class FileSystemTreeProvider : IFileSystemTreeProvider
{
    private readonly IFileSystemService _fileSystemService;

    public FileSystemTreeProvider(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    public ObservableCollection<FileSystemTreeNode> CreateRoot(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            return new ObservableCollection<FileSystemTreeNode>();
        }

        var rootEntry = new FileSystemEntry
        {
            Name = folderPath,
            FullPath = folderPath,
            IsFolder = true,
        };

        var rootNode = new FileSystemTreeNode(rootEntry);
        rootNode.Children.Add(new FileSystemTreeNode(new FileSystemEntry { Name = string.Empty, FullPath = string.Empty, IsFolder = true }));

        return new ObservableCollection<FileSystemTreeNode> { rootNode };
    }

    public void LoadChildren(FileSystemTreeNode node)
    {
        if (!node.IsFolder || node.IsLoaded)
        {
            return;
        }

        node.Children.Clear();

        foreach (var entry in _fileSystemService.GetEntries(node.FullPath))
        {
            var child = new FileSystemTreeNode(entry);

            if (child.IsFolder)
            {
                child.Children.Add(new FileSystemTreeNode(new FileSystemEntry { Name = string.Empty, FullPath = string.Empty, IsFolder = true }));
            }

            node.Children.Add(child);
        }

        node.IsLoaded = true;
    }
}
