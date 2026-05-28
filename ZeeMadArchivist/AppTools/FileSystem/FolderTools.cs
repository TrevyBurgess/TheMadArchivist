using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

#nullable enable

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CyberFeedForward.TheMadArchivist.AppTools.FileSystem
{
    public static class FolderTools
    {
        public static bool TryOpenFolderInExplorer(string folderPath, out string errorMessage, Func<ProcessStartInfo, Process?>? processStart = null)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                errorMessage = "Folder path cannot be empty.";
                return false;
            }

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(folderPath);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            if (!Directory.Exists(fullPath))
            {
                errorMessage = "Folder does not exist.";
                return false;
            }

            try
            {
                var start = processStart ?? (psi => Process.Start(psi));
                var psi = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = string.Concat('"', fullPath, '"'),
                    UseShellExecute = true,
                };

                var proc = start(psi);
                if (proc is null)
                {
                    errorMessage = "Unable to launch File Explorer.";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static bool TryRenameIconFile(string customIconsFolderPath, string originalFilePath, string newBaseName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(customIconsFolderPath))
            {
                errorMessage = "Custom icons folder path cannot be empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(originalFilePath))
            {
                errorMessage = "Original file path cannot be empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(newBaseName))
            {
                errorMessage = "New icon name cannot be empty.";
                return false;
            }

            var trimmedNewBaseName = newBaseName.Trim();
            if (trimmedNewBaseName.Length == 0)
            {
                errorMessage = "New icon name cannot be empty.";
                return false;
            }

            if (trimmedNewBaseName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                errorMessage = "New icon name contains invalid characters.";
                return false;
            }

            try
            {
                var folderFullPath = Path.GetFullPath(customIconsFolderPath);
                var originalFullPath = Path.GetFullPath(originalFilePath);

                if (!Directory.Exists(folderFullPath))
                {
                    errorMessage = "Custom icons folder does not exist.";
                    return false;
                }

                if (!File.Exists(originalFullPath))
                {
                    errorMessage = "Original icon file does not exist.";
                    return false;
                }

                if (!folderFullPath.EndsWith(Path.DirectorySeparatorChar))
                {
                    folderFullPath += Path.DirectorySeparatorChar;
                }

                if (!originalFullPath.StartsWith(folderFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Icon file is not in the custom icons folder.";
                    return false;
                }

                var extension = Path.GetExtension(originalFullPath);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".ico";
                }

                var destFullPath = Path.Combine(folderFullPath, trimmedNewBaseName + extension);
                destFullPath = Path.GetFullPath(destFullPath);

                if (!destFullPath.StartsWith(folderFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "New icon name resolves outside the custom icons folder.";
                    return false;
                }

                if (File.Exists(destFullPath))
                {
                    errorMessage = "A file with the new name already exists.";
                    return false;
                }

                File.Move(originalFullPath, destFullPath);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static int LoadDefaultIcons(string customIconsFolderPath, string? iconsFolderPath = null)
        {
            if (string.IsNullOrWhiteSpace(customIconsFolderPath))
            {
                return 0;
            }

            var sourceFolder = iconsFolderPath;
            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                var baseDirectory = AppContext.BaseDirectory;

                sourceFolder = Path.Combine(baseDirectory, "Icons");
                if (!Directory.Exists(sourceFolder))
                {
                    sourceFolder = Path.Combine(baseDirectory, "AppTools", "Icons");
                }

                if (!Directory.Exists(sourceFolder))
                {
                    sourceFolder = Path.Combine(baseDirectory, "AppX", "Icons");
                    if (!Directory.Exists(sourceFolder))
                    {
                        sourceFolder = Path.Combine(baseDirectory, "AppX", "AppTools", "Icons");
                    }
                }

                if (!Directory.Exists(sourceFolder))
                {
                    var assemblyFolder = Path.GetDirectoryName(typeof(FolderTools).Assembly.Location);
                    if (!string.IsNullOrWhiteSpace(assemblyFolder))
                    {
                        sourceFolder = Path.Combine(assemblyFolder, "Icons");
                        if (!Directory.Exists(sourceFolder))
                        {
                            sourceFolder = Path.Combine(assemblyFolder, "AppTools", "Icons");
                        }

                        if (!Directory.Exists(sourceFolder))
                        {
                            sourceFolder = Path.Combine(assemblyFolder, "AppX", "Icons");
                            if (!Directory.Exists(sourceFolder))
                            {
                                sourceFolder = Path.Combine(assemblyFolder, "AppX", "AppTools", "Icons");
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(sourceFolder) || !Directory.Exists(sourceFolder))
            {
                return 0;
            }

            try
            {
                Directory.CreateDirectory(customIconsFolderPath);
            }
            catch
            {
                return 0;
            }

            var copied = 0;
            System.Collections.Generic.IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(sourceFolder);
            }
            catch
            {
                return 0;
            }

            foreach (var sourceFile in files)
            {
                if (string.IsNullOrWhiteSpace(sourceFile))
                {
                    continue;
                }

                var fileName = Path.GetFileName(sourceFile);
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    continue;
                }

                var destFile = Path.Combine(customIconsFolderPath, fileName);
                try
                {
                    if (File.Exists(destFile))
                    {
                        continue;
                    }

                    File.Copy(sourceFile, destFile, overwrite: false);
                    copied++;
                }
                catch
                {
                }
            }

            return copied;
        }

        public static bool UpdateFolderIcon(string iconFilePath, string folderPath)
        {
            if (string.IsNullOrWhiteSpace(iconFilePath) || string.IsNullOrWhiteSpace(folderPath))
            {
                return false;
            }

            if (!File.Exists(iconFilePath) || !Directory.Exists(folderPath))
            {
                return false;
            }

            try
            {
                var desktopIniPath = Path.Combine(folderPath, "desktop.ini");

                var desktopIniContents = new StringBuilder();
                desktopIniContents.AppendLine("[.ShellClassInfo]");
                desktopIniContents.AppendLine($"IconResource={iconFilePath},0");

                File.WriteAllText(desktopIniPath, desktopIniContents.ToString(), Encoding.Unicode);

                var iniAttributes = File.GetAttributes(desktopIniPath);
                File.SetAttributes(desktopIniPath, iniAttributes | FileAttributes.Hidden | FileAttributes.System);

                var folderAttributes = File.GetAttributes(folderPath);
                File.SetAttributes(folderPath, folderAttributes | FileAttributes.ReadOnly);

                SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATHW, folderPath, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int MapDrive(string folderPath, char driveLetter, string name)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return Win32ErrorInvalidParameter;
            }

            var normalizedLetter = char.ToUpperInvariant(driveLetter);
            if (normalizedLetter is < 'A' or > 'Z')
            {
                return Win32ErrorInvalidParameter;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return Win32ErrorInvalidParameter;
            }

            var localName = normalizedLetter + ":";

            var nr = new NetResource
            {
                dwType = ResourceTypeDisk,
                lpLocalName = localName,
                lpRemoteName = folderPath,
                lpProvider = null,
                lpComment = name,
            };

            return WNetAddConnection2(ref nr, null, null, ConnectUpdateProfile);
        }

        public static bool UnmapDrive(char driveLetter)
        {
            var normalizedLetter = char.ToUpperInvariant(driveLetter);
            if (normalizedLetter is < 'A' or > 'Z')
            {
                return false;
            }

            var localName = normalizedLetter + ":";
            var result = WNetCancelConnection2(localName, CancelUpdateProfile, true);

            return result == 0;
        }

        private const int Win32ErrorInvalidParameter = 87;
        private const int ResourceTypeDisk = 0x00000001;
        private const int ConnectUpdateProfile = 0x00000001;
        private const int CancelUpdateProfile = 0x00000001;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NetResource
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string? lpLocalName;
            public string? lpRemoteName;
            public string? lpComment;
            public string? lpProvider;
        }

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetAddConnection2(ref NetResource lpNetResource, string? lpPassword, string? lpUsername, int dwFlags);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

        private const uint SHCNE_UPDATEITEM = 0x00002000;
        private const uint SHCNF_PATHW = 0x0005;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, string? dwItem1, string? dwItem2);
    }
}
