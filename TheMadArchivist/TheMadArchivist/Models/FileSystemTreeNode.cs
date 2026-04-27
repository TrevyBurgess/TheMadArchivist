using System.Collections.ObjectModel;

namespace CyberFeedForward.TheMadArchivist.Models;

public sealed class FileSystemTreeNode
{
    public FileSystemTreeNode(FileSystemEntry entry)
    {
        Entry = entry;
        Children = new ObservableCollection<FileSystemTreeNode>();
    }

    public FileSystemEntry Entry { get; }
    public ObservableCollection<FileSystemTreeNode> Children { get; }

    public bool IsLoaded { get; set; }

    public string Name => Entry.Name;
    public string FullPath => Entry.FullPath;
    public bool IsFolder => Entry.IsFolder;
}
