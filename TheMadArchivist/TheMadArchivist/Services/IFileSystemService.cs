using System.Collections.Generic;
using TheMadArchivist.Models;

namespace TheMadArchivist.Services;

public interface IFileSystemService
{
    IReadOnlyList<FileSystemEntry> GetEntries(string folderPath);
}
