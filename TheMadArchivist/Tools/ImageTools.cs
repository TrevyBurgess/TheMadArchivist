using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace CyberFeedForward.TheMadArchivist.AppTools;

public static class ImageTools
{
    public static Icon ToIcon(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            throw new ArgumentException("Image path cannot be empty.", nameof(imagePath));
        }

        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("Image file not found.", imagePath);
        }

        using var bitmap = new Bitmap(imagePath);
        var hIcon = bitmap.GetHicon();

        try
        {
            using var tempIcon = Icon.FromHandle(hIcon);
            return (Icon)tempIcon.Clone();
        }
        finally
        {
            DestroyIcon(hIcon);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);
}
