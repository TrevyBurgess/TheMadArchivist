using System.Collections.ObjectModel;
using TheMadArchivist.Models;

namespace TheMadArchivist.Services;

public interface IFileSystemTreeProvider
{
    ObservableCollection<FileSystemTreeNode> CreateRoot(string folderPath);
    void LoadChildren(FileSystemTreeNode node);
}
