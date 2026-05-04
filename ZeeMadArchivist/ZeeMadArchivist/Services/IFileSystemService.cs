using CyberFeedForward.TheMadArchivist.Models;
using System.Collections.Generic;

namespace CyberFeedForward.TheMadArchivist.Services;

public interface IFileSystemService
{
    IReadOnlyList<FileSystemEntry> GetEntries(string folderPath);
}
