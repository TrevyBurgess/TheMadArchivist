using CyberFeedForward.TheMadArchivist.Models;
using System.Collections.ObjectModel;

namespace CyberFeedForward.TheMadArchivist.Services;

public interface IFileSystemTreeProvider
{
    ObservableCollection<FileSystemTreeNode> CreateRoot(string folderPath);
    void LoadChildren(FileSystemTreeNode node);
}
