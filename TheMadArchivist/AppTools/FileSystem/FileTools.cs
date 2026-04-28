using System;
using System.Drawing;
using System.IO;

namespace CyberFeedForward.TheMadArchivist.AppTools.FileSystem;

public static class FileTools
{

    public static bool IsIdentical(string filePath1, string filePath2)
    {
        if (string.IsNullOrWhiteSpace(filePath1) || string.IsNullOrWhiteSpace(filePath2))
        {
            return false;
        }

        if (string.Equals(filePath1, filePath2, StringComparison.OrdinalIgnoreCase))
        {
            return File.Exists(filePath1);
        }

        var fileInfo1 = new FileInfo(filePath1);
        var fileInfo2 = new FileInfo(filePath2);

        if (!fileInfo1.Exists || !fileInfo2.Exists)
        {
            return false;
        }

        if (fileInfo1.Length != fileInfo2.Length)
        {
            return false;
        }

        const int bufferSize = 1024 * 1024;

        using var stream1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);
        using var stream2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);

        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            var read1 = stream1.Read(buffer1, 0, buffer1.Length);
            var read2 = stream2.Read(buffer2, 0, buffer2.Length);

            if (read1 != read2)
            {
                return false;
            }

            if (read1 == 0)
            {
                return true;
            }

            if (!buffer1.AsSpan(0, read1).SequenceEqual(buffer2.AsSpan(0, read2)))
            {
                return false;
            }
        }
    }

}
