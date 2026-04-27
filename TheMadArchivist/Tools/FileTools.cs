using System;
using System.Drawing;
using System.IO;

namespace CyberFeedForward.TheMadArchivist.AppTools;

public static class FileTools
{
    public static void SaveIcon(Icon icon, string filePath)
    {
        ArgumentNullException.ThrowIfNull(icon);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        icon.Save(fileStream);
    }
}
