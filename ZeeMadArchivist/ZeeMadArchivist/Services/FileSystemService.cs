using CyberFeedForward.TheMadArchivist.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CyberFeedForward.TheMadArchivist.Services;

public sealed class FileSystemService : IFileSystemService
{
    public IReadOnlyList<FileSystemEntry> GetEntries(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            return new List<FileSystemEntry>();
        }

        try
        {
            var directories = Directory.EnumerateDirectories(folderPath)
                .Select(path => new FileSystemEntry { Name = Path.GetFileName(path), FullPath = path, IsFolder = true });

            var files = Directory.EnumerateFiles(folderPath)
                .Select(path => new FileSystemEntry { Name = Path.GetFileName(path), FullPath = path, IsFolder = false });

            return directories
                .Concat(files)
                .OrderByDescending(e => e.IsFolder)
                .ThenBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return new List<FileSystemEntry>();
        }
    }
}
